using UnityEngine;

namespace Station
{
    public class UiContainerWidget : MonoBehaviour
    {
        private ICharacterInventoryHandler _handler;
        private BaseCharacter _character;
        private ItemContainer _container;

        public void Init(ICharacterInventoryHandler handler, BaseCharacter character)
        {
            _handler = handler;
            _character = character;
            _container = _handler.GetContainer(_character.ToString());
            //_container.GetState().Slots
        }
    }
}

