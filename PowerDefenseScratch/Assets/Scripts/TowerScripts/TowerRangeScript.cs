using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TowerRangeScript : MonoBehaviour {
    List<GameObject> enemies;
    GameObject targetedEnemy;

	// Use this for initialization
    void Awake()
    {
        enemies = new List<GameObject>();
    }

	void Start () {
        Messenger<GameObject>.AddListener("Destroy Enemy", RemoveEnemy);
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        EnemyHealthScript em = col.gameObject.GetComponent<EnemyHealthScript>();
        if (em == null) return;
        if (em.IsDead()) return;
        enemies.Add(col.gameObject);
        if(targetedEnemy == null)
        {
            targetedEnemy = col.gameObject;
        }
        enemies.Sort(delegate(GameObject a, GameObject b){
            int res = a.GetComponent<EnemyHealthScript>().GetPriorityLevel().CompareTo(b.GetComponent<EnemyHealthScript>().GetPriorityLevel());
            if (res == 0) return enemies.IndexOf(a).CompareTo(enemies.IndexOf(b));
            return res;
        });
    }

    void OnTriggerExit2D(Collider2D col)
    {
        RemoveEnemy(col.gameObject);
    }

    public List<GameObject> GetEnemies(int i)
    {
        EnemyMantenance();
        if (i >= enemies.Count)
        {
            return enemies;
        }
        return enemies.GetRange(0, i);
    }

    void RemoveEnemy(GameObject go)
    {
        enemies.Remove(go);
        if (targetedEnemy == go)
        {
            if (enemies.Count == 0)
            {
                targetedEnemy = null;
            }
            else
            {
                targetedEnemy = enemies[0];
            }
        }
    }

    void OnMouseDown()
    {
        Debug.Log("Thought So");
    }

    void EnemyMantenance()
    {
        foreach (GameObject em in enemies)
        {
            if (em == null) enemies.Remove(em);
        }
    }
}
