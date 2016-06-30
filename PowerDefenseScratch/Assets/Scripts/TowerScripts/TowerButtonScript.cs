using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TowerButtonScript : RadialButtonScript
{
    public GameObject towerToBuild;

    bool cantBuildTower;

    public override void DisableIfNeeded()
    {
        if (towerToBuild == null) DisableButton();
    }

    public GameObject GetTower()
    {
        return towerToBuild;
    }

    public void TutorialCancel()
    {
        cantBuildTower = true;
        if (anim == null) anim = GetComponent<Animator>();
        anim.Stop();
        transform.GetChild(0).GetComponent<Image>().color = Color.white;
    }

    public bool TowerButtonDisabled()
    {
        return isDisabled() || cantBuildTower;
    }

}
