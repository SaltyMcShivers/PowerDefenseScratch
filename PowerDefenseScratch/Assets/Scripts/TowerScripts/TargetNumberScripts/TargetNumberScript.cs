using UnityEngine;
using System.Collections;

public class TargetNumberScript : MonoBehaviour {
    protected int defaultTargetsToHit;

    public float defaultRange;

    public int GetTargets(float power)
    {
        return defaultTargetsToHit;
    }

    public virtual float GetComboMultiplier(float power, GameObject target)
    {
        return 1f;
    }

    public float GetRange(float power)
    {
        return defaultRange;
    }

    public virtual float GetBlastRadius(float power)
    {
        return 0;
    }
}
