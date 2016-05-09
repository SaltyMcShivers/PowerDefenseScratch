using UnityEngine;
using System.Collections;

public class FastForwardScript : MonoBehaviour {
    public float fastForwardSpeed;
    
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = fastForwardSpeed;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Time.timeScale = 1f;
        }
	}
}
