using UnityEngine;
using System.Collections;

[System.Serializable]
public class TutorialConditionalTowerCheck {
    public TowerBaseScript towerBase;
    public GameObject towerType;
    public bool gettingPower;
    public bool careAboutPower;
    public bool nullTower;

    public bool IsValid()
    {
        if (nullTower) return towerBase.GetComponentInChildren<TowerPowerScript>() == null;
        if (towerType == null)
        {
            if (!careAboutPower) return true;
            return gettingPower == towerBase.IsGettingPower();
        }
        TowerPowerScript pow = towerBase.GetComponentInChildren<TowerPowerScript>();
        if(pow == null)
        {
            return false;
        }
        if(careAboutPower && pow.IsGettingPower() != gettingPower)
        {
            return false;
        }
        return pow.gameObject.name.Contains(towerType.name);
    }
}
