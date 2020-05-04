

using Station;

public interface ICharacterInventoryHandler
{
    //on item added removed used
    void AddItems(string containerId, ItemStack[] itemsAdded);
    void RemoveItems(string containerId, ItemStack[] itemsRemoved);
    ItemContainer GetContainer(string containerId);
}
