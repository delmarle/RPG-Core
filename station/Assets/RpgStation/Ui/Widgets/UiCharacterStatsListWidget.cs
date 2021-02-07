
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiCharacterStatsListWidget : MonoBehaviour
    {
#region FIELSS
[SerializeField] private LayoutGroup _statisticsLayout = null;
[SerializeField] private UiWidget _statisticsPrefab = null;

private StatisticDb _statisticsDb;
private GenericUiList<WidgetData, UiWidget> _statistics;
#endregion

public void Setup()
{
    if (_statisticsDb == null)
    {
        _statisticsDb = RpgStation.GetDb<StatisticDb>();
    }
   
    if (_statistics == null)
    {
        _statistics = new GenericUiList<WidgetData, UiWidget>(_statisticsPrefab.gameObject, _statisticsLayout);
    }
}


        public void UpdateStatistics(BaseCharacter character)
        {
            if(character == null) { return; }

            Setup();

            var stats = character.Stats;
            List<WidgetData> statsList = new List<WidgetData>();
            foreach (var attr in stats.Statistics)
            {
                WidgetData widget = new WidgetData();
                var staticData = _statisticsDb.GetEntry(attr.Key);
                widget.VisualInfo = staticData.Name + ": ";
                widget.VisualValue = attr.Value.MaximumValue.ToString();
                widget.IconColor = Color.white;
                widget.Icon = staticData.Icon;
       
                statsList.Add(widget);
            }
            _statistics.Generate(statsList, (entry, uiWidget) =>
            {
                uiWidget.Setup(entry);
            });
        }
    } 
}

