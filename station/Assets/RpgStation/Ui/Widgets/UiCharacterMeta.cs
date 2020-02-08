using UnityEngine;

namespace Station
{
    public class UiCharacterMeta : UiWidget
    {
        [SerializeField] private string _meta;

        public void Init(BaseCharacter character)
        {
            var metaValue = character.GetMeta(_meta);
            var data = new WidgetData {VisualInfo = _meta, VisualValue = metaValue};
            Setup(data);
        }
    }
}

