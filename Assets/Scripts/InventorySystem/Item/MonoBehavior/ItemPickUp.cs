using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public ItemData_SO itemData;

    bool inTrigger = false;

    void Update()
    {
        if (inTrigger)
        {
            InventoryManager.Instance.SetItemPickUpTip(itemData.itemName, true);

            // TODO: 改为InputManager
            if (Input.GetKeyDown(KeyCode.E))
            {
                PickUpItem();
            }
        }
        else if (!inTrigger)
        {
            if (InventoryManager.Instance.itemToPickUpTipTrans.localScale == Vector3.one)
                InventoryManager.Instance.SetItemPickUpTip(itemData.itemName, false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inTrigger = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inTrigger = false;
        }
    }

    void OnDestroy()
    {
        InventoryManager.Instance.SetItemPickUpTip(itemData.itemName, false);
        inTrigger = false;
    }

    /// <summary>
    /// 捡起物品
    /// </summary>
    void PickUpItem()
    {
        InventoryManager.Instance.inventoryData.AddItem(itemData, itemData.itemAmount);
        InventoryManager.Instance.inventoryUI.RefreshUI();

        Destroy(gameObject);
    }
}
