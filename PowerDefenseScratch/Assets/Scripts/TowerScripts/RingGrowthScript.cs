using UnityEngine;
using System.Collections;

public class RingGrowthScript : MonoBehaviour {

    public Color ringStartColor;
    public Color ringEndColor;
    public float ringFadeStart;
    public float ringSpeed;
    public int pointCount = 32;

    float maxRadius;
    float currentRadius;
    LineRenderer line;

    public void SetMaxRadius(float f)
    {
        maxRadius = f;
    }

    // Use this for initialization
    void Start () {
        line = GetComponent<LineRenderer>();
        line.SetColors(ringStartColor, ringEndColor);
        line.SetVertexCount(pointCount+1);
        for(int i=0; i< pointCount; i++)
        {
            line.SetPosition(i, Vector3.zero);
        }
        line.SetPosition(pointCount, Vector3.zero);
        currentRadius = 0f;
    }
	
	// Update is called once per frame
	void Update () {
        currentRadius += ringSpeed * Time.deltaTime;
        if (currentRadius > maxRadius)
        {
            Destroy(gameObject);
            return;
        }
        else if(currentRadius > ringFadeStart)
        {
            Color newColor = Color.Lerp(ringStartColor, ringEndColor, (currentRadius - ringFadeStart) / (maxRadius - ringFadeStart));
            line.SetColors(newColor, newColor);
        }

        float newRadians = 2 * Mathf.PI / pointCount;
        for (int i = 0; i < pointCount; i++)
        {
            line.SetPosition(i, new Vector3(Mathf.Cos(newRadians * i), Mathf.Sin(newRadians * i), 0f) * currentRadius);
        }
        line.SetPosition(pointCount, Vector3.right * currentRadius);
    }
}
