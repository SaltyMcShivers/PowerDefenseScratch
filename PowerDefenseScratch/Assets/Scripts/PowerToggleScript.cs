using UnityEngine;
using System.Collections;

public class PowerToggleScript : MonoBehaviour {
    public ElectricPathNode sourceNode;

    public GameObject switchGraphic;

    // Use this for initialization
    void Start()
    {
        if (sourceNode.previousNode != null) switchGraphic.transform.rotation = Quaternion.LookRotation(Vector3.forward, sourceNode.previousNode.transform.position - sourceNode.transform.position);
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
            switchGraphic.transform.Rotate(Vector3.back, 90);
            Messenger.Invoke("Switch Flipped");
        }
    }
}
