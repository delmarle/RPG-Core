
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
        EditorStatic.DrawSectionTitle(28,"Default");
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
        
        EditorStatic.DrawSectionTitle(28,"States");
        if (EditorStatic.SizeableButton(250, 40, "New State", "plus"))
        {
            simpleStateModels.Add(new StateModel());
        }
        

        int itemToRemoveFromIndex = -1;
        for (var index = 0; index < simpleStateModels.Count; index++)
        {
            StateModel stateModel = simpleStateModels[index];
            if (!_foldoutCache.ContainsKey(stateModel))
            {
                _foldoutCache[stateModel] = string.IsNullOrEmpty(stateModel.State);
            }

            EditorGUILayout.BeginHorizontal();
            if (EditorStatic.SizeableButton(100, 20, "Play state", "resultset_next"))
            {
                stateComponent.PlayState(stateModel.State,false,true);
            }

      
            _foldoutCache[stateModel] =       EditorStatic.LevelFoldout("State : " + (string.IsNullOrEmpty(stateModel.State) ? "[ state ]" : stateModel.State), _foldoutCache[stateModel], 24, Color.white);
           
        
            EditorGUILayout.EndHorizontal();
            if (_foldoutCache[stateModel])
            {
                EditorGUILayout.BeginVertical("box");
                DrawEachElement(stateModel);
                EditorStatic.DrawLargeLine(2);
                if (GUILayout.Button("Remove State"))
                {
                    itemToRemoveFromIndex = index;
                }
                EditorGUILayout.EndVertical();
            }
            EditorStatic.DrawThinLine(2);
        }

        if (itemToRemoveFromIndex != -1)
        {
            var simpleStateModelToRemove = simpleStateModels[itemToRemoveFromIndex];
            simpleStateModels.RemoveAt(itemToRemoveFromIndex);
            if (_foldoutCache.ContainsKey(simpleStateModelToRemove))
                _foldoutCache.Remove(simpleStateModelToRemove);
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
       
        if (EditorStatic.SizeableButton(170, 18, "Play state >>", "resultset_next"))
        {
            StateAnimation fSimpleStateComponent = (StateAnimation)target;
            fSimpleStateComponent.PlayState(stateModel.State,false,true);
        }
        stateModel.State = EditorGUILayout.TextField(stateModel.State, GUILayout.Width(Screen.width * 0.3f));
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
            EditorStatic.DrawLargeLine(2);
            EditorGUILayout.BeginHorizontal();
            StateActionModel stateActionModel = stateModel.StateActionModels[index];
            stateActionModel.uiAction = (UiAction)EditorGUILayout.EnumPopup(stateActionModel.uiAction, GUILayout.Height(22));

            
            if (EditorStatic.SizeableButton(80, 22, "", "plus"))
            {
                itemToAddAfterIndex = index;
            }

            if (EditorStatic.SizeableButton(80, 22, "", "cross"))
            {
                itemToRemoveFromIndex = index;
            }

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
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
        switch (stateActionModel.uiAction)
        {
            case UiAction.NoAction:
                EditorGUILayout.LabelField("no action");
                break;
            case UiAction.PlayAnimation:
                AnimationActionData animationActionData = stateActionModel.animationActionData;
                int clipIndex = string.IsNullOrEmpty(animationActionData.Clip) ? 0 : _animationClips.IndexOf(animationActionData.Clip);
                clipIndex = EditorGUILayout.Popup(clipIndex, _animationClips.ToArray());
                clipIndex = clipIndex.Clamp(0, _animationClips.Count);
                if (clipIndex < _animationClips.Count)
                    animationActionData.Clip = _animationClips[clipIndex];
                animationActionData.Layer = EditorGUILayout.IntField("Layer", animationActionData.Layer);
                animationActionData.Weight = EditorGUILayout.Slider(animationActionData.Weight, 0.0f, 1.0f);
                break;

            case UiAction.StopAnimation:
                StopAnimationActionData stopAnimationActionData = stateActionModel.stopAnimationActionData;
                int stopClipIndex = string.IsNullOrEmpty(stopAnimationActionData.Clip) ? 0 : _animationClips.IndexOf(stopAnimationActionData.Clip);
                stopClipIndex = EditorGUILayout.Popup(stopClipIndex, _animationClips.ToArray());
                stopClipIndex = stopClipIndex.Clamp(0, _animationClips.Count);
                if (stopClipIndex < _animationClips.Count)
                    stopAnimationActionData.Clip = _animationClips[stopClipIndex];
                break;
            case UiAction.SetSprite:
                UpdateSpriteActionData updateSpriteActionData = stateActionModel.updateSpriteActionData;
                updateSpriteActionData.Image = (Image)EditorGUILayout.ObjectField(updateSpriteActionData.Image, typeof(Image), true);
                updateSpriteActionData.Sprite = (Sprite)EditorGUILayout.ObjectField(updateSpriteActionData.Sprite, typeof(Sprite), true);
                break;
            case UiAction.UpdateCanvasGroup:
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                UpdateCanvasGroupActionData updateCanvasGroupActionData = stateActionModel.updateCanvasGroupActionData;
                updateCanvasGroupActionData.CanvasGroup = (CanvasGroup)EditorGUILayout.ObjectField(updateCanvasGroupActionData.CanvasGroup, typeof(CanvasGroup), true);
                updateCanvasGroupActionData.Alpha = EditorGUILayout.Slider("Alpha", updateCanvasGroupActionData.Alpha, 0.0f, 1.0f);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                updateCanvasGroupActionData.Interact =
                    EditorGUILayout.Toggle("Interactible", updateCanvasGroupActionData.Interact);
                updateCanvasGroupActionData.BlockRayCasts = EditorGUILayout.Toggle("BlockRaycasts", updateCanvasGroupActionData.BlockRayCasts);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                break;
            case UiAction.UpdateMeshRenderer:
                EnableMeshActionData enableMeshActionData = stateActionModel.enableMeshActionData;
                enableMeshActionData.MeshRenderer = (MeshRenderer)EditorGUILayout.ObjectField(enableMeshActionData.MeshRenderer, typeof(MeshRenderer), true);
                enableMeshActionData.Enabled = EditorGUILayout.Toggle("Enabled", enableMeshActionData.Enabled);
                break;
            case UiAction.UpdateCollider:
                ColliderActionData colliderActionData = stateActionModel.colliderActionData;
                colliderActionData.Collider = (Collider)EditorGUILayout.ObjectField(colliderActionData.Collider, typeof(Collider), true);
                colliderActionData.Enabled = EditorGUILayout.Toggle("Enabled", colliderActionData.Enabled);
                break;
            case UiAction.SwapMaterial:
                ChangeMaterialActionData changeMaterialActionData = stateActionModel.changeMaterialActionData;
                changeMaterialActionData.Renderer = (Renderer)EditorGUILayout.ObjectField(changeMaterialActionData.Renderer, typeof(Renderer), true);
                List<string> materialIndexes = new List<string> { "0" };
                if (changeMaterialActionData.Renderer != null)
                {
                    materialIndexes = new List<string>();
                    for (int i = 0; i < changeMaterialActionData.Renderer.sharedMaterials.Length; i++)
                    {
                        materialIndexes.Add(i.ToString());
                    }
                }
                changeMaterialActionData.Index = EditorGUILayout.Popup(changeMaterialActionData.Index, materialIndexes.ToArray());
                changeMaterialActionData.Material = (Material)EditorGUILayout.ObjectField(changeMaterialActionData.Material, typeof(Material), true);
                break;
            case UiAction.SwapMaterialList:
                ChangeMaterialsActionData changeMaterialsActionData = stateActionModel.changeMaterialsActionData;
                EditorGUILayout.BeginVertical();
                var materialDropZone = DropZone("Drop elements here: ", 100, 50);
                if (changeMaterialsActionData.Renderers == null)
                {
                    changeMaterialsActionData.Renderers = new List<Renderer>();
                }
                var mList = changeMaterialsActionData.Renderers;
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
                changeMaterialsActionData.Index = EditorGUILayout.IntField("Index: ", changeMaterialsActionData.Index);
                changeMaterialsActionData.Material = (Material)EditorGUILayout.ObjectField(changeMaterialsActionData.Material, typeof(Material), true);
                EditorGUILayout.EndVertical();
                break;
            case UiAction.SetActive:
                SetActiveActionData setActiveActionData = stateActionModel.setActiveActionData;
                setActiveActionData.Target = (GameObject)EditorGUILayout.ObjectField(setActiveActionData.Target, typeof(GameObject), true);
                setActiveActionData.Active = EditorGUILayout.Toggle("Enabled", setActiveActionData.Active);
                EditorGUILayout.BeginVertical();
                EditorGUILayout.EndVertical();
                break;
            case UiAction.SetTransform:
                SetTransformActionData setTransformActionData = stateActionModel.setTransformActionData;
                EditorGUILayout.BeginVertical();
                setTransformActionData.Target = (GameObject)EditorGUILayout.ObjectField(setTransformActionData.Target, typeof(GameObject), true);
                setTransformActionData.Position = (Vector3)EditorGUILayout.Vector3Field("Position",setTransformActionData.Position);
                setTransformActionData.Rotation = (Vector3)EditorGUILayout.Vector3Field("Rotation",setTransformActionData.Rotation);
                setTransformActionData.Scale = (Vector3)EditorGUILayout.Vector3Field("Scale",setTransformActionData.Scale);
                EditorGUILayout.EndVertical();
                break;
            case UiAction.AnimateColor:
                TweenColorActionData tweenColorActionData = stateActionModel.tweenColorActionData;
                EditorGUILayout.BeginVertical();
                var dropZone =  DropZone("Drop elements here: ", 100, 50);
                if (tweenColorActionData.Target == null)
                {
                    tweenColorActionData.Target = new List<Renderer>();
                }
                var list = tweenColorActionData.Target;
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

                tweenColorActionData.TargetColor = (Color)EditorGUILayout.ColorField("Color:",tweenColorActionData.TargetColor);
                tweenColorActionData.MaterialId = EditorGUILayout.IntField("Material Id:",tweenColorActionData.MaterialId);
                tweenColorActionData.Duration = EditorGUILayout.FloatField("Duration:",tweenColorActionData.Duration);
                EditorGUILayout.EndVertical();
                break;  
            case UiAction.TweenSharedColor:
                TweenSharedColorActionData tweenSharedColorActionData = stateActionModel.tweenSharedColorActionData;
                EditorGUILayout.BeginVertical();
                var sharedDropZone =  DropZone("Drop elements here: ", 100, 50);
                if (tweenSharedColorActionData.Target == null)
                {
                    tweenSharedColorActionData.Target = new List<Renderer>();
                }
                var sList = tweenSharedColorActionData.Target;
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

                tweenSharedColorActionData.TargetColor = (Color)EditorGUILayout.ColorField("Color:", tweenSharedColorActionData.TargetColor);
                tweenSharedColorActionData.MaterialId = EditorGUILayout.IntField("Material Id:", tweenSharedColorActionData.MaterialId);
                tweenSharedColorActionData.Duration = EditorGUILayout.FloatField("Duration:", tweenSharedColorActionData.Duration);
                EditorGUILayout.EndVertical();
                break;
            case UiAction.SetTransformProperty:
                SetPropertyActionData propertyActionData = stateActionModel.setPropertyActionData;
                EditorGUILayout.BeginVertical();
                if (propertyActionData.Targets == null)
                {
                    propertyActionData.Targets = new List<Transform>();
                }
                var tList = propertyActionData.Targets;
                int tCount = Mathf.Max(0, EditorGUILayout.DelayedIntField("Renderers size", tList.Count));
                while (tCount < tList.Count)
                    tList.RemoveAt(tList.Count - 1);
                while (tCount > tList.Count)
                    tList.Add(null);

                for (int i = 0; i < tList.Count; i++)
                {
                    tList[i] = (Transform)EditorGUILayout.ObjectField("Renderers", tList[i], typeof(Transform), true);
                }

                propertyActionData.TransformType =
                    (PropertyType) EditorGUILayout.EnumPopup(propertyActionData.TransformType);
                propertyActionData.Value = EditorGUILayout.FloatField("Value:", propertyActionData.Value);
                EditorGUILayout.EndVertical();
                break;
            case UiAction.SetRectProperty:
                SetRectPropertyActionData rectPropertyActionData = stateActionModel.setRectPropertyActionData;
                EditorGUILayout.BeginVertical();
                if (rectPropertyActionData.Targets == null)
                {
                    rectPropertyActionData.Targets = new List<RectTransform>();
                }
                var rList = rectPropertyActionData.Targets;
                int rCount = Mathf.Max(0, EditorGUILayout.DelayedIntField("Renderers size", rList.Count));
                while (rCount < rList.Count)
                    rList.RemoveAt(rList.Count - 1);
                while (rCount > rList.Count)
                    rList.Add(null);

                for (int i = 0; i < rList.Count; i++)
                {
                    rList[i] = (RectTransform)EditorGUILayout.ObjectField("Rect", rList[i], typeof(RectTransform), true);
                }

                rectPropertyActionData.TransformType =
                    (PropertyType)EditorGUILayout.EnumPopup(rectPropertyActionData.TransformType);
                rectPropertyActionData.Value = EditorGUILayout.FloatField("Value:", rectPropertyActionData.Value);
                EditorGUILayout.EndVertical();
                break;
            case UiAction.SetText:
                EditorGUILayout.BeginVertical();
                SetTextActionData setTextData = stateActionModel.setTextActionData;
                setTextData.TextComponent = (UiText)EditorGUILayout.ObjectField("Text Component", setTextData.TextComponent, typeof(UiText), true);
                setTextData.Text = EditorGUILayout.TextArea(setTextData.Text);
                EditorGUILayout.EndVertical();
                break;
            case UiAction.SetLocalizedKey:
                EditorGUILayout.BeginVertical();
                UpdateLocalizationActionData updateLocalization = stateActionModel.updateLocalizationActionData;
                updateLocalization.TextComponent = (UiText)EditorGUILayout.ObjectField("Text Component", updateLocalization.TextComponent, typeof(UiText), true);
                updateLocalization.Key = EditorGUILayout.TextField(updateLocalization.Key);
                EditorGUILayout.EndVertical();
                break;
            case UiAction.SetTextColor:
                EditorGUILayout.BeginVertical();
                UpdateTextColorActionData updateTextColor = stateActionModel.updateTextColorActionData;
                updateTextColor.TextComponent = (UiText)EditorGUILayout.ObjectField("Text Component", updateTextColor.TextComponent, typeof(UiText), true);
                updateTextColor.Color = EditorGUILayout.ColorField(updateTextColor.Color);
                EditorGUILayout.EndVertical();
                break;
            case UiAction.SetImageColor:
                EditorGUILayout.BeginVertical();
                SetImageColorActionModel setImageColor = stateActionModel.SetImageColorActionModel;
                setImageColor.Image = (Image)EditorGUILayout.ObjectField("Image Component", setImageColor.Image, typeof(Image), true);
                setImageColor.Color = EditorGUILayout.ColorField(setImageColor.Color);
                EditorGUILayout.EndVertical();
                break;
            case UiAction.SetGraphicsRayCasters:
                DrawSetGraphicsRaycasters(stateActionModel);
                break;
            case UiAction.UpdateMultiCanvasGroup:
                SetCanvasGroupStateActionData setCanvasGroupStateActionData = stateActionModel.setCanvasGroupStateActionData;
                if (setCanvasGroupStateActionData.CanvasGroups.IsEmptyOrNull())
                {
                    setCanvasGroupStateActionData.CanvasGroups = new List<CanvasGroup>(){null};
                }
                EditorGUILayout.BeginVertical();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("F|All"))
                {
                    setCanvasGroupStateActionData.CanvasGroups = ((StateAnimation) target)
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
                            setCanvasGroupStateActionData.CanvasGroups = canvasGroups;
                        else
                            setCanvasGroupStateActionData.CanvasGroups = new List<CanvasGroup>(){null};
                    }
                }

                if (GUILayout.Button("Clear All"))
                {
                    setCanvasGroupStateActionData.CanvasGroups = new List<CanvasGroup>(){null};
                }

                EditorGUILayout.EndHorizontal();

                int itemToAddAfterIndex = -1;
                int itemToRemoveFromIndex = -1;
                for (var index = 0; index < setCanvasGroupStateActionData.CanvasGroups.Count; index++)
                {
                    CanvasGroup canvasGroup = setCanvasGroupStateActionData.CanvasGroups[index];
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
                    setCanvasGroupStateActionData.CanvasGroups.Insert(itemToAddAfterIndex + 1, null);
                }
                else if (itemToRemoveFromIndex != -1 && setCanvasGroupStateActionData.CanvasGroups.Count > 0)
                {
                    setCanvasGroupStateActionData.CanvasGroups.RemoveAt(itemToRemoveFromIndex);
                }

                setCanvasGroupStateActionData.Alpha = EditorGUILayout.Slider("Alpha", setCanvasGroupStateActionData.Alpha, 0.0f, 1.0f);
                setCanvasGroupStateActionData.Interact =
                    EditorGUILayout.Toggle("Interactible", setCanvasGroupStateActionData.Interact);
                setCanvasGroupStateActionData.BlockRayCasts = EditorGUILayout.Toggle("BlockRaycasts", setCanvasGroupStateActionData.BlockRayCasts);
                EditorGUILayout.EndVertical();
                break;     
                
            case UiAction.UpdateCanvasGroupInteraction:
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                SetCanvasInteractionActionData setCanvasInteractionActionData = stateActionModel.setCanvasInteractionActionData;
                setCanvasInteractionActionData.CanvasGroup = (CanvasGroup)EditorGUILayout.ObjectField(setCanvasInteractionActionData.CanvasGroup, typeof(CanvasGroup), true);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                setCanvasInteractionActionData.Interact =
                    EditorGUILayout.Toggle("Interactible", setCanvasInteractionActionData.Interact);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                break;
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawSetGraphicsRaycasters(StateActionModel model)
    {
        SetGraphicsRayCasterActionData setGraphicsRayCasterActionData = model.setGraphicsRayCasterActionData;
        if (setGraphicsRayCasterActionData.RayCasters.IsEmptyOrNull())
        {
            setGraphicsRayCasterActionData.RayCasters = new List<GraphicRaycaster>() { null };
        }
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("F|All"))
        {
            setGraphicsRayCasterActionData.RayCasters = ((StateAnimation)target)
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
                    setGraphicsRayCasterActionData.RayCasters = raycastersGroups;
                else
                    setGraphicsRayCasterActionData.RayCasters = new List<GraphicRaycaster>() { null };
            }
        }

        if (GUILayout.Button("Clear All"))
        {
            setGraphicsRayCasterActionData.RayCasters = new List<GraphicRaycaster>() { null };
        }

        EditorGUILayout.EndHorizontal();
        int itemToAddAfterIndex = -1;
        int itemToRemoveFromIndex = -1;
        for (var index = 0; index < setGraphicsRayCasterActionData.RayCasters.Count; index++)
        {
            GraphicRaycaster raycaster = setGraphicsRayCasterActionData.RayCasters[index];
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
            setGraphicsRayCasterActionData.RayCasters.Insert(itemToAddAfterIndex + 1, null);
        }
        else if (itemToRemoveFromIndex != -1 && setGraphicsRayCasterActionData.RayCasters.Count > 0)
        {
            setGraphicsRayCasterActionData.RayCasters.RemoveAt(itemToRemoveFromIndex);
        }

        setGraphicsRayCasterActionData.IsEnabled = EditorGUILayout.Toggle("Enabled: ", setGraphicsRayCasterActionData.IsEnabled);
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

