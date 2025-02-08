using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DialogueUI : Singleton<DialogueUI>
{
    [Header("Basic Elements")]
    public Image icon;
    public Text MainText;
    public Button NextBtn;
    public GameObject dialoguePanel;

    [Header("Options")]
    public RectTransform optionPanel;
    public OptionUI optionPrefab;

    [Header("Data")]
    public DialogueData_SO currentDialogue;
    int currentIndex = 0;

    protected override void Awake()
    {
        base.Awake();
        NextBtn.onClick.AddListener(ContinueDialogue);
    }

    void ContinueDialogue()
    {
        if (currentIndex < currentDialogue.dialoguePieces.Count)
            UpdateMainDialogue(currentDialogue.dialoguePieces[currentIndex]);
        else dialoguePanel.SetActive(false);
    }

    public void UpdateDialogueData(DialogueData_SO data)
    {
        currentDialogue = data;
        currentIndex = 0;
    }

    public void UpdateMainDialogue(DialoguePiece piece)
    {
        dialoguePanel.SetActive(true);
        currentIndex++;

        if (piece.image != null)
        {
            icon.enabled = true;
            icon.sprite = piece.image;
        }
        else icon.enabled = false;

        MainText.text = "";
        // MainText.text = piece.text;
        MainText.DOText(piece.text, 1f);

        if (piece.dialogueOption.Count == 0 && currentDialogue.dialoguePieces.Count > 0)
        {
            NextBtn.interactable = true;
            NextBtn.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            NextBtn.interactable = false;
            NextBtn.transform.GetChild(0).gameObject.SetActive(false);
        }

        // Create Options
        CreateOptions(piece);
    }

    void CreateOptions(DialoguePiece piece)
    {
        // Clear the previous options
        if (optionPanel.childCount > 0)
        {
            for (int i = 0; i < optionPanel.childCount; i++)
            {
                Destroy(optionPanel.GetChild(i).gameObject);
            }
        }

        // Create new options
        for (int i = 0; i < piece.dialogueOption.Count; i++)
        {
            var option = Instantiate(optionPrefab, optionPanel);
            option.UpdateOption(piece, piece.dialogueOption[i]);
        }
    }
}
