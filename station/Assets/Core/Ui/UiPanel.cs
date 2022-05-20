using UnityEngine;

namespace Station
{
    public class UiPanel : UiElementAnim
    {
        [Header("Show when close previous")] public bool DefaultPanel = false;
        protected override void Start()
        {
            base.Start();
            UiSystem.RegisterPanel(this);
            if(DefaultPanel) UiSystem.RegisterDefaultPanel(this);
        }
        
        protected virtual void OnDestroy()
        {
            UiSystem.UnRegisterPanel(this);
        }

      
    }

    public abstract class UiPanel<T> : UiPanel
    {
        public virtual void ShowPanel(T data)
        {
        }
    }
}

