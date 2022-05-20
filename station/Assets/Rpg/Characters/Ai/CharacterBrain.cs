using UnityEngine;

namespace Station
{
    public abstract class CharacterBrain : MonoBehaviour
    {
        public abstract void Setup(BaseCharacter owner);
        public abstract void TickBrain();
    }
}
