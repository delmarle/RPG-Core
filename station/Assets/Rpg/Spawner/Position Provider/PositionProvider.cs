using UnityEngine;

namespace Station
{
    
    public class PositionProvider : MonoBehaviour
    {
        public virtual void Generate()
        {
            
        }

        public virtual Vector3 GetPosition()
        {
            return Vector3.zero;
        }

        public virtual Vector3 GetRotation()
        {
            return Vector3.zero;
        }
    }

}

