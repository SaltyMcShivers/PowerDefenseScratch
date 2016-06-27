using UnityEngine;
using System.Collections;

public class EnemyAttackScript : MonoBehaviour {
    public float attackInterval;
    public float attackDamage;
    public float attackRecovery;

    bool attacking;

    public void StartAttacking()
    {
        attacking = true;
        StartCoroutine(AttackCoroutine());
    }

    IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(attackInterval);
        Messenger<float>.Invoke("Enemy Attacks", attackDamage);
        yield return new WaitForSeconds(attackRecovery);
        StartCoroutine(AttackCoroutine());
    }

    public void StopAttacking()
    {
        attacking = false;
        StopAllCoroutines();
    }

    void OnDisabled()
    {
        StopAttacking();
    }

    public bool IsAttacking()
    {
        return attacking;
    }
}
