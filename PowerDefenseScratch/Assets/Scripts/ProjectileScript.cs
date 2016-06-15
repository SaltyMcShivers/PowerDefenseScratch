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
    float energyPower;

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
        energyPower = pow;
        foreach (ParticleAdjustments part in parts)
        {
            part.sys.startColor = Color32.Lerp(part.lowColor, part.highColor, energyPower);
            part.sys.startSize = Mathf.Lerp(part.lowSize, part.highSize, energyPower);
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
            DeathParticleScript deathPart = deadObject.GetComponent<DeathParticleScript>();
            if (deathPart != null)
            {
                deathPart.ScaleParticles(energyPower);
            }
        }
        ClearProjectile();
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
                DeathParticleScript deathPart = deadObject.GetComponent<DeathParticleScript>();
                if (deathPart != null)
                {
                    deathPart.ScaleParticles(energyPower);
                }
            }
        }
        ClearProjectile();
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
                DeathParticleScript deathPart = deadObject.GetComponent<DeathParticleScript>();
                if (deathPart != null)
                {
                    deathPart.ScaleParticles(energyPower);
                }
            }
        }
        EnemyHealthScript health = target.GetComponent<EnemyHealthScript>();
        if(health != null)
        {
            health.DoDamage(projectileDamage);
        }
        ClearProjectile();
    }

    void Start() {
        Messenger.AddListener("ResetWave", ClearTarget);
    }

    void OnDestroy()
    {
        Messenger.RemoveListener("ResetWave", ClearTarget);
    }

    void ClearTarget()
    {
        DestroyImmediate(this.gameObject);
    }

    void Update()
    {
        if (bulletSpeed == 0) return;
        Vector3 trajectory = (target.transform.position - transform.position).normalized;
        transform.position += trajectory * bulletSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0f, 0f, Vector3.Angle(Vector3.left, transform.position - target.transform.position) * Mathf.Sign(target.transform.position.y - transform.position.y));
    }

    void ClearProjectile()
    {
        GetComponent<Collider2D>().enabled = false;
        bulletSpeed = 0;
        foreach(ParticleSystem part in GetComponentsInChildren<ParticleSystem>())
        {
            part.Stop();
        }
        foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
        {
            mesh.enabled = false;
        }
        Destroy(gameObject, 1.2f);
    }
}
