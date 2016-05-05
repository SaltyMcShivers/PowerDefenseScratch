using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//This should only appear for UI; remove when it loads
public class EnemyInfoScript : MonoBehaviour {

    public Sprite icon;

	void Start () {
        Destroy(this);
	}
}
