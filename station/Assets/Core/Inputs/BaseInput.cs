
using UnityEngine;

namespace Station
{
    public class BaseInput : MonoBehaviour
    {
        public void Activate(CoreCharacter character)
        {
            
            OnActive(character);
        }

        public void Desactivate(CoreCharacter character)
        {
            OnDeactivate(character);
        }

        public virtual Vector3 Movement()
        {
            return Vector3.zero;
        }

        public virtual Quaternion Rotation()
        {
            return Quaternion.identity;
        }

        public virtual int MovementType()
        {
            return 0;
        }

        public virtual bool Jump()
        {
            return false;
        }

        protected virtual void OnActive(CoreCharacter character)
        {
        }

        protected virtual void OnDeactivate(CoreCharacter character)
        {
        }
    }
}