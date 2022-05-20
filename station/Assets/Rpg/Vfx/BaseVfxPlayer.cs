using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public abstract class BaseVfxPlayer : PooledItem
    {
        public abstract void PlayEffect(BaseCharacter owner, BaseCharacter target, Transform root);

        public void Despawn()
        {
            PoolSystem.Despawn(gameObject);
        }
    }
    
    
}

