using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialBuildRestrictionScript : MonoBehaviour {
    public List<TowerBaseScript> basesToDisable;
    public GameObject towerToBuild;
    public TowerManagerScript manager;

	void OnEnable()
    {
        foreach (TowerBaseScript dis in basesToDisable)
        {
            dis.SetBuildDisable(true);
        }
        manager.TutorialTower = towerToBuild;
    }

    void OnDisable()
    {
        foreach (TowerBaseScript dis in basesToDisable)
        {
            dis.SetBuildDisable(false);
        }
        manager.TutorialTower = null;
    }
}
