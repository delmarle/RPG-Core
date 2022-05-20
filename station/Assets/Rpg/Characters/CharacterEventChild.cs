using UnityEngine;

namespace Station
{
    public class CharacterEventChild : MonoBehaviour
    {
        protected BaseCharacter Character;
    
        private void Awake()
        {
            Character = GetComponentInParent<BaseCharacter>();
            Subscribe();
        }

        private void OnDestroy()
        {
            UnSubscribe();
        }

        private void Subscribe()
        {
            if (Character == null) return;
        
            Character.OnDamaged += OnDamaged;
            Character.OnHealed += OnHealed;
        
        }

        private void UnSubscribe()
        {
            if (Character == null) return;
        
            Character.OnDamaged -= OnDamaged;
            Character.OnHealed -= OnHealed;
        }

        private void OnHealed(BaseCharacter character, VitalChangeData data)
        {
        
        }

        private void OnDamaged(BaseCharacter character, VitalChangeData data)
        {
        
        }
    }
}

