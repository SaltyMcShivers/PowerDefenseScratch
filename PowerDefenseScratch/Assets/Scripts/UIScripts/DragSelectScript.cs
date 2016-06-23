using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DragSelectScript : MonoBehaviour {
    public float minDistance;
    public RectTransform highlightObject;
    public TowerManagerScript towers;

    public Color enableColor;
    public Color disableColor;
    public Color swapColor;

    Vector3 startPosition;
    bool dragging;
    bool leftDrag;
    bool usedKeyboard;

	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            if (dragging) return;
            dragging = true;
            if (Input.GetKey(KeyCode.LeftControl))
            {
                usedKeyboard = true;
                startPosition = Input.mousePosition;
                SwapAllSetup();
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                usedKeyboard = true;
                TurnOffAllSetup();
            }
            else
            {
                TurnOnAllSetup();
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (dragging && !leftDrag) return;
            if (dragging)
            {
                SwapAllSetup();
            }
            else
            {
                dragging = true;
                TurnOffAllSetup();
            }
        }
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            if (!dragging) return;
            if (Vector2.Distance(Input.mousePosition, startPosition) > minDistance && !highlightObject.GetComponent<Image>().enabled)
            {
                highlightObject.GetComponent<Image>().enabled = true;
            }
            else if (Vector2.Distance(Input.mousePosition, startPosition) < minDistance && highlightObject.GetComponent<Image>().enabled)
            {
                highlightObject.GetComponent<Image>().enabled = false;
            }
            highlightObject.position = (Input.mousePosition + startPosition) * 0.5f;
            highlightObject.sizeDelta = new Vector2(Mathf.Abs(Input.mousePosition.x - startPosition.x), Mathf.Abs(Input.mousePosition.y - startPosition.y));
        }
        //Left Mouse Lift - Default
        if (dragging && leftDrag && Input.GetMouseButtonUp(0))
        {
            Debug.Log("C");
            FinalizeDrag(Input.GetMouseButton(1) || usedKeyboard);
        }
        //Left Mouse Lift - Shift
        if (dragging && usedKeyboard && !leftDrag && Input.GetMouseButtonUp(0))
        {
            Debug.Log("D");
            FinalizeDrag(Input.GetMouseButton(1));
        }
        //Right Mouse Lift - Left Mouse Down
        if (dragging && !leftDrag && Input.GetMouseButtonUp(1))
        {
            FinalizeDrag();
        }
        //Right Mouse Lift - Default
        if (dragging && leftDrag && Input.GetMouseButtonUp(1))
        {
            highlightObject.GetComponent<Image>().color = enableColor;
        }
    }

    void TurnOnAllSetup()
    {
        leftDrag = true;
        startPosition = Input.mousePosition;
        highlightObject.GetComponent<Image>().color = enableColor;
    }

    void TurnOffAllSetup()
    {
        leftDrag = false;
        startPosition = Input.mousePosition;
        highlightObject.GetComponent<Image>().color = disableColor;
    }

    void SwapAllSetup()
    {
        highlightObject.GetComponent<Image>().color = swapColor;
    }

    public static Bounds GetViewportBounds(Camera camera, Vector3 screenPosition1, Vector3 screenPosition2)
    {
        var v1 = Camera.main.ScreenToViewportPoint(screenPosition1);
        var v2 = Camera.main.ScreenToViewportPoint(screenPosition2);
        var min = Vector3.Min(v1, v2);
        var max = Vector3.Max(v1, v2);
        min.z = camera.nearClipPlane;
        max.z = camera.farClipPlane;

        var bounds = new Bounds();
        bounds.SetMinMax(min, max);
        return bounds;
    }

    void FinalizeDrag(bool swapDrag=false)
    {
        dragging = false;
        usedKeyboard = false;
        highlightObject.GetComponent<Image>().enabled = false;
        if (Vector2.Distance(Input.mousePosition, startPosition) < minDistance)
        {
            return;
        }
        Bounds dragBounds = GetViewportBounds(Camera.main, startPosition, Input.mousePosition);
        towers.FindTowersWithinBounds(dragBounds, !leftDrag, swapDrag);
    }
}
