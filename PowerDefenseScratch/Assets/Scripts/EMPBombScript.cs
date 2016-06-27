using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EMPBombScript : MonoBehaviour {
    public float pressDelay;
    public float disableTime;
    public float cooldown;

    public Image timer;
    public Color chargeColor;
    public Color disabledColor;
    public Color cooldownColor;

    Button interactor;
    float lastPressTime;

	// Use this for initialization
	void Start () {
        interactor = GetComponent<Button>();
    }

    void Update()
    {
        if(timer.color == chargeColor)
        {
            timer.fillAmount = (Time.time - lastPressTime) / pressDelay;
        }else if (timer.color == disabledColor)
        {
            timer.fillAmount = 1 - (Time.time - lastPressTime - pressDelay) / disableTime;
        }else if(timer.color == cooldownColor)
        {
            timer.fillAmount = (Time.time - lastPressTime - pressDelay - disableTime) / cooldown;
        }
    }

    public void RunEMP()
    {
        StartCoroutine("EMPCoroutine");
    }

    IEnumerator EMPCoroutine()
    {
        interactor.interactable = false;
        lastPressTime = Time.time;
        timer.color = chargeColor;
        yield return new WaitForSeconds(pressDelay);
        Messenger<float>.Invoke("Launch EMP", disableTime);
        timer.color = disabledColor;
        yield return new WaitForSeconds(disableTime);
        timer.color = cooldownColor;
        yield return new WaitForSeconds(cooldown);
        interactor.interactable = true;
        timer.color = new Color(0f, 0f, 0f, 0f);
    }
}
