
using UnityEngine;

namespace Station
{
    public class AttachedVfxPlayer : BaseVfxPlayer
    {
        public AttachType Target;
        public bool Parented;
        public string Limb = "left_hand";
        public int PoolSize = 10;
        
        public override void PlayEffect(BaseCharacter owner, BaseCharacter target, Transform root)
        {
            switch (Target)
            {
                case AttachType.Owner:
                    AttachOnCharacter(owner);
                    break;
                case AttachType.Target:
                    AttachOnCharacter(target);
                    break;
                case AttachType.Root:
                    break;
            }
        }

        private void AttachOnCharacter(BaseCharacter owner)
        {
            var limb = owner.GetLimbs(Limb);
            transform.SetPositionAndRotation(limb.transform.position, Quaternion.identity);
            if (Parented)
            {
                transform.SetParent(limb);
            }

        }

        public enum AttachType
        {
            Owner, Target, Root
        }
    }
}

