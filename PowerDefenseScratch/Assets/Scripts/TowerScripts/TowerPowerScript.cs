using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TowerPowerScript : MonoBehaviour {
    private float power;
    private bool disabled;
    public Image powerDisplay;

    public float GetCurrentPower()
    {
        if (disabled) return 0;
        return Mathf.Min(power, 100f);
    }

    public void SetCurrentPower(float f)
    {
        power = f;
        powerDisplay.fillAmount = Mathf.Min(power, 100f) / 100f;
    }

    public void TogglePower()
    {
        disabled = !disabled;
        if (disabled)
        {
            Messenger<TowerPowerScript>.Invoke("Tower Off", this);
            powerDisplay.fillAmount = 0f;
        }
        else
        {
            Messenger<TowerPowerScript>.Invoke("Tower On", this);
            powerDisplay.fillAmount = Mathf.Min(power, 100f) / 100f;
        }
    }
}
