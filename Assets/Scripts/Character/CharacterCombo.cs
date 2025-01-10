using UnityEngine;
using UnityEngine.VFX;

public class CharacterCombo : MonoBehaviour
{
    Animator animator;

    public static int numberOfClicks = 0;
    float lastClickTime = 0;

    public float maxComboDelay = 0.2f;

    public float slashStaminaChange = -1.5f;

    [Header("VFX Slash Settings")]
    public VisualEffect vfx_slash_blue_left;
    public VisualEffect vfx_slash_red_right;

    void Awake()
    {
        animator = GetComponent<Animator>();

        vfx_slash_blue_left.Stop();
        vfx_slash_red_right.Stop();

        InputManager.Instance.onSlash += () =>
        {
            if (CharacterNumController.Instance.mModel.PlayerStamina.Value >= 1f)
            {
                lastClickTime = Time.time;
                OnClick();
            }
        };
    }

    void Update()
    {
        if (Time.time - lastClickTime >= maxComboDelay)
        {
            numberOfClicks = 0;
            animator.SetBool("attack1", false);
            animator.SetBool("attack2", false);
            animator.SetBool("attack3", false);
        }

        /*if (CharacterNumController.Instance.mModel.PlayerStamina.Value >= 1.5f)
        {
            if (Input.GetMouseButtonDown(0))
            {
                lastClickTime = Time.time;
                OnClick();
            }
        }*/

        // Debug.Log(numberOfClicks);
    }

    void OnClick()
    {
        numberOfClicks++;
        numberOfClicks = Mathf.Clamp(numberOfClicks, 0, 3);
        if (numberOfClicks == 1)
        {
            animator.SetBool("attack1", true);
        }
        if (numberOfClicks >= 2
            && animator.GetCurrentAnimatorStateInfo(1).normalizedTime > 0.6f
            && animator.GetCurrentAnimatorStateInfo(1).IsName("attack1"))
        {
            animator.SetBool("attack1", false);
            animator.SetBool("attack2", true);
        }
        if (numberOfClicks == 3
           && animator.GetCurrentAnimatorStateInfo(1).normalizedTime > 0.6f
           && animator.GetCurrentAnimatorStateInfo(1).IsName("attack2"))
        {
            animator.SetBool("attack2", false);
            animator.SetBool("attack3", true);
        }
    }

    public void PlayVfxAnimationEvent_left()
    {
        vfx_slash_blue_left.Play();
        CharacterController.Instance.StaminChange(slashStaminaChange);
    }
    public void PlayVfxAnimationEvent_right()
    {
        vfx_slash_red_right.Play();
        CharacterController.Instance.StaminChange(slashStaminaChange);
    }
    public void Reset()
    {
        numberOfClicks = 0;
        animator.SetBool("attack1", false);
        animator.SetBool("attack2", false);
        animator.SetBool("attack3", false);
    }
}
