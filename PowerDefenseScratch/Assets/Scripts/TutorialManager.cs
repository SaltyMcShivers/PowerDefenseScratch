using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public struct TutorialStep
{
    public string boxMessage;
    public string eventToListenFor;
}

[System.Serializable]
public struct TutorialSection
{
    public List<TutorialStep> steps;
}

public class TutorialManager : MonoBehaviour {
    public GameObject continueButton;
    public List<TutorialSection> sections;
    public SpawnManagementScript spawner;

    public GameObject messageBox;
    public Text messageText;

    int currentSection;
    int currentStep;

    // Use this for initialization
    void Start ()
    {
        currentSection = -1;
        Messenger.AddListener("WaveCompleted", MoveOnWithTutorial);
        MoveOnWithTutorial();
    }

    void OnDestroy()
    {
        Messenger.RemoveListener("WaveCompleted", MoveOnWithTutorial);
        if(currentSection < sections.Count && currentStep < sections[currentSection].steps.Count && sections[currentSection].steps[currentStep].eventToListenFor != "")
        {
            Messenger.RemoveListener(sections[currentSection].steps[currentStep].eventToListenFor, NextStep);
        }
    }

    void MoveOnWithTutorial()
    {
        currentSection++;
        if (currentSection >= sections.Count) return;
        currentStep = 0;
        messageBox.SetActive(true);
        SetUpCurrentBox();

    }

    public void NextStep()
    {
        if (sections[currentSection].steps[currentStep].eventToListenFor == "")
        {
            continueButton.SetActive(false);
        }
        else
        {
            Messenger.RemoveListener(sections[currentSection].steps[currentStep].eventToListenFor, NextStep);
        }
        currentStep++;
        if (currentStep >= sections[currentSection].steps.Count)
        {
            messageBox.SetActive(false);
            spawner.SendNextWave();
        }
        else
        {
            SetUpCurrentBox();
        }
    }

    void SetUpCurrentBox()
    {
        messageText.text = sections[currentSection].steps[currentStep].boxMessage;
        if (sections[currentSection].steps[currentStep].eventToListenFor == "")
        {
            continueButton.SetActive(true);
        }
        else
        {
            Messenger.AddListener(sections[currentSection].steps[currentStep].eventToListenFor, NextStep);
        }
    }
}
