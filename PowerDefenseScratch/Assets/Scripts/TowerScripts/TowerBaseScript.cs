using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerBaseScript : MonoBehaviour {
    public TowerManagerScript manager;

    TowerPowerScript currentTower;

    public GameObject towerSelectionUI;

    public Transform menuContainer;
    public GameObject towerReadyIcon;

    public ElectricPathNode electricSource;

    bool preBuiltTower;
    bool disabled;

    void Start()
    {
        StartCoroutine("SetUpPreBuiltTower");
        Messenger.AddListener("Switch Menu Added", DisableInteraction);
        Messenger.AddListener("Switch Menu Removed", EnableInteraction);
    }

    public bool IsGettingPower()
    {
        return electricSource.IsGettingEnergy();
    }

    IEnumerator SetUpPreBuiltTower()
    {
        yield return new WaitForFixedUpdate();
        TowerPowerScript pow = GetComponentInChildren<TowerPowerScript>();
        if (pow != null)
        {
            preBuiltTower = true;
            currentTower = pow;
            currentTower.electricSource = electricSource;
            Messenger<TowerPowerScript>.Invoke("Tower Built", currentTower);
        }
    }

    void EnableInteraction()
    {
        disabled = false;
    }

    void DisableInteraction()
    {
        disabled = true;
    }

    void OnMouseOver()
    {
        if (disabled) return;
        if (Time.timeScale == 0) return;
        if (Input.GetMouseButtonUp(0))
        {
            if (currentTower == null && menuContainer.childCount == 0)
            {
                if (!manager.CanBuildTower()) return;
                GameObject newTS = Instantiate(towerSelectionUI, transform.position, Quaternion.identity) as GameObject;
                newTS.transform.SetParent(menuContainer);
                newTS.GetComponent<TowerSelectionScript>().SetUpTowerSelection(this, manager.legalTowers);
                /*
                GameObject newTower = Instantiate(towerToBuild, transform.position, Quaternion.identity) as GameObject;
                currentTower = newTower.GetComponent<TowerPowerScript>();
                Messenger<TowerPowerScript>.Invoke("Tower Built", currentTower);
                */
            }
            else if (menuContainer.childCount == 1)
            {
                menuContainer.GetComponentInChildren<TowerSelectionScript>().RemoveMenu();
            }
            else if (manager.IsTowerDeleting())
            {
                RemoveTower();
            }
            else
            {
                currentTower.TogglePower();
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            RemoveTower();
        }
    }

    public void RemoveTower()
    {
        if (currentTower == null) return;
        if (preBuiltTower) return;
        Messenger<TowerPowerScript>.Invoke("Tower Destroyed", currentTower.GetComponentInChildren<TowerPowerScript>());
        StartCoroutine("KillTowerCoroutine");
    }

    IEnumerator KillTowerCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        GameObject oldTower = currentTower.gameObject;
        currentTower = null;
        Destroy(oldTower);
        
    }

    public void BuildTower(GameObject tower)
    {
        GameObject newTower = Instantiate(tower, transform.position, Quaternion.identity) as GameObject;
        currentTower = newTower.GetComponent<TowerPowerScript>();
        currentTower.electricSource = electricSource;
        newTower.transform.SetParent(transform);
        if (!manager.CanBuildTower()) SetBuildReadyIcon();
        Messenger<TowerPowerScript>.Invoke("Tower Built", currentTower);
    }

    public void SetBuildReadyIcon(bool makeActive = false)
    {
        towerReadyIcon.SetActive(makeActive && currentTower == null);
    }

    public void RemoveTowerReset()
    {
        GameObject oldTower = currentTower.gameObject;
        currentTower = null;
        DestroyImmediate(oldTower);
    }
}
