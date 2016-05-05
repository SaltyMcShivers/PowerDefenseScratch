using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour {
    public float projectileDamage;
    float detonationTime = 0f;
    float explosionRadius;
    float slowDownRate;
    public GameObject deathPrefab;

    public void SetUpBullet(float damage, float boom, float rad, float slow)
    {
        projectileDamage = damage;
        detonationTime = boom;
        explosionRadius = rad;
        slowDownRate = slow;
        if(explosionRadius != 0f){
            GetComponent<Collider2D>().enabled = false;
            StartCoroutine(ExplodeCoroutine());
        }
    }

    IEnumerator ExplodeCoroutine()
    {
        yield return new WaitForSeconds(detonationTime);
        if (deathPrefab != null)
        {
            GameObject deadObject = Instantiate(deathPrefab, transform.position + Vector3.forward, Quaternion.identity) as GameObject;
            ExplosionScript exp = deadObject.GetComponent<ExplosionScript>();
            if (exp != null)
            {
                exp.SetUpExplosion(projectileDamage, explosionRadius, slowDownRate);
            }
        }
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Destroy(gameObject);
    }
}
