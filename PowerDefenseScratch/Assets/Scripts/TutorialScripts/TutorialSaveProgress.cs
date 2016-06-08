using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialSaveTowers
{
    public TowerBaseScript towerBase;
    //This should save the tower type instead.
    public bool hasTower;
}

public class TutorialSaveProgress : MonoBehaviour {

    public TowerManagerScript towerManager;

    float baseHealth;
    int waveToSpawn;
    int metalCount;
    float energyCount;
    List<TutorialSaveTowers> bases;

    void Awake()
    {
        bases = new List<TutorialSaveTowers>();
    }

    public void SaveWave(int wave)
    {
        waveToSpawn = wave;
        GameObject spawner = GameObject.Find("SpawnManager");
        Vector2 addedResources = spawner.GetComponent<SpawnManagementScript>().GetWaveResources(waveToSpawn);
        energyCount = towerManager.startingPower + addedResources.x;
        metalCount = towerManager.startingMetal + Mathf.RoundToInt(addedResources.y);
        bases.Clear();
        baseHealth = GameObject.Find("PowerSource").GetComponent<PowerSourceHealth>().GetHealth();
        foreach (TowerBaseScript based in towerManager.GetComponentsInChildren<TowerBaseScript>())
        {
            TutorialSaveTowers tst = new TutorialSaveTowers();
            tst.towerBase = based;
            if(based.GetComponentInChildren<TowerPowerScript>() != null)
            {
                tst.hasTower = true;
                metalCount -= 100;
            }
            bases.Add(tst);
        }
    }

    public void RevertToWave()
    {
        Messenger.Invoke("ResetWave");
        GameObject.Find("PowerSource").GetComponent<PowerSourceHealth>().SetHealth(baseHealth);
        int refundedTowers = 0;
        foreach(TutorialSaveTowers tow in bases)
        {
            if(tow.towerBase.GetComponentInChildren<TowerPowerScript>() != null && !tow.hasTower)
            {
                refundedTowers++;
                towerManager.RemoveTowerFromList(tow.towerBase.GetComponentInChildren<TowerPowerScript>());
                tow.towerBase.RemoveTowerReset();
            }
        }
        towerManager.SetCurrentResources(energyCount, metalCount);

        GameObject spawner = GameObject.Find("SpawnManager");
        
        spawner.GetComponent<SpawnManagementScript>().SkipToWave(waveToSpawn);
        spawner.GetComponent<SpawnManagementScript>().ClearAllEnemies();


        TutorialManager tut = spawner.GetComponent<TutorialManager>();
        if (tut != null)
        {
            tut.SetCurrentSection(waveToSpawn);
            tut.ActivateTutorial();
        }
    }
}
