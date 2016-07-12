using UnityEngine;
using System.Collections;

public class MobileExclusiveScript : MonoBehaviour {
    public bool enableOnMobile;

    // Use this for initialization
    void Awake() {
        if (enableOnMobile) { 
            if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.Android) return;
            Destroy(gameObject);
        }else{
            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android) return;
            Destroy(gameObject);
        }
    }
}
