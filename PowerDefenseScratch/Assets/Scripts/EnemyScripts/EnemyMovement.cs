using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour {
    public float baseSpeed;

    public float edgeOffset;

    float currentSpeed;

    Vector3 travelVector;

    PathNode targetPosition;

    protected bool killed;
    protected bool pauseMovement;

    public virtual void Start()
    {
        currentSpeed = baseSpeed;
        Messenger<GameObject>.AddListener("Destroy Enemy", CheckDeath);
    }
	

	public virtual void Update () {
        if(targetPosition == null)
        {
            return;
        }
        if (killed) return;
        if (pauseMovement) return;
        Vector3 targetDetination = targetPosition.GetPathTarget(this);
        float differenceDistance = Vector3.Distance(targetDetination, transform.position);
        float distanceToTravel = currentSpeed * Time.deltaTime;
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

    public void TogglePauseMovement()
    {
        pauseMovement = !pauseMovement;
    }

    public void StartFollowing(PathNode pn, float offset)
    {
        travelVector = new Vector3(0f, 0f, 0f);
        edgeOffset = offset;
        GoToNextNode(pn);
    }

    void GoToNextNode(PathNode pn)
    {
        PathNode next = pn.nextNode;
        if (next == null)
        {
            transform.position = pn.GetPathTarget(this);
            travelVector = Vector3.zero;
            GetComponent<EnemyAttackScript>().StartAttacking();
        }
        else
        {
            travelVector = next.transform.position - pn.transform.position;
            travelVector.Normalize();
        }
        targetPosition = next;
    }

    public virtual void CheckDeath(GameObject go)
    {
        if(go.transform.parent.gameObject == gameObject)
        {
            KillOff();
        }
    }

    void KillOff()
    {
        killed = true;
        EnemyAttackScript attack = GetComponent<EnemyAttackScript>();
        if (attack != null) attack.StopAttacking();
        Messenger<GameObject>.RemoveListener("Destroy Enemy", CheckDeath);
    }
}
