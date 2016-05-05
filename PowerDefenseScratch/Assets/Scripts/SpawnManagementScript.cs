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

    public List<IncomingEnemyDisplay> incomings;

    int currentWave = -1;
    //int currentEnemy = -1;
    bool allWavesSpawned;

    List<EnemyPartnerManager> partnerManagers;

    List<GameObject> enemyTracker;

    void Awake()
    {
        currentWave = -1;
    }

    // Use this for initialization
    void Start ()
    {
        partnerManagers = new List<EnemyPartnerManager>();
        enemyTracker = new List<GameObject>();
        Messenger<GameObject>.AddListener("Destroy Enemy", RemoveFromTracker);
        Messenger<GameObject>.AddListener("CreepSpawnsEnemy", AddExtraEnemy);
        if (enemyWaves[currentWave + 1].autoAdvance && autoStart) StartCoroutine("SpawnWave");
        else {
            foreach (IncomingEnemyDisplay dis in incomings)
            {
                dis.SetUpDisplay(enemyWaves[currentWave + 1]);
            }
        }
    }

    void AddExtraEnemy(GameObject enemy)
    {
        enemyTracker.Add(enemy);
    }

    void OnDestroy()
    {
        Messenger<GameObject>.RemoveListener("Destroy Enemy", RemoveFromTracker);
        Messenger<GameObject>.RemoveListener("CreepSpawnsEnemy", AddExtraEnemy);
    }

    public Vector2 SkipToWave(int wave)
    {
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
        EnemyPartnerScript partner = enemy.GetComponentInChildren<EnemyPartnerScript>();
        if(partner != null)
        {
            foreach (EnemyPartnerManager man in partnerManagers)
            {
                if (man.HasPartner(partner))
                {
                    man.RemovePartner(partner);
                    if (man.AllPartnersDestroyed())
                    {
                        partnerManagers.Remove(man);
                        Destroy(man);
                    }
                    break;
                }
            }
        }
        if(enemyTracker.Count == 0)
        {
            Messenger.Invoke("WaveCompleted");
            if(allWavesSpawned)
            {
                if(enemyWaves[enemyWaves.Count-1].autoAdvance) Messenger<bool>.Invoke("End Game", true);
            }
            else
            {
                foreach (IncomingEnemyDisplay dis in incomings)
                {
                    dis.SetUpDisplay(enemyWaves[currentWave+1]);
                }
            }
        }
    }

    IEnumerator SpawnWave()
    {
        currentWave++;
        if (currentWave >= enemyWaves.Count)
        {
            if (allWavesSpawned)
            {
                Messenger<bool>.Invoke("End Game", true);
            }
            allWavesSpawned = true;
            yield break;
        }

        foreach(IncomingEnemyDisplay dis in incomings)
        {
            if(dis.SetUpDisplay(enemyWaves[currentWave])) dis.StartUpTimer(enemyWaves[currentWave].startDelay);
        }

        yield return new WaitForSeconds(enemyWaves[currentWave].startDelay);
        foreach (SubWave sw in enemyWaves[currentWave].waves)
        {
            StartCoroutine(SubWaveCoroutine(sw));
        }
        if (currentWave < enemyWaves.Count - 1 && enemyWaves[currentWave + 1].autoAdvance)
        {
            StartCoroutine("SpawnWave");
        }
        else if (currentWave + 1 >= enemyWaves.Count)
        {
            allWavesSpawned = true;
        }
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

        EnemyPartnerScript partner = newEnemy.GetComponentInChildren<EnemyPartnerScript>();
        if(partner != null)
        {
            bool partnerFound = false;
            foreach(EnemyPartnerManager man in partnerManagers)
            {
                if (man.AddPartner(partner))
                {
                    partnerFound = true;
                    break;
                }
            }
            if (!partnerFound)
            {
                EnemyPartnerManager partManager = gameObject.AddComponent(System.Type.GetType(partner.classToAdd)) as EnemyPartnerManager;
                partManager.SetUpManager(partner.GetVariables());
                partManager.AddPartner(partner);
                partnerManagers.Add(partManager);
            }
        }

        foreach (IncomingEnemyDisplay dis in incomings)
        {
            if (dis.startPoint == startPoint)
            {
                dis.RemoveEnemy(enemy);
                break;
            }
        }
        en.StartFollowing(startPoint, offset);
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
