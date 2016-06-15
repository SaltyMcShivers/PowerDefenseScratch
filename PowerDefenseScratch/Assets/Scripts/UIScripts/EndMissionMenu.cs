using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;

public class EndMissionMenu : MonoBehaviour {
    public GameObject menu;
    public Text missionText;

    public List<GameObject> failList;
    public GameObject replayWaveButton;
    public TutorialSaveProgress tutorialProgress;

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
            foreach (GameObject go in failList)
            {
                go.SetActive(true);
            }
            replayWaveButton.SetActive(false);
            missionText.text = "Generator Protected";
        }
        else
        {
            if(tutorialProgress != null)
            {
                replayWaveButton.SetActive(true);
            }
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

    public void ResetWave()
    {
        Time.timeScale = 1;
        menu.SetActive(false);
        GetComponent<Image>().enabled = false;
        tutorialProgress.RevertToWave();
    }
}
