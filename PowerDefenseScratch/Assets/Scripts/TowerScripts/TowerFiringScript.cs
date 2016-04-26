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

    float currentComboBonus;
    GameObject lastEnemyHit;

	// Use this for initialization
    void Start()
    {
        currentCritChance = startingCritChance;
        currentComboBonus = 1f;
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
            projectileDamage = Mathf.Lerp(minimumProjectileDamage, maximumProjectileDamage, powerManagement.GetCurrentPower() / 100f);
            if (maximumComboBonus > 1f)
            {
                if (lastEnemyHit == null || lastEnemyHit != targets[0])
                {
                    currentComboBonus = 1f;
                    lastEnemyHit = targets[0];
                }else{
                    currentComboBonus = Mathf.Min(currentComboBonus + Mathf.Lerp(minimumComboBuildFactor, maximumComboBuildFactor, powerManagement.GetCurrentPower() / 100f), maximumComboBonus); 
                }
                projectileDamage *= currentComboBonus;
            }
        }
        float explodeSize = minimumExplosionSize;
        float timeToExplode = 0f;
        projectile.GetComponent<Rigidbody2D>().AddForce(projectileForce * distance);
        if(minimumExplosionSize != maximumExplosionSize){
            explodeSize = Mathf.Lerp(minimumExplosionSize, maximumExplosionSize, powerManagement.GetCurrentPower() / 100f);
        }
        (projectile.GetComponent<ProjectileScript>() as ProjectileScript).SetUpBullet(projectileDamage, explosionTime, explodeSize);
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
}
