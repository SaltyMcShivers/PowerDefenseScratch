using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class SpawnOnDeathInformation
{
    public GameObject enemy;
    public float offset;
    public float spawnDelay;
}

public class EnemyDeathSpawnScript : MonoBehaviour {
    public List<SpawnOnDeathInformation> spawningObjects;
    public float startDelay;

	// Use this for initialization
	void Start ()
    {
        Messenger<GameObject>.AddListener("Destroy Enemy", SpawnChecker);
    }

    void OnDestroy()
    {
        StopAllCoroutines();
        Messenger<GameObject>.RemoveListener("Destroy Enemy", SpawnChecker);
    }

    void SpawnChecker(GameObject go)
    {
        if(go.transform.parent == transform)
        {
            StartCoroutine(SpawningCoroutine());
        }
    }

    IEnumerator SpawningCoroutine()
    {
        yield return new WaitForSeconds(startDelay);
        EnemyMovement sourceMover = GetComponent<EnemyMovement>();
        if (sourceMover == null) yield break;
        foreach(SpawnOnDeathInformation info in spawningObjects)
        {
            StartCoroutine(SubSpawningCoroutine(info));
        }
    }

    IEnumerator SubSpawningCoroutine(SpawnOnDeathInformation spawner)
    {
        yield return new WaitForSeconds(spawner.spawnDelay);
        EnemyMovement sourceMover = GetComponent<EnemyMovement>();
        if (sourceMover == null) yield break;
        GameObject newEnemy = Instantiate(spawner.enemy, transform.position, Quaternion.identity) as GameObject;
        newEnemy.GetComponent<EnemyMovement>().StartFollowing(sourceMover.GetTarget(), spawner.offset + sourceMover.edgeOffset, true);
        Messenger<GameObject>.Invoke("CreepSpawnsEnemy", newEnemy);
    }
}
