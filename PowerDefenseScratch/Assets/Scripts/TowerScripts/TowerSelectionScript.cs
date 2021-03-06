﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerSelectionScript : MonoBehaviour {
    bool cursorOver = false;
    Vector3 lastMousePosition;
    int lastPick = -1;
    public Vector2 limits;

    public RectTransform tutorialTransform;
    public List<TowerButtonScript> buttons;
    GameObject tutorialTower;

    TowerBaseScript baseForTower;
    bool selectionMade;

    public void SetUpTowerSelection(TowerBaseScript towerBase, List<GameObject> legalTowers, GameObject tutTower = null)
    {
        baseForTower = towerBase;
        tutorialTower = tutTower;
        if (tutorialTower == null) tutorialTransform.gameObject.SetActive(false);
        foreach (TowerButtonScript tow in buttons)
        {
            if (tow.GetTower() == null || !legalTowers.Contains(tow.GetTower())) tow.DisableButton();
            if (tutorialTower != null && legalTowers.Contains(tow.GetTower()))
            {
                if (tutorialTower == tow.GetTower())
                {
                    tutorialTransform.localRotation = tow.GetComponent<RectTransform>().rotation;
                }
                else
                {
                    tow.TutorialCancel();
                }
            }
        }
        PointerEnter();
    }

    public void RemoveMenu()
    {
        StartCoroutine("KillOffMenu");
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (selectionMade) return;
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10f;
            if (Vector3.Distance(currentMousePosition, transform.position) < limits.x) return;
            int hoverPick = FindPick(currentMousePosition);
            if (hoverPick >= 0)
            {
                if (!buttons[hoverPick].TowerButtonDisabled())
                {
                    baseForTower.BuildTower(buttons[hoverPick].GetTower());
                    StartCoroutine("KillOffMenu");
                }
            }
            else
            {
                StartCoroutine("KillOffMenu");
            }
        }

        if (cursorOver)
        {
            Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int hoverPick = FindPick(new Vector3(currentMousePosition.x, currentMousePosition.y, 0f));
            if (hoverPick < 0)
            {
                if (lastPick != hoverPick)
                {
                    UnhighlightButton(lastPick);
                    GetComponent<Animator>().SetInteger("HoverChoice", -1);
                    lastPick = hoverPick;
                }
                return;
            }
            if (lastPick != hoverPick)
            {
                if(lastPick != -1) UnhighlightButton(lastPick);
                HighlightButton(hoverPick);
            }
            lastPick = hoverPick;
        }
    }

    IEnumerator KillOffMenu()
    {
        GetComponent<Animator>().SetTrigger("SelectionMade");
        selectionMade = true;
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    public void PointerEnter()
    {
        cursorOver = true;
        lastMousePosition = Input.mousePosition;
    }

    public void PointerExit()
    {
        cursorOver = false;
    }

    int FindPick(Vector3 Source)
    {
        float pointerDistance = Vector3.Distance(Source, transform.position);
        if (pointerDistance > limits.x && pointerDistance < limits.y)
        {
            float pointerAngle = Vector3.Angle(Vector3.up, Source - transform.position) * Mathf.Sign(Source.x - transform.position.x) + 180;
            int hoverPick = Mathf.FloorToInt(pointerAngle / 40f);
            return hoverPick;
        }
        return -1;
    }

    void HighlightButton(int i)
    {
        if (i >= buttons.Count) return;
        buttons[i].HighlightButton();
        GetComponent<Animator>().SetInteger("HoverChoice", Mathf.FloorToInt(i / 3f));
    }

    void UnhighlightButton(int i)
    {
        if (i >= buttons.Count) return;
        buttons[i].UnhighlightButton();
    }
}
