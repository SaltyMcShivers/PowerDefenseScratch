using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ElectricPathNode : MonoBehaviour {
    public ElectricPathNode previousNode;
    public List<ElectricPathNode> nextNodes;

    public SpriteRenderer centerSprite;
    public SpriteRenderer lineSprite;

    public Color highEnergyColor;
    public Color activeEnergyColor;
    public Color unactiveEnergyColor;

    public ElectricPathNode nextForPath;

    public List<Animator> highlightAnims;

    public float activeEffectSpeed = 1.0f;

    TowerManagerScript baseOfPower;
    LineRenderer activeEffect;

    public void SetUpStart(TowerManagerScript tms)
    {
        baseOfPower = tms;
    }

    public void SetNextForPath(ElectricPathNode next)
    {
        if(next == null || !nextNodes.Contains(next))
        {
            nextForPath = null;
        }
        else
        {
            nextForPath = next;
        }
        foreach (ElectricPathNode node in nextNodes)
        {
            node.ChangeColoration();
        }
    }

    public void ChangeColoration()
    {
        if (IsGettingEnergy())
        {
            centerSprite.color = activeEnergyColor;
            activeEffect.gameObject.SetActive(true);
            //lineSprite.color = activeEnergyColor;

        }
        else
        {
            centerSprite.color = unactiveEnergyColor;
            activeEffect.gameObject.SetActive(false);
            //lineSprite.color = unactiveEnergyColor;
        }
        foreach (ElectricPathNode node in nextNodes)
        {
            node.ChangeColoration();
        }
    }

	// Use this for initialization
	void Start () {
        if (nextNodes.Count == 1)
        {
            nextForPath = nextNodes[0];
        }
        centerSprite.color = activeEnergyColor;
        //lineSprite.color = activeEnergyColor;
        if (previousNode == null)
        {
            lineSprite.transform.parent.localScale = Vector3.zero;
        }
        else
        {
            lineSprite.transform.parent.localScale = new Vector3(Vector3.Distance(transform.position, previousNode.transform.position), 1f, 1f);
            lineSprite.transform.parent.Rotate(Vector3.back, Vector3.Angle(Vector3.right, previousNode.transform.position - transform.position));
            //var cross:Vector3 = Vector3.Cross(vectorA, vectorB);
            //if (cross.y < 0) angle = -angle;
            if (previousNode.transform.position.y > transform.position.y) lineSprite.transform.parent.Rotate(Vector3.back, 180f);
        }
        activeEffect = GetComponentInChildren<LineRenderer>();
        if (activeEffect != null)
        {
            activeEffect.material.SetTextureScale("_MainTex", lineSprite.transform.parent.localScale);
        }
    }

    void Update()
    {
        if (activeEffect != null)
        {
            activeEffect.material.SetTextureOffset("_MainTex", activeEffect.material.GetTextureOffset("_MainTex") + Vector2.right * activeEffectSpeed * Time.deltaTime);
        }
        if (IsGettingEnergy())
        {
            centerSprite.color = Color.Lerp(activeEnergyColor, highEnergyColor, Mathf.PingPong(Time.time * activeEffectSpeed * 2, 1.0f));
        }
    }

    public void ChangeTarget(ElectricPathNode ele)
    {
        if (ele == null || !nextNodes.Contains(ele)) nextForPath = null;
        else nextForPath = ele;
    }

    public float TowerEnergyAmount()
    {
        return previousNode.GetEnergyAmount(this);
    }

    public float GetEnergyAmount(ElectricPathNode from)
    {
        float baseAmount;

        if (previousNode == null)
        {
            baseAmount = baseOfPower.currentPower;
        }
        else
        {
            baseAmount = previousNode.GetEnergyAmount(this);
        }

        if (nextNodes.Count > 1 && nextForPath == null)
        {
            return baseAmount / nextNodes.Count;
        }
        if(nextForPath != from)
        {
            return 0f;
        }
        return baseAmount;
    }

    public bool IsGettingEnergy()
    {
        if (previousNode == null) return true;
        return previousNode.IsGettingEnergy(this);
    }

    public bool IsGettingEnergy(ElectricPathNode from)
    {
        if (previousNode == null) return true;
        if (nextNodes.Count > 1 && nextForPath == null) return previousNode.IsGettingEnergy(this);
        if (nextForPath != from) return false;
        return previousNode.IsGettingEnergy(this);
    }

    public void ToggleEnergy()
    {
        if (nextForPath == null) nextForPath = nextNodes[0];
        else nextForPath = null;

        foreach (ElectricPathNode node in nextNodes)
        {
            node.ChangeColoration();
        }
    }

    public void DisableHighlight()
    {
        foreach (Animator highlightAnim in highlightAnims)
        {
            highlightAnim.SetTrigger("HighlightSwitch");
        }
    }
}
