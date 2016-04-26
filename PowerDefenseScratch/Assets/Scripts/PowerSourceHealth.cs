using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PowerSourceHealth : MonoBehaviour {
    public float maximumHealth;
    public Slider healthSlider;
    float currentHealth;

    void Start()
    {
        Messenger<float>.AddListener("Enemy Attacks", RemoveHealth);
        ResetHealth();
    }

    void ResetHealth()
    {
        currentHealth = maximumHealth;
        healthSlider.value = 1.0f;
    }

    void RemoveHealth(float f){
        if(currentHealth <= 0) return;
        currentHealth -= f;
        if(currentHealth <= 0){
            Debug.Log("Game Over");
        }
        healthSlider.value = Mathf.Max(0f, currentHealth / maximumHealth);
    }

    void OnDisabled()
    {
        Messenger<float>.RemoveListener("Enemy Attacks", RemoveHealth);
    }
}
