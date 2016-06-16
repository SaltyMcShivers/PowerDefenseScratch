using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlowTowerVisual : TowerEffectScript
{
    public Renderer gridRenderer;

    public ParticleSystem fireSystem;

    public GameObject ringPrefab;
    public float ringSpawnInterval;
    public float largestRingLow;
    public float largerstRingHigh;

    Material manipulativeMaterial;
    bool webOn;
    bool occupied;

    IEnumerator RingSpawnCoroutine()
    {
        yield break;
    }

	// Use this for initialization
	void Start () {
        manipulativeMaterial = gridRenderer.material;
    }
    
    void OnDestroy()
    {
        StopAllCoroutines();
    }

    public override void SetUpEffect(float pow)
    {
        currentPower = pow;
        if(pow == 0f)
        {
            webOn = false;
            if (occupied)
            {
                TurnWebLow();
            }
        }
        else
        {
            if (!webOn)
            {
                webOn = true;
                if (occupied) {
                    SetUpOccupiedEffect(true);
                }
            }
        }
    }

    public override void SetUpOccupiedEffect(bool occ)
    {
        occupied = occ;
        if (occupied && webOn)
        {
            fireSystem.Play();
            manipulativeMaterial.SetFloat("_StartOffset", Time.timeSinceLevelLoad);
            StartCoroutine("RingCoroutine");
        }
        else
        {
            TurnWebLow();
        }
    }

    void TurnWebLow()
    {
        StopAllCoroutines();
        fireSystem.Stop();
        manipulativeMaterial.SetFloat("_EndOffset", Time.timeSinceLevelLoad);
    }

    IEnumerator RingCoroutine()
    {
        GameObject newRing = Instantiate(ringPrefab, transform.position, Quaternion.identity) as GameObject;
        newRing.transform.SetParent(transform);
        RingGrowthScript ringu = newRing.GetComponent<RingGrowthScript>();
        ringu.SetMaxRadius(Mathf.Lerp(largestRingLow, largerstRingHigh, currentPower));
        yield return new WaitForSeconds(ringSpawnInterval);
        StartCoroutine("RingCoroutine");
    }
}
