using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

[RequireComponent(typeof(DialogueController))]
public class QuestGiver : MonoBehaviour
{
    DialogueController dialogueController;
    QuestData_SO currentQuestData;

    public DialogueData_SO startDialogueData; // Dialogue to start the quest
    public DialogueData_SO progressDialogueData; // Dialogue to show the progress of the quest
    public DialogueData_SO completedDialogueData; // Dialogue to show when the quest is completed
    public DialogueData_SO finishedDialogueData; // Dialogue to show when the quest is finished

    public string questName;

    #region Get Quest Status
    public bool isStarted
    {
        get
        {
            if (QuestManager.Instance.HaveQuest(currentQuestData))
            {
                return QuestManager.Instance.GetQuestTask(currentQuestData).IsStarted;
            }
            else return false;
        }
    }

    public bool isCompleted
    {
        get
        {
            if (QuestManager.Instance.HaveQuest(currentQuestData))
            {
                return QuestManager.Instance.GetQuestTask(currentQuestData).IsCompleted;
            }
            else return false;
        }
    }

    public bool isFinished
    {
        get
        {
            if (QuestManager.Instance.HaveQuest(currentQuestData))
            {
                return QuestManager.Instance.GetQuestTask(currentQuestData).IsFinished;
            }
            else return false;
        }
    }
    #endregion

    void Awake()
    {
        dialogueController = GetComponent<DialogueController>();
    }

    void Start()
    {
        dialogueController.currentDialogue = startDialogueData;
        currentQuestData = dialogueController.currentDialogue.GetQuest(questName);
    }

    void Update()
    {
        if (isStarted)
        {
            if (isCompleted)
                dialogueController.currentDialogue = completedDialogueData;
            else dialogueController.currentDialogue = progressDialogueData;
        }

        if (isFinished)
            dialogueController.currentDialogue = finishedDialogueData;
    }
}
