using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TutorialConditionalORCheck {
    public List<TutorialConditionalANDCheck> requirements;

    public bool IsValid()
    {
        foreach(TutorialConditionalANDCheck option in requirements)
        {
            if (option.IsValid()) return true;
        }
        return false;
    }
	
}
