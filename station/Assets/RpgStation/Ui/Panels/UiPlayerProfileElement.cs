using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiPlayerProfileElement : UiPanel
    {
         #region [[ FIELDS ]]
    private BaseCharacter _character;
    [SerializeField] private UiCharacterMeta _playerName = null;
    [SerializeField] private UiCharacterMeta _playerLevel = null;
    [SerializeField] private LayoutGroup _attributesLayout = null;
    [SerializeField] private UiWidget _attributesPrefab = null;
    [SerializeField] private LayoutGroup _vitalsLayout = null;
    [SerializeField] private UiWidget _vitalsPrefab = null;
    [SerializeField] private LayoutGroup _statisticsLayout = null;
    [SerializeField] private UiWidget _statisticsPrefab = null;

    private GenericUiList<WidgetData> _attributes;
    private GenericUiList<WidgetData> _vitals;
    private GenericUiList<WidgetData> _statistics;
    private AttributesDb _attributesDb;
    private StatisticDb _statisticsDb;
    private VitalsDb _vitalsDb;
    #endregion
    
    #region [[ MONOBEHAVIOURS ]]

    protected override void Awake()
    {
      base.Awake();
      var dbSystem = RpgStation.GetSystemStatic<DbSystem>();
      _attributesDb = dbSystem.GetDb<AttributesDb>();
      _statisticsDb = dbSystem.GetDb<StatisticDb>();
      _vitalsDb = dbSystem.GetDb<VitalsDb>();
      TeamSystem.OnLeaderChanged.AddListener(FollowCharacter);
      if (_attributes == null)
      {
        _attributes = new GenericUiList<WidgetData>(_attributesPrefab.gameObject, _attributesLayout);
      }

      if (_vitals == null)
      {
        _vitals = new GenericUiList<WidgetData>(_vitalsPrefab.gameObject, _vitalsLayout);
      }
      
      if (_statistics == null)
      {
        _statistics = new GenericUiList<WidgetData>(_statisticsPrefab.gameObject, _statisticsLayout);
      }
    }
    
    private void OnDestroy()
    {
      TeamSystem.OnLeaderChanged.RemoveListener(FollowCharacter);
      UnFollowCharacter();
    }
    #endregion

    public void ClosePanel()
    {
      PanelSystem.HidePanel<UiPlayerProfileElement>(true);
    }

    private void UnFollowCharacter()
    {
      if (_character)
      {
        _character.OnVitalsUpdated -= OnVitalsUpdated;
        _character.OnStatisticUpdated -= OnStatisticsUpdated;
        _character.OnAttributesUpdated -= OnAttributesUpdated;
      }
    }

    private void FollowCharacter(BaseCharacter character)
    {
      UnFollowCharacter();

      _character = character;
      character.OnVitalsUpdated += OnVitalsUpdated;
      character.OnStatisticUpdated += OnStatisticsUpdated;
      character.OnAttributesUpdated += OnAttributesUpdated;
      UpdateUi();
    }

    private void OnAttributesUpdated(BaseCharacter character)
    {
      UpdateAttributes();
    }

    private void OnVitalsUpdated(BaseCharacter character)
    {
      UpdateVitals();
    }

    private void OnStatisticsUpdated(BaseCharacter character)
    {
      UpdateStatistics();
    }

    private void UpdateAttributes()
    {
      if(_character == null) { return; }
  
      var stats = _character.Stats;
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
      _attributes.Generate<UiWidget>(attrList, (entry, price) =>
      {
        price.Setup(entry);
      });
    }

    private void UpdateVitals()
    {
      if(_character == null) { return; }
  
      var stats = _character.Stats;
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
      
      _vitals.Generate<UiWidget>(vitalList, (entry, price) =>
      {
        price.Setup(entry);
      });
    }
    
    private void UpdateStatistics()
    {
      if(_character == null) { return; }

      var stats = _character.Stats;
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
      _statistics.Generate<UiWidget>(statsList, (entry, uiWidget) =>
      {
        uiWidget.Setup(entry);
      });
    }

    private void UpdateMeta()
    {
      _playerName.Init(_character);
      _playerLevel.Init(_character);
    }

    public void UpdateUi()
    {
      UpdateMeta();
      UpdateAttributes();
      UpdateStatistics();
      UpdateVitals();
    } 
    }

    public class UiCharacterMetaReader
    {
        [SerializeField] private  string _metaName = null;
        [SerializeField] private TextMeshProUGUI _metaText = null;

        public void AssignMeta(BaseCharacter character)
        {
            var value = character.GetMeta(_metaName);
            _metaText.text = value;
        }
    }
}
