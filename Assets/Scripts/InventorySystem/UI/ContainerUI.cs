using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ContainerUI : MonoBehaviour
{
    public SlotHolder[] slots;
    public Button SortItemsBtn;

    void Start()
    {
        if (SortItemsBtn != null)
            SortItemsBtn.onClick.AddListener(SortItems);
    }

    public void RefreshUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].itemUI.Index = i;
            slots[i].UpdateItem();
        }
    }

    // Sort items by item type
    public void SortItems()
    {
        var bag = InventoryManager.Instance.inventoryData;
        var items = bag.items;

        // sort items by item name(null items will be at the end)
        var sortedItems = items
            .Where(itemSlot => itemSlot.itemData != null)
            .OrderBy(itemSlot => itemSlot.itemData.itemType == ItemType.Weapon ? 0 : 1)
            .ThenBy(itemSlot => itemSlot.itemData.itemType == ItemType.Armor ? 0 : 1)
            .ToList();

        // calculate the number of empty slots
        int nullItemCount = items.Count - sortedItems.Count;

        // add empty slots to the end of the list
        for (int i = 0; i < nullItemCount; i++)
        {
            sortedItems.Add(new InventoryItem()); // create a new empty slot
        }

        bag.items = sortedItems;
        RefreshUI();
    }
}