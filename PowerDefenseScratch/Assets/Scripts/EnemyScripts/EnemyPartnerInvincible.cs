using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyPartnerInvincible : EnemyPartnerScript {
    public EnemyHealthScript health;
    public EnemyMovement move;

    public float invulnerableTime;
    public float pauseTime;

    public override List<float> GetVariables()
    {
        List<float> res = new List<float>();
        res.Add(invulnerableTime);
        res.Add(pauseTime);
        return res;
    }

    // Use this for initialization
    public override void Execute()
    {
        health.MakeInvincible();
    }

    //If one partner is left, remove any effects the partner script is causing
    public override void Revert()
    {
        health.MakeVulnerable();
    }

    //Start Up Transition for partner interaction
    public override void StartTransition()
    {
        move.SetPauseMovement(true);
    }

    //End transition for partner interaction
    public override void StopTransition()
    {
        move.SetPauseMovement(false);
    }
}
