using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ElectricSwitchMenuScript : MonoBehaviour {

    public ElectricPathNode source;
    public List<ElectricSwitchButtonScript> buttons;
    public PowerSwitchScript switchScript;

    bool cursorOver = false;
    int lastPick = -1;
    public Vector2 limits;

    public void RemoveMenu()
    {
        StartCoroutine("KillOffMenu");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10f;
            if (Vector3.Distance(currentMousePosition, transform.position + Vector3.back * transform.position.z) < limits.x) return;
            int hoverPick = FindPick(currentMousePosition - Vector3.back * transform.position.z);
            if (hoverPick >= 0)
            {
                if (!buttons[hoverPick].isDisabled())
                {
                    switchScript.ChangeSwitch(buttons[hoverPick].GetSource());
                }
            }
            StartCoroutine("KillOffMenu");
        }

        if (cursorOver)
        {
            Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int hoverPick = FindPick(new Vector3(currentMousePosition.x, currentMousePosition.y, transform.position.z));
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
                if (lastPick != -1) UnhighlightButton(lastPick);
                HighlightButton(hoverPick);
            }
            lastPick = hoverPick;
        }
    }

    void OnEnable()
    {
        GetComponent<Animator>().SetTrigger("PopUpMenu");
        foreach (ElectricSwitchButtonScript button in buttons)
        {
            button.DisableIfNeeded();
        }
    }

    IEnumerator KillOffMenu()
    {
        GetComponent<Animator>().SetTrigger("SelectionMade");
        yield return new WaitForSeconds(0.1f);
        Messenger.Invoke("Switch Menu Removed");
        yield return new WaitForSeconds(0.2f);
        foreach (ElectricSwitchButtonScript button in buttons)
        {
            button.UnhighlightButton();
        }
        gameObject.SetActive(false);
    }

    public void PointerEnter()
    {
        cursorOver = true;
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
            float pointerAngle = Vector3.Angle(Vector3.up, Source - transform.position) * -Mathf.Sign(Source.x - transform.position.x) + 45;
            int hoverPick = Mathf.FloorToInt(pointerAngle / 90f + 4) % 4;
            return hoverPick;
        }
        return -1;
    }

    void HighlightButton(int i)
    {
        buttons[i].HighlightButton();
        GetComponent<Animator>().SetInteger("HoverChoice", Mathf.FloorToInt(i / 4f));
    }

    void UnhighlightButton(int i)
    {
        buttons[i].UnhighlightButton();
    }
}
