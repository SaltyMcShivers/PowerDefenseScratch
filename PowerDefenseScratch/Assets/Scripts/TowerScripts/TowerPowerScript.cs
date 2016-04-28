using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TowerPowerScript : MonoBehaviour {
    private float power;
    public bool disabled;
    public Image powerDisplay;

    public ElectricPathNode electricSource;

    public float GetCurrentPower()
    {
        if (disabled) return 0;
        if (!IsGettingPower()) return 0;
        return Mathf.Min(power, 100f);
    }

    public bool IsGettingPower()
    {
        if (disabled) return false;
        return electricSource.IsGettingEnergy();
    }

    public void SetCurrentPower()
    {
        power = electricSource.TowerEnergyAmount();
        powerDisplay.fillAmount = Mathf.Min(power, 100f) / 100f;
    }

    public void SetCurrentPower(float f)
    {
        if (f == 0 && !electricSource.IsGettingEnergy()) disabled = false;
        power = f;
        powerDisplay.fillAmount = Mathf.Min(power, 100f) / 100f;
    }

    public void TogglePower()
    {
        if (!electricSource.IsGettingEnergy()) return;
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

    public bool IsDisabled()
    {
        return disabled;
    }
}
