
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiCharacterVitalsListWidget : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private LayoutGroup _layout = null;
        [SerializeField] private UiWidget _vitalPrefab = null;

        private VitalsDb _vitalsDb;
        private GenericUiList<WidgetData, UiWidget> _vitalsList;

        #endregion

        public void Setup()
        {
            if (_vitalsDb == null)
            {
                _vitalsDb = GameInstance.GetDb<VitalsDb>();
            }
           
            if (_vitalsList == null)
            {
                _vitalsList = new GenericUiList<WidgetData, UiWidget>(_vitalPrefab.gameObject, _layout);
            }
        }


        public void UpdateVitals(BaseCharacter character)
        {
            if (character == null)
            {
                return;
            }

            Setup();

            var stats = character.Stats;
            List<WidgetData> vitalList = new List<WidgetData>();

            foreach (var energy in stats.Vitals)
            {
                WidgetData energyWidget = new WidgetData();
                var staticData = _vitalsDb.GetEntry(energy.Key);
                energyWidget.VisualInfo = staticData.Name + ": ";
                energyWidget.VisualValue = energy.Value.Current + "/" + energy.Value.MaximumValue;
                energyWidget.Icon = staticData.Icon;
                energyWidget.IconColor = staticData.Color;
                vitalList.Add(energyWidget);
            }

            _vitalsList.Generate(vitalList, (entry, price) => { price.Setup(entry); });
        }
    }
}

