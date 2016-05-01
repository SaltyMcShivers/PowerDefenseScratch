using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NextSceneButton : MonoBehaviour {
    public int sceneToGoTo;

    public void GoToNextScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneToGoTo);
    }
}
