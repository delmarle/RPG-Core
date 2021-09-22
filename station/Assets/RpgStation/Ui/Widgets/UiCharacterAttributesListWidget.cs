
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiCharacterAttributesListWidget : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private LayoutGroup _layout = null;
        [SerializeField] private UiWidget _widgetPrefab = null;

        private AttributesDb _attributesDb;
        private GenericUiList<WidgetData, UiWidget> _attributesList;

        #endregion

        public void Setup()
        {
            if (_attributesDb == null)
            {
                _attributesDb = GameInstance.GetDb<AttributesDb>();
            }
            
            if (_attributesList == null)
            {
                _attributesList = new GenericUiList<WidgetData, UiWidget>(_widgetPrefab.gameObject, _layout);
            }
        }


        public void UpdateAttributes(BaseCharacter character)
        {
            if(character == null) { return; }
            Setup();
            var stats = character.Stats;
            List<WidgetData> attrList = new List<WidgetData>();
            foreach (var attr in stats.Attributes)
            {
                WidgetData widget = new WidgetData();
                var staticData = _attributesDb.GetEntry(attr.Key);
                widget.VisualInfo = staticData.Name+": ";
                widget.VisualValue =  attr.Value.MaximumValue.ToString();
                widget.IconColor = Color.white;
                widget.Icon = staticData.Icon;
       
                attrList.Add(widget);
            }
            _attributesList.Generate(attrList, (entry, uiObject) =>
            {
                uiObject.Setup(entry);
            });

        }
    }
}

