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

    public List<TutorialConditional> stepsTest;

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
            Messenger<TowerPowerScript>.RemoveListener(sections[currentSection].steps[currentStep].eventToListenFor, NextStepMessage);
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

    public void NextStepMessage(TowerPowerScript tBase)
    {
        NextStep();
    }

    public void NextStepButton()
    {
        NextStep();
    }

    public void NextStep()
    {
        if (sections[currentSection].steps[currentStep].eventToListenFor == "")
        {
            continueButton.SetActive(false);
        }
        else
        {
            Messenger<TowerPowerScript>.RemoveListener(sections[currentSection].steps[currentStep].eventToListenFor, NextStepMessage);
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
            StartCoroutine(ToggleButtonCoroutine());
        }
        else
        {
            Messenger<TowerPowerScript>.AddListener(sections[currentSection].steps[currentStep].eventToListenFor, NextStepMessage);
        }
    }

    IEnumerator ToggleButtonCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        continueButton.SetActive(true);
    }
}
