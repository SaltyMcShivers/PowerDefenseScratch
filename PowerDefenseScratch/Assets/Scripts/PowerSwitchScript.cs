using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerSwitchScript : MonoBehaviour {
    public ElectricPathNode sourceNode;
    public ElectricSwitchMenuScript menu;

    public GameObject switchGraphic;

    public Transform arrowParent;
    public GameObject sourceIndicatorIconPrefab;
    public GameObject directionIconPrefab;

    List<LineRenderer> directionIcons;
    LineRenderer sourceIcon;

	// Use this for initialization
	void Start () {
        if (sourceNode.previousNode != null)
        {
            Vector3 dir = sourceNode.previousNode.transform.position - sourceNode.transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            switchGraphic.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.right);
            GameObject dGO = Instantiate(sourceIndicatorIconPrefab, transform.position, Quaternion.identity) as GameObject;
            dGO.transform.SetParent(arrowParent);
            dGO.transform.localScale = Vector3.one;
            dGO.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            sourceIcon = dGO.GetComponent<LineRenderer>();
            directionIcons = new List<LineRenderer>();
            foreach (ElectricPathNode node in sourceNode.nextNodes)
            {

                Vector3 nextDir = node.transform.position - sourceNode.transform.position;
                float nextAngle = Mathf.Atan2(nextDir.y, nextDir.x) * Mathf.Rad2Deg;
                GameObject nGO = Instantiate(directionIconPrefab, transform.position, Quaternion.identity) as GameObject;
                nGO.transform.SetParent(arrowParent);
                nGO.transform.localScale = Vector3.one;
                nGO.transform.rotation = Quaternion.AngleAxis(nextAngle, Vector3.forward);
                directionIcons.Add(nGO.GetComponent<LineRenderer>());
            }
        }
        menu.gameObject.SetActive(false);
        Messenger.AddListener("Switch Flipped", UpdateVisual);
    }

    void OnDestroy()
    {
        Messenger.RemoveListener("Switch Flipped", UpdateVisual);
    }
    
    void UpdateVisual()
    {

        if (sourceNode.IsGettingEnergy())
        {
            if (sourceNode.nextForPath == null)
            {
                foreach (LineRenderer arrow in directionIcons)
                {
                    arrow.gameObject.SetActive(true);
                }
            }
            else
            {
                foreach (LineRenderer arrow in directionIcons)
                {
                    Vector3 dir = sourceNode.nextForPath.transform.position - sourceNode.transform.position;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    arrow.gameObject.SetActive(Mathf.Round(angle) == Mathf.Round(arrow.transform.eulerAngles.z));
                }
            }
        }
    }

    public void ChangeSwitch(ElectricPathNode newTarget)
    {
        if (newTarget != null)
        {
            Vector3 dir = newTarget.transform.position - sourceNode.transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            switchGraphic.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.right);
        }
        sourceNode.SetNextForPath(newTarget);
        Messenger.Invoke("Switch Flipped");
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (!menu.gameObject.activeSelf)
            {
                menu.gameObject.SetActive(true);
                Messenger.Invoke("Switch Menu Added");
            }
            else
            {
                menu.RemoveMenu();
            }
        }
    }
}
