using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct SubSpawnStats
{
    public GameObject spawnObject;
    public Vector3 spawnOffset;
}

public class EnemySpawnerScript : MonoBehaviour {
    public List<SubSpawnStats> spawners;
    public Transform spawnLocation;
    public float spawnInterval;
    public float spawnTime;
    public EnemyMovement mover;

    public float spawnTimePerEnemy;

    float movementStartTime;
    bool activatedEMP;
    IEnumerator spawnCoroutine;

    List<GameObject> enemies;


	// Use this for initialization
    void Start()
    {
        enemies = new List<GameObject>();
        movementStartTime = Time.time;
        spawnCoroutine = SpawningCoroutine(spawnInterval);
        StartCoroutine(spawnCoroutine);
        Messenger<GameObject>.AddListener("Destroy Enemy", CheckDeath);
        //Messenger<float>.AddListener("Launch EMP", EMPAction);
    }

    void OnDestroy()
    {
        Messenger<GameObject>.RemoveListener("Destroy Enemy", CheckDeath);
        //Messenger<float>.RemoveListener("Launch EMP", EMPAction);
    }

    IEnumerator SpawningCoroutine(float movementTime)
    {
        yield return new WaitForSeconds(movementTime);
        if (mover != null) mover.SetPauseMovement(true);
        float preSpawnTime = spawnTime - (spawners.Count * spawnTimePerEnemy);
        yield return new WaitForSeconds(preSpawnTime);
        foreach (SubSpawnStats spawn in spawners)
        {
            GameObject enemySpawned = Instantiate(spawn.spawnObject, spawnLocation.position + spawn.spawnOffset, Quaternion.identity) as GameObject;
            if (spawnLocation == transform)
            {
                enemySpawned.transform.SetParent(spawnLocation);
                enemies.Add(enemySpawned);
            }
            Messenger<GameObject>.Invoke("CreepSpawnsEnemy", enemySpawned);
            yield return new WaitForSeconds(spawnTimePerEnemy);
        }
        if (activatedEMP) yield break;
        if (mover != null) mover.SetPauseMovement(false);
        movementStartTime = Time.time;
        spawnCoroutine = SpawningCoroutine(spawnInterval);
        StartCoroutine(spawnCoroutine);
    }

    void CheckDeath(GameObject go)
    {
        if (go.transform.parent == transform)
        {
            foreach (GameObject en in enemies)
            {
                en.GetComponentInChildren<EnemyHealthScript>().KillEnemy();
            }
            StopAllCoroutines();
            Messenger<GameObject>.RemoveListener("Destroy Enemy", CheckDeath);
            return;
        }
        foreach (GameObject child in enemies)
        {
            if (child == go.transform.parent.gameObject)
            {
                enemies.Remove(child);
                return;
            }
        }
    }

    public void EMPAction(float disableTime)
    {
        StartCoroutine(EMPCoroutine(disableTime));
    }

    IEnumerator EMPCoroutine(float disableTime)
    {
        activatedEMP = true;
        if (mover == null) yield break;
        float moveOffset;
        if (mover.IsMovementPaused())
        {
            moveOffset = spawnInterval;
        }
        else
        {
            moveOffset = spawnInterval - Time.time + movementStartTime;
            StopCoroutine(spawnCoroutine);
        }
        yield return new WaitForSeconds(disableTime);
        activatedEMP = false;
        movementStartTime = Time.time - moveOffset;
        spawnCoroutine = SpawningCoroutine(moveOffset);
        StartCoroutine(spawnCoroutine);
    }
}
