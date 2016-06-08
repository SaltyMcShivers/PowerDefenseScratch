using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerSelectionScript : MonoBehaviour {
    bool cursorOver = false;
    Vector3 lastMousePosition;
    int lastPick = -1;
    public Vector2 limits;

    public List<TowerButtonScript> buttons;

    TowerBaseScript baseForTower;

    public void SetUpTowerSelection(TowerBaseScript towerBase, List<GameObject> legalTowers)
    {
        baseForTower = towerBase;
        foreach (TowerButtonScript tow in buttons)
        {
            if (tow.GetTower() == null || !legalTowers.Contains(tow.GetTower())) tow.DisableButton();
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
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10f;
            if (Vector3.Distance(currentMousePosition, transform.position) < limits.x) return;
            int hoverPick = FindPick(currentMousePosition);
            if (hoverPick >= 0)
            {
                if (!buttons[hoverPick].isDisabled())
                {
                    baseForTower.BuildTower(buttons[hoverPick].GetTower());
                }
            }
            StartCoroutine("KillOffMenu");
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
