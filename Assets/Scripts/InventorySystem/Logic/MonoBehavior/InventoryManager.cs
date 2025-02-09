using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : Singleton<InventoryManager>
{
    // use to store the original holder and parent of the item being dragged
    // convenient to return the item to its original slot
    public class DragData
    {
        public SlotHolder originalHolder;
        public RectTransform originalParent;
    }

    [Header("Inventory Data")]
    public InventoryData_SO inventoryTemplate;
    public InventoryData_SO inventoryData;

    public InventoryData_SO equipmentTemplate;
    public InventoryData_SO equipmentData;

    [Header("Containers")]
    public ContainerUI inventoryUI;
    public ContainerUI equipmentUI;

    [Header("Drag Canvas")]
    public Canvas dragCanvas;
    public DragData currentDrag;

    [Header("UI Panels")]
    public GameObject BagPanel;
    public GameObject EquipmentPanel;

    [Header("Status Text")]
    public Text healthText;
    public Text staminaText;
    public Text attackText;
    public Text DefenceText;

    [Header("Tooltip")]
    public ItemTooltip itemTooltip;

    [Header("Pick Up Item Tip")]
    public Text itemToPickUpTxt;
    public Transform itemToPickUpTipTrans;

    // bool isOpen = false;
    bool isAnimating = false; // ����UI�����Ƿ����ڲ���
    [HideInInspector] public bool isOpenInventory = false;

    protected override void Awake()
    {
        base.Awake();

        if (inventoryTemplate != null)
            inventoryData = Instantiate(inventoryTemplate);
        if (equipmentTemplate != null)
            equipmentData = Instantiate(equipmentTemplate);
    }

    void Start()
    {
        // LoadData();
        inventoryUI.RefreshUI();
        equipmentUI.RefreshUI();
    }

    void Update()
    {
        if (!isAnimating)
        {
            isAnimating = true;

            if (InputManager.Instance.inputOpenInventory)
            {
                OpenInventoryUI();
                if (QuestUI.Instance.isOpenQuest)
                {
                    InputManager.Instance.inputOpenQuest = false;
                    QuestUI.Instance.CloseQuestUI();
                }
            }
            else if (!InputManager.Instance.inputOpenInventory)
            {
                CloseInventoryUI();
            }
        }

        if (CharacterNumController.Instance != null)
        {
            UpdateStatusText(CharacterNumController.Instance.mModel.PlayerHealth.Value,
                CharacterNumController.Instance.mModel.PlayerStamina.Value);
        }
    }

    /*public void SaveData()
    {
        SaveManager.Instance.Save(inventoryData, inventoryData.name);
        SaveManager.Instance.Save(actionData, actionData.name);
        SaveManager.Instance.Save(equipmentData, equipmentData.name);
    }
    public void LoadData()
    {
        SaveManager.Instance.Load(inventoryData, inventoryData.name);
        SaveManager.Instance.Load(actionData, actionData.name);
        SaveManager.Instance.Load(equipmentData, equipmentData.name);
    }*/

    public void UpdateStatusText(float health, float stamina)
    {
        healthText.text = health.ToString("F1");
        staminaText.text = stamina.ToString("F1");
    }

    #region Equipments Logic
    public void SwitchWeapon(ItemData_SO itemData)
    {
        Instantiate(itemData.WeaponPrefab, Character.Instance.weaponTrans);

        Character.Instance.combo = itemData.weaponAttackCombo;

        StageManager.Instance.SendSwitchWeapon(itemData);
    }
    public void UnEquipWeapon()
    {
        Character.Instance.combo = Character.Instance.originalCombo;

        foreach (Transform child in Character.Instance.weaponTrans)
        {
            Destroy(child.gameObject);
        }

        StageManager.Instance.SendUnEquipWeapon();
    }
    public void SwitchArmor(ItemData_SO itemData) { }
    public void UnEquipArmor() { }
    #endregion

    #region Judge the item being dragged is inside the range of the target slot
    public bool CheckInInventoryUI(Vector3 position)
    {
        for (int i = 0; i < inventoryUI.slots.Length; i++)
        {
            // same as => (RectTransform) inventoryUI.slots[i].transform, typecasting
            RectTransform t = inventoryUI.slots[i].transform as RectTransform;

            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
                return true;
        }
        return false;
    }

    public bool CheckInEquipmentUI(Vector3 position)
    {
        for (int i = 0; i < equipmentUI.slots.Length; i++)
        {
            // same as => (RectTransform) inventoryUI.slots[i].transform, typecasting
            RectTransform t = equipmentUI.slots[i].transform as RectTransform;

            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
                return true;
        }
        return false;
    }
    #endregion

    #region Check if the quest item already exists in the inventory, if so, update the quest progress
    public void CheckQuestItemInBag(string questItemName)
    {
        foreach (var item in inventoryData.items)
        {
            if (item.itemData != null)
            {
                if (item.itemData.itemName == questItemName) { }
                QuestManager.Instance.UpdateQuestProgress(questItemName, item.amount);
            }
        }
    }
    #endregion

    // Check if the quest item is in the inventory or action slot
    public InventoryItem QuestItemInBag(ItemData_SO questItem)
    {
        return inventoryData.items.Find(i => i.itemData == questItem);
    }

    /// <summary>
    /// ��ʾ������Ʒ��ʾ��ʾ
    /// </summary>
    public void SetItemPickUpTip(string itemName, bool active)
    {
        itemToPickUpTxt.text = itemName;

        if (active)
        {
            itemToPickUpTipTrans.DOScale(Vector3.one, 0.2f);
        }
        else if (!active)
        {
            itemToPickUpTipTrans.DOScale(Vector3.zero, 0.2f);
        }
    }

    public void OpenInventoryUI()
    {
        isOpenInventory = true;

        BagPanel.SetActive(true);
        EquipmentPanel.SetActive(true);

        BagPanel.transform.localPosition.rc_To(new Vector3(500, 0, 0), 0.3f,
            (Vector3 pos) => BagPanel.transform.localPosition = pos,
            () => { isAnimating = false; });

        EquipmentPanel.transform.localPosition.rc_To(new Vector3(-500, 0, 0), 0.3f,
            (Vector3 pos) => EquipmentPanel.transform.localPosition = pos,
            () => { isAnimating = false; });

        // �����������ת
        if (CameraManager.Instance.FreeLookCam != null)
        {
            CameraManager.Instance.FreeLookCam.m_XAxis.m_InputAxisName = "";
            CameraManager.Instance.FreeLookCam.m_YAxis.m_InputAxisName = "";
        }
    }
    public void CloseInventoryUI()
    {
        isOpenInventory = false;

        BagPanel.transform.localPosition.rc_To(new Vector3(1500, 0, 0), 0.3f,
                    (Vector3 pos) => BagPanel.transform.localPosition = pos,
                    () => { BagPanel.SetActive(false); isAnimating = false; });

        EquipmentPanel.transform.localPosition.rc_To(new Vector3(-1500, 0, 0), 0.3f,
            (Vector3 pos) => EquipmentPanel.transform.localPosition = pos,
            () => { EquipmentPanel.SetActive(false); isAnimating = false; });

        // �ָ��������ת
        if (CameraManager.Instance.FreeLookCam != null
            && CameraManager.Instance.FreeLookCam.m_XAxis.m_InputAxisName == ""
            && CameraManager.Instance.FreeLookCam.m_YAxis.m_InputAxisName == ""
            && !QuestUI.Instance.isOpenQuest
            && !DialogueUI.Instance.isTalking)
        {
            CameraManager.Instance.FreeLookCam.m_XAxis.m_InputAxisName = "Mouse X";
            CameraManager.Instance.FreeLookCam.m_YAxis.m_InputAxisName = "Mouse Y";
        }
    }
}
