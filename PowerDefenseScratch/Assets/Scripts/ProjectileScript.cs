using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ParticleAdjustments
{
    public ParticleSystem sys;
    public Color32 lowColor;
    public Color32 highColor;
    public float lowSize;
    public float highSize;
}

public class ProjectileScript : MonoBehaviour {
    public float projectileDamage;
    float detonationTime = 0f;
    float explosionRadius;
    float slowDownRate;
    GameObject target;
    public GameObject deathPrefab;
    public List<ParticleAdjustments> parts;
    float bulletSpeed;

    public void SetSpeed(float speed)
    {
        bulletSpeed = speed;
    }

    public void SetUpBullet(float damage, float boom, float rad, float slow, float pow=0, GameObject targ=null)
    {
        projectileDamage = damage;
        detonationTime = boom;
        explosionRadius = rad;
        slowDownRate = slow;
        target = targ;
        if (explosionRadius != 0f){
            GetComponent<Collider2D>().enabled = false;
            StartCoroutine(ExplodeCoroutine());
        }
        foreach(ParticleAdjustments part in parts)
        {
            part.sys.startColor = Color32.Lerp(part.lowColor, part.highColor, pow);
            part.sys.startSize = Mathf.Lerp(part.lowSize, part.highSize, pow);
            part.sys.Play();
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
        if(deathPrefab != null)
        {
            if(deathPrefab.GetComponent<ExplosionScript>() == null)
            {
                GameObject deadObject = Instantiate(deathPrefab, transform.position + Vector3.forward, Quaternion.identity) as GameObject;
                deadObject.transform.eulerAngles = transform.eulerAngles + Vector3.back * 180f;
                Destroy(deadObject, 1f);
            }
        }
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (target == null) return;
        if (target != col.gameObject) return;
        if (deathPrefab != null)
        {
            if (deathPrefab.GetComponent<ExplosionScript>() == null)
            {
                GameObject deadObject = Instantiate(deathPrefab, transform.position + Vector3.forward, Quaternion.identity) as GameObject;
                deadObject.transform.eulerAngles = transform.eulerAngles + Vector3.back * 180f;
                Destroy(deadObject, 2f);
            }
        }
        EnemyHealthScript health = target.GetComponent<EnemyHealthScript>();
        if(health != null)
        {
            health.DoDamage(projectileDamage);
        }
        Destroy(gameObject);
    }

    void Update()
    {
        if (bulletSpeed == 0) return;
        Vector3 trajectory = (target.transform.position - transform.position).normalized;
        transform.position += trajectory * bulletSpeed * Time.deltaTime;
    }
}
