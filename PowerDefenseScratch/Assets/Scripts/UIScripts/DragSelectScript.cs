using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DragSelectScript : MonoBehaviour {
    public float minDistance;
    public RectTransform highlightObject;
    public TowerManagerScript towers;

    Vector3 startPosition;

	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            if (Vector2.Distance(Input.mousePosition, startPosition) > minDistance && !highlightObject.GetComponent<Image>().enabled)
            {
                highlightObject.GetComponent<Image>().enabled = true;
            }
            highlightObject.position = (Input.mousePosition + startPosition) * 0.5f;
            highlightObject.sizeDelta = new Vector2(Mathf.Abs(Input.mousePosition.x - startPosition.x), Mathf.Abs(Input.mousePosition.y - startPosition.y));
        }
        if (Input.GetMouseButtonUp(0))
        {
            highlightObject.GetComponent<Image>().enabled = false;
            Bounds dragBounds = GetViewportBounds(Camera.main, startPosition, Input.mousePosition);
            towers.FindTowersWithinBounds(dragBounds);
        }
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
}
