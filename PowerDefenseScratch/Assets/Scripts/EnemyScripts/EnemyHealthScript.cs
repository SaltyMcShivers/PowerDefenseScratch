﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EnemyHealthScript : MonoBehaviour {
    public int maxHealth;
    int currentHealth;

    public float droppedEnergy;
    public int droppedMetal;

    public int firePriority;
    public int invulnerablePriority;

    int currentPriority;

    public Slider healthSlider;
    public Image healthMeter;
    public Image physResistMeter;
    public Image energyResistMeter;

    public float minMeterWidth;
    public float meterWidth;

    float physicalResistance = 1.0f;
    float energyResistance = 1.0f;

    bool invulnerable;

    public Color defaultColor;
    public Color invincibleColor;

    Animator anim;

    void Awake()
    {
        anim = GetComponentInParent<Animator>();
        currentPriority = firePriority;
    }

	// Use this for initialization
	void Start ()
    {
        healthSlider.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(minMeterWidth + (1.0f * maxHealth) / meterWidth, 1.0f);
        //healthSlider.transform.parent.GetComponent<RectTransform>().localScale = new Vector2(minMeterWidth + (1.0f * maxHealth) / meterWidth, 1.0f);
        currentHealth = maxHealth;
        healthSlider.value = (float) currentHealth / (float) maxHealth;
        healthSlider.gameObject.SetActive(false);
        if (invulnerable) healthMeter.color = invincibleColor;
        else healthMeter.color = defaultColor;
    }

    public void MakeInvincible()
    {
        invulnerable = true;
        currentPriority = invulnerablePriority;
        healthMeter.color = invincibleColor;
        anim.SetBool("ShieldOn", true);
        anim.SetBool("SheildTransition", false);
    }

    public void MakeVulnerable()
    {
        invulnerable = false;
        currentPriority = firePriority;
        healthMeter.color = defaultColor;
        anim.SetBool("ShieldOn", false);
        anim.SetBool("SheildTransition", false);
    }

    public void VulnerableTransition()
    {
        anim.SetBool("SheildTransition", true);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        ProjectileScript proj = col.gameObject.GetComponent<ProjectileScript>() as ProjectileScript;
        if (proj == null) return;
        //Destroy(proj.gameObject);
    }

    public void DoDamage(float damageToDo, TowerFiringScript.TowerDamageType dType)
    {
        DamageWithDelay(damageToDo, dType, 0f);
    }

    public void DamageWithDelay(float damageToDo, TowerFiringScript.TowerDamageType dType, float damageDelay)
    {
        StartCoroutine(DoDCoroutine(Mathf.RoundToInt(damageToDo), dType, damageDelay));
    }

    IEnumerator DoDCoroutine(int damageToDo, TowerFiringScript.TowerDamageType dType, float damageDelay)
    {
        yield return new WaitForSeconds(damageDelay);
        if (currentHealth <= 0) yield break;
        if (invulnerable) yield break;
        if (currentHealth == maxHealth && damageToDo > 0)
        {
            healthSlider.gameObject.SetActive(true);
        }
        float resistance = 1.0f;
        if (dType == TowerFiringScript.TowerDamageType.Physical) resistance = physicalResistance;
        else if (dType == TowerFiringScript.TowerDamageType.Electric) resistance = energyResistance;
        currentHealth -= Mathf.RoundToInt(damageToDo * resistance);
        healthSlider.value = (float)currentHealth / (float)maxHealth;
        if (currentHealth <= 0) StartCoroutine("KillEnemyCoroutine");
    }

    IEnumerator KillEnemyCoroutine()
    {
        Messenger<GameObject>.Invoke("Destroy Enemy", gameObject);
        if(anim != null)
        {
            anim.SetBool("ShieldOn", false);
            anim.SetBool("SheildTransition", false);
        }
        transform.parent.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
        transform.parent.SetParent(null);
        healthSlider.gameObject.SetActive(false);
        yield return new WaitForSeconds(2f);
        Destroy(transform.parent.gameObject);
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    public int GetPriorityLevel()
    {
        return currentPriority;
    }

    public void SetResistance(float resistAmount, TowerFiringScript.TowerDamageType dType)
    {
        if(dType == TowerFiringScript.TowerDamageType.Physical)
        {
            physicalResistance *= resistAmount;
            physResistMeter.fillAmount = 0.5f * (1.0f - physicalResistance);
        }
        else
        {
            energyResistance *= resistAmount;
            energyResistMeter.fillAmount = 0.5f * (1.0f - energyResistance);
        }
    }

    public float GetResistance(bool isPhys)
    {
        return isPhys ? physicalResistance : energyResistance;
    }

    public void KillEnemy()
    {
        DoDamage(maxHealth, TowerFiringScript.TowerDamageType.Status);
    }
}
