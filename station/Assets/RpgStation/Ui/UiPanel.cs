namespace Station
{
    public class UiPanel : UiElementAnim
    {
        protected override void Start()
        {
            base.Start();

            PanelSystem.RegisterPanel(this);
            if(DefaultPanel) PanelSystem.RegisterDefaultPanel(this);
        }
        
        private void OnDestroy()
        {
            PanelSystem.UnRegisterPanel(this);
        }

      
    }

    public abstract class UiPanel<T> : UiPanel
    {
        public virtual void ShowPanel(T data)
        {
        }
    }
}

