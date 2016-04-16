using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyHealthScript : MonoBehaviour {
    public float maxHealth;
    public float currentHealth;

    public float droppedEnergy;

    public Slider healthSlider;

	// Use this for initialization
	void Start () {
        currentHealth = maxHealth;
        healthSlider.value = currentHealth / maxHealth;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter2D(Collision2D col)
    {
        ProjectileScript proj = col.gameObject.GetComponent<ProjectileScript>() as ProjectileScript;
        if (proj == null) return;
        Destroy(proj.gameObject);
    }

    public void DamageWithDelay(float damageToDo, float damageDelay)
    {
        StartCoroutine(DoDCoroutine(damageToDo, damageDelay));
    }

    IEnumerator DoDCoroutine(float damageToDo, float damageDelay)
    {
        yield return new WaitForSeconds(damageDelay);
        if (currentHealth <= 0) yield break;
        currentHealth -= damageToDo;
        healthSlider.value = currentHealth / maxHealth;
        if (currentHealth <= 0f) StartCoroutine("KillEnemy");
    }

    IEnumerator KillEnemy()
    {
        Messenger<GameObject>.Invoke("Destroy Enemy", gameObject);
        healthSlider.gameObject.SetActive(false);
        yield return new WaitForSeconds(2f);
        Destroy(transform.parent.gameObject);
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }
}
