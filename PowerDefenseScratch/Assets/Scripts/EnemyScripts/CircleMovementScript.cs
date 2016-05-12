using UnityEngine;
using System.Collections;

public class CircleMovementScript : EnemyMovement {
    public Transform centerTransform;

    public float travelRadius;

    public float startAngle;

    public float timeToRotate;

    float anglesPerSecond;
    float currentAngle;

	// Use this for initialization
    public override void Start()
    {
        base.Start();
        //float diameterTravel = 2 * Mathf.PI * travelRadius;
        //float travel360 = diameterTravel / baseSpeed;
        anglesPerSecond = (2f * Mathf.PI) / timeToRotate;
        currentAngle = startAngle;
	}
	
	// Update is called once per frame
    public override void Update()
    {
        if(killed) return;
        if (pauseMovement) return;
        currentAngle = (currentAngle + anglesPerSecond * Time.deltaTime) % (2f * Mathf.PI);
        transform.localPosition = new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle)) * travelRadius;
	}
    

    public override Vector3 PredictPosition(float timePassed)
    {
        if (killed || pauseMovement) return transform.position;
        Vector3 parentPos;
        EnemyMovement parentMovement = transform.parent.gameObject.GetComponentInParent<EnemyMovement>();
        if(parentMovement == null)
        {
            Debug.Log("Wrong Object");
            parentPos = transform.parent.position;
        }
        else
        {
            parentPos = parentMovement.PredictPosition(timePassed);
        }
        float predictedAngle = currentAngle + timePassed * anglesPerSecond;
        return parentPos + new Vector3(Mathf.Cos(predictedAngle), Mathf.Sin(predictedAngle)) * travelRadius;
    }
}
