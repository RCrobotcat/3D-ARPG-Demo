using UnityEngine;
using UnityEngine.EventSystems;

public enum SlotType { BAG, WEAPON, ARMOR, ACTION }
public class SlotHolder : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ItemUI itemUI;
    public SlotType slotType;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount % 2 == 0)
        {
            UseItem();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemUI.GetItem())
        {
            InventoryManager.Instance.itemTooltip.SetUpTooltip(itemUI.GetItem());
            InventoryManager.Instance.itemTooltip.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryManager.Instance.itemTooltip.gameObject.SetActive(false);
    }

    public void UseItem()
    {
        if (itemUI.GetItem() != null)
            if (itemUI.GetItem().itemType == ItemType.Usable && itemUI.Bag.items[itemUI.Index].amount > 0)
            {
                itemUI.Bag.items[itemUI.Index].amount--; // decrease the amount by 1
                CharacterNumController.Instance.HealthChange(itemUI.GetItem().usableItemData.RestoreHealthPoint);
            }
        UpdateItem();

        QuestManager.Instance.UpdateQuestProgress(itemUI.GetItem().itemName, -1);
    }

    public void UpdateItem()
    {
        switch (slotType)
        {
            case SlotType.BAG:
                itemUI.Bag = InventoryManager.Instance.inventoryData;
                break;
            case SlotType.WEAPON:
                itemUI.Bag = InventoryManager.Instance.equipmentData;
                if (itemUI.Bag.items[itemUI.Index].itemData != null)
                {
                    InventoryManager.Instance.SwitchWeapon(itemUI.Bag.items[itemUI.Index].itemData);
                }
                else
                {
                    InventoryManager.Instance.UnEquipWeapon();
                }
                break;
            case SlotType.ARMOR:
                itemUI.Bag = InventoryManager.Instance.equipmentData;
                if (itemUI.Bag.items[itemUI.Index].itemData != null)
                {
                    InventoryManager.Instance.SwitchArmor(itemUI.Bag.items[itemUI.Index].itemData);
                }
                else
                {
                    InventoryManager.Instance.UnEquipArmor();
                }
                break;
        }

        var item = itemUI.Bag.items[itemUI.Index];
        itemUI.SetUpItemUI(item.itemData, item.amount);
    }

    void OnDisable()
    {
        InventoryManager.Instance.itemTooltip.gameObject.SetActive(false);
    }
}
