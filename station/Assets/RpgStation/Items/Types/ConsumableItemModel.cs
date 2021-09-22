
 
using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Station
{
    [Serializable]
    public class ConsumableItemModel : BaseItemModel
    {
        #region USE DATA

        public bool ForceEffectSelf = true;
        public EffectHolder Effects = new EffectHolder();
        #endregion
        public override void OnUse(BaseCharacter user)
        {
            BaseCharacter redefinedTarget = null;
            if (ForceEffectSelf)
            {
                redefinedTarget = user;
            }
            else
            {
                redefinedTarget = user.Target;
            }

            if (redefinedTarget)
            {
                Effects.ApplyEffects(user, redefinedTarget);
            }
            else
            {
                var configDb = GameInstance.GetDb<NotificationConfigsDb>();
                var channels = configDb.Get().UseItemFail;
                if (channels.Any())
                {
                    var dict = new Dictionary<string, object> {{UiConstants.TEXT_MESSAGE, $"the item cannot be used without target: {Name.GetValue()}"}};
                    UiNotificationSystem.ShowNotification(channels, dict);
                }
                
            }
            
        }
    }
}

