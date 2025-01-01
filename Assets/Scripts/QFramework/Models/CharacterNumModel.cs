using QFramework;
using UnityEngine;

public interface ICharacterNumModel : IModel
{
    BindableProperty<float> PlayerHealth { get; }
    BindableProperty<float> PlayerStamina { get; }

    void PlayerHealthChange(float changeVal);
    void PlayerStaminaChange(float changeVal);
}

public class CharacterNumModel : AbstractModel, ICharacterNumModel
{
    public BindableProperty<float> PlayerHealth { get; } = new BindableProperty<float>(CharacterNumController.Instance.currentMaxHealth);

    public BindableProperty<float> PlayerStamina { get; } = new BindableProperty<float>(CharacterNumController.Instance.currentMaxStamina);

    public void PlayerHealthChange(float changeVal)
    {
        float currentHealth = PlayerHealth.Value + changeVal;
        PlayerHealth.Value = Mathf.Clamp(currentHealth, 0, CharacterNumController.Instance.currentMaxHealth);
    }

    public void PlayerStaminaChange(float changeVal)
    {
        float currentStamina = PlayerStamina.Value + changeVal;
        PlayerStamina.Value = Mathf.Clamp(currentStamina, 0, CharacterNumController.Instance.currentMaxStamina);
    }

    protected override void OnInit() { }
}
