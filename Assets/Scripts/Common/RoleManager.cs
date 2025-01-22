using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleManager : Singleton<RoleManager>
{
    public List<GameObject> allRoles; // ���н�ɫ

    public Text roleNameTxt; // ��ɫ��

    public Button leftBtn; // ��ť
    public Button rightBtn; // �Ұ�ť
    public Button selectBtn; // ѡ��ť

    private int currentRoleIndex = 0; // ��ǰ��ɫ����

    protected override void Awake()
    {
        base.Awake();

        leftBtn.onClick.AddListener(OnLeftBtnClick);
        rightBtn.onClick.AddListener(OnRightBtnClick);
        selectBtn.onClick.AddListener(OnSelectBtnClick);
    }

    void Update()
    {
        roleNameTxt.text = allRoles[currentRoleIndex].name;
    }

    public void ShowRoleByRoleName(string roleName)
    {
        foreach (var role in allRoles)
        {
            if (role.name == roleName)
            {
                // roleNameTxt.text = roleName;
                role.SetActive(true);
            }
            else
            {
                role.SetActive(false);
            }
        }
    }

    void OnLeftBtnClick()
    {
        currentRoleIndex--;
        if (currentRoleIndex < 0)
        {
            currentRoleIndex = allRoles.Count - 1;
        }
        ShowRoleByRoleName(allRoles[currentRoleIndex].name);
    }
    void OnRightBtnClick()
    {
        currentRoleIndex++;
        if (currentRoleIndex >= allRoles.Count)
        {
            currentRoleIndex = 0;
        }
        ShowRoleByRoleName(allRoles[currentRoleIndex].name);
    }

    void OnSelectBtnClick()
    {
        Debug.Log("Selected Role��" + allRoles[currentRoleIndex].name);
    }
}
