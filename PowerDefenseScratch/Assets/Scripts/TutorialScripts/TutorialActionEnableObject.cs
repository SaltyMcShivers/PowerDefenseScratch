using UnityEngine;
using System.Collections;

[System.Serializable]
public class TutorialActionEnableObject {
    public GameObject target;
    public bool activate;

    public TutorialConditionalTowerCheck[] conditions;

    public void DoAction()
    {
        foreach (TutorialConditionalTowerCheck tow in conditions)
        {
            if (!tow.IsValid())
            {
                return;
            }
        }
        target.SetActive(activate);
    }
}
