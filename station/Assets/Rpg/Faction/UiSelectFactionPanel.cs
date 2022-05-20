using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiSelectFactionPanel : UiPanel
    {
        #region FIELDS
        [SerializeField] private LayoutGroup _layout;
        [SerializeField] private UiFactionWidget _widgetPrefab;

        private GenericUiList<FactionModel, UiFactionWidget> _widgetList;
        private StationAction<FactionModel> _callback;
        #endregion

        protected override void Awake()
        {
            if (_widgetList == null)
            {
                _widgetList = new GenericUiList<FactionModel, UiFactionWidget>(_widgetPrefab.gameObject, _layout);
            }
        }

        public void Setup(IEnumerable<FactionModel> models, StationAction<FactionModel> onSelect)
        {
            if (_widgetList == null)
            {
                Awake();
            }
            _callback = onSelect;
            var first = models.FirstOrDefault();
            _widgetList.Generate(models, (entry, item) =>
            {
                item.SetupFactionData(entry, InvokeSelect);
            });
            
            InvokeSelect(first);
        }

        private void InvokeSelect(FactionModel selection)
        {
            var widgets = _widgetList.GetEntries();
            foreach (var widget in widgets)
            {
                bool isSelected = widget.GetFactionModel() == selection;
                if (isSelected)
                {
                    widget.SetSelectedState();
                }
                else
                {
                    widget.SetNormalState();
                }
            }
            
            _callback.Invoke(selection);
        }
    }

}
