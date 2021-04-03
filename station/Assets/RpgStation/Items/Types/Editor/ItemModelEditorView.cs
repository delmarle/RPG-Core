
using System;
using System.Collections.Generic;

namespace Station
{
    public static class ItemModelEditorView
    {
        private static Dictionary<Type, ItemEditorViewer> _itemActionMap = new Dictionary<Type, ItemEditorViewer>();

        public static void Register()
        {
            var listFound = ReflectionUtils.FindAllClassFromInterface(typeof(ItemEditorViewer));

            foreach (var typeFound in listFound)
            {
                var instanceView = (ItemEditorViewer)Activator.CreateInstance(typeFound);
            
                if (_itemActionMap.ContainsKey(instanceView.TypeId()))
                {
                    _itemActionMap[instanceView.TypeId()] = instanceView;
                }
                else
                {
                    _itemActionMap.Add(instanceView.TypeId(), instanceView);
                }
            }
        }

        public static void DrawSpecificView(BaseItemModel model)
        {
            var target = model.GetType();
            if (_itemActionMap.ContainsKey(target))
            {
                var instance = _itemActionMap[target];
                instance?.DrawView(model);
            }
        }
    }

    public interface ItemEditorViewer
    {
        Type TypeId();
        void DrawView(BaseItemModel model);
    }

    public class ConsumableItemEditor: ItemEditorViewer
    {
        public Type TypeId()
        {
            return typeof(ConsumableItemModel);
        }

        public void DrawView(BaseItemModel model)
        {
            var consumable = (ConsumableItemModel)model;
            if (consumable)
            {
                AbilityEffectEditor.DrawEffectStack(consumable.Effects);
            }
        }
    }
}
