using UnityEngine;
using System.Collections;

public class TowerToggle : MonoBehaviour {
    public TowerPowerScript pow;

    void OnMouseDown()
    {
        pow.TogglePower();
    }
}
