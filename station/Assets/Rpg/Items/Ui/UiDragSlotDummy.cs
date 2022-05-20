using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiDragSlotDummy : MonoBehaviour
    {
        [SerializeField] private Image _icon = null;

        private void Awake()
        {
            Reset();
        }

        public void Initialize(Sprite icon)
        {
            _icon.raycastTarget = false;
            _icon.sprite = icon;
            gameObject.SetActive(true);
        }

        public void SetPosition(Vector2 pos)
        {
            transform.position = pos;
        }

        public void Reset()
        {
            //hide
            gameObject.SetActive(false);
        }


    }

}

