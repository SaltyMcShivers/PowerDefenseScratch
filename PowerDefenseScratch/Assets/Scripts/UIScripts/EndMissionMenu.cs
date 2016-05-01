using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class EndMissionMenu : MonoBehaviour {
    public GameObject menu;
    public Text missionText;

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
        if (passed)
        {
            missionText.text = "Generator Protected";
        }
        else
        {
            missionText.text = "Generator Destroyed";
        }
    }

    public void GoToNextScene(int scene)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(scene);
    }
}
