using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PowerSourceHealth : MonoBehaviour {
    public float maximumHealth;
    public List<Slider> healthSliders;
    float currentHealth;

    void Start()
    {
        Messenger<float>.AddListener("Enemy Attacks", RemoveHealth);
        ResetHealth();
    }

    void OnDestroy()
    {
        Messenger<float>.RemoveListener("Enemy Attacks", RemoveHealth);
    }

    void ResetHealth()
    {
        currentHealth = maximumHealth;
        foreach(Slider slide in healthSliders)
        {
            slide.value = 1.0f;
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
