using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestManager : Singleton<QuestManager>
{
    [System.Serializable]
    public class QuestTask
    {
        public QuestData_SO questData;
        public bool IsStarted { get { return questData.isStarted; } set { questData.isStarted = value; } }
        public bool IsCompleted { get { return questData.isCompleted; } set { questData.isCompleted = value; } }
        public bool IsFinished { get { return questData.isFinished; } set { questData.isFinished = value; } }
    }

    public List<QuestTask> questTasks = new List<QuestTask>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        LoadQuestManager();
    }

    public void SaveQuestManager()
    {
        PlayerPrefs.SetInt("QuestCount", questTasks.Count);
        /*for (int i = 0; i < questTasks.Count; i++)
        {
            SaveManager.Instance.Save(questTasks[i].questData, "task" + i);
        }*/
    }

    public void LoadQuestManager()
    {
        var questCount = PlayerPrefs.GetInt("QuestCount");
        questTasks.Clear();
        for (int i = 0; i < questCount; i++)
        {
            var newQuest = ScriptableObject.CreateInstance<QuestData_SO>();
            // SaveManager.Instance.Load(newQuest, "task" + i);
            questTasks.Add(new QuestTask { questData = newQuest });
        }
    }

    public bool HaveQuest(QuestData_SO quest)
    {
        // Check if the quest is in the questTasks list, using linq's Any method
        if (quest != null)
            return questTasks.Any(q => q.questData.questName == quest.questName);
        else return false;
    }

    public QuestTask GetQuestTask(QuestData_SO data)
    {
        return questTasks.Find(q => q.questData.questName == data.questName);
    }

    // When kill a monster or collect an item, etc, update the quest progress
    public void UpdateQuestProgress(string requireName, int amount)
    {
        foreach (var task in questTasks)
        {
            if (task.IsFinished)
                continue;
            var matchTask = task.questData.questRequirements.Find(r => r.name == requireName);
            if (matchTask != null)
                matchTask.currentAmount += amount;

            // check if the quest is completed.
            task.questData.CheckTaskProgress();
        }
    }
}
