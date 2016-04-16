using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TowerButtonScript : MonoBehaviour {
    Animator anim;
    public GameObject towerToBuild;
    void Awake()
    {
        anim = GetComponent<Animator>();
        if (towerToBuild == null) anim.SetTrigger("Disabled");
    }

    public GameObject GetTower()
    {
        return towerToBuild;
    }

    public void DisableButton()
    {
        anim.SetTrigger("Disabled");
    }

    public void HighlightButton()
    {
        if(anim.GetBool("Disabled")) return;
        anim.SetTrigger("Highlighted");
    }

    public void UnhighlightButton()
    {
        if (anim.GetBool("Disabled")) return;
        anim.SetTrigger("Normal");
    }

    public bool isDisabled()
    {
        return anim.GetBool("Disabled");
    }

}
