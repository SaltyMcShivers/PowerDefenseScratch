using UnityEngine;
using System.Collections;

public class EnergyDamageScript : DamageTypeScript
{
    public float bestDamage;

    public override float GetDamage(float power)
    {
        return Mathf.Lerp(defaultDamage, bestDamage, power);
    }
}
