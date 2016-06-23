using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PowerSourceHealth : MonoBehaviour {
    public float maximumHealth;
    public List<Slider> healthSliders;
    public Color baseColor;
    public float minGlow;
    public float maxGlow;

    public Color healthyColor;
    public Color unhealthyColor;
    public float blinkRateBase;
    public Color warningColor;
    public float warningHealth;
    public float blinkRateWorst;

    float currentHealth;

    Material glowMat;

    void Start()
    {
        Messenger<float>.AddListener("Enemy Attacks", RemoveHealth);
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null) glowMat = rend.material;
        ResetHealth();
    }

    void OnDestroy()
    {
        Messenger<float>.RemoveListener("Enemy Attacks", RemoveHealth);
    }

    void Update()
    {
        if (glowMat == null) return;
        if (currentHealth == maximumHealth) return;
        if (currentHealth > warningHealth)
        {
            Color currentMinColor = Color.Lerp(unhealthyColor, healthyColor, (currentHealth - warningHealth) / (maximumHealth - warningHealth));
            glowMat.SetColor("_EmissionColor", Color32.Lerp(currentMinColor, healthyColor, Mathf.PingPong(Time.time * blinkRateBase, 1.0f)));
        }
        else
        {
            float currentBlinkRate = Mathf.Lerp(blinkRateWorst, blinkRateBase, currentHealth / warningHealth);
            glowMat.SetColor("_EmissionColor", Color32.Lerp(unhealthyColor, warningColor, Mathf.PingPong(Time.time * currentBlinkRate, 1.0f)));
        }
    }

    public float GetHealth()
    {
        return currentHealth;
    }

    public void SetHealth(float health)
    {
        currentHealth = health;
        foreach (Slider slide in healthSliders)
        {
            slide.value = Mathf.Max(0f, currentHealth / maximumHealth);
        }
        /*
        if (glowMat != null)
        {
            float intensity = Mathf.Lerp(minGlow, maxGlow, currentHealth / maximumHealth);
            glowMat.SetColor("_EmissionColor", baseColor * intensity);
        }
        */
    }

    void ResetHealth()
    {
        currentHealth = maximumHealth;
        foreach(Slider slide in healthSliders)
        {
            slide.value = 1.0f;
        }
        if (glowMat != null)
        {
            glowMat.SetColor("_EmissionColor", healthyColor);
            //glowMat.SetColor("_EmissionColor", baseColor * maxGlow);
        }
    }

    void RemoveHealth(float f){
        if(currentHealth <= 0) return;
        currentHealth -= f;
        if(currentHealth <= 0)
        {
            Messenger<bool>.Invoke("End Game", false);
            Time.timeScale = 0;
        }
        /*
        if (glowMat != null)
        {
            if (currentHealth == 0)
            {
                glowMat.SetColor("_EmissionColor", Color.black);
            }
            else
            {
                float intensity = Mathf.Lerp(minGlow, maxGlow, currentHealth / maximumHealth);
                glowMat.SetColor("_EmissionColor", baseColor * intensity);
            }
        }
        */
            foreach (Slider slide in healthSliders)
        {
            if (!slide.gameObject.activeSelf) slide.gameObject.SetActive(true);
            slide.value = Mathf.Max(0f, currentHealth / maximumHealth);
        }
    }

    void OnDisabled()
    {
        Messenger<float>.RemoveListener("Enemy Attacks", RemoveHealth);
    }
}
