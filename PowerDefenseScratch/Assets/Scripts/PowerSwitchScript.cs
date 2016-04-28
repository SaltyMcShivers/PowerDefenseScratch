using UnityEngine;
using System.Collections;

public class PowerSwitchScript : MonoBehaviour {
    public ElectricPathNode sourceNode;
    public ElectricSwitchMenuScript menu;

    public GameObject switchGraphic;

	// Use this for initialization
	void Start () {
        switchGraphic.transform.rotation = Quaternion.LookRotation(Vector3.forward, sourceNode.previousNode.transform.position - sourceNode.transform.position);
        menu.gameObject.SetActive(false);
	}
    
    public void ChangeSwitch(ElectricPathNode newTarget)
    {
        if(newTarget != null) switchGraphic.transform.rotation = Quaternion.LookRotation(Vector3.forward, newTarget.transform.position - sourceNode.transform.position);
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
