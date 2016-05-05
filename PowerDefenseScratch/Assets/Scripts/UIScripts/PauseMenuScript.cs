using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseMenuScript : MonoBehaviour {

    public GameObject quitMenu;

    Image backgroundImage;

	// Use this for initialization
	void Start () {
        backgroundImage = GetComponent<Image>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (quitMenu.activeSelf) return;
            if (backgroundImage.enabled) UnpauseMenu();
            else PauseMenu();
        }
	}

    public void PauseMenu()
    {
        if (Time.timeScale == 0) return;
        Time.timeScale = 0;
        backgroundImage.enabled = true;
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void UnpauseMenu()
    {
        Time.timeScale = 1;
        backgroundImage.enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
