using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TowerPowerScript : MonoBehaviour {
    private float power;
    public bool disabled;
    public Image powerDisplay;

    public SpriteRenderer towerImageTemp;
    public Color32 towerColorActiveTemp;
    public Color32 towerColorInactiveTemp;

    public ElectricPathNode electricSource;

    Renderer towerRenderer;
    public Color lightColorBase;
    public float offIntensity;
    public float minPowIntensity;
    public float maxPowIntensity;

    void Awake()
    {
        towerRenderer = GetComponentInChildren<Renderer>();
    }

    public float GetCurrentPower()
    {
        if (disabled) return 0;
        if (!IsGettingPower()) return 0;
        return Mathf.Min(power, 100f);
    }

    public bool IsGettingPower()
    {
        if (disabled) return false;
        if (electricSource == null) return false;
        return electricSource.IsGettingEnergy();
    }

    public void SetCurrentPower()
    {
        power = electricSource.TowerEnergyAmount();
        VisualizePowerAmount(Mathf.Min(power, 100f) / 100f);
    }

    public void SetCurrentPower(float f)
    {
        //if (f == 0 && !electricSource.IsGettingEnergy()) disabled = false;
        if(f == 0 || !electricSource.IsGettingEnergy())
        {
            if(towerImageTemp != null) towerImageTemp.color = towerColorInactiveTemp;
        }
        else
        {
            if (towerImageTemp != null) towerImageTemp.color = towerColorActiveTemp;
        }
        power = f;
        VisualizePowerAmount(Mathf.Min(power, 100f) / 100f);
    }

    public void TogglePower()
    {
        TogglePower(!disabled);
    }

    public void TogglePower(bool pow)
    {
        if (pow == disabled) return;
        disabled = pow;
        if (!electricSource.IsGettingEnergy())
        {
            electricSource.DisableHighlight();
            return;
        }
        if (disabled)
        {
            Messenger<TowerPowerScript>.Invoke("Tower Off", this);
            if (towerImageTemp != null) towerImageTemp.color = towerColorInactiveTemp;
            VisualizePowerAmount(0f);
        }
        else
        {
            Messenger<TowerPowerScript>.Invoke("Tower On", this);
            if (towerImageTemp != null) towerImageTemp.color = towerColorActiveTemp;
            VisualizePowerAmount(Mathf.Min(power, 100f) / 100f);
        }
    }

    public bool IsDisabled()
    {
        return disabled;
    }

    void VisualizePowerAmount(float power)
    {
        powerDisplay.fillAmount = power;
        if (towerRenderer == null)
        {
            return;
        }
        float intensity;
        if (power == 0) intensity = offIntensity;
        else intensity = Mathf.Lerp(minPowIntensity, maxPowIntensity, power);
        towerRenderer.material.SetColor("_EmissionColor", lightColorBase * intensity);
    }
}
