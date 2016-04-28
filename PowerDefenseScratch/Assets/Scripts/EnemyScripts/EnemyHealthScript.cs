using UnityEngine;
using System.Collections;
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

    bool invulnerable;

    public Color defaultColor;
    public Color invincibleColor;

    void Awake()
    {
        currentPriority = firePriority;
    }

	// Use this for initialization
	void Start () {
        currentHealth = maxHealth;
        healthSlider.value = (float) currentHealth / (float) maxHealth;
        if(invulnerable) healthMeter.color = invincibleColor;
        else healthMeter.color = defaultColor;
    }

    public void MakeInvincible()
    {
        invulnerable = true;
        currentPriority = invulnerablePriority;
        healthMeter.color = invincibleColor;
    }

    public void MakeVulnerable()
    {
        invulnerable = false;
        currentPriority = firePriority;
        healthMeter.color = defaultColor;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        ProjectileScript proj = col.gameObject.GetComponent<ProjectileScript>() as ProjectileScript;
        if (proj == null) return;
        //Destroy(proj.gameObject);
    }

    public void DoDamage(float damageToDo)
    {
        DamageWithDelay(damageToDo, 0f);
    }

    public void DamageWithDelay(float damageToDo, float damageDelay)
    {
        StartCoroutine(DoDCoroutine(Mathf.RoundToInt(damageToDo), damageDelay));
    }

    IEnumerator DoDCoroutine(int damageToDo, float damageDelay)
    {
        yield return new WaitForSeconds(damageDelay);
        if (currentHealth <= 0) yield break;
        if (invulnerable) yield break;
        currentHealth -= damageToDo;
        healthSlider.value = (float)currentHealth / (float)maxHealth;
        if (currentHealth <= 0) StartCoroutine("KillEnemy");
    }

    IEnumerator KillEnemy()
    {
        Messenger<GameObject>.Invoke("Destroy Enemy", gameObject);
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
}
