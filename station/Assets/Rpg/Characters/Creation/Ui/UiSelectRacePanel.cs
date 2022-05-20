using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Station;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiSelectRacePanel : UiPanel
    {
        #region FIELDS
       
        
        [SerializeField] private LayoutGroup _layout;
        [SerializeField] private UiRaceWidget _widgetPrefab;
        [SerializeField] private UiRaceWidget _selectedRace;
        //GENDER
        [SerializeField] private UiSelectableButtonList _genderSelection;

        
        private GenericUiList<RaceModel, UiRaceWidget> _widgetList;

        private Dictionary<RaceModel,int> _raceDicts = new Dictionary<RaceModel, int>();
        private StationAction _completeCallBack;
        private StationAction<int> _onSelectRace;

        #endregion
        
        protected override void Awake()
        {
            base.Awake();
            if (_widgetList == null)
            {
                _widgetList = new GenericUiList<RaceModel, UiRaceWidget>(_widgetPrefab.gameObject, _layout);
            }
        }
        
       public void Setup(List<RaceModel> raceModels, StationAction<int> onSelectRace,List<GenderModel> genderModels,StationAction<int> onSelectGender, StationAction onCompleteStep)
        {
            if (_widgetList == null)
            {
                Awake();
            }

            _onSelectRace = onSelectRace;
            _completeCallBack = onCompleteStep;
            _raceDicts.Clear();
            for (int i = 0; i < raceModels.Count; i++)
            {
                
                _raceDicts.Add(raceModels[i], i);
            }
            
            
            var first = raceModels.FirstOrDefault();
            _widgetList.Generate(raceModels, (entry, item) =>
            {
                item.SetupRaceData(entry, InvokeSelectRace);
            });

            _genderSelection.Setup(genderModels.Count(), onSelectGender);
            for (int i = 0; i < genderModels.Count; i++)
            {
                var entry = genderModels[i];
                _genderSelection.SetEntryVisualData(i, entry.Name.GetValue(), entry.Icon);
            }
            InvokeSelectRace(first);
        }

  

        private void InvokeSelectRace(RaceModel selection)
        {
            var widgets = _widgetList.GetEntries();
            foreach (var widget in widgets)
            {
                bool isSelected = widget.GetModel() == selection;
                if (isSelected)
                {
                    widget.SetSelectedState();
                }
                else
                {
                    widget.SetNormalState();
                }
            }
            _selectedRace.SetupRaceData(selection);
            int raceIndex = _raceDicts[selection];
            _onSelectRace.Invoke(raceIndex);
        }



        public void OnClickComplete()
        {
            _completeCallBack?.Invoke();
        }
      
    }

  
}
