using UnityEngine;
using System.Collections;

public class EnemyAttackScript : MonoBehaviour {
    public float attackInterval;
    public float attackDamage;
    public float attackRecovery;

    public void StartAttacking()
    {
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
        StopAllCoroutines();
    }

    void OnDisabled()
    {
        StopAttacking();
    }
}
