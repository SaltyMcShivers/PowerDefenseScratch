using UnityEngine;
using System.Collections;

public class EnemyAOEScript : MonoBehaviour {
    public GameObject slowEffectPrefab;
    public float explosionSize;
    public float speedUpAmount;
    public float fireRate;


    void Start()
    {
        StartCoroutine(SlowDownCoroutine());
        Messenger<GameObject>.AddListener("Destroy Enemy", CheckDeath);
    }

    void OnDestroy()
    {
        Messenger<GameObject>.RemoveListener("Destroy Enemy", CheckDeath);
    }

    IEnumerator SlowDownCoroutine()
    {
        GameObject newBoom = Instantiate(slowEffectPrefab, transform.position + Vector3.forward, Quaternion.identity) as GameObject;
        newBoom.transform.SetParent(transform);
        ExplosionScript boomCS = newBoom.GetComponent<ExplosionScript>();
        boomCS.SetUpExplosion(0f, explosionSize, speedUpAmount);
        yield return new WaitForSeconds(fireRate);
        StartCoroutine(SlowDownCoroutine());
    }

    void CheckDeath(GameObject go)
    {
        if (go.transform.parent == transform)
        {
            StopAllCoroutines();
        }
    }
}
