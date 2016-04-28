using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TowerManagerScript : MonoBehaviour {
    public float startingPower;
    public float currentPower;

    public Text TotalPowerDisplay;
    public Text TotalMetalDisplay;
    public Text PowerDistributeDisplay;
    public Text TowerCountDisplay;

    public List<GameObject> legalTowers;
    public ElectricPathNode electricPathRoot;

    public int metalToBuild;
    public int startingMetal;

    public int waveToStart;
    int currentMetal;

    List<TowerPowerScript> allTowers;
    List<TowerPowerScript> activeTowers;

	// Use this for initialization
	void Start () {
        electricPathRoot.SetUpStart(this);
        currentPower = startingPower;
        currentMetal = startingMetal;
        allTowers = new List<TowerPowerScript>();
        activeTowers = new List<TowerPowerScript>();
        TotalPowerDisplay.text = startingPower.ToString() + " Jolts";
        TotalMetalDisplay.text = startingMetal.ToString() + " Metal";
        PowerDistributeDisplay.text = "0%";
        TowerCountDisplay.text = "0/0";
        Messenger<TowerPowerScript>.AddListener("Tower Built", AddNewTower);
        Messenger<TowerPowerScript>.AddListener("Tower Destroyed", RemoveTower);
        Messenger<TowerPowerScript>.AddListener("Tower Off", DeactivateTower);
        Messenger<TowerPowerScript>.AddListener("Tower On", ActivateTower);
        Messenger<GameObject>.AddListener("Destroy Enemy", GetEnergyFromEnemy);
        Messenger.AddListener("Switch Flipped", FindNewActiveTowers);
        if (waveToStart > 0) GetResources();
    }

    void GetResources()
    {
        Vector2 addedResources = GameObject.Find("SpawnManager").GetComponent<SpawnManagementScript>().SkipToWave(waveToStart);
        currentPower += addedResources.x;
        currentMetal += (int)addedResources.y;
        TotalPowerDisplay.text = currentPower.ToString() + " Jolts";
        TotalMetalDisplay.text = currentMetal.ToString() + " Metal";
    }
	
    void AddNewTower(TowerPowerScript tps)
    {
        currentMetal -= metalToBuild;
        TotalMetalDisplay.text = currentMetal.ToString() + " Metal";
        if (!CanBuildTower())
        {
            TotalMetalDisplay.color = Color.red;
        }
        allTowers.Add(tps);
        if (tps.IsGettingPower()) ActivateTower(tps);
        else tps.SetCurrentPower(0f);
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

    void RemoveTower(TowerPowerScript tps)
    {
        currentMetal += metalToBuild;
        TotalMetalDisplay.text = currentMetal.ToString() + " Metal";
        if (CanBuildTower())
        {
            TotalMetalDisplay.color = Color.black;
        }
        activeTowers.Remove(tps);
        allTowers.Remove(tps);
        SetActiveTowerEnergies();
    }

    void FindNewActiveTowers()
    {
        activeTowers.Clear();
        foreach (TowerPowerScript pow in allTowers)
        {
            if (pow.IsGettingPower()) activeTowers.Add(pow);
            else pow.SetCurrentPower(0f);
        }
        SetActiveTowerEnergies();
    }

    void GetEnergyFromEnemy(GameObject enemy)
    {
        EnemyHealthScript health = enemy.GetComponent<EnemyHealthScript>();
        if (health == null) return;
        currentPower += health.droppedEnergy;
        currentMetal += health.droppedMetal;
        TotalPowerDisplay.text = currentPower.ToString() + " Jolts";
        TotalMetalDisplay.text = currentMetal.ToString() + " Metal";
        if (CanBuildTower())
        {
            TotalMetalDisplay.color = Color.black;
        }
        SetActiveTowerEnergies();
    }

    void SetActiveTowerEnergies()
    {
        TowerCountDisplay.text = activeTowers.Count.ToString() + "/" + allTowers.Count.ToString();
        float powerPerTower;
        if (activeTowers.Count == 0)
        {
            powerPerTower = 0f;
        }
        else
        {
            powerPerTower = currentPower / activeTowers.Count;
        }
        PowerDistributeDisplay.text = powerPerTower.ToString("F0") + "%";
        foreach (TowerPowerScript tp in activeTowers)
        {
            tp.SetCurrentPower(powerPerTower);
        }
    }

    public void FindTowersWithinBounds(Bounds highlight)
    {
        List<TowerPowerScript> hTowers = new List<TowerPowerScript>();
        foreach (TowerPowerScript pow in allTowers)
        {
            if(highlight.Contains(Camera.main.WorldToViewportPoint(pow.transform.position))){
                hTowers.Add(pow);
            }
        }
        //bool powerOn = false;
        foreach (TowerPowerScript powah in hTowers)
        {
            powah.TogglePower();
            /*if (powah.IsDisabled())
            {
                powerOn = true;
                break;
            }*/
        }
    }

    public bool CanBuildTower()
    {
        return currentMetal >= metalToBuild;
    }
}
