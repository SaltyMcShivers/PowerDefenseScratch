using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerToggleScript : MonoBehaviour {
    public ElectricPathNode sourceNode;

    public GameObject switchGraphic;

    public Transform arrowParent;
    public GameObject sourceIndicatorIconPrefab;

    List<LineRenderer> directionIcons;

    // Use this for initialization
    void Start()
    {
        //switchGraphic.transform.rotation = Quaternion.LookRotation(Vector3.forward, sourceNode.previousNode.transform.position - sourceNode.transform.position);

        directionIcons = new List<LineRenderer>();
        Vector3 dir = sourceNode.previousNode.transform.position - sourceNode.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        switchGraphic.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.right);
        GameObject dGO = Instantiate(sourceIndicatorIconPrefab, transform.position, Quaternion.identity) as GameObject;
        dGO.transform.SetParent(arrowParent);
        dGO.transform.localScale = Vector3.one;
        dGO.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        directionIcons.Add(dGO.GetComponent<LineRenderer>());
        foreach (ElectricPathNode node in sourceNode.nextNodes)
        {
            Vector3 nextDir = node.transform.position - sourceNode.transform.position;
            float nextAngle = Mathf.Atan2(nextDir.y, nextDir.x) * Mathf.Rad2Deg;
            GameObject nGO = Instantiate(sourceIndicatorIconPrefab, transform.position, Quaternion.identity) as GameObject;
            nGO.transform.SetParent(arrowParent);
            nGO.transform.localScale = Vector3.one;
            nGO.transform.rotation = Quaternion.AngleAxis(nextAngle, Vector3.forward);
            directionIcons.Add(nGO.GetComponent<LineRenderer>());
        }
        Messenger.AddListener("Switch Flipped", UpdateVisual);
    }

    void OnDestroy()
    {
        Messenger.RemoveListener("Switch Flipped", UpdateVisual);
    }

    void UpdateVisual()
    {
        foreach (LineRenderer arrow in directionIcons)
        {
            arrow.gameObject.SetActive(sourceNode.IsGettingEnergy());
        }
    }

    public void ChangeSwitch(ElectricPathNode newTarget)
    {
        if (newTarget != null) switchGraphic.transform.rotation = Quaternion.LookRotation(Vector3.forward, newTarget.transform.position - sourceNode.transform.position);
        sourceNode.SetNextForPath(newTarget);
        Messenger.Invoke("Switch Flipped");
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(0))
        {
            sourceNode.ToggleEnergy();
            switchGraphic.transform.Rotate(Vector3.left, 90);
            Messenger.Invoke("Switch Flipped");
        }
    }
}
