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
    public float endOffset;
}

public class SpawnManagementScript : MonoBehaviour {
    public PathNode startNode;
    public GameObject enemyPrefab;
    public List<Wave> enemyWaves;

    public bool autoAdvance = true;
    public bool autoStart = true;
    public bool autoEnd = true;

    public List<IncomingEnemyDisplay> incomings;

    int currentWave = -1;
    bool disableWaveCheck;
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

    public Vector2 GetWaveResources(int wave)
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
        return res;
    }

    public Vector2 SkipToWave(int wave)
    {
        Vector2 res = GetWaveResources(wave);
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
        if (enemy.GetComponentInParent<EnemyDeathSpawnScript>() != null)
        {
            StopCoroutine("CompleteDisableCoroutine");
            disableWaveCheck = true;
            StartCoroutine("CompleteDisableCoroutine");
        }
        if (enemyTracker.Count == 0)
        {
            if (disableWaveCheck) return;
            Messenger.Invoke("WaveCompleted");
            if(allWavesSpawned && autoEnd)
            {
                if(enemyWaves[enemyWaves.Count-1].autoAdvance) Messenger<bool>.Invoke("End Game", true);
            }
            else
            {
                if (enemyWaves[currentWave].autoAdvance && currentWave + 1 >= enemyWaves.Count) return;
                foreach (IncomingEnemyDisplay dis in incomings)
                {
                    dis.SetUpDisplay(enemyWaves[currentWave+1]);
                }
            }
        }
    }

    IEnumerator CompleteDisableCoroutine()
    {
        yield return new WaitForSeconds(2f);
        disableWaveCheck = false;
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

    IEnumerator SubWaveCoroutine(SubWave sw)
    {
        yield return new WaitForSeconds(sw.startDelay);
        foreach (GameObject em in sw.enemies)
        {
            SpawnEnemy(em, sw.startNode, sw.edgeOffset, sw.endOffset);
            yield return new WaitForSeconds(sw.spawnInterval);
        }
    }

    void SpawnEnemy(GameObject enemy, PathNode startPoint, float offset, float backOffset)
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
        en.StartFollowing(startPoint, offset, backOffset);
        enemyTracker.Add(newEnemy);
    }

    public void SendNextWave()
    {
        if (allWavesSpawned && autoEnd) Messenger<bool>.Invoke("End Game", true);
        else StartCoroutine("SpawnWave");
    }

    public void ClearAllEnemies()
    {
        StopAllCoroutines();
        allWavesSpawned = false;
        foreach(GameObject enemy in enemyTracker)
        {
            Destroy(enemy);
        }
        enemyTracker.Clear();
        foreach (EnemyPartnerManager man in partnerManagers)
        {
            Destroy(man);
        }
        partnerManagers.Clear();
        foreach (IncomingEnemyDisplay dis in incomings)
        {
            dis.ClearDisplay();
            dis.SetUpDisplay(enemyWaves[currentWave + 1]);
        }
    }
}
