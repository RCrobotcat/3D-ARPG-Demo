using UnityEngine;
using UnityEngine.UI;

public class GameOver : Singleton<GameOver>
{
    public GameObject restartPanel;

    public Button restartBtn;
    public Button quitBtn;

    protected override void Awake()
    {
        base.Awake();

        restartBtn.onClick.AddListener(RestartGame);
        quitBtn.onClick.AddListener(() => Application.Quit());
    }

    public void OpenRestartPanel()
    {
        restartPanel.SetActive(true);
    }

    /// <summary>
    /// 重新开始游戏
    /// </summary>
    public void RestartGame()
    {
        CharacterNumController.Instance.mModel.PlayerHealth.Value = CharacterNumController.Instance.currentMaxHealth;
        restartPanel.SetActive(false);

        Character.Instance.isDead = false;
        Character.Instance.standingState.IsDead = false;
        Character.Instance.sprintState.IsDead = false;
        Character.Instance.comboState.IsDead = false;
        Character.Instance.animator.SetTrigger("Alive");
        Character.Instance.transform.position = new Vector3(1, 0, 1);
    }
}
