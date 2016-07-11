using UnityEngine;
using System.Collections;

public class FastForwardScript : MonoBehaviour {
    public float fastForwardSpeed;
    
	
	// Update is called once per frame
	void Update () {
        if (Time.timeScale == 0) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = fastForwardSpeed;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Time.timeScale = 1f;
        }
	}

    public void SetSpeed(bool speedUp)
    {
        if (speedUp)
        {
            Time.timeScale = fastForwardSpeed;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
