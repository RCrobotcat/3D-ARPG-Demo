using DG.Tweening;
using QFramework;
using RCProtocol;
using UnityEngine;
using UnityEngine.UI;

public class CharacterNumController : Singleton<CharacterNumController>, IController
{
    [Header("Basic Nums Settings")]
    public float currentMaxHealth = 10f;
    public float currentMaxStamina = 20f;
    public float RunStaminaCost = 0.5f;

    [Header("UIs")]
    public Transform PlayerHealthBar;
    Image HealthSlider;
    public Transform PlayerStaminaBar;
    Image StaminaSlider;
    public Text healthTxt;
    public Text staminaTxt;
    public Text accountTxt;

    public ICharacterNumModel mModel;

    protected override void Awake()
    {
        base.Awake();

        HealthSlider = PlayerHealthBar.GetChild(0).GetComponent<Image>();
        StaminaSlider = PlayerStaminaBar.GetChild(0).GetComponent<Image>();

        NetManager.Instance.RegisterNtfHandler(CMD.PlayerBeAttacked, PlayerBeAttack);
    }

    void Start()
    {
        mModel = this.GetModel<ICharacterNumModel>();

        mModel.PlayerHealth.RegisterWithInitValue(health =>
        {
            UpdateHealthBar();
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        mModel.PlayerStamina.RegisterWithInitValue(stamina =>
        {
            UpdateStaminaBar();
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        accountTxt.text = NetManager.Instance?.account;
    }

    void Update()
    {
        if (Character.Instance != null)
        {
            HandleStaminaChangeWhenRunning();
        }
    }

    /// <summary>
    /// 精力值变化接口
    /// </summary>
    public void StaminaChange(float val)
    {
        this.SendCommand(new PlayerStaminaChangeCommand(val));
    }
    void HandleStaminaChangeWhenRunning()
    {
        if (Character.Instance.isSprint && Character.Instance.agent.velocity.magnitude > 0.1f)
        {
            float cost = RunStaminaCost * Time.deltaTime * -1f;
            this.SendCommand(new PlayerStaminaChangeCommand(cost));
        }
        else
        {
            float add = RunStaminaCost * Time.deltaTime;
            this.SendCommand(new PlayerStaminaChangeCommand(add));
        }
    }
    /// <summary>
    /// 生命值变化接口
    /// </summary>
    public void HealthChange(float val)
    {
        this.SendCommand(new PlayerHealthChangeCommand(val));
    }

    void UpdateHealthBar()
    {
        float SliderPercent = (float)mModel.PlayerHealth.Value / currentMaxHealth;
        HealthSlider.DOFillAmount(SliderPercent, 0.3f);
        healthTxt.text = mModel.PlayerHealth.Value.ToString("F1") + "/" + currentMaxHealth.ToString("F1");
    }
    void UpdateStaminaBar()
    {
        float SliderPercent = (float)mModel.PlayerStamina.Value / currentMaxStamina;
        StaminaSlider.DOFillAmount(SliderPercent, 0.3f);
        staminaTxt.text = mModel.PlayerStamina.Value.ToString("F1") + "/" + currentMaxStamina.ToString("F1");
    }

    /// <summary>
    /// 玩家被攻击回调
    /// </summary>
    void PlayerBeAttack(NetMsg msg)
    {
        float attackDamage = msg.playerBeAttack.damage;
        HealthChange(-attackDamage);

        Character.Instance.animator.SetTrigger("BeHit");
    }

    public IArchitecture GetArchitecture()
    {
        return CharacterApp.Interface;
    }
}
