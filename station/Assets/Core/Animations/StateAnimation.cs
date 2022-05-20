//using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Station
{
    public class StateAnimation : BaseAnimation
    {
        
        [SerializeField] public List<StateModel> SimpleStateModels = new List<StateModel>();
        [SerializeField] public bool UseDefaultState;
        [SerializeField] public string DefaultState;

        private Dictionary<string, StateModel> _cacheData;
        private Dictionary<string, List<string>> _cacheAnims;
        private Dictionary<int, string> _layerStatus;
        private string _currentState = String.Empty;
        private Animation _animation;
        private bool _initialized;
        
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
            {
                return;
            }
            _cacheData = new Dictionary<string, StateModel>();
            _cacheAnims = new Dictionary<string, List<string>>();
            _layerStatus = new Dictionary<int, string>();
            _animation = GetComponent<Animation>();
            if (_animation != null)
            {
                _animation.playAutomatically = false;
            }

            if (SimpleStateModels == null)
            {
                SimpleStateModels = new List<StateModel>();
            }

            foreach (StateModel stateModel in SimpleStateModels)
            {
                if (stateModel.StateActionModels.IsEmptyOrNull())
                    continue;

                _cacheData[stateModel.State] = stateModel;
                foreach (StateActionModel actionModel in stateModel.StateActionModels)
                {
                    switch (actionModel.uiAction)
                    {
                        case UiAction.PlayAnimation:
                        if (_animation == null)
                            continue;
                        AnimationClip animationClip = _animation.GetClip(actionModel.animationActionData.Clip);
                        if (animationClip == null)
                        {
                            //Throw error
                            continue;
                        }
                        actionModel.animationActionData.Length = animationClip.length;
                        if (actionModel.animationActionData.Length > stateModel.Duration)
                        {
                            stateModel.Duration = actionModel.animationActionData.Length;
                        }

                        if (!_cacheAnims.ContainsKey(stateModel.State))
                        {
                            _cacheAnims[stateModel.State] = new List<string>();
                        }
                        _cacheAnims[stateModel.State].Add(actionModel.animationActionData.Clip);
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

        public void PlayState(string stateName)
        {
            PlayState(stateName, false,false);
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
                        var previousAnims = _cacheAnims[previousState];
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
                foreach (var actionModel in stateModel.StateActionModels)
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
            switch (actionModel.uiAction)
            {
                case UiAction.PlayAnimation:
                    ExecutePlayAnimation(actionModel);
                    break;
                case UiAction.SetSprite:
                    ExecuteSetSpriteAction(actionModel);
                    break;
                case UiAction.UpdateCanvasGroup:
                    ExecuteUpdateCanvasGroup(actionModel);
                    break;
                case UiAction.UpdateMeshRenderer:
                    ExecuteUpdateMeshRenderer(actionModel);
                    break;
                case UiAction.UpdateCollider:
                    ExecuteUpdateCollider(actionModel);
                    break;
                case UiAction.SwapMaterial:
                    ExecuteSwapMaterial(actionModel);
                    break;
                case UiAction.SwapMaterialList:
                    ExecuteSwapMaterialList(actionModel);
                    break;
                case UiAction.SetActive:
                    ExecuteSetActive(actionModel);
                    break;
                case UiAction.SetTransform:
                    ExecuteSetTransformAction(actionModel);
                    break;
                case UiAction.AnimateColor:
                    ExecuteAnimateColor(actionModel);
                    break;
                case UiAction.TweenSharedColor:
#if !UNITY_EDITOR
                    ExecuteSharedAnimateColor(actionModel);
#else
                    ExecuteAnimateColor(actionModel);
#endif
                    break;
                case UiAction.SetTransformProperty:
                    ExecuteSetPropertyAction(actionModel);
                    break;
                case UiAction.UpdateMultiCanvasGroup:
                    ExecuteUpdateMultiCanvasGroup(actionModel);
                    break;
                case UiAction.UpdateCanvasGroupInteraction:
                    ExecuteCanvasGroupInteraction(actionModel);
                    break;
                case UiAction.StopAnimation:
                    ExecuteStopAnimation(actionModel);
                    break;
                case UiAction.SetText:
                    ExecuteSetText(actionModel);
                    break;
                case UiAction.SetLocalizedKey:
                    ExecuteSetLocalizedKey(actionModel);
                    break;
                case UiAction.SetTextColor:
                    ExecuteSetTextColor(actionModel);
                    break;
                case UiAction.SetImageColor:
                    ExecuteSetImageColor(actionModel);
                    break;
                case UiAction.SetGraphicsRayCasters:
                    ExecuteSetGraphicsRaycaster(actionModel);
                    break;
                case UiAction.SetRectProperty:
                    ExecuteSetRectPropertyAction(actionModel);
                    break;

            }
        }

        private void ExecuteSwapMaterial(StateActionModel actionModel)
        {
            if (actionModel.changeMaterialActionData == null)
                return;
            if (actionModel.changeMaterialActionData.Renderer == null || actionModel.changeMaterialActionData.Material == null)
                return;
            Material[] materials = new Material[actionModel.changeMaterialActionData.Renderer.materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                if (i == actionModel.changeMaterialActionData.Index)
                {
                    materials[i] = actionModel.changeMaterialActionData.Material;
                }
                else
                {
                    materials[i] = actionModel.changeMaterialActionData.Renderer.materials[i];
                }
            }
            actionModel.changeMaterialActionData.Renderer.materials = materials;
        }
        private void ExecuteSwapMaterialList(StateActionModel actionModel)
        {
            if (actionModel.changeMaterialsActionData == null)
                return;
            if (actionModel.changeMaterialsActionData.Renderers == null || actionModel.changeMaterialsActionData.Material == null)
                return;
            foreach (Renderer rndr in actionModel.changeMaterialsActionData.Renderers)
            {
                if (rndr == null)
                {
                    continue;
                }
                Material[] materials = new Material[rndr.materials.Length];
                for (int i = 0; i < materials.Length; i++)
                {
                    if (i == actionModel.changeMaterialsActionData.Index)
                    {
                        materials[i] = actionModel.changeMaterialsActionData.Material;
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
            if (actionModel.colliderActionData == null)
                return;
            if (actionModel.colliderActionData.Collider == null)
                return;
            actionModel.colliderActionData.Collider.enabled =
                actionModel.colliderActionData.Enabled;
        }

        private static void ExecuteUpdateMeshRenderer(StateActionModel actionModel)
        {
            if (actionModel.enableMeshActionData == null)
                return;
            if (actionModel.enableMeshActionData.MeshRenderer == null)
                return;
            actionModel.enableMeshActionData.MeshRenderer.enabled =
                actionModel.enableMeshActionData.Enabled;
        }

        private static void ExecuteUpdateCanvasGroup(StateActionModel actionModel)
        {
            if (actionModel.updateCanvasGroupActionData == null)
                return;
            if (actionModel.updateCanvasGroupActionData.CanvasGroup == null)
                return;
            actionModel.updateCanvasGroupActionData.CanvasGroup.alpha = actionModel.updateCanvasGroupActionData.Alpha;
            actionModel.updateCanvasGroupActionData.CanvasGroup.interactable = actionModel.updateCanvasGroupActionData.Interact;
            actionModel.updateCanvasGroupActionData.CanvasGroup.blocksRaycasts = actionModel.updateCanvasGroupActionData.BlockRayCasts;
        }

        private static void ExecuteSetSpriteAction(StateActionModel actionModel)
        {
            if (actionModel.updateSpriteActionData == null)
                return;
            if (actionModel.updateSpriteActionData.Sprite == null ||
                actionModel.updateSpriteActionData.Image == null)
                return;
            actionModel.updateSpriteActionData.Image.sprite = actionModel.updateSpriteActionData.Sprite;
        }

        private void ExecutePlayAnimation(StateActionModel actionModel)
        {
            if (actionModel.animationActionData?.Clip == null)
                return;
            if (_animation == null)
                return;

            _animation[actionModel.animationActionData.Clip].layer = actionModel.animationActionData.Layer;
            _animation[actionModel.animationActionData.Clip].weight = actionModel.animationActionData.Weight;
            _animation.Play(actionModel.animationActionData.Clip);
        }

        public void AnimationSample()
        {
            _animation.Sample();
        }


        private void ExecuteSetTransformAction(StateActionModel actionModel)
        {
            GameObject target = actionModel.setTransformActionData.Target;
            if (target == null)
            {
                Debug.LogWarning("Target gameObject not assigned in Simple State component in "+gameObject);
                return;
            }
            target.transform.SetPositionAndRotation(actionModel.setTransformActionData.Position, Quaternion.Euler(actionModel.setTransformActionData.Rotation));
            target.transform.localScale = actionModel.setTransformActionData.Scale;
        }

        private void ExecuteSetPropertyAction(StateActionModel actionModel)
        {
            List<Transform> targets = actionModel.setPropertyActionData.Targets;
            if (targets == null)
            {
                Debug.LogWarning("Target transforms are null in Simple State component in {0}", gameObject);
                return;
            }

            foreach (Transform target in targets)
            {
                Vector3 vector = Vector3.zero;
                switch (actionModel.setPropertyActionData.TransformType)
                {
                    case PropertyType.PositionX:
                        vector = target.position;
                        vector.x = actionModel.setPropertyActionData.Value;
                        target.position = vector;
                        break;
                    case PropertyType.PositionY:
                        vector = target.position;
                        vector.y = actionModel.setPropertyActionData.Value;
                        target.position = vector;
                        break;
                    case PropertyType.PositionZ:
                        vector = target.position;
                        vector.z = actionModel.setPropertyActionData.Value;
                        target.position = vector;
                        break;
                    case PropertyType.RotationX:
                        vector = target.rotation.eulerAngles;
                        vector.x = actionModel.setPropertyActionData.Value;
                        target.rotation = Quaternion.Euler(vector);
                        break;
                    case PropertyType.RotationY:
                        vector = target.rotation.eulerAngles;
                        vector.y = actionModel.setPropertyActionData.Value;
                        target.rotation = Quaternion.Euler(vector);
                        break;
                    case PropertyType.RotationZ:
                        vector = target.rotation.eulerAngles;
                        vector.z = actionModel.setPropertyActionData.Value;
                        target.rotation = Quaternion.Euler(vector);
                        break;
                    case PropertyType.ScaleX:
                        vector = target.localScale;
                        vector.x = actionModel.setPropertyActionData.Value;
                        target.localScale = vector;
                        break;
                    case PropertyType.ScaleY:
                        vector = target.localScale;
                        vector.y = actionModel.setPropertyActionData.Value;
                        target.localScale = vector;
                        break;
                    case PropertyType.ScaleZ:
                        vector = target.localScale;
                        vector.z = actionModel.setPropertyActionData.Value;
                        target.localScale = vector;
                        break;
                }
            }
            
        }

        private void ExecuteSetRectPropertyAction(StateActionModel actionModel)
        {
            List<RectTransform> targets = actionModel.setRectPropertyActionData.Targets;
            if (targets == null)
            {
                Debug.LogWarning("Target transforms are null in Simple State component in {0}", gameObject);
                return;
            }

            foreach (RectTransform target in targets)
            {
                Vector3 vector = Vector3.zero;
                switch (actionModel.setRectPropertyActionData.TransformType)
                {
                    case PropertyType.PositionX:
                        vector = target.anchoredPosition;
                        vector.x = actionModel.setRectPropertyActionData.Value;
                        target.anchoredPosition = vector;
                        break;
                    case PropertyType.PositionY:
                        vector = target.anchoredPosition;
                        vector.y = actionModel.setRectPropertyActionData.Value;
                        target.anchoredPosition = vector;
                        break;
                    case PropertyType.PositionZ:
                        vector = target.anchoredPosition;
                        vector.z = actionModel.setRectPropertyActionData.Value;
                        target.anchoredPosition = vector;
                        break;
                    case PropertyType.RotationX:
                        vector = target.rotation.eulerAngles;
                        vector.x = actionModel.setRectPropertyActionData.Value;
                        target.rotation = Quaternion.Euler(vector);
                        break;
                    case PropertyType.RotationY:
                        vector = target.rotation.eulerAngles;
                        vector.y = actionModel.setRectPropertyActionData.Value;
                        target.rotation = Quaternion.Euler(vector);
                        break;
                    case PropertyType.RotationZ:
                        vector = target.rotation.eulerAngles;
                        vector.z = actionModel.setRectPropertyActionData.Value;
                        target.rotation = Quaternion.Euler(vector);
                        break;
                    case PropertyType.ScaleX:
                        vector = target.localScale;
                        vector.x = actionModel.setRectPropertyActionData.Value;
                        target.localScale = vector;
                        break;
                    case PropertyType.ScaleY:
                        vector = target.localScale;
                        vector.y = actionModel.setRectPropertyActionData.Value;
                        target.localScale = vector;
                        break;
                    case PropertyType.ScaleZ:
                        vector = target.localScale;
                        vector.z = actionModel.setRectPropertyActionData.Value;
                        target.localScale = vector;
                        break;
                }
            }

        }
        private void ExecuteSetActive(StateActionModel actionModel)
        {
            GameObject target = actionModel.setActiveActionData.Target;
            if (target == null)
            {
                Debug.LogWarning("Your Target game object is missing in Simple State Component in {0}", gameObject);
                return;
            }

            target.SetActive(actionModel.setActiveActionData.Active);
        }

        private void ExecuteAnimateColor(StateActionModel actionModel)
        {
            List<Renderer> target = actionModel.tweenColorActionData.Target;
            Color targetColor = actionModel.tweenColorActionData.TargetColor;
            float duration = actionModel.tweenColorActionData.Duration;
            int id = actionModel.tweenColorActionData.MaterialId;
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
            List<Renderer> target = actionModel.tweenSharedColorActionData.Target;
            Color targetColor = actionModel.tweenSharedColorActionData.TargetColor;
            float duration = actionModel.tweenSharedColorActionData.Duration;
            int id = actionModel.tweenSharedColorActionData.MaterialId;
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
            if (actionModel.setCanvasGroupStateActionData == null)
                return;
            if (actionModel.setCanvasGroupStateActionData.CanvasGroups.IsEmptyOrNull())
                return;
            foreach (CanvasGroup canvasGroup in actionModel.setCanvasGroupStateActionData.CanvasGroups)
            {
                if (canvasGroup == null)
                    continue;
                canvasGroup.alpha = actionModel.setCanvasGroupStateActionData.Alpha;
                canvasGroup.interactable = actionModel.setCanvasGroupStateActionData.Interact;
                canvasGroup.blocksRaycasts = actionModel.setCanvasGroupStateActionData.BlockRayCasts;
            }
        }
        
        private static void ExecuteCanvasGroupInteraction(StateActionModel actionModel)
        {
            if (actionModel.setCanvasInteractionActionData == null)
                return;
            if (actionModel.setCanvasInteractionActionData.CanvasGroup == null)
                return;
            actionModel.setCanvasInteractionActionData.CanvasGroup.interactable =
                actionModel.setCanvasInteractionActionData.Interact;
        }


        private void ExecuteStopAnimation(StateActionModel actionModel)
        {
            _animation.Stop(actionModel.stopAnimationActionData.Clip);
        }
        
        private void ExecuteSetText(StateActionModel actionModel)
        {
            UiText textComponent = actionModel.setTextActionData.TextComponent;
            if (textComponent != null)
            {
                textComponent.SetText(actionModel.setTextActionData.Text);
            }
        }

        private void ExecuteSetTextColor(StateActionModel actionModel)
        {
            UiText textComponent = actionModel.updateTextColorActionData.TextComponent;
            if (textComponent != null)
            {
                textComponent.color = actionModel.updateTextColorActionData.Color;
            }
        }

        private void ExecuteSetLocalizedKey(StateActionModel actionModel)
        {
            UiText textComponent = actionModel.updateLocalizationActionData.TextComponent;
            if (textComponent != null)
            {
                textComponent.LocalizedKey = actionModel.updateLocalizationActionData.Key;
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
            if (actionModel.setGraphicsRayCasterActionData == null)
                return;
            if (actionModel.setGraphicsRayCasterActionData.RayCasters.IsEmptyOrNull())
                return;

            foreach (GraphicRaycaster graphicRaycaster in actionModel.setGraphicsRayCasterActionData.RayCasters)
            {
                graphicRaycaster.enabled = actionModel.setGraphicsRayCasterActionData.IsEnabled;
            }
        }
#endregion

        public string CurrentState(int layer = -1)
        {
            return _currentState;
        }
        public override float GetStateDuration(string stateName)
        {
            if (_cacheData == null)
            {
                Initialize();
            }
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
        public UiAction uiAction;
        public AnimationActionData animationActionData;
        public UpdateSpriteActionData updateSpriteActionData;
        public UpdateCanvasGroupActionData updateCanvasGroupActionData;
        public EnableMeshActionData enableMeshActionData;
        public ColliderActionData colliderActionData;
        public ChangeMaterialActionData changeMaterialActionData;
        public ChangeMaterialsActionData changeMaterialsActionData;
        public SetActiveActionData setActiveActionData;
        public SetTransformActionData setTransformActionData;
        public TweenColorActionData tweenColorActionData;
        public TweenSharedColorActionData tweenSharedColorActionData;
        public SetPropertyActionData setPropertyActionData;
        public SetCanvasGroupStateActionData setCanvasGroupStateActionData;
        public SetCanvasInteractionActionData setCanvasInteractionActionData;
        public StopAnimationActionData stopAnimationActionData;
        public SetTextActionData setTextActionData;
        public UpdateLocalizationActionData updateLocalizationActionData;
        public UpdateTextColorActionData updateTextColorActionData;
        public SetImageColorActionModel SetImageColorActionModel;
        public SetGraphicsRayCasterActionData setGraphicsRayCasterActionData;
        public SetRectPropertyActionData setRectPropertyActionData;
    }

    [Serializable]
    public class AnimationActionData
    {
        public string Clip;
        public float Weight = 0.5f;
        public int Layer;
        [NonSerialized]
        public float Length;
    }

    [Serializable]
    public class UpdateSpriteActionData
    {
        public Image Image;
        public Sprite Sprite;
    }

    [Serializable]
    public class UpdateCanvasGroupActionData
    {
        public CanvasGroup CanvasGroup;
        public float Alpha;
        public bool Interact;
        public bool BlockRayCasts;
    }

    [Serializable]
    public class EnableMeshActionData
    {
        public MeshRenderer MeshRenderer;
        public bool Enabled;
    }

    [Serializable]
    public class ColliderActionData
    {
        public Collider Collider;
        public bool Enabled;
    }

    [Serializable]
    public class ChangeMaterialActionData
    {
        public Renderer Renderer;
        public int Index;
        public Material Material;
    }
    
    [Serializable] 
    public class ChangeMaterialsActionData
    {
        public List<Renderer> Renderers;
        public int Index;
        public Material Material;
    }

    [Serializable]
    public class SetActiveActionData
    {
        public GameObject Target;
        public bool Active;
    }

    [Serializable]
    public class TweenColorActionData
    {
        public List<Renderer> Target;
        public Color TargetColor;
        public int MaterialId;
        public float Duration;
    }
    [Serializable]
    public class TweenSharedColorActionData
    {
        public List<Renderer> Target;
        public Color TargetColor;
        public int MaterialId;
        public float Duration;
    }

    [Serializable]
    public class SetTransformActionData
    {
        public GameObject Target;
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale = Vector3.one;
    }


    [Serializable]
    public class SetPropertyActionData
    {
        public List<Transform> Targets;
        public PropertyType TransformType;
        public float Value;
    }

    [Serializable]
    public class SetRectPropertyActionData
    {
        public List<RectTransform> Targets;
        public PropertyType TransformType;
        public float Value;
    }

    [Serializable]
    public class SetCanvasGroupStateActionData
    {
        public List<CanvasGroup> CanvasGroups;
        public float Alpha;
        public bool Interact;
        public bool BlockRayCasts;
    }
    
    [Serializable]
    public class SetCanvasInteractionActionData
    {
        public CanvasGroup CanvasGroup;
        public bool Interact;
    }

    [Serializable]
    public class StopAnimationActionData
    {
        public string Clip;
    }

    [Serializable]
    public class SetGraphicsRayCasterActionData
    {
        public List<GraphicRaycaster> RayCasters;
        public bool IsEnabled;
    }

    // UI ACTIONS
    [Serializable]
    public class SetTextActionData
    {
        public UiText TextComponent;
        public string Text;
    }

    [Serializable]
    public class UpdateLocalizationActionData
    {
        public UiText TextComponent;
        public string Key;
    }

    [Serializable]
    public class UpdateTextColorActionData
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
    public enum UiAction
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
        UpdateMultiCanvasGroup,
        StopAnimation,
        SetText,
        SetLocalizedKey,
        SetTextColor,
        SetImageColor,
        SetGraphicsRayCasters,
        SetRectProperty,
        SwapMaterialList,
        TweenSharedColor,
        UpdateCanvasGroupInteraction
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