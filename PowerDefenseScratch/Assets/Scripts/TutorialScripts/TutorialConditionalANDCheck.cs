using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TutorialConditionalANDCheck:TutorialConditional {

    public List<TutorialConditional> conditions;
	
    public override bool IsValid()
    {
        foreach(TutorialConditional cond in conditions)
        {
            if (!cond.IsValid()) return false;
        }
        return true;
    }
}
