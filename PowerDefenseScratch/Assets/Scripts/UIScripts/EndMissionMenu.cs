using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;

public class EndMissionMenu : MonoBehaviour {
    public GameObject menu;
    public Text missionText;

    public List<GameObject> failList;

    void Start()
    {
        Messenger<bool>.AddListener("End Game", SetUpMenu);
    }

    void OnDestroy()
    {
        Messenger<bool>.RemoveListener("End Game", SetUpMenu);
    }

    void SetUpMenu(bool passed)
    {
        menu.SetActive(true);
        GetComponent<Image>().enabled = true;
        if (passed)
        {
            missionText.text = "Generator Protected";
        }
        else
        {
            foreach(GameObject go in failList)
            {
                go.SetActive(false);
            }
            missionText.text = "Generator Destroyed";
        }
    }

    public void GoToNextScene(int scene)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(scene);
    }
}
