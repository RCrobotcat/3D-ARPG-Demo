using UnityEngine;
using UnityEngine.UI;

public class QuestUI : Singleton<QuestUI>
{
    [Header("Elements")]
    public GameObject questListPanel;
    public GameObject questContentPanel;
    public GameObject rewardPanel;
    public GameObject itemTooltip;
    // bool isOpen;

    [Header("Quest Name")]
    public RectTransform questListTransform;
    public QuestNameBtn questNameBtn;

    [Header("Quest Content")]
    public Text questContentTxt;

    [Header("Requirements")]
    public RectTransform requirementsTransform;
    public QuestRequirement questRequirement;

    [Header("Rewards")]
    public RectTransform rewardsTransform;
    public ItemUI rewardUI;

    bool isAnimating = false; // 任务UI动画是否正在播放
    [HideInInspector] public bool isOpenQuest = false;

    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.H))
        {
            isOpen = !isOpen;
            questListPanel.SetActive(isOpen);
            questContentTxt.text = string.Empty;

            if (!isOpen)
                itemTooltip.gameObject.SetActive(false);

            SetUpQuestList();
        }*/

        if (!isAnimating)
        {
            isAnimating = true;

            if (InputManager.Instance.inputOpenQuest)
            {
                OpenQuestUI();
                if (InventoryManager.Instance.isOpenInventory)
                {
                    InputManager.Instance.inputOpenInventory = false;
                    InventoryManager.Instance.CloseInventoryUI();
                }
            }
            else if (!InputManager.Instance.inputOpenQuest)
            {
                CloseQuestUI();
            }

            SetUpQuestList();
        }
    }

    public void SetUpQuestList()
    {
        foreach (Transform item in questListTransform)
            Destroy(item.gameObject);

        foreach (Transform item in requirementsTransform)
            Destroy(item.gameObject);

        foreach (Transform item in rewardsTransform)
            Destroy(item.gameObject);

        foreach (var task in QuestManager.Instance.questTasks)
        {
            var newTask = Instantiate(questNameBtn, questListTransform);
            newTask.SetUpNameBtn(task.questData);
            newTask.questContentTxt = questContentTxt;
        }
    }

    public void SetUpRequirements(QuestData_SO questData)
    {
        foreach (Transform item in requirementsTransform)
            Destroy(item.gameObject);

        foreach (var req in questData.questRequirements)
        {
            var newReq = Instantiate(questRequirement, requirementsTransform);
            if (questData.isFinished)
                newReq.SetUpRequirements(req.name, true);
            else newReq.SetUpRequirements(req.name, req.requiredAmount, req.currentAmount);
        }
    }

    public void SetUpRewardItem(ItemData_SO itemData, int amount)
    {
        var item = Instantiate(rewardUI, rewardsTransform);
        item.SetUpItemUI(itemData, amount);
    }

    public void OpenQuestUI()
    {
        isOpenQuest = true;

        questListPanel.SetActive(true);
        questContentPanel.SetActive(true);
        rewardPanel.SetActive(true);

        questListPanel.transform.localPosition.rc_To(new Vector3(-500, 0, 0), 0.3f,
            (Vector3 pos) => questListPanel.transform.localPosition = pos,
            () => { isAnimating = false; });

        questContentPanel.transform.localPosition.rc_To(new Vector3(500, 0, 0), 0.3f,
            (Vector3 pos) => questContentPanel.transform.localPosition = pos,
            () => { isAnimating = false; });

        rewardPanel.transform.localPosition.rc_To(new Vector3(513, 0, 0), 0.3f,
            (Vector3 pos) => rewardPanel.transform.localPosition = pos,
            () => { isAnimating = false; });

        // 禁用摄像机旋转
        if (CameraManager.Instance.FreeLookCam != null)
        {
            CameraManager.Instance.FreeLookCam.m_XAxis.m_InputAxisName = "";
            CameraManager.Instance.FreeLookCam.m_YAxis.m_InputAxisName = "";
        }
    }
    public void CloseQuestUI()
    {
        isOpenQuest = false;

        questListPanel.transform.localPosition.rc_To(new Vector3(-1500, 0, 0), 0.3f,
                    (Vector3 pos) => questListPanel.transform.localPosition = pos,
                    () => { isAnimating = false; questListPanel.SetActive(false); });

        questContentPanel.transform.localPosition.rc_To(new Vector3(1500, 0, 0), 0.3f,
            (Vector3 pos) => questContentPanel.transform.localPosition = pos,
            () => { isAnimating = false; questContentPanel.SetActive(false); });

        rewardPanel.transform.localPosition.rc_To(new Vector3(1513, 0, 0), 0.3f,
            (Vector3 pos) => rewardPanel.transform.localPosition = pos,
            () => { isAnimating = false; rewardPanel.SetActive(false); });

        // 恢复摄像机旋转
        if (CameraManager.Instance.FreeLookCam != null
            && CameraManager.Instance.FreeLookCam.m_XAxis.m_InputAxisName == ""
            && CameraManager.Instance.FreeLookCam.m_YAxis.m_InputAxisName == "")
        {
            CameraManager.Instance.FreeLookCam.m_XAxis.m_InputAxisName = "Mouse X";
            CameraManager.Instance.FreeLookCam.m_YAxis.m_InputAxisName = "Mouse Y";
        }
    }
}
