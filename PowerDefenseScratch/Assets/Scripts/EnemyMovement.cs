using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour {
    public float baseSpeed;
    float currentSpeed;

    Vector3 travelVector;

    PathNode targetPosition;

    bool killed;

    void Start()
    {
        currentSpeed = baseSpeed;
        Messenger<GameObject>.AddListener("Destroy Enemy", CheckDeath);
    }
	

	void Update () {
        if(targetPosition == null)
        {
            return;
        }
        if (killed) return;
        float differenceDistance = Vector3.Distance(targetPosition.transform.position, transform.position);
        float distanceToTravel = currentSpeed * Time.deltaTime;
        if (differenceDistance <= distanceToTravel)
        {
            distanceToTravel -= differenceDistance;
            GoToNextNode(targetPosition);
            if (targetPosition == null)
            {
                return;
            }
        }
        transform.position += distanceToTravel * travelVector;
        
    }

    public void StartFollowing(PathNode pn)
    {
        travelVector = new Vector3(0f, 0f, 0f);
        GoToNextNode(pn);
    }

    void GoToNextNode(PathNode pn)
    {
        PathNode next = pn.nextNode;
        if (next == null)
        {
            transform.position = pn.transform.position;
            travelVector = Vector3.zero;
        }
        else
        {
            travelVector = next.transform.position - pn.transform.position;
            travelVector.Normalize();
        }
        targetPosition = next;
    }

    void CheckDeath(GameObject go)
    {
        if(go.transform.parent.gameObject == gameObject)
        {
            killed = true;
            Messenger<GameObject>.RemoveListener("Destroy Enemy", CheckDeath);
        }
    }
}
