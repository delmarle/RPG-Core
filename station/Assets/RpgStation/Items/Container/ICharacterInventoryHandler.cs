

using Station;

public interface ICharacterInventoryHandler
{
    //on item added removed used
    void AddItems(BaseCharacter owner, ItemStack[] itemsAdded);
    void RemoveItems(BaseCharacter owner, ItemStack[] itemsRemoved);
    ItemStack[] GetItems(BaseCharacter owner);
}
