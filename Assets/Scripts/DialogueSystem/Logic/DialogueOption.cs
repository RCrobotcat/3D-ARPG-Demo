using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // can be shown in inspector
public class DialogueOption
{
    public string text;
    public string targetID;
    public bool takeQuest;
    public bool followPlayer;
}
