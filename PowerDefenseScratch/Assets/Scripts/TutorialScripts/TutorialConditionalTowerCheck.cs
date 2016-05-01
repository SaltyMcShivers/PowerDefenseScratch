using UnityEngine;
using System.Collections;

[System.Serializable]
public class TutorialConditionalTowerCheck {
    public TowerBaseScript towerBase;
    public GameObject towerType;
    public bool gettingPower;

    public bool IsValid()
    {
        if(towerType == null)
        {
            return gettingPower == towerBase.IsGettingPower();
        }
        TowerPowerScript pow = towerBase.GetComponentInChildren<TowerPowerScript>();
        if(pow == null)
        {
            return false;
        }
        return pow.IsGettingPower() == gettingPower && pow.gameObject.name.Contains(towerType.name);
    }
}
