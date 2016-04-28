using UnityEngine;
using System.Collections;

public class DamageTypeScript : MonoBehaviour {
    public float defaultFireRate;

    public float defaultDamage;

    public virtual float GetFireRate(float power)
    {
        return defaultFireRate;
    }

    public virtual float GetDamage(float power)
    {
        return defaultDamage;
    }
}
