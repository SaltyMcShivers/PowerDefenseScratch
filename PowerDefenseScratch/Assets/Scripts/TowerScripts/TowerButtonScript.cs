using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TowerButtonScript : RadialButtonScript
{
    public GameObject towerToBuild;

    public override void DisableIfNeeded()
    {
        if (towerToBuild == null) DisableButton();
    }

    public GameObject GetTower()
    {
        return towerToBuild;
    }

}
