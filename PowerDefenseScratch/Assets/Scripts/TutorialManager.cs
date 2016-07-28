using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public struct TutorialStep
{
    public string boxMessage;
    public string eventToListenFor;
    public TutorialConditionalORCheck conditions;
    public List<TutorialActionEnableObject> actions;
}

[System.Serializable]
public struct TutorialSection
{
    public int waveToEnd;
    public List<TutorialStep> steps;
}

public class TutorialManager : MonoBehaviour {
    public GameObject continueButton;
    public List<TutorialSection> sections;
    public SpawnManagementScript spawner;

    public TutorialSaveProgress progress;

    public GameObject messageBox;
    public Text messageText;

    int currentSection;
    int currentStep;

    public List<TutorialConditional> stepsTest;

    public void SetCurrentSection(int i)
    {

        currentStep = 0;
        currentSection = 0;
        while (sections[currentSection].waveToEnd < i-1)
        {
            currentSection++;
        }
        if(sections[currentSection].waveToEnd < i - 1)
        {
            MoveOnWithTutorial();
        }
        else
        {
            spawner.SendNextWave();
        }
    }

    void Awake()
    {
        currentSection = -1;
    }

    // Use this for initialization
    void Start ()
    {
        Messenger<int>.AddListener("WaveCompleted", WaveEndCheck);
        if(currentSection < 0) MoveOnWithTutorial();
    }

    void OnDestroy()
    {
        Messenger<int>.RemoveListener("WaveCompleted", WaveEndCheck);
        RemoveTriggers();
    }

    public void ActivateTutorial()
    {
        MoveOnWithTutorial();
    }

    void WaveEndCheck(int wv)
    {
        if (sections[currentSection].waveToEnd != wv)
        {
            return;
        }
        MoveOnWithTutorial();
    }

    void MoveOnWithTutorial()
    {
        currentSection++;
        progress.SaveWave(currentSection);
        if (currentSection >= sections.Count)
        {
            spawner.SendNextWave();
            return;
        }
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
        if (sections[currentSection].steps[currentStep].conditions.requirements.Count == 0)
        {
            continueButton.SetActive(false);
        }
        else
        {
            //Messenger<TowerPowerScript>.RemoveListener(sections[currentSection].steps[currentStep].eventToListenFor, NextStepMessage);
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
        /*
        if (sections[currentSection].steps[currentStep].eventToListenFor == "")
        {
            StartCoroutine(ToggleButtonCoroutine());
        }
        else
        {
            Messenger<TowerPowerScript>.AddListener(sections[currentSection].steps[currentStep].eventToListenFor, NextStepMessage);
        }
        */
        foreach (TutorialActionEnableObject en in sections[currentSection].steps[currentStep].actions)
        {
            en.DoAction();
        }
        if (sections[currentSection].steps[currentStep].conditions.requirements.Count == 0)
        {
            StartCoroutine(ToggleButtonCoroutine());
        }
        else
        {
            if (sections[currentSection].steps[currentStep].conditions.IsValid())
            {
                NextStep();
            }
            else
            {
                SetTriggers();
            }
        }
    }

    void CheckConditionsTPS(TowerPowerScript tps)
    {
        CheckConditions();
    }

    void CheckConditions()
    {
        if(sections[currentSection].steps[currentStep].conditions.IsValid())
        {
            RemoveTriggers();
            NextStep();
        }
    }

    IEnumerator ToggleButtonCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        continueButton.SetActive(true);
    }

    void SetTriggers()
    {
        Messenger<TowerPowerScript>.AddListener("Tower Built", CheckConditionsTPS);
        Messenger<TowerPowerScript>.AddListener("Tower Off", CheckConditionsTPS);
        Messenger<TowerPowerScript>.AddListener("Tower On", CheckConditionsTPS);
        Messenger.AddListener("Switch Flipped", CheckConditions);
    }

    void RemoveTriggers()
    {
        Messenger<TowerPowerScript>.RemoveListener("Tower Built", CheckConditionsTPS);
        Messenger<TowerPowerScript>.RemoveListener("Tower Off", CheckConditionsTPS);
        Messenger<TowerPowerScript>.RemoveListener("Tower On", CheckConditionsTPS);
        Messenger.RemoveListener("Switch Flipped", CheckConditions);
    }
}
