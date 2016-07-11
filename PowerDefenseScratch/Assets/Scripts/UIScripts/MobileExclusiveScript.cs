using UnityEngine;
using System.Collections;

public class MobileExclusiveScript : MonoBehaviour {

	// Use this for initialization
	void Awake () {
	    if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android) return;
        Destroy(gameObject);
    }
}
