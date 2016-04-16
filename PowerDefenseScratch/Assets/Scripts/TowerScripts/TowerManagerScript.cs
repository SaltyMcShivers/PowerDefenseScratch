using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TowerManagerScript : MonoBehaviour {
    public float startingPower;
    public float currentPower;

    public Text TotalPowerDisplay;
    public Text PowerDistributeDisplay;
    public Text TowerCountDisplay;

    List<TowerPowerScript> allTowers;
    List<TowerPowerScript> activeTowers;

	// Use this for initialization
	void Start () {
        currentPower = startingPower;
        allTowers = new List<TowerPowerScript>();
        activeTowers = new List<TowerPowerScript>();
        TotalPowerDisplay.text = startingPower.ToString() + " Jolts";
        PowerDistributeDisplay.text = "0%";
        TowerCountDisplay.text = "0/0";
        Messenger<TowerPowerScript>.AddListener("Tower Built", AddNewTower);
        Messenger<TowerPowerScript>.AddListener("Tower Off", DeactivateTower);
        Messenger<TowerPowerScript>.AddListener("Tower On", ActivateTower);
        Messenger<GameObject>.AddListener("Destroy Enemy", GetEnergyFromEnemy);
    }
	
    void AddNewTower(TowerPowerScript tps)
    {
        allTowers.Add(tps);
        ActivateTower(tps);
    }

    void ActivateTower(TowerPowerScript tps)
    {
        activeTowers.Add(tps);
        SetActiveTowerEnergies();
    }

    void DeactivateTower(TowerPowerScript tps)
    {
        activeTowers.Remove(tps);
        SetActiveTowerEnergies();
    }

    void GetEnergyFromEnemy(GameObject enemy)
    {
        EnemyHealthScript health = enemy.GetComponent<EnemyHealthScript>();
        if (health == null) return;
        currentPower += health.droppedEnergy;
        TotalPowerDisplay.text = currentPower.ToString() + " Jolts";
        SetActiveTowerEnergies();
    }

    void SetActiveTowerEnergies()
    {
        TowerCountDisplay.text = activeTowers.Count.ToString() + "/" + allTowers.Count.ToString();
        if (activeTowers.Count == 0)
        {
            PowerDistributeDisplay.text = "0%";
            return;
        }
        float powerPerTower = currentPower / activeTowers.Count;
        PowerDistributeDisplay.text = powerPerTower.ToString() + "%";
        foreach (TowerPowerScript tp in activeTowers)
        {
            tp.SetCurrentPower(powerPerTower);
        }
    }
}
