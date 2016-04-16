using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerFiringScript : MonoBehaviour {
    public TowerRangeScript rangeManagement;
    public TowerPowerScript powerManagement;

    public float minimumShotInterval = 1.0f;
    public float maximumShotInterval = 1.0f;

    public GameObject projectilePrefab;
    public float projectileForce;
    float lastShotTime = 0;

    public float minimumProjectileDamage;
    public float maximumProjectileDamage;

    public float startingCritChance;
    public float critIncement;
    float currentCritChance;

	// Use this for initialization
    void Start()
    {
        currentCritChance = startingCritChance;
	}
	
	// Update is called once per frame
	void Update () {
        if (powerManagement.GetCurrentPower() == 0) return;
        float shotInterval = minimumShotInterval;
        if(minimumShotInterval != maximumShotInterval)
        {
            shotInterval = Mathf.Lerp(maximumShotInterval, minimumShotInterval, powerManagement.GetCurrentPower() / 100f);
        }
	    if(Time.time - lastShotTime > shotInterval)
        {
            FireAtTargets();
        }
	}

    void FireAtTargets()
    {
        List<GameObject> targets = rangeManagement.GetEnemies(1);
        if (targets.Count == 0) return;
        lastShotTime = Time.time;
        Vector3 distance = Vector3.Normalize(targets[0].transform.position - transform.position);
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity) as GameObject;
        float projectileDamage = minimumProjectileDamage;
        if (minimumProjectileDamage != maximumProjectileDamage)
        {
            if (critIncement == 0f)
            {
                projectileDamage = Mathf.Lerp(minimumProjectileDamage, maximumProjectileDamage, powerManagement.GetCurrentPower() / 100f);
            }
            else
            {
                if (CheckIfCrit())
                {
                    projectileDamage = maximumProjectileDamage;
                }
            }
        }
        (projectile.GetComponent<ProjectileScript>() as ProjectileScript).projectileDamage = projectileDamage;
        projectile.GetComponent<Rigidbody2D>().AddForce(projectileForce * distance);
        EnemyHealthScript health = targets[0].GetComponentInChildren<EnemyHealthScript>();
        health.DamageWithDelay(projectileDamage, distance.magnitude / projectileForce);
    }

    bool CheckIfCrit()
    {
        if (Random.Range(0f, 100f) < currentCritChance)
        {
            currentCritChance = startingCritChance;
            return true;
        }
        currentCritChance += Mathf.Lerp(0f, critIncement, powerManagement.GetCurrentPower());
        return false;
    }
}
