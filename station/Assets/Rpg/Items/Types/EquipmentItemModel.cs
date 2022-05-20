 
using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Station
{
    [Serializable]
    public class EquipmentItemModel : BaseItemModel
    {
        #region FIELDS
        public string EquipmentType;
        public List<IdIntegerValue> VitalBonuses = new List<IdIntegerValue>();
        public List<IdIntegerValue> AttributesBonuses = new List<IdIntegerValue>();
        public List<IdFloatValue> StatisticsBonuses = new List<IdFloatValue>();

        #endregion
        
        public override void OnUse(BaseCharacter user)
        {
           
        }
    }
}

