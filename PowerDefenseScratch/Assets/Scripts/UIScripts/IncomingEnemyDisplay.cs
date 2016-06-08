using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class IncomingWave
{
    public Wave nextWave;
    public float queueTime;
}

public class IncomingEnemyDisplay : MonoBehaviour {
    public GameObject elementContainer;
    public Transform gridContainer;
    public PathNode startPoint;
    public Text titleText;
    public GameObject elePrefab;
    public float spacingTime;

    List<IncomingWave> nextWaves;
    Wave currentWave;
    bool enemiesSpawned;
    float spawnTime;

    void Start()
    {
        nextWaves = new List<IncomingWave>();
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    public bool SetUpDisplay(Wave nextWave)
    {
        if (currentWave == nextWave) return true;
        if (gridContainer.childCount > 0)
        {
            IncomingWave inc = new IncomingWave();
            inc.nextWave = nextWave;
            inc.queueTime = Time.time;
            nextWaves.Add(inc);
            return false;
        }
        currentWave = nextWave;
        Dictionary<GameObject, int> enemiesToSpawn = new Dictionary<GameObject, int>();
        titleText.text = "Enemies Incoming";
        foreach (SubWave sub in nextWave.waves)
        {
            if (startPoint != sub.startNode) continue;
            foreach(GameObject enemy in sub.enemies)
            {
                if (enemiesToSpawn.ContainsKey(enemy))
                {
                    int newVal = enemiesToSpawn[enemy] + 1;
                    enemiesToSpawn.Remove(enemy);
                    enemiesToSpawn.Add(enemy, newVal);
                }
                else
                {
                    enemiesToSpawn.Add(enemy, 1);
                }
            }
        }
        if(enemiesToSpawn.Count == 0)
        {
            return true;
        }

        elementContainer.SetActive(true);
        foreach (KeyValuePair<GameObject, int> entry in enemiesToSpawn)
        {
            GameObject newEle = Instantiate(elePrefab) as GameObject;
            IncomingEnemyElement enEle = newEle.GetComponent<IncomingEnemyElement>();
            if(enEle != null)
            {
                enEle.enemyTracking = entry.Key;
                enEle.SetUpElement(entry.Value, entry.Key.GetComponent<EnemyInfoScript>().icon);
                newEle.transform.SetParent(gridContainer);
            }
        }
        return true;
        //StartCoroutine(PreSpawnCoroutine(nextWave.startDelay));
    }

    public void StartUpTimer(float time)
    {
        StartCoroutine(PreSpawnCoroutine(time));
    }

    void Update()
    {
        if (Time.time > spawnTime) return;
        titleText.text = "Enemies Incoming: " + Mathf.CeilToInt(spawnTime - Time.time).ToString("F0");
    }

    public IEnumerator PreSpawnCoroutine(float time)
    {
        spawnTime = Time.time + time;
        enemiesSpawned = false;
        yield return new WaitForSeconds(time);
        enemiesSpawned = true;
        titleText.text = "Enemies Arrived";
    }

    public void RemoveEnemy(GameObject enemy)
    {
        foreach(IncomingEnemyElement ele in gridContainer.GetComponentsInChildren<IncomingEnemyElement>())
        {
            if(enemy == ele.enemyTracking)
            {
                if (ele.UpdateCounter(1))
                {
                    DestroyImmediate(ele.gameObject);
                    break;
                }
            }
        }
        if(gridContainer.childCount <= 0){
            elementContainer.SetActive(false);
            if (nextWaves.Count > 0) StartCoroutine(PauseDelayCoroutine());
        }
    }

    IEnumerator PauseDelayCoroutine()
    {
        yield return new WaitForSeconds(spacingTime);
        if (nextWaves.Count == 0) yield break;
        IncomingWave newWave = nextWaves[0];
        nextWaves.RemoveAt(0);

        SetUpDisplay(newWave.nextWave);

        float timeSinceCall = Time.time - newWave.queueTime;
        StartUpTimer(newWave.nextWave.startDelay - timeSinceCall);

    }

    public void ClearDisplay()
    {
        currentWave = null;
        StopAllCoroutines();
        nextWaves.Clear();
        foreach(IncomingEnemyElement ele in gridContainer.GetComponentsInChildren<IncomingEnemyElement>())
        {
            DestroyImmediate(ele.gameObject);
        }
        elementContainer.SetActive(false);
    }
}
