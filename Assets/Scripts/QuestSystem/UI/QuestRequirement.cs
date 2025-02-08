using UnityEngine;
using UnityEngine.UI;

public class QuestRequirement : MonoBehaviour
{
    Text requirementName;
    Text progressNum;

    void Awake()
    {
        requirementName = GetComponent<Text>();
        progressNum = transform.GetChild(0).GetComponent<Text>();
    }

    public void SetUpRequirements(string name, int amount, int currentAmount)
    {
        requirementName.text = name;
        progressNum.text = currentAmount.ToString() + "/" + amount.ToString();
    }

    public void SetUpRequirements(string name, bool isFinished)
    {
        if (isFinished)
        {
            requirementName.text = name;
            progressNum.text = "Finished";
            requirementName.color = Color.green;
            progressNum.color = Color.green;
        }
    }
}
