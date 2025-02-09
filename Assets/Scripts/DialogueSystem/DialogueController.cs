using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public DialogueData_SO currentDialogue;
    [HideInInspector] public bool canTalk = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && currentDialogue != null)
        {
            canTalk = true;
            DialogueUI.Instance.isTalking = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTalk = false;
            DialogueUI.Instance.dialoguePanel.SetActive(false);
            DialogueUI.Instance.isTalking = false;
        }
    }

    void Update()
    {
        if (canTalk && Input.GetKeyDown(KeyCode.X))
        {
            OpenDialogue();
            // ½ûÓÃÉãÏñ»úÐý×ª
            if (CameraManager.Instance.FreeLookCam != null)
            {
                CameraManager.Instance.FreeLookCam.m_XAxis.m_InputAxisName = "";
                CameraManager.Instance.FreeLookCam.m_YAxis.m_InputAxisName = "";
            }
        }
    }

    void OpenDialogue()
    {
        DialogueUI.Instance.UpdateDialogueData(currentDialogue);
        DialogueUI.Instance.UpdateMainDialogue(currentDialogue.dialoguePieces[0]);
    }
}
