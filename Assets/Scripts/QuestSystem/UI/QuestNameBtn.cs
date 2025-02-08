using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestNameBtn : MonoBehaviour
{
    public Text questNameTxt;
    public QuestData_SO currentQuestData;
    public Text questContentTxt;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(UpdateQuestContent);
    }

    void UpdateQuestContent()
    {
        questContentTxt.text = currentQuestData.description;
        QuestUI.Instance.SetUpRequirements(currentQuestData);

        foreach (Transform item in QuestUI.Instance.rewardsTransform)
            Destroy(item.gameObject);

        foreach (var item in currentQuestData.questRewards)
            QuestUI.Instance.SetUpRewardItem(item.itemData, item.amount);
    }

    public void SetUpNameBtn(QuestData_SO questData)
    {
        currentQuestData = questData;

        if (questData.isCompleted)
            questNameTxt.text = questData.questName + " (Completed)";
        else questNameTxt.text = questData.questName;
    }
}
