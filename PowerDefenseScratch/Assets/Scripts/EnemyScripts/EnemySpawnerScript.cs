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

    List<GameObject> enemies;


	// Use this for initialization
    void Start()
    {
        enemies = new List<GameObject>();
        StartCoroutine(SpawningCoroutine());
        Messenger<GameObject>.AddListener("Destroy Enemy", CheckDeath);
	}

    void OnDestroy()
    {
        Messenger<GameObject>.RemoveListener("Destroy Enemy", CheckDeath);
    }

    IEnumerator SpawningCoroutine()
    {
        yield return new WaitForSeconds(spawnInterval);
        if (mover != null) mover.TogglePauseMovement();
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
            yield return new WaitForSeconds(spawnTimePerEnemy);
        }
        if (mover != null) mover.TogglePauseMovement();
        StartCoroutine(SpawningCoroutine());
    }

    void CheckDeath(GameObject go)
    {
        if (go.transform.parent == transform)
        {
            foreach (GameObject en in enemies)
            {
                en.transform.SetParent(null);
                EnemyMovement childMove = en.GetComponent<EnemyMovement>();
                if (childMove != null)
                {
                    childMove.TogglePauseMovement();
                }
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
}
