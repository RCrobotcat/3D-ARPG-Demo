using UnityEngine;

public class CharacterCombo : MonoBehaviour
{
    Animator animator;

    public static int numberOfClicks = 0;
    float lastClickTime = 0;

    float maxComboDelay = 0.3f;

    void Awake()
    {
        animator = GetComponent<Animator>();
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

        if (Input.GetMouseButtonDown(0))
        {
            lastClickTime = Time.time;
            OnClick();
        }

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
            && animator.GetCurrentAnimatorStateInfo(1).normalizedTime > 0.7f
            && animator.GetCurrentAnimatorStateInfo(1).IsName("attack1"))
        {
            animator.SetBool("attack1", false);
            animator.SetBool("attack2", true);
        }
        if (numberOfClicks >= 3
           && animator.GetCurrentAnimatorStateInfo(1).normalizedTime > 0.7f
           && animator.GetCurrentAnimatorStateInfo(1).IsName("attack2"))
        {
            animator.SetBool("attack2", false);
            animator.SetBool("attack3", true);
        }
    }
}