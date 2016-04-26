using UnityEngine;
using System.Collections;

public class PathNode : MonoBehaviour {

    public PathNode prevNode;
    public PathNode nextNode;

    public GameObject lineSprite;

    void Start()
    {
        if (prevNode == null)
        {
            lineSprite.transform.parent.localScale = Vector3.zero;
        }
        else
        {
            lineSprite.transform.localScale = new Vector3(Vector3.Distance(transform.position, prevNode.transform.position) - 1f, 1f, 1f);
            lineSprite.transform.parent.Rotate(Vector3.back, Vector3.Angle(Vector3.right, prevNode.transform.position - transform.position));
            //var cross:Vector3 = Vector3.Cross(vectorA, vectorB);
            //if (cross.y < 0) angle = -angle;
            if (prevNode.transform.position.y > transform.position.y) lineSprite.transform.parent.Rotate(Vector3.back, 180f);
        }
    }

    public Vector3 GetPathTarget(EnemyMovement mover)
    {
        if (nextNode == null)
        {
            Quaternion rot = Quaternion.Euler(0f, 0f, Vector3.Angle(Vector3.right, Vector3.Normalize(transform.position - prevNode.transform.position)));
            Vector3 offset = Vector3.up * mover.edgeOffset;
            return rot * offset + transform.position;
        }
        else if (prevNode == null)
        {
            Quaternion rot = Quaternion.Euler(0f, 0f, Vector3.Angle(Vector3.right, Vector3.Normalize(nextNode.transform.position - transform.position)));
            Vector3 offset = Vector3.up * mover.edgeOffset;
            return rot * offset + transform.position;
        }
        Vector3 nextPath = Vector3.Normalize(nextNode.transform.position - transform.position);
        Vector3 prevPath = Vector3.Normalize(prevNode.transform.position - transform.position);
        Vector3 offsetVector;
        if (Vector3.Angle(nextPath, prevPath) > Vector3.Angle(nextPath, transform.position - mover.transform.position))
        {
            //Inside the curve
            offsetVector = -nextPath - prevPath;
        }
        else
        {
            //Outside the Curve
            offsetVector = prevPath + nextPath;
        }
        return transform.position + offsetVector * Mathf.Abs(mover.edgeOffset);
    }
}
