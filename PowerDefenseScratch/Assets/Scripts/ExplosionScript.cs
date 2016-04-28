using UnityEngine;
using System.Collections;

public class ExplosionScript : MonoBehaviour {

    float explosionDamage;
    float slowAmount;

    public float explosionTime;
    public float explosionLinger = 2.0f;
    public float slowTime;

    public void SetUpExplosion(float damage, float size, float slow)
    {
        transform.localScale = Vector3.one * size;
        explosionDamage = damage;
        slowAmount = slow;
        StartCoroutine(ExplodeCoroutine());
    }

    IEnumerator ExplodeCoroutine()
    {
        yield return new WaitForSeconds(explosionTime);
        GetComponentInChildren<Collider2D>().enabled = false;
        yield return new WaitForSeconds(explosionLinger);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        EnemyHealthScript health = col.gameObject.GetComponent<EnemyHealthScript>();
        if (health != null)
        {
            health.DoDamage(explosionDamage);
        }
        if (slowTime != 0)
        {
            EnemyMovement mover = col.gameObject.GetComponentInParent<EnemyMovement>();
            if(mover != null)
            {
                mover.SlowDownEnemy(slowAmount, slowTime);
            }
        }
    }
}
