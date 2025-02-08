using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    public Text optionText;
    Button btn;

    DialoguePiece currentPiece;

    string nextPieceID;
    bool takeQuest;

    void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnOptionClicked);
    }

    public void UpdateOption(DialoguePiece piece, DialogueOption option)
    {
        currentPiece = piece;
        optionText.text = option.text;
        nextPieceID = option.targetID;
        takeQuest = option.takeQuest;
    }

    public void OnOptionClicked()
    {
        if (currentPiece.questData != null)
        {
            var newTask = new QuestManager.QuestTask { questData = Instantiate(currentPiece.questData) };
            if (takeQuest)
            {
                // check if player already have the quest
                if (QuestManager.Instance.HaveQuest(newTask.questData))
                {
                    // the quest is completed and the player can finish the quest
                    if (QuestManager.Instance.GetQuestTask(newTask.questData).IsCompleted)
                    {
                        newTask.questData.GiveQuestRewards();
                        QuestManager.Instance.GetQuestTask(newTask.questData).IsFinished = true;
                    }
                }
                else
                {
                    QuestManager.Instance.questTasks.Add(newTask);
                    QuestManager.Instance.GetQuestTask(newTask.questData).IsStarted = true;

                    // Update the quest progress
                    foreach (var reqName in newTask.questData.GetRequiredTargetName())
                        InventoryManager.Instance.CheckQuestItemInBag(reqName);
                }
            }
        }

        if (nextPieceID == "")
        {
            DialogueUI.Instance.dialoguePanel.SetActive(false);
            return;
        }
        else
        {
            DialogueUI.Instance.UpdateMainDialogue(DialogueUI.Instance.currentDialogue.dialogueDict[nextPieceID]);
        }
    }
}
