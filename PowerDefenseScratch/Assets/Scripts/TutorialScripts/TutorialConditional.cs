using UnityEngine;
using System.Collections;

[System.Serializable]
public class TutorialConditional : ScriptableObject
{

    public virtual bool IsValid()
    {
        return true;
    }
}