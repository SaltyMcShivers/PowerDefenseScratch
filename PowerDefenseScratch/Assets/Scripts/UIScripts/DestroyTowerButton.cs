using UnityEngine;
using System.Collections;

public class DestroyTowerButton : MonoBehaviour {

    public GameObject onButton;
    public GameObject offButton;

	// Use this for initialization
	void Start () {
        Messenger<TowerPowerScript>.AddListener("Tower Destroyed", TowerDeleted);
	}

    void OnDestroy()
    {
        Messenger<TowerPowerScript>.AddListener("Tower Destroyed", TowerDeleted);
    }

    public bool DeletingTower()
    {
        return gameObject.activeSelf && offButton.activeSelf;
    }

    void TowerDeleted(TowerPowerScript pow)
    {
        if (gameObject.activeSelf)
        {
            onButton.SetActive(true);
            offButton.SetActive(false);
        }
    }
}
