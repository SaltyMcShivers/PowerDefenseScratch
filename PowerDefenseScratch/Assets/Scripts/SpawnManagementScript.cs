using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Wave
{
    public float startDelay;
    public List<SubWave> waves;
    public bool autoAdvance = true;
}

[System.Serializable]
public class SubWave
{
    public float startDelay;
    public List<GameObject> enemies;
    public float spawnInterval;
    public PathNode startNode;
    public float edgeOffset;
}

public class SpawnManagementScript : MonoBehaviour {
    public PathNode startNode;
    public GameObject enemyPrefab;
    public List<Wave> enemyWaves;

    public bool autoAdvance = true;
    public bool autoStart = true;

    int currentWave = -1;
    int currentEnemy = -1;

    public List<GameObject> enemyTracker;

    // Use this for initialization
    void Start ()
    {
        enemyTracker = new List<GameObject>();
        Messenger<GameObject>.AddListener("Destroy Enemy", RemoveFromTracker);
        if (enemyWaves[currentWave + 1].autoAdvance && autoStart) StartCoroutine("SpawnWave");
    }

    public Vector2 SkipToWave(int wave)
    {
        TowerManagerScript towerSystem = GameObject.Find("TowerManager").GetComponent<TowerManagerScript>();
        Vector2 res = Vector2.zero;
        for (int i = 0; i < wave; i++)
        {
            foreach (SubWave sub in enemyWaves[i].waves)
            {
                foreach (GameObject enemy in sub.enemies)
                {
                    EnemyHealthScript health = enemy.GetComponentInChildren<EnemyHealthScript>();
                    res += new Vector2(health.droppedEnergy, health.droppedMetal);
                }
            }
        }
        currentWave = wave - 1;
        return res;
    }

    void RemoveFromTracker(GameObject enemy)
    {
        enemyTracker.Remove(enemy.transform.parent.gameObject);
        if(enemyTracker.Count == 0)
        {
            Messenger.Invoke("WaveCompleted");
        }
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
        if (currentWave < enemyWaves.Count - 1 && enemyWaves[currentWave + 1].autoAdvance) StartCoroutine("SpawnWave");
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
            SpawnEnemy(em, sw.startNode, sw.edgeOffset);
            yield return new WaitForSeconds(sw.spawnInterval);
        }
    }

    void SpawnEnemy(GameObject enemy, PathNode startPoint, float offset)
    {
        GameObject newEnemy = Instantiate(enemy, startPoint.transform.position, Quaternion.identity) as GameObject;
        EnemyMovement en = newEnemy.GetComponent<EnemyMovement>();
        if (en == null)
        {
            Debug.Log("Can't find EnemyMovementScript");
            return;
        }
        en.StartFollowing(startPoint, offset);
        newEnemy.transform.position = startPoint.GetPathTarget(en);
        enemyTracker.Add(newEnemy);
    }

    public void SendNextWave()
    {
        StartCoroutine("SpawnWave");
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
