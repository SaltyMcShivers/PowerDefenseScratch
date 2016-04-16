using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Wave
{
    public float startDelay;
    public List<SubWave> waves;
}

[System.Serializable]
public class SubWave
{
    public float startDelay;
    public List<GameObject> enemies;
    public float spawnInterval;
    public PathNode startNode;
}

public class SpawnManagementScript : MonoBehaviour {
    public PathNode startNode;
    public GameObject enemyPrefab;
    public List<Wave> enemyWaves;
    int currentWave = -1;
    int currentEnemy = -1;

	// Use this for initialization
	void Start ()
    {
        StartCoroutine("SpawnWave");
    }

    IEnumerator SpawnWave()
    {
        currentWave++;
        if (currentWave >= enemyWaves.Count) yield break;
        yield return new WaitForSeconds(enemyWaves[currentWave].startDelay);
        foreach (SubWave sw in enemyWaves[currentWave].waves)
        {
            StartCoroutine(SubWaveCoroutine(sw));
        }
        StartCoroutine("SpawnWave");
    }

    /*
    IEnumerator SpawnCoroutine()
    {
        currentEnemy++;
        if (currentEnemy >= enemyWaves[currentWave].enemies.Count)
        {
            currentEnemy = -1;
            StartCoroutine("SpawnWave");
            yield break;
        }
        SpawnEnemy();
        yield return new WaitForSeconds(enemyWaves[currentWave].spawnInterval);
        StartCoroutine("SpawnCoroutine");
    }
    */

    IEnumerator SubWaveCoroutine(SubWave sw)
    {
        yield return new WaitForSeconds(sw.startDelay);
        foreach (GameObject em in sw.enemies)
        {
            SpawnEnemy(em, sw.startNode);
            yield return new WaitForSeconds(sw.spawnInterval);
        }
    }

    void SpawnEnemy(GameObject enemy, PathNode startPoint)
    {
        GameObject newEnemy = Instantiate(enemy, startPoint.transform.position, Quaternion.identity) as GameObject;
        EnemyMovement en = newEnemy.GetComponent<EnemyMovement>();
        if (en == null)
        {
            Debug.Log("Can't find EnemyMovementScript");
            return;
        }
        en.StartFollowing(startPoint);
    }
    /*
    void SpawnEnemy()
    {
        GameObject newEnemy = Instantiate(enemyWaves[currentWave].enemies[currentEnemy], startNode.transform.position, Quaternion.identity) as GameObject;
        EnemyMovement en = newEnemy.GetComponent<EnemyMovement>();
        if(en == null)
        {
            Debug.Log("Can't find EnemyMovementScript");
            return;
        }
        en.StartFollowing(startNode);
    }
    */
}
