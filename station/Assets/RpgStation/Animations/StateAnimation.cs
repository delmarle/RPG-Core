//using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Station
{
    public class StateAnimation : BaseAnimation
    {
        private bool _initialized;

        [SerializeField] public List<StateModel> SimpleStateModels = new List<StateModel>();
        [SerializeField] public bool UseDefaultState;
        [SerializeField] public string DefaultState;

        private Dictionary<string, StateModel> _cacheData;
        private Dictionary<string, List<string>> _cacheAnims;
        private Animation _animation;
        private Dictionary<int, string> _layerStatus;
        private string _currentState = String.Empty;

        protected override void Initialize()
        {
            CacheData();
            if (UseDefaultState)
            {
                PlayState(DefaultState);
            }
        }

        private void CacheData()
        {
            if (_initialized)
                return;
            if (SimpleStateModels == null)
                SimpleStateModels = new List<StateModel>();
            _animation = GetComponent<Animation>();
            if (_animation != null)
            {
                _animation.playAutomatically = false;
            }

            _cacheData = new Dictionary<string, StateModel>();
            _cacheAnims = new Dictionary<string, List<string>>();
            _layerStatus = new Dictionary<int, string>();
            foreach (StateModel stateModel in SimpleStateModels)
            {
                if (stateModel.StateActionModels.IsEmptyOrNull())
                    continue;

                _cacheData[stateModel.State] = stateModel;
                foreach (StateActionModel actionModel in stateModel.StateActionModels)
                {
                    switch (actionModel.ActionType)
                    {
                        case ActionType.PlayAnimation:
                        if (_animation == null)
                            continue;
                        AnimationClip animationClip = _animation.GetClip(actionModel.AnimationActionModel.Clip);
                        if (animationClip == null)
                        {
                            //Throw error
                            continue;
                        }
                        actionModel.AnimationActionModel.Length = animationClip.length;
                        if (actionModel.AnimationActionModel.Length > stateModel.Duration)
                        {
                            stateModel.Duration = actionModel.AnimationActionModel.Length;
                        }

                        if (!_cacheAnims.ContainsKey(stateModel.State))
                        {
                            _cacheAnims[stateModel.State] = new List<string>();
                        }
                        _cacheAnims[stateModel.State].Add(actionModel.AnimationActionModel.Clip);
                        break;
                    }
                }
            }

            if (_cacheAnims.Count <= 0 && _animation!=null)
            {
                _animation.enabled = false;
            }
            _initialized = true;
        }

        public StateModel GetSimpleStateModel(string stateName)
        {
            if (_cacheData == null)
            {
                CacheData();
            }
            if (_cacheData == null)
                return null;
            _cacheData.TryGetValue(stateName, out var result);
            return result;
        }

        public override void PlayState(string stateName, bool checkIsActiveInHierarchy = false, bool forcePlay = false)
        {
            try
            {
                if (_cacheData == null)
                { 
                    CacheData();
                }
                if (_cacheData == null)
                {
                    return;
                }
                if (checkIsActiveInHierarchy && !gameObject.activeInHierarchy)
                {
                    return;
                }
                if (!_cacheData.ContainsKey(stateName))
                {
                    return;
                }

                if (_currentState == stateName && !forcePlay)
                {
                    return;
                }

                _currentState = stateName;
                StateModel stateModel = _cacheData[stateName];

                if (_layerStatus.ContainsKey(stateModel.LayerIndex))
                {
                    string previousState = _layerStatus[stateModel.LayerIndex];
                    if (_cacheAnims.ContainsKey(previousState))
                    {
                        List<string> previousAnims = _cacheAnims[previousState];
                        foreach (string previousClip in previousAnims)
                        {
                            AnimationState animationState = _animation[previousClip];
                            if (animationState != null && animationState.enabled)
                            {
                                animationState.enabled = false;
                            }
                        }
                    }
                    _layerStatus.Remove(stateModel.LayerIndex);
                }
                foreach (StateActionModel actionModel in stateModel.StateActionModels)
                {
                    ExecuteEachStateAction(actionModel, stateModel.LayerIndex);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("Exception Playing State : "+stateName+" Object name: "+ transform.name);
                Debug.LogWarning(e.StackTrace);
            }
        }

        #region ACTIONS
        private void ExecuteEachStateAction(StateActionModel actionModel , int layerIndex)
        {
            switch (actionModel.ActionType)
            {
                case ActionType.PlayAnimation:
                    ExecutePlayAnimation(actionModel);
                    break;
                case ActionType.SetSprite:
                    ExecuteSetSpriteAction(actionModel);
                    break;
                case ActionType.UpdateCanvasGroup:
                    ExecuteUpdateCanvasGroup(actionModel);
                    break;
                case ActionType.UpdateMeshRenderer:
                    ExecuteUpdateMeshRenderer(actionModel);
                    break;
                case ActionType.UpdateCollider:
                    ExecuteUpdateCollider(actionModel);
                    break;
                case ActionType.SwapMaterial:
                    ExecuteSwapMaterial(actionModel);
                    break;
                case ActionType.SwapMaterialList:
                    ExecuteSwapMaterialList(actionModel);
                    break;
                case ActionType.SetActive:
                    ExecuteSetActive(actionModel);
                    break;
                case ActionType.SetTransform:
                    ExecuteSetTransformAction(actionModel);
                    break;
                case ActionType.AnimateColor:
                    ExecuteAnimateColor(actionModel);
                    break;
                case ActionType.AnimateSharedColor:
#if !UNITY_EDITOR
                    ExecuteSharedAnimateColor(actionModel);
#else
                    ExecuteAnimateColor(actionModel);
#endif
                    break;
                case ActionType.SetTransformProperty:
                    ExecuteSetPropertyAction(actionModel);
                    break;
                case ActionType.SetGameCamera:
                    ExecuteSetGameCameraAction(actionModel);
                    break;
                case ActionType.UpdateMultiCanvasGroup:
                    ExecuteUpdateMultiCanvasGroup(actionModel);
                    break;
                case ActionType.UpdateCanvasGroupInteraction:
                    ExecuteCanvasGroupInteraction(actionModel);
                    break;
                case ActionType.StopAnimation:
                    ExecuteStopAnimation(actionModel);
                    break;
                //UI
                case ActionType.SetText:
                    ExecuteSetText(actionModel);
                    break;
                case ActionType.SetLocalizedKey:
                    ExecuteSetLocalizedKey(actionModel);
                    break;
                case ActionType.SetTextColor:
                    ExecuteSetTextColor(actionModel);
                    break;
                case ActionType.SetImageColor:
                    ExecuteSetImageColor(actionModel);
                    break;
                case ActionType.SetGraphicsRaycasters:
                    ExecuteSetGraphicsRaycaster(actionModel);
                    break;
                case ActionType.SetRectProperty:
                    ExecuteSetRectPropertyAction(actionModel);
                    break;

            }
        }

        private void ExecuteSetGameCameraAction(StateActionModel actionModel)
        {
            if (actionModel.SetGameCameraActionModel == null)
                return;
            if (string.IsNullOrEmpty(actionModel.SetGameCameraActionModel.CameraId))
                return;
            //if(actionModel.SetGameCameraActionModel.IsActive)
             //   FalconCore.FCameraSystem.ActivateCamera(actionModel.SetGameCameraActionModel.CameraId);
           // else
               // FalconCore.FCameraSystem.DeActivateCamera(actionModel.SetGameCameraActionModel.CameraId);
        }

        private void ExecuteSwapMaterial(StateActionModel actionModel)
        {
            if (actionModel.SwapMaterialActionModel == null)
                return;
            if (actionModel.SwapMaterialActionModel.Renderer == null || actionModel.SwapMaterialActionModel.Material == null)
                return;
            Material[] materials = new Material[actionModel.SwapMaterialActionModel.Renderer.materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                if (i == actionModel.SwapMaterialActionModel.Index)
                {
                    materials[i] = actionModel.SwapMaterialActionModel.Material;
                }
                else
                {
                    materials[i] = actionModel.SwapMaterialActionModel.Renderer.materials[i];
                }
            }
            actionModel.SwapMaterialActionModel.Renderer.materials = materials;
        }
        private void ExecuteSwapMaterialList(StateActionModel actionModel)
        {
            if (actionModel.SwapMaterialListActionModel == null)
                return;
            if (actionModel.SwapMaterialListActionModel.Renderers == null || actionModel.SwapMaterialListActionModel.Material == null)
                return;
            foreach (Renderer rndr in actionModel.SwapMaterialListActionModel.Renderers)
            {
                if (rndr == null)
                {
                    continue;
                }
                Material[] materials = new Material[rndr.materials.Length];
                for (int i = 0; i < materials.Length; i++)
                {
                    if (i == actionModel.SwapMaterialListActionModel.Index)
                    {
                        materials[i] = actionModel.SwapMaterialListActionModel.Material;
                    }
                    else
                    {
                        materials[i] = rndr.materials[i];
                    }
                }
                rndr.materials = materials;
            }
           
        }

        private static void ExecuteUpdateCollider(StateActionModel actionModel)
        {
            if (actionModel.ColliderActionModel == null)
                return;
            if (actionModel.ColliderActionModel.Collider == null)
                return;
            actionModel.ColliderActionModel.Collider.enabled =
                actionModel.ColliderActionModel.Enabled;
        }

        private static void ExecuteUpdateMeshRenderer(StateActionModel actionModel)
        {
            if (actionModel.MeshRendererActionModel == null)
                return;
            if (actionModel.MeshRendererActionModel.MeshRenderer == null)
                return;
            actionModel.MeshRendererActionModel.MeshRenderer.enabled =
                actionModel.MeshRendererActionModel.Enabled;
        }

        private static void ExecuteUpdateCanvasGroup(StateActionModel actionModel)
        {
            if (actionModel.UpdateCanvasGroupActionModel == null)
                return;
            if (actionModel.UpdateCanvasGroupActionModel.CanvasGroup == null)
                return;
            actionModel.UpdateCanvasGroupActionModel.CanvasGroup.alpha = actionModel.UpdateCanvasGroupActionModel.Alpha;
            actionModel.UpdateCanvasGroupActionModel.CanvasGroup.interactable = actionModel.UpdateCanvasGroupActionModel.IsInteractible;
            actionModel.UpdateCanvasGroupActionModel.CanvasGroup.blocksRaycasts = actionModel.UpdateCanvasGroupActionModel.BlockRaycasts;
        }

        private static void ExecuteSetSpriteAction(StateActionModel actionModel)
        {
            if (actionModel.SetSpriteActionModel == null)
                return;
            if (actionModel.SetSpriteActionModel.Sprite == null ||
                actionModel.SetSpriteActionModel.Image == null)
                return;
            actionModel.SetSpriteActionModel.Image.sprite = actionModel.SetSpriteActionModel.Sprite;
        }

        private void ExecutePlayAnimation(StateActionModel actionModel)
        {
            if (actionModel.AnimationActionModel?.Clip == null)
                return;
            if (_animation == null)
                return;

            _animation[actionModel.AnimationActionModel.Clip].layer = actionModel.AnimationActionModel.Layer;
            _animation[actionModel.AnimationActionModel.Clip].weight = actionModel.AnimationActionModel.Weight;
            _animation.Play(actionModel.AnimationActionModel.Clip);
        }

        public void AnimationSample()
        {
            _animation.Sample();
        }


        private void ExecuteSetTransformAction(StateActionModel actionModel)
        {
            GameObject target = actionModel.SetTransformActionModel.Target;
            if (target == null)
            {
                Debug.LogWarning("Target gameObject not assigned in Simple State component in "+gameObject);
                return;
            }
            target.transform.SetPositionAndRotation(actionModel.SetTransformActionModel.Position, Quaternion.Euler(actionModel.SetTransformActionModel.Rotation));
            target.transform.localScale = actionModel.SetTransformActionModel.Scale;
        }

        private void ExecuteSetPropertyAction(StateActionModel actionModel)
        {
            List<Transform> targets = actionModel.SetPropertyActionModel.Targets;
            if (targets == null)
            {
                Debug.LogWarning("Target transforms are null in Simple State component in {0}", gameObject);
                return;
            }

            foreach (Transform target in targets)
            {
                Vector3 vector = Vector3.zero;
                switch (actionModel.SetPropertyActionModel.TransformType)
                {
                    case PropertyType.PositionX:
                        vector = target.position;
                        vector.x = actionModel.SetPropertyActionModel.Value;
                        target.position = vector;
                        break;
                    case PropertyType.PositionY:
                        vector = target.position;
                        vector.y = actionModel.SetPropertyActionModel.Value;
                        target.position = vector;
                        break;
                    case PropertyType.PositionZ:
                        vector = target.position;
                        vector.z = actionModel.SetPropertyActionModel.Value;
                        target.position = vector;
                        break;
                    case PropertyType.RotationX:
                        vector = target.rotation.eulerAngles;
                        vector.x = actionModel.SetPropertyActionModel.Value;
                        target.rotation = Quaternion.Euler(vector);
                        break;
                    case PropertyType.RotationY:
                        vector = target.rotation.eulerAngles;
                        vector.y = actionModel.SetPropertyActionModel.Value;
                        target.rotation = Quaternion.Euler(vector);
                        break;
                    case PropertyType.RotationZ:
                        vector = target.rotation.eulerAngles;
                        vector.z = actionModel.SetPropertyActionModel.Value;
                        target.rotation = Quaternion.Euler(vector);
                        break;
                    case PropertyType.ScaleX:
                        vector = target.localScale;
                        vector.x = actionModel.SetPropertyActionModel.Value;
                        target.localScale = vector;
                        break;
                    case PropertyType.ScaleY:
                        vector = target.localScale;
                        vector.y = actionModel.SetPropertyActionModel.Value;
                        target.localScale = vector;
                        break;
                    case PropertyType.ScaleZ:
                        vector = target.localScale;
                        vector.z = actionModel.SetPropertyActionModel.Value;
                        target.localScale = vector;
                        break;
                }
            }
            
        }

        private void ExecuteSetRectPropertyAction(StateActionModel actionModel)
        {
            List<RectTransform> targets = actionModel.SetRectPropertyActionModel.Targets;
            if (targets == null)
            {
                Debug.LogWarning("Target transforms are null in Simple State component in {0}", gameObject);
                return;
            }

            foreach (RectTransform target in targets)
            {
                Vector3 vector = Vector3.zero;
                switch (actionModel.SetRectPropertyActionModel.TransformType)
                {
                    case PropertyType.PositionX:
                        vector = target.anchoredPosition;
                        vector.x = actionModel.SetRectPropertyActionModel.Value;
                        target.anchoredPosition = vector;
                        break;
                    case PropertyType.PositionY:
                        vector = target.anchoredPosition;
                        vector.y = actionModel.SetRectPropertyActionModel.Value;
                        target.anchoredPosition = vector;
                        break;
                    case PropertyType.PositionZ:
                        vector = target.anchoredPosition;
                        vector.z = actionModel.SetRectPropertyActionModel.Value;
                        target.anchoredPosition = vector;
                        break;
                    case PropertyType.RotationX:
                        vector = target.rotation.eulerAngles;
                        vector.x = actionModel.SetRectPropertyActionModel.Value;
                        target.rotation = Quaternion.Euler(vector);
                        break;
                    case PropertyType.RotationY:
                        vector = target.rotation.eulerAngles;
                        vector.y = actionModel.SetRectPropertyActionModel.Value;
                        target.rotation = Quaternion.Euler(vector);
                        break;
                    case PropertyType.RotationZ:
                        vector = target.rotation.eulerAngles;
                        vector.z = actionModel.SetRectPropertyActionModel.Value;
                        target.rotation = Quaternion.Euler(vector);
                        break;
                    case PropertyType.ScaleX:
                        vector = target.localScale;
                        vector.x = actionModel.SetRectPropertyActionModel.Value;
                        target.localScale = vector;
                        break;
                    case PropertyType.ScaleY:
                        vector = target.localScale;
                        vector.y = actionModel.SetRectPropertyActionModel.Value;
                        target.localScale = vector;
                        break;
                    case PropertyType.ScaleZ:
                        vector = target.localScale;
                        vector.z = actionModel.SetRectPropertyActionModel.Value;
                        target.localScale = vector;
                        break;
                }
            }

        }
        private void ExecuteSetActive(StateActionModel actionModel)
        {
            GameObject target = actionModel.SetActiveActionModel.Target;
            if (target == null)
            {
                Debug.LogWarning("Your Target game object is missing in Simple State Component in {0}", gameObject);
                return;
            }

            target.SetActive(actionModel.SetActiveActionModel.State);
        }

        private void ExecuteAnimateColor(StateActionModel actionModel)
        {
            List<Renderer> target = actionModel.AnimateColorActionModel.Target;
            Color targetColor = actionModel.AnimateColorActionModel.TargetColor;
            float duration = actionModel.AnimateColorActionModel.Duration;
            int id = actionModel.AnimateColorActionModel.MaterialId;
            foreach (Renderer rndr in target)
            {
                if (rndr == null)
                    continue;

                try
                {
                    //TODO
                   // rndr.materials[id].DOColor(targetColor, "_BaseColor", duration);
                }
                catch (IndexOutOfRangeException error)
                {
                    Debug.LogWarning( $"Invalid index [{id}] for MaterialId in FSimpleStateAnimation AnimateColor for gameObject: [{rndr.name}]  Error: {error}");
                }
            }
            
        }
        private void ExecuteSharedAnimateColor(StateActionModel actionModel)
        {
            List<Renderer> target = actionModel.AnimateSharedColorActionModel.Target;
            Color targetColor = actionModel.AnimateSharedColorActionModel.TargetColor;
            float duration = actionModel.AnimateSharedColorActionModel.Duration;
            int id = actionModel.AnimateSharedColorActionModel.MaterialId;
            foreach (Renderer rndr in target)
            {
                if (rndr == null)
                    continue;

                try
                {
                    if (duration != 0)
                    {
                        //TODO
                        //rndr.sharedMaterials[id].DOColor(targetColor, "_BaseColor", duration);
                    }
                    else
                    {
                        rndr.sharedMaterials[id].SetColor("_BaseColor", targetColor);
                    }
                }
                catch (IndexOutOfRangeException error)
                {
                    Debug.LogWarning($"Invalid index [{id}] for MaterialId AnimateColor for gameObject: [{rndr.name}] error: {error}");
                }
            }
            
        }

        private static void ExecuteUpdateMultiCanvasGroup(StateActionModel actionModel)
        {
            if (actionModel.UpdateMultiCanvasGroupActionModel == null)
                return;
            if (actionModel.UpdateMultiCanvasGroupActionModel.CanvasGroups.IsEmptyOrNull())
                return;
            foreach (CanvasGroup canvasGroup in actionModel.UpdateMultiCanvasGroupActionModel.CanvasGroups)
            {
                if (canvasGroup == null)
                    continue;
                canvasGroup.alpha = actionModel.UpdateMultiCanvasGroupActionModel.Alpha;
                canvasGroup.interactable = actionModel.UpdateMultiCanvasGroupActionModel.IsInteractible;
                canvasGroup.blocksRaycasts = actionModel.UpdateMultiCanvasGroupActionModel.BlockRaycasts;
            }
        }
        
        private static void ExecuteCanvasGroupInteraction(StateActionModel actionModel)
        {
            if (actionModel.UpdateCanvasGroupInteractionActionModel == null)
                return;
            if (actionModel.UpdateCanvasGroupInteractionActionModel.CanvasGroup == null)
                return;
            actionModel.UpdateCanvasGroupInteractionActionModel.CanvasGroup.interactable =
                actionModel.UpdateCanvasGroupInteractionActionModel.IsInteractible;
        }


        private void ExecuteStopAnimation(StateActionModel actionModel)
        {
            _animation.Stop(actionModel.StopAnimationActionModel.Clip);
        }
        
        private void ExecuteSetText(StateActionModel actionModel)
        {
            UiText textComponent = actionModel.SetTextActionModel.TextComponent;
            if (textComponent != null)
            {
                textComponent.SetText(actionModel.SetTextActionModel.Text);
            }
        }

        private void ExecuteSetTextColor(StateActionModel actionModel)
        {
            UiText textComponent = actionModel.SetTextColorActionModel.TextComponent;
            if (textComponent != null)
            {
                textComponent.color = actionModel.SetTextColorActionModel.Color;
            }
        }

        private void ExecuteSetLocalizedKey(StateActionModel actionModel)
        {
            UiText textComponent = actionModel.SetLocalizedKeyActionModel.TextComponent;
            if (textComponent != null)
            {
                textComponent.LocalizedKey = actionModel.SetLocalizedKeyActionModel.Key;
            }
        }

        private void ExecuteSetImageColor(StateActionModel actionModel)
        {
            Image imageComponent = actionModel.SetImageColorActionModel.Image;
            if (imageComponent != null)
            {
                imageComponent.color = actionModel.SetImageColorActionModel.Color;
            }
        }

        private void ExecuteSetGraphicsRaycaster(StateActionModel actionModel)
        {
            if (actionModel.SetGraphicsRaycasterActionModel == null)
                return;
            if (actionModel.SetGraphicsRaycasterActionModel.Raycasters.IsEmptyOrNull())
                return;

            foreach (GraphicRaycaster graphicRaycaster in actionModel.SetGraphicsRaycasterActionModel.Raycasters)
            {
                graphicRaycaster.enabled = actionModel.SetGraphicsRaycasterActionModel.IsEnabled;
            }
        }
#endregion

        public string CurrentState(int layer = -1)
        {
            return _currentState;
        }
        public override float GetStateDuration(string stateName)
        {
            if (!_cacheData.ContainsKey(stateName))
            {
                return 0;
            }
            return _cacheData[stateName].Duration;
        }

        public override void StopAllAnimations()
        {
            if (_animation)
            {
                _animation.Stop();
            }
        }

    }

    [Serializable]
    public class StateModel
    {
        public List<StateActionModel> StateActionModels;
        public string State;
        public int LayerIndex;
        [NonSerialized]
        public float Duration;
    }

    [Serializable]
    public class StateActionModel
    {
        public ActionType ActionType;
        public AnimationActionModel AnimationActionModel;
        public SetSpriteActionModel SetSpriteActionModel;
        public UpdateCanvasGroupActionModel UpdateCanvasGroupActionModel;
        public MeshRendererActionModel MeshRendererActionModel;
        public ColliderActionModel ColliderActionModel;
        public SwapMaterialActionModel SwapMaterialActionModel;
        public SwapMaterialListActionModel SwapMaterialListActionModel;
        public SetActiveActionModel SetActiveActionModel;
        public SetTransformActionModel SetTransformActionModel;
        public AnimateColorActionModel AnimateColorActionModel;
        public AnimateSharedColorActionModel AnimateSharedColorActionModel;
        public SetPropertyActionModel SetPropertyActionModel;
        public SetGameCameraActionModel SetGameCameraActionModel;
        public UpdateMultiCanvasGroupActionModel UpdateMultiCanvasGroupActionModel;
        public UpdateCanvasGroupInteractionActionModel UpdateCanvasGroupInteractionActionModel;
        public StopAnimationActionModel StopAnimationActionModel;
        public SetTextActionModel SetTextActionModel;
        public SetLocalizedKeyActionModel SetLocalizedKeyActionModel;
        public SetTextColorActionModel SetTextColorActionModel;
        public SetImageColorActionModel SetImageColorActionModel;
        public SetGraphicsRaycasterActionModel SetGraphicsRaycasterActionModel;
        public SetRectPropertyActionModel SetRectPropertyActionModel;
    }

    [Serializable]
    public class AnimationActionModel
    {
        public string Clip;
        public float Weight = 0.5f;
        public int Layer;
        [NonSerialized]
        public float Length;
    }

    [Serializable]
    public class SetSpriteActionModel
    {
        public Image Image;
        public Sprite Sprite;
    }

    [Serializable]
    public class UpdateCanvasGroupActionModel
    {
        public CanvasGroup CanvasGroup;
        public float Alpha;
        public bool IsInteractible;
        public bool BlockRaycasts;
    }

    [Serializable]
    public class MeshRendererActionModel
    {
        public MeshRenderer MeshRenderer;
        public bool Enabled;
    }

    [Serializable]
    public class ColliderActionModel
    {
        public Collider Collider;
        public bool Enabled;
    }

    [Serializable]
    public class SwapMaterialActionModel
    {
        public Renderer Renderer;
        public int Index;
        public Material Material;
    }
    
    [Serializable] 
    public class SwapMaterialListActionModel
    {
        public List<Renderer> Renderers;
        public int Index;
        public Material Material;
    }

    [Serializable]
    public class SetActiveActionModel
    {
        public GameObject Target;
        public bool State;
    }

    [Serializable]
    public class AnimateColorActionModel
    {
        public List<Renderer> Target;
        public Color TargetColor;
        public int MaterialId;
        public float Duration;
    }
    [Serializable]
    public class AnimateSharedColorActionModel
    {
        public List<Renderer> Target;
        public Color TargetColor;
        public int MaterialId;
        public float Duration;
    }

    [Serializable]
    public class SetTransformActionModel
    {
        public GameObject Target;
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale = Vector3.one;
    }


    [Serializable]
    public class SetPropertyActionModel
    {
        public List<Transform> Targets;
        public PropertyType TransformType;
        public float Value;
    }

    [Serializable]
    public class SetRectPropertyActionModel
    {
        public List<RectTransform> Targets;
        public PropertyType TransformType;
        public float Value;
    }

    [Serializable]
    public class SetGameCameraActionModel
    {
        public string CameraId;
        public bool IsActive;
    }

    [Serializable]
    public class UpdateMultiCanvasGroupActionModel
    {
        public List<CanvasGroup> CanvasGroups;
        public float Alpha;
        public bool IsInteractible;
        public bool BlockRaycasts;
    }
    
    [Serializable]
    public class UpdateCanvasGroupInteractionActionModel
    {
        public CanvasGroup CanvasGroup;
        public bool IsInteractible;
    }

    [Serializable]
    public class StopAnimationActionModel
    {
        public string Clip;
    }

    [Serializable]
    public class SetGraphicsRaycasterActionModel
    {
        public List<GraphicRaycaster> Raycasters;
        public bool IsEnabled;
    }

    // UI ACTIONS
    [Serializable]
    public class SetTextActionModel
    {
        public UiText TextComponent;
        public string Text;
    }

    [Serializable]
    public class SetLocalizedKeyActionModel
    {
        public UiText TextComponent;
        public string Key;
    }

    [Serializable]
    public class SetTextColorActionModel
    {
        public UiText TextComponent;
        public Color Color;
    }

    [Serializable]
    public class SetImageColorActionModel
    {
        public Image Image;
        public Color Color;
    }

    [Serializable]
    public enum ActionType
    {
        NoAction,
        PlayAnimation,
        SetSprite,
        UpdateCanvasGroup,
        UpdateMeshRenderer,
        UpdateCollider,
        SwapMaterial,
        SetActive,
        SetTransform,
        AnimateColor,
        SetTransformProperty,
        SetGameCamera,
        UpdateMultiCanvasGroup,
        StopAnimation,
        SetText,
        SetLocalizedKey,
        SetTextColor,
        SetImageColor,
        SetGraphicsRaycasters,
        SetRectProperty,
        SwapMaterialList,
        AnimateSharedColor,
        UpdateCanvasGroupInteraction
        // ALWAYS ADD NEW ACTIONS AT THE BOTTOM
    }

    [Serializable]
    public enum PropertyType
    {
        PositionX,
        PositionY,
        PositionZ,
        RotationX,
        RotationY,
        RotationZ,
        ScaleX,
        ScaleY,
        ScaleZ
    }
}