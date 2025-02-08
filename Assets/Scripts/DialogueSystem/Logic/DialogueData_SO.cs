using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Data", menuName = "Dialogue/Dialogue Data")]
public class DialogueData_SO : ScriptableObject
{
    public List<DialoguePiece> dialoguePieces = new List<DialoguePiece>();
    public Dictionary<string, DialoguePiece> dialogueDict = new Dictionary<string, DialoguePiece>();

#if UNITY_EDITOR
    // Activate this method when the script is loaded or a value is changed in the inspector
    void OnValidate()
    {
        dialogueDict.Clear();
        foreach (var piece in dialoguePieces)
            if (!dialogueDict.ContainsKey(piece.ID))
                dialogueDict.Add(piece.ID, piece);
    }
#else
    void Awake() // 保证在打包执行的游戏里第一时间获得对话的所有字典匹配 
    {
        dialogueDict.Clear();
        foreach (var piece in dialoguePieces)
        {
            if (!dialogueDict.ContainsKey(piece.ID))
                dialogueDict.Add(piece.ID, piece);
        }
    }
#endif

    public QuestData_SO GetQuest(string questName)
    {
        QuestData_SO currentQuest = null;
        foreach (var piece in dialoguePieces)
        {
            if (piece.questData != null)
            {
                if (piece.questData.name == questName)
                {
                    currentQuest = piece.questData;
                    break;
                }
            }
        }

        return currentQuest;
    }
}
