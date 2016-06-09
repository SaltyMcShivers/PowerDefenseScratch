using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class DeathParticleScaling
{
    public ParticleSystem sys;
    public Color lowColor;
    public Color highColor;
}

public class DeathParticleScript : MonoBehaviour {
    public List<DeathParticleScaling> parts;
	// Use this for initialization
	public void ScaleParticles (float f) {
	    foreach(DeathParticleScaling death in parts)
        {
            death.sys.startColor = Color.Lerp(death.lowColor, death.highColor, f);
        }
	}
}
