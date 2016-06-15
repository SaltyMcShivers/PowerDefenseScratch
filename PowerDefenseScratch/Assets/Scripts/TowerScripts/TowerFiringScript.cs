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

    public float minimumComboBonus = 1;
    public float maximumComboBonus;
    public float minimumComboBuildFactor;
    public float maximumComboBuildFactor;

    public int targetsToHit;

    public float minimumSlow;
    public float maximumSlow;

    int currentComboBonus;
    GameObject lastEnemyHit;

    public Transform turretJoint;
    public Transform projectileSource;
    public List<Transform> projectileSources;
    
    void Start()
    {
        currentCritChance = startingCritChance;
        currentComboBonus = 0;
        if (powerManagement.GetCurrentPower() == 0)
        {
            rangeManagement.DisableTower();
        }
        if(projectileSources.Count == 0 && projectileSource != null)
        {
            projectileSources.Add(projectileSource);
        }
    }
	
	void Update () {
        if (powerManagement.GetCurrentPower() == 0)
        {
            rangeManagement.DisableTower();
            return;
        }

        rangeManagement.EnableTower();

        rangeManagement.AlterScale(GetPowerPercent());

        if (turretJoint != null)
        {
            CalculateModelRotations();
        }

        float shotInterval = minimumShotInterval;
        if (minimumShotInterval != maximumShotInterval)
        {
            shotInterval = Mathf.Lerp(maximumShotInterval, minimumShotInterval, GetPowerPercent());
        }
        float timeSinceLastShot = Time.time - lastShotTime;
        if (timeSinceLastShot > shotInterval)
        {
            if (timeSinceLastShot > shotInterval * 2)
            {
                FireAtTargets();
            }
            else
            {
                FireAtTargets(timeSinceLastShot - shotInterval);
            }

        }
	}

    void CalculateModelRotations()
    {
        List<GameObject> targets = rangeManagement.GetEnemies(targetsToHit);
        if (targets.Count == 0) return;
        float rotDifference = Vector3.Angle(Vector3.left, transform.position - targets[0].transform.position) * -Mathf.Sign(transform.position.y - targets[0].transform.position.y);
        turretJoint.localEulerAngles = new Vector3(rotDifference, 0f, 0f);
    }

    void FireAtTargets(float overTime=0f)
    {
        List<GameObject> targets = rangeManagement.GetEnemies(targetsToHit);
        if (targets.Count == 0)
        {
            lastEnemyHit = null;
            return;
        }
        lastShotTime = Time.time - overTime;
        Vector3 targetPosition;
        EnemyMovement enMov = targets[0].GetComponentInParent<EnemyMovement>();
        if(enMov == null)
        {
            targetPosition = targets[0].transform.position;
        }
        else
        {
            targetPosition = enMov.PredictPosition(0.2f);
        }
        Transform sourceForShot = transform;
        if (projectileSources.Count > 0)
        {
            sourceForShot = projectileSources[0];
            projectileSources.RemoveAt(0);
            projectileSources.Add(sourceForShot);
        }

        if(sourceForShot.gameObject.GetComponentInChildren<ParticleSystem>() != null)
        {
            sourceForShot.gameObject.GetComponentInChildren<ParticleSystem>().Play();
        }

        Vector3 distance = Vector3.Normalize(targetPosition - sourceForShot.position);
        
        GameObject projectile = Instantiate(projectilePrefab, sourceForShot.position, Quaternion.identity) as GameObject;

        projectile.transform.Rotate(Vector3.back, Vector3.Angle(Vector3.left, distance) * Mathf.Sign(distance.y) + 180);

        float projectileDamage = minimumProjectileDamage;
        if (minimumProjectileDamage != maximumProjectileDamage)
        {
            projectileDamage = Mathf.Lerp(minimumProjectileDamage, maximumProjectileDamage, GetPowerPercent());
            if (maximumComboBonus > 1f)
            {
                float comboBonus;
                if (lastEnemyHit == null || lastEnemyHit != targets[0])
                {
                    currentComboBonus = 0;
                    comboBonus = 1f;
                    lastEnemyHit = targets[0];
                }else{
                    float percent = GetPowerPercent();
                    currentComboBonus = Mathf.FloorToInt(Mathf.Min(currentComboBonus + 1, 5));
                    comboBonus = 1 + currentComboBonus * Mathf.Lerp(minimumComboBuildFactor, maximumComboBuildFactor, percent);
                }
                projectileDamage *= comboBonus;
            }
        }
        float explodeSize = minimumExplosionSize;
        //projectile.GetComponent<Rigidbody2D>().AddForce(projectileForce * distance);
        if(minimumExplosionSize != maximumExplosionSize){
            explodeSize = Mathf.Lerp(minimumExplosionSize, maximumExplosionSize, GetPowerPercent());
        }

        float slowAmount = Mathf.Lerp(minimumSlow, maximumSlow, GetPowerPercent());
        ProjectileScript proj = projectile.GetComponent<ProjectileScript>();
        proj.SetUpBullet(projectileDamage, explosionTime, explodeSize, slowAmount, GetPowerPercent(), targets[0].GetComponentInChildren<Rigidbody2D>().gameObject);
        if (explosionTime < 0.2f) proj.SetSpeed(projectileForce / 50f);
        else
        {
            proj.SetSpeed(distance.magnitude / explosionTime);
        }
        EnemyHealthScript health = targets[0].GetComponentInChildren<EnemyHealthScript>();
        //if(minimumExplosionSize == 0f) health.DamageWithDelay(projectileDamage, distance.magnitude / projectileForce);
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
