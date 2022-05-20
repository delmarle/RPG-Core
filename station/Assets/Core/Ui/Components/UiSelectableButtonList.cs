using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiSelectableButtonList : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private UiButton _entryPrefab;
        [SerializeField] private LayoutGroup _layoutGroup;


        private List<UiButton> _entriesList = new List<UiButton>();
        private StationAction<int> _onSelectCallback;

        #endregion

        public void Setup(int size, StationAction<int> callback, int indexSelected = 0)
        {
            _onSelectCallback = callback;
            for (int i = 0; i < size; i++)
            {
                if (_entriesList.Count <= i)
                {
                    var instance = Object.Instantiate(_entryPrefab, _layoutGroup.transform, false);
                    _entriesList.Add(instance);
                    instance.SetIndex(i);
                    instance.SetCallBack(OnButtonClick);
                    instance.gameObject.SetActive(true);
                }
            }

            SetSelected(indexSelected);
        }

        private void OnButtonClick(int index)
        {
            SetSelected(index);
        }

        private void SetSelected(int selectedIndex)
        {
            for (int i = 0; i < _entriesList.Count; i++)
            {
                var entry = _entriesList[i];
                if (selectedIndex == i)
                {
                    entry.SetStateSelected();
                }
                else
                {
                    entry.SetStateUnSelected();
                }
            }
            _onSelectCallback?.Invoke(selectedIndex);
        }
        public void SetEntryVisualData(int index, string entryName, Sprite sprite = null, bool locked = false)
        {
            int count = _entriesList.Count;
            if (count >= index)
            {
                var widget = _entriesList[index];
                widget.SetName(entryName);
                if (sprite != null)
                {
                    widget.SetIcon(sprite);
                }
                widget.SetLocked(locked);
            }
            else
            {
                
            }
        }

     
    }
}

