using UnityEngine;
using System.Collections;

public class ScaleSystemsScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        float originalScale = transform.localScale.x;
        transform.localScale = transform.parent.localScale * originalScale;
        foreach(Transform child in transform)
        {
            transform.localScale = transform.localScale;
        }
        GetComponent<ParticleSystem>().Play();
	}
}
