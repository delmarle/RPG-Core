using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class UiSelectClassPanel : UiPanel
    {
        #region FIELDS

        [SerializeField] private UiClassWidget _classWidget;
        [SerializeField] private UiSelectableButtonList _buttonList;
        
        private StationAction<int> _selectionCallback;
        private StationAction _completeStepCallback;
        private List<string> _classIds;
        private PlayerClassDb _classDb;
        #endregion

        protected override void Awake()
        {
            base.Awake();
            _classDb = GameInstance.GetDb<PlayerClassDb>();
        }

        public void Setup(List<string> classIds, string optionalRaceRestriction, StationAction<int> selectCallback, StationAction completeStepAction)
        {
            _selectionCallback = selectCallback;
            _classIds = classIds;
            _buttonList.Setup(_classIds.Count, OnSelectClass,0);

            for (int i = 0; i < classIds.Count; i++)
            {
                var classId = classIds[i];
                var classMeta = _classDb.GetEntry(classId);
                bool isAllowed = classMeta.IsRaceAllowed(optionalRaceRestriction);
                _buttonList.SetEntryVisualData(i, classMeta.Name.GetValue(),classMeta.Icon, !isAllowed);
           
            }

            _completeStepCallback = completeStepAction;
        }

        private void OnSelectClass(int classIndex)
        {
            _selectionCallback?.Invoke(classIndex);
            var classId = _classIds[classIndex];
            var classMeta = _classDb.GetEntry(classId);
            _classWidget.SetupData(classMeta);
        }

        public void OnClickNext()
        {
            _completeStepCallback?.Invoke();
        }
    }
}
