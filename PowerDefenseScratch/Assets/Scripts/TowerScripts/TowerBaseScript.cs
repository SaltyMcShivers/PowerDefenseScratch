using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerBaseScript : MonoBehaviour {
    public List<GameObject> buildables;
    TowerPowerScript currentTower;

    public GameObject towerSelectionUI;

    public Canvas menuContainer;

    void OnMouseDown()
    {
        if (currentTower == null && menuContainer.transform.childCount == 0)
        {
            GameObject newTS = Instantiate(towerSelectionUI, transform.position, Quaternion.identity) as GameObject;
            newTS.transform.SetParent(menuContainer.transform);
            newTS.GetComponent<TowerSelectionScript>().SetUpTowerSelection(this, buildables);
            /*
            GameObject newTower = Instantiate(towerToBuild, transform.position, Quaternion.identity) as GameObject;
            currentTower = newTower.GetComponent<TowerPowerScript>();
            Messenger<TowerPowerScript>.Invoke("Tower Built", currentTower);
            */
        }
        else
        {
            currentTower.TogglePower();
        }
    }

    public void BuildTower(GameObject tower)
    {
        GameObject newTower = Instantiate(tower, transform.position, Quaternion.identity) as GameObject;
        currentTower = newTower.GetComponent<TowerPowerScript>();
        Messenger<TowerPowerScript>.Invoke("Tower Built", currentTower);
    }
}
