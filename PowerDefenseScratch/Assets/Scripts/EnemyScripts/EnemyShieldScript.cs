using UnityEngine;
using System.Collections;

public class EnemyShieldScript : MonoBehaviour {
    public TowerFiringScript.TowerDamageType resistType;
    public float damageResistance;

    EnemyHealthScript enemy;

    void Start()
    {
        enemy = transform.parent.gameObject.GetComponentInChildren<EnemyHealthScript>();
        enemy.SetResistance(0.0f, resistType);
        Messenger<GameObject>.AddListener("Destroy Enemy", CheckDeath);
    }

    void OnDestroy()
    {
        Messenger<GameObject>.RemoveListener("Destroy Enemy", CheckDeath);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (transform.parent == col.transform.parent) return;
        if (enemy.IsDead()) return;
        EnemyHealthScript en = col.gameObject.GetComponent<EnemyHealthScript>();
        if(en != null) en.SetResistance(damageResistance, resistType);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (transform.parent == col.transform.parent) return;
        EnemyHealthScript en = col.gameObject.GetComponent<EnemyHealthScript>();
        if (en != null) en.SetResistance(1.0f / damageResistance, resistType);
    }

    void CheckDeath(GameObject go)
    {
        if(go == enemy.gameObject)
        {
            transform.localScale = Vector3.zero;
        }
    }
}
