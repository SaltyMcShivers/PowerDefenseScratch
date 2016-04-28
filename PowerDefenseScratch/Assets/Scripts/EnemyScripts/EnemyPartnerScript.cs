using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyPartnerScript : MonoBehaviour {

    public string classToAdd;

    public virtual List<float> GetVariables()
    {
        List<float> res = new List<float>();
        return res;
    }

    //When all the partners have been found, initialize the partners
    public virtual void Execute()
    {

    }

    //If one partner is left, remove any effects the partner script is causing
    public virtual void Revert()
    {

    }

    //Start Up Transition for partner interaction
    public virtual void StartTransition()
    {

    }

    //End transition for partner interaction
    public virtual void StopTransition()
    {

    }
}
