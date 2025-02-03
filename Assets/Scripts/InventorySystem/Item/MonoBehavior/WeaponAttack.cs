using AIActor_RC;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    public List<AttackSO> attackSOs;
    float attackDamage;

    void Awake()
    {
        attackDamage = attackSOs[0].damage;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            return;

        if (collision.gameObject.CompareTag("Enemy") && Character.Instance.isAttacking)
        {
            collision.gameObject.GetComponent<AIActor>().BeAttack(attackDamage);
        }
    }
}
