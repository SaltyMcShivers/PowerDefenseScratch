using UnityEngine;
using System.Collections;

public class RadialButtonScript : MonoBehaviour {
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        //DisableIfNeeded();
    }

    public virtual void DisableIfNeeded()
    {

    }

    public void DisableButton()
    {
        if (anim == null) anim = GetComponent<Animator>();
        anim.SetTrigger("Disabled");
    }

    public void HighlightButton()
    {
        if (anim.GetBool("Disabled")) return;
        anim.SetTrigger("Highlighted");
    }

    public void UnhighlightButton()
    {
        //if (anim == null) return;
        if (anim.GetBool("Disabled")) return;
        anim.SetTrigger("Normal");
    }

    public bool isDisabled()
    {
        return anim.GetBool("Disabled");
    }
}
