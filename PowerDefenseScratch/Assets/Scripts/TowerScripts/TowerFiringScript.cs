using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerFiringScript : MonoBehaviour {
    public enum TowerDamageType
    {
        Physical,
        Electric,
        Status
    };

    public TowerRangeScript rangeManagement;
    public TowerPowerScript powerManagement;

    public float minimumShotInterval = 1.0f;
    public float maximumShotInterval = 1.0f;

    public float explosionTime;
    public float minimumExplosionSize;
    public float maximumExplosionSize;

    public GameObject projectilePrefab;
    public float projectileForce;
    float lastShotTime = 0;

    public float minimumProjectileDamage;
    public float maximumProjectileDamage;

    public float startingCritChance;
    public float critIncement;

    public TowerDamageType damageType;
    float currentCritChance;

    public float maximumComboBonus;
    public float minimumComboBuildFactor;
    public float maximumComboBuildFactor;

    public int targetsToHit;

    public float minimumSlow;
    public float maximumSlow;

    float currentComboBonus;
    GameObject lastEnemyHit;
    
    void Start()
    {
        currentCritChance = startingCritChance;
        currentComboBonus = 1f;
	}
	
	void Update () {
        if (powerManagement.GetCurrentPower() == 0) return;

        rangeManagement.AlterScale(GetPowerPercent());

        float shotInterval = minimumShotInterval;
        if (minimumShotInterval != maximumShotInterval)
        {
            shotInterval = Mathf.Lerp(maximumShotInterval, minimumShotInterval, GetPowerPercent());
        }
	    if(Time.time - lastShotTime > shotInterval)
        {
            FireAtTargets();
        }
	}

    void FireAtTargets()
    {
        List<GameObject> targets = rangeManagement.GetEnemies(targetsToHit);
        if (targets.Count == 0)
        {
            lastEnemyHit = null;
            return;
        }
        lastShotTime = Time.time;
        Vector3 distance = Vector3.Normalize(targets[0].transform.position - transform.position);
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity) as GameObject;
        float projectileDamage = minimumProjectileDamage;
        if (minimumProjectileDamage != maximumProjectileDamage)
        {
            projectileDamage = Mathf.Lerp(minimumProjectileDamage, maximumProjectileDamage, GetPowerPercent());
            if (maximumComboBonus > 1f)
            {
                if (lastEnemyHit == null || lastEnemyHit != targets[0])
                {
                    currentComboBonus = 1f;
                    lastEnemyHit = targets[0];
                }else{
                    currentComboBonus = Mathf.Min(currentComboBonus + Mathf.Lerp(minimumComboBuildFactor, maximumComboBuildFactor, GetPowerPercent()), maximumComboBonus); 
                }
                projectileDamage *= currentComboBonus;
            }
        }
        float explodeSize = minimumExplosionSize;
        projectile.GetComponent<Rigidbody2D>().AddForce(projectileForce * distance);
        if(minimumExplosionSize != maximumExplosionSize){
            explodeSize = Mathf.Lerp(minimumExplosionSize, maximumExplosionSize, GetPowerPercent());
        }

        float slowAmount = Mathf.Lerp(minimumSlow, maximumSlow, GetPowerPercent());
        (projectile.GetComponent<ProjectileScript>() as ProjectileScript).SetUpBullet(projectileDamage, explosionTime, explodeSize, slowAmount);
        EnemyHealthScript health = targets[0].GetComponentInChildren<EnemyHealthScript>();
        if(minimumExplosionSize == 0f) health.DamageWithDelay(projectileDamage, distance.magnitude / projectileForce);
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

    public float GetPowerPercent()
    {
        return powerManagement.GetCurrentPower() / 100f;
    }
}
