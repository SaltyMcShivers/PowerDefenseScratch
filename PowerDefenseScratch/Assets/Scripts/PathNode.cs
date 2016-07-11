using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PathNode : MonoBehaviour {

    public PathNode prevNode;
    public PathNode nextNode;

    public List<PathNode> prevNodes;

    public GameObject lineSprite;

    void Start()
    {
        if(nextNode == null)
        {
            lineSprite.transform.parent.localScale = Vector3.zero;
        }
        else
        {
            lineSprite.transform.localScale = new Vector3(Vector3.Distance(transform.position, nextNode.transform.position) - 1f, 1f, 1f);
            lineSprite.transform.parent.Rotate(Vector3.back, Vector3.Angle(Vector3.right, nextNode.transform.position - transform.position));
            //var cross:Vector3 = Vector3.Cross(vectorA, vectorB);
            //if (cross.y < 0) angle = -angle;
            if (nextNode.transform.position.y > transform.position.y) lineSprite.transform.parent.Rotate(Vector3.back, 180f);
        }
    }

    public Vector3 GetPathTarget(EnemyMovement mover, PathNode oldNode = null)
    {
        if (mover.edgeOffset == 0) return transform.position;
        if (nextNode == null)
        {
            Vector3 directionPath = Vector3.Normalize(transform.position - oldNode.transform.position) * -1f;
            return new Vector3(-directionPath.y, directionPath.x) * mover.edgeOffset + transform.position;
        }
        else if (oldNode == null)
        {
            Vector3 upcomingPath = Vector3.Normalize(nextNode.transform.position - transform.position);
            return new Vector3(upcomingPath.y, -upcomingPath.x) * mover.edgeOffset + transform.position;
        }
        Vector3 nextPath = Vector3.Normalize(nextNode.transform.position - transform.position);
        Vector3 prevPath = Vector3.Normalize(oldNode.transform.position- transform.position);
        Vector3 offsetVector;
        if(nextPath == -1f * prevPath)
        {
            offsetVector = new Vector3(nextPath.y, -nextPath.x);
            return transform.position + offsetVector * mover.edgeOffset;
        }
        else if (Vector3.Angle(nextPath, prevPath) > Vector3.Angle(nextPath, transform.position - mover.transform.position))
        {
            //Inside the curve
            offsetVector = - prevPath - nextPath;
        }
        else
        {
            //Outside the Curve
            offsetVector = nextPath + prevPath;
        }
        return transform.position + offsetVector * Mathf.Abs(mover.edgeOffset);
    }
}
