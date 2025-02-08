using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Quest Data")]
public class QuestData_SO : ScriptableObject
{
    [System.Serializable]
    public class QuestRequirement
    {
        public string name;
        public int requiredAmount;
        public int currentAmount;
    }

    public string questName;
    [TextArea]
    public string description;

    public bool isStarted; // is the quest started
    public bool isCompleted; // is the quest completed
    public bool isFinished; // is the quest finished(after completed and received rewards)

    public List<QuestRequirement> questRequirements = new List<QuestRequirement>();
    public List<InventoryItem> questRewards = new List<InventoryItem>();

    public void CheckTaskProgress()
    {
        var finishedRequires = questRequirements.Where(r => r.currentAmount >= r.requiredAmount);
        isCompleted = finishedRequires.Count() == questRequirements.Count;

        if (isCompleted)
            Debug.Log("Quest Completed!");
    }

    // Get the name of quest requirements
    public List<string> GetRequiredTargetName()
    {
        List<string> targetNameList = new List<string>();
        foreach (var require in questRequirements)
            targetNameList.Add(require.name);
        return targetNameList;
    }

    public void GiveQuestRewards()
    {
        foreach (var reward in questRewards)
        {
            if (reward.amount < 0)
            {
                int requireCount = Mathf.Abs(reward.amount);

                // if the item is in the bag, remove the item from the bag
                if (InventoryManager.Instance.QuestItemInBag(reward.itemData) != null)
                {
                    // if the item amount in the bag is less than the required amount,
                    // then remove the item from the bag and action bar
                    if (InventoryManager.Instance.QuestItemInBag(reward.itemData).amount <= requireCount)
                    {
                        requireCount -= InventoryManager.Instance.QuestItemInBag(reward.itemData).amount;
                        InventoryManager.Instance.QuestItemInBag(reward.itemData).amount = 0;
                    }
                    else
                    {
                        InventoryManager.Instance.QuestItemInBag(reward.itemData).amount -= requireCount;
                    }
                }
            }
            else
            {
                InventoryManager.Instance.inventoryData.AddItem(reward.itemData, reward.amount);
            }

            InventoryManager.Instance.inventoryUI.RefreshUI();
            InventoryManager.Instance.equipmentUI.RefreshUI();
        }
    }
}
