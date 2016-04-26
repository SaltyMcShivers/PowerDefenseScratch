using UnityEngine;
using System.Collections;

public class ExplosionScript : MonoBehaviour {

    float explosionDamage;
    public float explosionTime;

    public void SetUpExplosion(float damage, float size)
    {
        transform.localScale = Vector3.one * size;
        explosionDamage = damage;
        StartCoroutine(ExplodeCoroutine());
    }

    IEnumerator ExplodeCoroutine()
    {
        yield return new WaitForSeconds(explosionTime);
        GetComponentInChildren<Collider2D>().enabled = false;
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        EnemyHealthScript health = col.gameObject.GetComponent<EnemyHealthScript>();
        if (health == null) return;
        health.DoDamage(explosionDamage);
    }
}
