using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IncomingEnemyElement : MonoBehaviour {
    public GameObject enemyTracking;

    Image icon;
    Text counter;

    int enemyCount;

    public void SetUpElement(int count, Sprite spr)
    {
        icon = GetComponentInChildren<Image>();
        counter = GetComponentInChildren<Text>();
        icon.sprite = spr;
        UpdateCounter(-count);
    }
	
	// Update is called once per frame
	public bool UpdateCounter (int count) {
        enemyCount -= count;
        if (enemyCount <= 0)
        {
            return true;
        }
        counter.text = "x" + enemyCount.ToString();
        return false;
	}
}
