using UnityEngine;
using System.Collections;

public class QuitGameMenuScript : MonoBehaviour {

    public void QuitGame()
    {
        /*
        if (Application.isEditor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }
        */
        Application.Quit();
    }
}
