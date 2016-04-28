using UnityEngine;
using System.Collections;

public class PhysicalDamageScript : DamageTypeScript
{
    public float bestFireRate;

	public override float GetFireRate(float power)
    {
        return Mathf.Lerp(defaultFireRate, bestFireRate, power);
    }
}
