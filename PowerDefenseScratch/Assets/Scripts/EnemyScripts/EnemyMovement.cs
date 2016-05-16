using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour {
    public float baseSpeed;

    public float edgeOffset;

    float speedMultiplier = 1f;

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
    }

    public void TogglePauseMovement()
    {
        pauseMovement = !pauseMovement;
    }

    public void StartFollowing(PathNode pn, float offset, bool keepPosition=false)
    {
        travelVector = new Vector3(0f, 0f, 0f);
        edgeOffset = offset;
        if(!keepPosition) transform.position = pn.GetPathTarget(this);
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
            travelScale = Vector3.Distance(nextTarget, pn.GetPathTarget(this)) / Vector3.Distance(pn.transform.position, next.transform.position);
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
    }

    public void SlowDownEnemy(float slowRate, float slowTime)
    {
        StartCoroutine(SlowDownCoroutine(slowRate, slowTime));
    }

    IEnumerator SlowDownCoroutine(float slowRate, float slowTime)
    {
        speedMultiplier *= slowRate;
        yield return new WaitForSeconds(slowTime);
        speedMultiplier /= slowRate;
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
}
