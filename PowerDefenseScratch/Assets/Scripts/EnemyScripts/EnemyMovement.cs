using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour {
    public float baseSpeed;

    public float edgeOffset;

    public ParticleSystem mainSlowEffect;
    public Color slowCloudColorLow;
    public Color slowCloudColorHigh;

    public ParticleSystem slowStreakEffect;
    public int streaksAmountLow;
    public int streaksAmountHigh;

    public Vector3 targDest;

    float speedMultiplier = 1f;

    float slowAmount = 1f;

    float currentSpeed;
    float travelScale;

    Vector3 travelVector;

    PathNode targetPosition;
    PathNode previousTarget;

    protected bool killed;
    protected bool pauseMovement;

    public virtual void Start()
    {
        currentSpeed = baseSpeed;
        Messenger<GameObject>.AddListener("Destroy Enemy", CheckDeath);
    }

    public void OnDestroy()
    {
        Messenger<GameObject>.RemoveListener("Destroy Enemy", CheckDeath);
    }
	

	public virtual void Update () {
        if(targetPosition == null)
        {
            return;
        }
        if (killed) return;
        if (pauseMovement) return;
        Vector3 targetDetination = targetPosition.GetPathTarget(this, previousTarget);
        float differenceDistance = Vector3.Distance(targetDetination, transform.position);
        float distanceToTravel = currentSpeed * speedMultiplier * Time.deltaTime * travelScale;
        if (differenceDistance <= distanceToTravel)
        {
            distanceToTravel -= differenceDistance;
            transform.position = targetDetination;
            GoToNextNode(targetPosition);
            if (targetPosition == null)
            {
                return;
            }
        }

        transform.position += distanceToTravel * travelVector;
        targDest = targetDetination;
        /*
        if(Mathf.Abs(Vector3.Angle(travelVector, targetDetination - transform.position)) > 170f)
        {
            Debug.Log("BROKE");
            GoToNextNode(targetPosition);
            if (targetPosition == null)
            {
                return;
            }
        }
        */
        //Error check to make sure it doesn't overshoot
        /*
        if (Vector3.Normalize(travelVector) != Vector3.Normalize(targetDetination - transform.position))
        {
            Debug.Log("Something Screwed Up");
            GoToNextNode(targetPosition);
            if (targetPosition == null)
            {
                return;
            }
        }
        */
    }

    public void SetPauseMovement(bool pause)
    {
        pauseMovement = pause;
        if (!pauseMovement && targetPosition == null)
        {
            EnemyAttackScript attack = GetComponent<EnemyAttackScript>();
            if (attack != null) attack.StartAttacking();
        }
        else if (pauseMovement)
        {
            EnemyAttackScript attack = GetComponent<EnemyAttackScript>();
            if (attack != null) attack.StopAttacking();
        }
    }

    public void TogglePauseMovement()
    {
        pauseMovement = !pauseMovement;
        if(!pauseMovement && targetPosition == null)
        {
            EnemyAttackScript attack = GetComponent<EnemyAttackScript>();
            if (attack != null) attack.StartAttacking();
        }
        else if(pauseMovement)
        {
            EnemyAttackScript attack = GetComponent<EnemyAttackScript>();
            if (attack != null) attack.StopAttacking();
        }
    }

    public bool IsMovementPaused()
    {
        return pauseMovement;
    }

    public void StartFollowing(PathNode pn, float offset, float fallBackOffset, bool keepPosition=false)
    {
        travelVector = new Vector3(0f, 0f, 0f);
        edgeOffset = offset;
        if(!keepPosition) transform.position = pn.GetPathTarget(this) + (pn.nextNode.transform.position - pn.transform.position).normalized * fallBackOffset;
        GoToNextNode(pn);
    }

    void GoToNextNode(PathNode pn)
    {
        PathNode next = pn.nextNode;
        if (next == null)
        {
            transform.position = pn.GetPathTarget(this, previousTarget);
            travelVector = Vector3.zero;
            GetComponent<EnemyAttackScript>().StartAttacking();
        }
        else
        {
            Vector3 nextTarget = next.GetPathTarget(this, pn);
            travelScale = Vector3.Distance(nextTarget, pn.GetPathTarget(this, previousTarget)) / Vector3.Distance(pn.transform.position, next.transform.position);
            travelVector = next.transform.position - pn.transform.position;
            travelVector.Normalize();
        }
        previousTarget = pn;
        targetPosition = next;
    }

    public virtual void CheckDeath(GameObject go)
    {
        if(go.transform.parent.gameObject == gameObject)
        {
            KillOff();
            Messenger<GameObject>.RemoveListener("Destroy Enemy", CheckDeath);
        }
    }

    void KillOff()
    {
        killed = true;
        EnemyAttackScript attack = GetComponent<EnemyAttackScript>();
        if (attack != null) attack.StopAttacking();
        StopAllCoroutines();
        mainSlowEffect.Stop();
    }

    public void SlowDownEnemy(float slowRate, float slowTime)
    {
        if (killed) return;
        StartCoroutine(SlowDownCoroutine(slowRate, slowTime));
    }

    IEnumerator SlowDownCoroutine(float slowRate, float slowTime)
    {
        speedMultiplier *= slowRate;
        if (slowRate < 1f)
        {
            slowAmount *= slowRate;
            if (mainSlowEffect != null && slowStreakEffect != null)
            {
                SlowParticles();
            }
        }
        yield return new WaitForSeconds(slowTime);
        speedMultiplier /= slowRate;
        if (slowRate < 1f)
        {
            slowAmount /= slowRate;
            if (mainSlowEffect != null && slowStreakEffect != null)
            {
                SlowParticles();
            }
        }
    }

    public virtual Vector3 PredictPosition(float timePassed)
    {
        if (killed || pauseMovement) return transform.position;
        return transform.position + timePassed * travelVector * speedMultiplier;
    }

    public PathNode GetTarget()
    {
        return previousTarget;
    }

    void SlowParticles()
    {
        if (!mainSlowEffect.isPlaying)
        {
            mainSlowEffect.Play();
        }
        if (slowAmount == 1f)
        {
            mainSlowEffect.Stop();
        }
        mainSlowEffect.startColor = Color.Lerp(slowCloudColorHigh, slowCloudColorLow, slowAmount);
        var streakEmission = slowStreakEffect.emission;
        var streakRate = streakEmission.rate;
        streakRate.constantMax = Mathf.Lerp(streaksAmountHigh, streaksAmountLow, slowAmount);
        streakEmission.rate = streakRate;
    }
}
