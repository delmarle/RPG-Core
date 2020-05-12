
using System.Collections.Generic;
using System.Linq;
using RPG.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    [CustomEditor(typeof(StateAnimation))]
public class StateAnimationEditor : UnityEditor.Editor
{
    private SerializedProperty _useDefaultState;
    private List<string> _animationClips;
    private Dictionary<StateModel, bool> _foldoutCache;
    
    private void OnEnable()
    {
        _useDefaultState = serializedObject.FindProperty("UseDefaultState");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Default", EditorStyles.boldLabel);
        StateAnimation stateComponent = (StateAnimation)target;
        _animationClips = new List<string>();
        if (_foldoutCache == null)
        {
            _foldoutCache = new Dictionary<StateModel, bool>();
        }
        Animation component = stateComponent.GetComponent<Animation>();
        if (component != null)
        {
            foreach (AnimationState animationState in component)
            {
                _animationClips.Add(animationState.clip.name);
            }
        }

        List<StateModel> simpleStateModels = stateComponent.SimpleStateModels;
        EditorGUILayout.PropertyField(_useDefaultState, new GUIContent("Use Default State"));
        
        if (stateComponent.UseDefaultState)
        {
            string[] states = stateComponent.SimpleStateModels.Select(model => model.State).ToArray();
            int defaultChoice = string.IsNullOrEmpty(stateComponent.DefaultState) ? 0 : states.FindIndex(s => s == stateComponent.DefaultState);
            defaultChoice = defaultChoice.Clamp(0, states.Length);
            defaultChoice = EditorGUILayout.Popup(defaultChoice, states);
            stateComponent.DefaultState = states[defaultChoice];
        }
        else
        {
            stateComponent.DefaultState = null;
        }


        EditorGUILayout.LabelField("States", EditorStyles.boldLabel);
        int itemToRemoveFromIndex = -1;
        for (var index = 0; index < simpleStateModels.Count; index++)
        {
            StateModel stateModel = simpleStateModels[index];
            if (!_foldoutCache.ContainsKey(stateModel))
            {
                _foldoutCache[stateModel] = string.IsNullOrEmpty(stateModel.State);
            }

            EditorGUILayout.BeginHorizontal();
            _foldoutCache[stateModel] = GUILayout.Toggle(_foldoutCache[stateModel], "State : " + (string.IsNullOrEmpty(stateModel.State) ? "[ state ]" : stateModel.State), "Foldout", GUILayout.ExpandWidth(false));
            if (GUILayout.Button("Play State", GUILayout.Width(100)))
            {
                stateComponent.PlayState(stateModel.State,false,true);
            }
            EditorGUILayout.EndHorizontal();
            if (_foldoutCache[stateModel])
            {
                Rect stateRect = EditorGUILayout.BeginVertical("window");
                DrawEachElement(stateModel);
                if (GUILayout.Button("Remove State"))
                {
                    itemToRemoveFromIndex = index;
                }
                EditorGUILayout.EndVertical();
            }
        }

        if (itemToRemoveFromIndex != -1)
        {
            var simpleStateModelToRemove = simpleStateModels[itemToRemoveFromIndex];
            simpleStateModels.RemoveAt(itemToRemoveFromIndex);
            if (_foldoutCache.ContainsKey(simpleStateModelToRemove))
                _foldoutCache.Remove(simpleStateModelToRemove);
        }


        if (GUILayout.Button("Add New State"))
        {
            simpleStateModels.Add(new StateModel());
        }

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(stateComponent);
        }
    }

    private void DrawEachElement(StateModel stateModel)
    {
        EditorGUILayout.BeginHorizontal();
        stateModel.State = EditorGUILayout.TextField(stateModel.State, GUILayout.Width(Screen.width * 0.3f));
        if (GUILayout.Button("Play State"))
        {
            StateAnimation fSimpleStateComponent = (StateAnimation)target;
            fSimpleStateComponent.PlayState(stateModel.State,false,true);
        }

        EditorGUILayout.EndHorizontal();

        int itemToAddAfterIndex = -1;
        int itemToRemoveFromIndex = -1;

        if (stateModel.StateActionModels == null)
        {
            stateModel.StateActionModels = new List<StateActionModel>();
        }

        if (stateModel.StateActionModels.Count == 0)
        {
            stateModel.StateActionModels.Add(new StateActionModel());
        }

        for (var index = 0; index < stateModel.StateActionModels.Count; index++)
        {
            EditorGUILayout.BeginHorizontal();
            StateActionModel stateActionModel = stateModel.StateActionModels[index];
            stateActionModel.ActionType = (ActionType)EditorGUILayout.EnumPopup(stateActionModel.ActionType);
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("+"))
            {
                itemToAddAfterIndex = index;
            }
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("X"))
            {
                itemToRemoveFromIndex = index;
            }

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
            DrawEachStateAction(stateActionModel);
        }

        if (itemToAddAfterIndex != -1)
        {
            stateModel.StateActionModels.Insert(itemToAddAfterIndex + 1, new StateActionModel());
        }
        else if (itemToRemoveFromIndex != -1)
        {
            if (stateModel.StateActionModels.Count <= 1)
                return;

            stateModel.StateActionModels.RemoveAt(itemToRemoveFromIndex);
        }
    }

    private void DrawEachStateAction(StateActionModel stateActionModel)
    {
        
        EditorGUILayout.BeginHorizontal("Box");
        switch (stateActionModel.ActionType)
        {
            case ActionType.NoAction:
                EditorGUILayout.LabelField("no action");
                break;
            case ActionType.PlayAnimation:
                AnimationActionModel animationActionModel = stateActionModel.AnimationActionModel;
                int clipIndex = string.IsNullOrEmpty(animationActionModel.Clip) ? 0 : _animationClips.IndexOf(animationActionModel.Clip);
                clipIndex = EditorGUILayout.Popup(clipIndex, _animationClips.ToArray());
                clipIndex = clipIndex.Clamp(0, _animationClips.Count);
                if (clipIndex < _animationClips.Count)
                    animationActionModel.Clip = _animationClips[clipIndex];
                animationActionModel.Layer = EditorGUILayout.IntField("Layer", animationActionModel.Layer);
                animationActionModel.Weight = EditorGUILayout.Slider(animationActionModel.Weight, 0.0f, 1.0f);
                break;

            case ActionType.StopAnimation:
                StopAnimationActionModel stopAnimationActionModel = stateActionModel.StopAnimationActionModel;
                int stopClipIndex = string.IsNullOrEmpty(stopAnimationActionModel.Clip) ? 0 : _animationClips.IndexOf(stopAnimationActionModel.Clip);
                stopClipIndex = EditorGUILayout.Popup(stopClipIndex, _animationClips.ToArray());
                stopClipIndex = stopClipIndex.Clamp(0, _animationClips.Count);
                if (stopClipIndex < _animationClips.Count)
                    stopAnimationActionModel.Clip = _animationClips[stopClipIndex];
                break;
            case ActionType.SetSprite:
                SetSpriteActionModel setSpriteActionModel = stateActionModel.SetSpriteActionModel;
                setSpriteActionModel.Image = (Image)EditorGUILayout.ObjectField(setSpriteActionModel.Image, typeof(Image), true);
                setSpriteActionModel.Sprite = (Sprite)EditorGUILayout.ObjectField(setSpriteActionModel.Sprite, typeof(Sprite), true);
                break;
            case ActionType.UpdateCanvasGroup:
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                UpdateCanvasGroupActionModel updateCanvasGroupActionModel = stateActionModel.UpdateCanvasGroupActionModel;
                updateCanvasGroupActionModel.CanvasGroup = (CanvasGroup)EditorGUILayout.ObjectField(updateCanvasGroupActionModel.CanvasGroup, typeof(CanvasGroup), true);
                updateCanvasGroupActionModel.Alpha = EditorGUILayout.Slider("Alpha", updateCanvasGroupActionModel.Alpha, 0.0f, 1.0f);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                updateCanvasGroupActionModel.IsInteractible =
                    EditorGUILayout.Toggle("Interactible", updateCanvasGroupActionModel.IsInteractible);
                updateCanvasGroupActionModel.BlockRaycasts = EditorGUILayout.Toggle("BlockRaycasts", updateCanvasGroupActionModel.BlockRaycasts);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                break;
            case ActionType.UpdateMeshRenderer:
                MeshRendererActionModel meshRendererActionModel = stateActionModel.MeshRendererActionModel;
                meshRendererActionModel.MeshRenderer = (MeshRenderer)EditorGUILayout.ObjectField(meshRendererActionModel.MeshRenderer, typeof(MeshRenderer), true);
                meshRendererActionModel.Enabled = EditorGUILayout.Toggle("Enabled", meshRendererActionModel.Enabled);
                break;
            case ActionType.UpdateCollider:
                ColliderActionModel colliderActionModel = stateActionModel.ColliderActionModel;
                colliderActionModel.Collider = (Collider)EditorGUILayout.ObjectField(colliderActionModel.Collider, typeof(Collider), true);
                colliderActionModel.Enabled = EditorGUILayout.Toggle("Enabled", colliderActionModel.Enabled);
                break;
            case ActionType.SwapMaterial:
                SwapMaterialActionModel swapMaterialActionModel = stateActionModel.SwapMaterialActionModel;
                swapMaterialActionModel.Renderer = (Renderer)EditorGUILayout.ObjectField(swapMaterialActionModel.Renderer, typeof(Renderer), true);
                List<string> materialIndexes = new List<string> { "0" };
                if (swapMaterialActionModel.Renderer != null)
                {
                    materialIndexes = new List<string>();
                    for (int i = 0; i < swapMaterialActionModel.Renderer.sharedMaterials.Length; i++)
                    {
                        materialIndexes.Add(i.ToString());
                    }
                }
                swapMaterialActionModel.Index = EditorGUILayout.Popup(swapMaterialActionModel.Index, materialIndexes.ToArray());
                swapMaterialActionModel.Material = (Material)EditorGUILayout.ObjectField(swapMaterialActionModel.Material, typeof(Material), true);
                break;
            case ActionType.SwapMaterialList:
                SwapMaterialListActionModel swapMaterialListActionModel = stateActionModel.SwapMaterialListActionModel;
                EditorGUILayout.BeginVertical();
                var materialDropZone = DropZone("Drop elements here: ", 100, 50);
                if (swapMaterialListActionModel.Renderers == null)
                {
                    swapMaterialListActionModel.Renderers = new List<Renderer>();
                }
                var mList = swapMaterialListActionModel.Renderers;
                int mNewCount = Mathf.Max(0, EditorGUILayout.DelayedIntField("Renderers size", mList.Count));
                while (mNewCount < mList.Count)
                    mList.RemoveAt(mList.Count - 1);
                while (mNewCount > mList.Count)
                    mList.Add(null);

                if (materialDropZone != null && materialDropZone.Length > 0)
                {
                    foreach (object o in materialDropZone)
                    {
                        GameObject go = (GameObject)o;
                        Renderer rnd = go.GetComponent<Renderer>();
                        if (rnd != null)
                            mList.Add(go.GetComponent<Renderer>());
                    }
                }
                for (int i = 0; i < mList.Count; i++)
                {
                    mList[i] = (Renderer)EditorGUILayout.ObjectField("Renderers", mList[i], typeof(Renderer), true);
                }
                swapMaterialListActionModel.Index = EditorGUILayout.IntField("Index: ", swapMaterialListActionModel.Index);
                swapMaterialListActionModel.Material = (Material)EditorGUILayout.ObjectField(swapMaterialListActionModel.Material, typeof(Material), true);
                EditorGUILayout.EndVertical();
                break;
            case ActionType.SetActive:
                SetActiveActionModel setActiveActionModel = stateActionModel.SetActiveActionModel;
                setActiveActionModel.Target = (GameObject)EditorGUILayout.ObjectField(setActiveActionModel.Target, typeof(GameObject), true);
                setActiveActionModel.State = EditorGUILayout.Toggle("Enabled", setActiveActionModel.State);
                EditorGUILayout.BeginVertical();
                EditorGUILayout.HelpBox("Please use this action with moderation !", MessageType.Warning);
                EditorGUILayout.EndVertical();
                break;
            case ActionType.SetTransform:
                SetTransformActionModel setTransformActionModel = stateActionModel.SetTransformActionModel;
                EditorGUILayout.BeginVertical();
                setTransformActionModel.Target = (GameObject)EditorGUILayout.ObjectField(setTransformActionModel.Target, typeof(GameObject), true);
                setTransformActionModel.Position = (Vector3)EditorGUILayout.Vector3Field("Position",setTransformActionModel.Position);
                setTransformActionModel.Rotation = (Vector3)EditorGUILayout.Vector3Field("Rotation",setTransformActionModel.Rotation);
                setTransformActionModel.Scale = (Vector3)EditorGUILayout.Vector3Field("Scale",setTransformActionModel.Scale);
                EditorGUILayout.EndVertical();
                break;
            case ActionType.AnimateColor:
                AnimateColorActionModel animateColorActionModel = stateActionModel.AnimateColorActionModel;
                EditorGUILayout.BeginVertical();
                var dropZone =  DropZone("Drop elements here: ", 100, 50);
                if (animateColorActionModel.Target == null)
                {
                    animateColorActionModel.Target = new List<Renderer>();
                }
                var list = animateColorActionModel.Target;
                int newCount = Mathf.Max(0, EditorGUILayout.DelayedIntField("Renderers size", list.Count));
                while (newCount < list.Count)
                    list.RemoveAt(list.Count - 1);
                while (newCount > list.Count)
                    list.Add(null);

                if (dropZone != null && dropZone.Length > 0)
                {
                    foreach (object o in dropZone)
                    {
                        GameObject go = (GameObject)o;
                        Renderer rnd = go.GetComponent<Renderer>();
                        if(rnd != null)
                            list.Add(go.GetComponent<Renderer>());
                    }
                }
                for (int i = 0; i < list.Count; i++)
                {
                    list[i] = (Renderer)EditorGUILayout.ObjectField("Renderers", list[i], typeof(Renderer), true);
                }

                animateColorActionModel.TargetColor = (Color)EditorGUILayout.ColorField("Color:",animateColorActionModel.TargetColor);
                animateColorActionModel.MaterialId = EditorGUILayout.IntField("Material Id:",animateColorActionModel.MaterialId);
                animateColorActionModel.Duration = EditorGUILayout.FloatField("Duration:",animateColorActionModel.Duration);
                EditorGUILayout.EndVertical();
                break;  
            case ActionType.AnimateSharedColor:
                AnimateSharedColorActionModel animateSharedColorActionModel = stateActionModel.AnimateSharedColorActionModel;
                EditorGUILayout.BeginVertical();
                var sharedDropZone =  DropZone("Drop elements here: ", 100, 50);
                if (animateSharedColorActionModel.Target == null)
                {
                    animateSharedColorActionModel.Target = new List<Renderer>();
                }
                var sList = animateSharedColorActionModel.Target;
                int sNewCount = Mathf.Max(0, EditorGUILayout.DelayedIntField("Renderers size", sList.Count));
                while (sNewCount < sList.Count)
                    sList.RemoveAt(sList.Count - 1);
                while (sNewCount > sList.Count)
                    sList.Add(null);

                if (sharedDropZone != null && sharedDropZone.Length > 0)
                {
                    foreach (object o in sharedDropZone)
                    {
                        GameObject go = (GameObject)o;
                        Renderer rnd = go.GetComponent<Renderer>();
                        if(rnd != null)
                            sList.Add(go.GetComponent<Renderer>());
                    }
                }
                for (int i = 0; i < sList.Count; i++)
                {
                    sList[i] = (Renderer)EditorGUILayout.ObjectField("Renderers", sList[i], typeof(Renderer), true);
                }

                animateSharedColorActionModel.TargetColor = (Color)EditorGUILayout.ColorField("Color:", animateSharedColorActionModel.TargetColor);
                animateSharedColorActionModel.MaterialId = EditorGUILayout.IntField("Material Id:", animateSharedColorActionModel.MaterialId);
                animateSharedColorActionModel.Duration = EditorGUILayout.FloatField("Duration:", animateSharedColorActionModel.Duration);
                EditorGUILayout.EndVertical();
                break;
            case ActionType.SetTransformProperty:
                SetPropertyActionModel propertyActionModel = stateActionModel.SetPropertyActionModel;
                EditorGUILayout.BeginVertical();
                if (propertyActionModel.Targets == null)
                {
                    propertyActionModel.Targets = new List<Transform>();
                }
                var tList = propertyActionModel.Targets;
                int tCount = Mathf.Max(0, EditorGUILayout.DelayedIntField("Renderers size", tList.Count));
                while (tCount < tList.Count)
                    tList.RemoveAt(tList.Count - 1);
                while (tCount > tList.Count)
                    tList.Add(null);

                for (int i = 0; i < tList.Count; i++)
                {
                    tList[i] = (Transform)EditorGUILayout.ObjectField("Renderers", tList[i], typeof(Transform), true);
                }

                propertyActionModel.TransformType =
                    (PropertyType) EditorGUILayout.EnumPopup(propertyActionModel.TransformType);
                propertyActionModel.Value = EditorGUILayout.FloatField("Value:", propertyActionModel.Value);
                EditorGUILayout.EndVertical();
                break;
            case ActionType.SetRectProperty:
                SetRectPropertyActionModel rectPropertyActionModel = stateActionModel.SetRectPropertyActionModel;
                EditorGUILayout.BeginVertical();
                if (rectPropertyActionModel.Targets == null)
                {
                    rectPropertyActionModel.Targets = new List<RectTransform>();
                }
                var rList = rectPropertyActionModel.Targets;
                int rCount = Mathf.Max(0, EditorGUILayout.DelayedIntField("Renderers size", rList.Count));
                while (rCount < rList.Count)
                    rList.RemoveAt(rList.Count - 1);
                while (rCount > rList.Count)
                    rList.Add(null);

                for (int i = 0; i < rList.Count; i++)
                {
                    rList[i] = (RectTransform)EditorGUILayout.ObjectField("Rect", rList[i], typeof(RectTransform), true);
                }

                rectPropertyActionModel.TransformType =
                    (PropertyType)EditorGUILayout.EnumPopup(rectPropertyActionModel.TransformType);
                rectPropertyActionModel.Value = EditorGUILayout.FloatField("Value:", rectPropertyActionModel.Value);
                EditorGUILayout.EndVertical();
                break;
            case ActionType.SetGameCamera:
                SetGameCameraActionModel setGameCameraActionModel = stateActionModel.SetGameCameraActionModel;
                EditorGUIUtility.labelWidth = Screen.width * 0.2f;
                setGameCameraActionModel.CameraId = EditorGUILayout.TextField("CameraId", setGameCameraActionModel.CameraId);
                setGameCameraActionModel.IsActive =
                    EditorGUILayout.Toggle("IsActive", setGameCameraActionModel.IsActive);
                break;
            // UI related Actions
            case ActionType.SetText:
                EditorGUILayout.BeginVertical();
                SetTextActionModel setTextModel = stateActionModel.SetTextActionModel;
                setTextModel.TextComponent = (UiText)EditorGUILayout.ObjectField("Text Component", setTextModel.TextComponent, typeof(UiText), true);
                setTextModel.Text = EditorGUILayout.TextArea(setTextModel.Text);
                EditorGUILayout.EndVertical();
                break;
            case ActionType.SetLocalizedKey:
                EditorGUILayout.BeginVertical();
                SetLocalizedKeyActionModel setLocalizedKey = stateActionModel.SetLocalizedKeyActionModel;
                setLocalizedKey.TextComponent = (UiText)EditorGUILayout.ObjectField("Text Component", setLocalizedKey.TextComponent, typeof(UiText), true);
                setLocalizedKey.Key = EditorGUILayout.TextField(setLocalizedKey.Key);
                EditorGUILayout.EndVertical();
                break;
            case ActionType.SetTextColor:
                EditorGUILayout.BeginVertical();
                SetTextColorActionModel setTextColor = stateActionModel.SetTextColorActionModel;
                setTextColor.TextComponent = (UiText)EditorGUILayout.ObjectField("Text Component", setTextColor.TextComponent, typeof(UiText), true);
                setTextColor.Color = EditorGUILayout.ColorField(setTextColor.Color);
                EditorGUILayout.EndVertical();
                break;
            case ActionType.SetImageColor:
                EditorGUILayout.BeginVertical();
                SetImageColorActionModel setImageColor = stateActionModel.SetImageColorActionModel;
                setImageColor.Image = (Image)EditorGUILayout.ObjectField("Image Component", setImageColor.Image, typeof(Image), true);
                setImageColor.Color = EditorGUILayout.ColorField(setImageColor.Color);
                EditorGUILayout.EndVertical();
                break;
            case ActionType.SetGraphicsRaycasters:
                DrawSetGraphicsRaycasters(stateActionModel);
                break;
            case ActionType.UpdateMultiCanvasGroup:
                UpdateMultiCanvasGroupActionModel updateMultiCanvasGroupActionModel = stateActionModel.UpdateMultiCanvasGroupActionModel;
                if (updateMultiCanvasGroupActionModel.CanvasGroups.IsEmptyOrNull())
                {
                    updateMultiCanvasGroupActionModel.CanvasGroups = new List<CanvasGroup>(){null};
                }
                EditorGUILayout.BeginVertical();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("F|All"))
                {
                    updateMultiCanvasGroupActionModel.CanvasGroups = ((StateAnimation) target)
                        .GetComponentsInChildren<CanvasGroup>().ToList();
                }
                if (GUILayout.Button("F|W|Canvas Only"))
                {
                    Canvas[] canvases = ((StateAnimation) target).GetComponentsInChildren<Canvas>();
                    if(canvases.Length>0)
                    {
                        List<CanvasGroup> canvasGroups = new List<CanvasGroup>();
                        foreach (Canvas canvase in canvases)
                        {
                            CanvasGroup canvasGroup = canvase.GetComponent<CanvasGroup>();
                            if(canvasGroup!=null)
                            {
                                canvasGroups.Add(canvasGroup);
                            }
                        }

                        if (canvasGroups.Count > 0)
                            updateMultiCanvasGroupActionModel.CanvasGroups = canvasGroups;
                        else
                            updateMultiCanvasGroupActionModel.CanvasGroups = new List<CanvasGroup>(){null};
                    }
                }

                if (GUILayout.Button("Clear All"))
                {
                    updateMultiCanvasGroupActionModel.CanvasGroups = new List<CanvasGroup>(){null};
                }

                EditorGUILayout.EndHorizontal();

                int itemToAddAfterIndex = -1;
                int itemToRemoveFromIndex = -1;
                for (var index = 0; index < updateMultiCanvasGroupActionModel.CanvasGroups.Count; index++)
                {
                    CanvasGroup canvasGroup = updateMultiCanvasGroupActionModel.CanvasGroups[index];
                    EditorGUILayout.BeginHorizontal();
                    canvasGroup = (CanvasGroup)EditorGUILayout.ObjectField(canvasGroup, typeof(CanvasGroup),true);
                    if (GUILayout.Button("+"))
                    {
                        itemToAddAfterIndex = index;
                    }
                    if (GUILayout.Button("-"))
                    {
                        itemToRemoveFromIndex = index;
                    }
                    EditorGUILayout.EndHorizontal();
                }

                if (itemToAddAfterIndex != -1)
                {
                    updateMultiCanvasGroupActionModel.CanvasGroups.Insert(itemToAddAfterIndex + 1, null);
                }
                else if (itemToRemoveFromIndex != -1 && updateMultiCanvasGroupActionModel.CanvasGroups.Count > 0)
                {
                    updateMultiCanvasGroupActionModel.CanvasGroups.RemoveAt(itemToRemoveFromIndex);
                }

                updateMultiCanvasGroupActionModel.Alpha = EditorGUILayout.Slider("Alpha", updateMultiCanvasGroupActionModel.Alpha, 0.0f, 1.0f);
                updateMultiCanvasGroupActionModel.IsInteractible =
                    EditorGUILayout.Toggle("Interactible", updateMultiCanvasGroupActionModel.IsInteractible);
                updateMultiCanvasGroupActionModel.BlockRaycasts = EditorGUILayout.Toggle("BlockRaycasts", updateMultiCanvasGroupActionModel.BlockRaycasts);
                EditorGUILayout.EndVertical();
                break;     
                
            case ActionType.UpdateCanvasGroupInteraction:
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                UpdateCanvasGroupInteractionActionModel updateCanvasGroupInteractionActionModel = stateActionModel.UpdateCanvasGroupInteractionActionModel;
                updateCanvasGroupInteractionActionModel.CanvasGroup = (CanvasGroup)EditorGUILayout.ObjectField(updateCanvasGroupInteractionActionModel.CanvasGroup, typeof(CanvasGroup), true);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                updateCanvasGroupInteractionActionModel.IsInteractible =
                    EditorGUILayout.Toggle("Interactible", updateCanvasGroupInteractionActionModel.IsInteractible);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                break;
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawSetGraphicsRaycasters(StateActionModel model)
    {
        SetGraphicsRaycasterActionModel setGraphicsRaycasterActionModel = model.SetGraphicsRaycasterActionModel;
        if (setGraphicsRaycasterActionModel.Raycasters.IsEmptyOrNull())
        {
            setGraphicsRaycasterActionModel.Raycasters = new List<GraphicRaycaster>() { null };
        }
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("F|All"))
        {
            setGraphicsRaycasterActionModel.Raycasters = ((StateAnimation)target)
                .GetComponentsInChildren<GraphicRaycaster>().ToList();
        }
        if (GUILayout.Button("F|W|Canvas Only"))
        {
            Canvas[] canvases = ((StateAnimation)target).GetComponentsInChildren<Canvas>();
            if (canvases.Length > 0)
            {
                List<GraphicRaycaster> raycastersGroups = new List<GraphicRaycaster>();
                foreach (Canvas canvase in canvases)
                {
                    GraphicRaycaster raycaster = canvase.GetComponent<GraphicRaycaster>();
                    if (raycaster != null)
                    {
                        raycastersGroups.Add(raycaster);
                    }
                }

                if (raycastersGroups.Count > 0)
                    setGraphicsRaycasterActionModel.Raycasters = raycastersGroups;
                else
                    setGraphicsRaycasterActionModel.Raycasters = new List<GraphicRaycaster>() { null };
            }
        }

        if (GUILayout.Button("Clear All"))
        {
            setGraphicsRaycasterActionModel.Raycasters = new List<GraphicRaycaster>() { null };
        }

        EditorGUILayout.EndHorizontal();
        int itemToAddAfterIndex = -1;
        int itemToRemoveFromIndex = -1;
        for (var index = 0; index < setGraphicsRaycasterActionModel.Raycasters.Count; index++)
        {
            GraphicRaycaster raycaster = setGraphicsRaycasterActionModel.Raycasters[index];
            EditorGUILayout.BeginHorizontal();
            raycaster = (GraphicRaycaster)EditorGUILayout.ObjectField(raycaster, typeof(GraphicRaycaster), true);
            if (GUILayout.Button("+"))
            {
                itemToAddAfterIndex = index;
            }
            if (GUILayout.Button("-"))
            {
                itemToRemoveFromIndex = index;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (itemToAddAfterIndex != -1)
        {
            setGraphicsRaycasterActionModel.Raycasters.Insert(itemToAddAfterIndex + 1, null);
        }
        else if (itemToRemoveFromIndex != -1 && setGraphicsRaycasterActionModel.Raycasters.Count > 0)
        {
            setGraphicsRaycasterActionModel.Raycasters.RemoveAt(itemToRemoveFromIndex);
        }

        setGraphicsRaycasterActionModel.IsEnabled = EditorGUILayout.Toggle("Enabled: ", setGraphicsRaycasterActionModel.IsEnabled);
        EditorGUILayout.EndVertical();
    }
    public object[] DropZone(string title, int w, int h)
    {
        Rect myRect = GUILayoutUtility.GetRect(w, h, GUILayout.ExpandWidth(true));
        GUI.Box(myRect, "Drag and Drop GameObjects to this Box!");
        if (myRect.Contains(Event.current.mousePosition))
        {
            EventType eventType = Event.current.type;
            bool isAccepted = false;

            if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (eventType == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    isAccepted = true;
                }

                Event.current.Use();
            }

            return isAccepted ? DragAndDrop.objectReferences : null;
        }

        return null;
    }
}
}

