using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TowerRangeScript : MonoBehaviour {
    public float minimumRange;
    public float maximumRange;

    public Color activeColorTemp;
    public Color inactiveColorTemp;

    List<GameObject> enemies;
    GameObject targetedEnemy;

    bool towerActive;
    
    void Awake()
    {
        enemies = new List<GameObject>();
        EnableTower();
    }

	void Start () {
        Messenger<GameObject>.AddListener("Destroy Enemy", RemoveEnemy);
        Messenger.AddListener("Priorities Changed", ReoganizeEnemies);
        SetScale();
    }

    void OnDestroy()
    {
        Messenger<GameObject>.RemoveListener("Destroy Enemy", RemoveEnemy);
        Messenger.RemoveListener("Priorities Changed", ReoganizeEnemies);
    }

    public void SetScale()
    {
        transform.localScale = Vector3.one * minimumRange;
    }

    public void AlterScale(float power)
    {
        if (minimumRange == maximumRange) return;
        transform.localScale = Vector3.one * Mathf.Lerp(minimumRange, maximumRange, power);
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
        ReoganizeEnemies();
    }

    void ReoganizeEnemies()
    {
        enemies.Sort(delegate (GameObject a, GameObject b) {
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

    public void DisableTower()
    {
        if (!towerActive) return;
        towerActive = false;
        GetComponent<SpriteRenderer>().color = inactiveColorTemp;
    }

    public void EnableTower()
    {
        if (towerActive) return;
        towerActive = true;
        GetComponent<SpriteRenderer>().color = activeColorTemp;
    }
}
