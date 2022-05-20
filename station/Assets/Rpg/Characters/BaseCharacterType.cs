using System;
using UnityEngine;

namespace Station
{
    public class DefaultPlayerBuilder: CharacterBuilder{}
    public abstract class CharacterBuilder: ScriptableObject
    {
        public virtual Type GetMatchingType()
        {
            return GetType();
        }

        public virtual void Build(BaseCharacter character,BaseCharacterData baseData, object[] data)
        {
        }
    }

    public abstract class BaseCharacterType
    {

    }

    public class PlayerCharacterType: BaseCharacterType
    {
    }

    public class NpcCharacterType : BaseCharacterType
    {
    }
    
    public class PetCharacterType : BaseCharacterType
    {
    }
}

