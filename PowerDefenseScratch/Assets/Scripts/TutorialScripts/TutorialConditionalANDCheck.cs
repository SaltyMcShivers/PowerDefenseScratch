using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TutorialConditionalANDCheck
{
    public List<TutorialConditionalTowerCheck> requirements;

    public bool IsValid()
    {
        foreach (TutorialConditionalTowerCheck option in requirements)
        {
            if (!option.IsValid()) return false;
        }
        return true;
    }
}
