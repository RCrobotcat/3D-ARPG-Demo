using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // can be shown in inspector
public class DialoguePiece
{
    public string ID;
    public Sprite image;

    [TextArea]
    public string text;

    public QuestData_SO questData;

    public List<DialogueOption> dialogueOption = new List<DialogueOption>();
}
