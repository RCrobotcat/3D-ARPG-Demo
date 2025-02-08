using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public DialogueData_SO currentDialogue;
    bool canTalk = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && currentDialogue != null)
            canTalk = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTalk = false;
            DialogueUI.Instance.dialoguePanel.SetActive(false);
        }
    }

    void Update()
    {
        if (canTalk && Input.GetKeyDown(KeyCode.X))
        {
            OpenDialogue();
        }
    }

    void OpenDialogue()
    {
        DialogueUI.Instance.UpdateDialogueData(currentDialogue);
        DialogueUI.Instance.UpdateMainDialogue(currentDialogue.dialoguePieces[0]);
    }
}
