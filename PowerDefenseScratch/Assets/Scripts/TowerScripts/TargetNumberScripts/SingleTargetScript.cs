using UnityEngine;
using System.Collections;

public class SingleTargetScript : TargetNumberScript
{
    protected int defaultTargetsToHit = 1;

    public float maximumDamageMultiplier;
    public float minimumComboBonus;
    public float maximumComboBonus;

    float currentComboBonus;
    GameObject currentTarget;

    void Start()
    {
        Messenger<GameObject>.AddListener("Destroy Enemy", RemoveEnemy);
    }

    void RemoveEnemy(GameObject go)
    {
        if (currentTarget == go)
        {
            currentTarget = null;
            currentComboBonus = 1f;
        }
    }

    public override float GetComboMultiplier(float power, GameObject target)
    {
        if(currentTarget != null && target == currentTarget)
        {
            currentComboBonus = Mathf.Min(currentComboBonus + Mathf.Lerp(minimumComboBonus, maximumComboBonus, power), maximumDamageMultiplier);
        }
        else
        {
            currentComboBonus = 1;
            currentTarget = target;
        }
        return currentComboBonus;
    }
}
