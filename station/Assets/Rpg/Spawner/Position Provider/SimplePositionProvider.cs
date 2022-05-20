using UnityEngine;

namespace Station
{
    
    public class SimplePositionProvider : PositionProvider
    {
        public override Vector3 GetPosition()
        {
            return transform.position;
        }

        public override Vector3 GetRotation()
        {
            return transform.rotation.eulerAngles;
        }
    }

}
