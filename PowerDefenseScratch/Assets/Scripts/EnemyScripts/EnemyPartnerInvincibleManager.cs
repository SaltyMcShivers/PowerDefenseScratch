using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyPartnerInvincibleManager : EnemyPartnerManager
{
    float invulnerableTime;
    float invulnerablePause;

    int currentInvulnerable;
    
    public override void SetUpManager(List<float> vars)
    {
        invulnerableTime = vars[0];
        invulnerablePause = vars[1];
    }

    //When all the partners have been found, initialize the partners
    public override void StartUpPartners()
    {
        currentInvulnerable = 0;
        StartCoroutine("InvulnerableCoroutine");
    }

    public override void SetUpNumberOfPartners()
    {
        base.SetUpNumberOfPartners();
        desiredNumberOfPartners = 2;
    }

    IEnumerator InvulnerableCoroutine()
    {
        partners[currentInvulnerable].Execute();
        partners[1 - currentInvulnerable].Revert();
        Messenger.Invoke("Priorities Changed");
        yield return new WaitForSeconds(invulnerableTime);
        foreach(EnemyPartnerScript enemy in partners)
        {
            enemy.StartTransition();
        }
        yield return new WaitForSeconds(invulnerablePause);
        foreach (EnemyPartnerScript enemy in partners)
        {
            enemy.StopTransition();
        }
        currentInvulnerable = (currentInvulnerable + 1) % partners.Count;
        StartCoroutine("InvulnerableCoroutine");
    }
}
