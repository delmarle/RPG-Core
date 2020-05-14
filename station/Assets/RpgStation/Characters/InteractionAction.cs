
namespace Station
{
    public class InteractionAction : CharacterAction
    {
        private Interactible _interactible = null;
   

      public void TryInteract(Interactible targetObject)
      {
        _interactible = targetObject;
        if (CanUse())
        {
            StartCasting();
        }
      }

        #region OVERRIDES
        
        public override float CalculateActionLength()
        {
            if (_interactible == null)
            {
                return 0;
            }

            return _interactible.Config.InteractionTime;
        }

        public override bool CanUse()
        {
            if (_interactible == null)
            {
                return false;
            }
            
            if (_interactible.CanUse(_user) == false)
            {
                return false;
            }

            return base.CanUse();
        }

        protected override void OnInvokeEffect()
        {
            _interactible.Interact(_user);
        }

        public override CastingData CastingData
        {
            get
            {
                if (_interactible == null)
                {
                    return null;
                }

                return _interactible.Config.CastingData;
            }
        }
        #endregion
        
     
    }
}