using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyPartnerManager : MonoBehaviour
{
    protected int desiredNumberOfPartners;

    protected List<EnemyPartnerScript> partners;
    protected bool allPartnersFound;
    bool emp;

    public virtual void SetUpManager(List<float> vars)
    {

    }

    public bool AllPartnersDestroyed()
    {
        return allPartnersFound && partners.Count <= 1;
    }

    //Return true if it has been added to the partner list
    public bool AddPartner(EnemyPartnerScript eps)
    {
        if(desiredNumberOfPartners == 0)
        {
            SetUpNumberOfPartners();
        }
        if (allPartnersFound) return false;
        partners.Add(eps);
        if (partners.Count == desiredNumberOfPartners)
        {
            allPartnersFound = true;
            StartUpPartners();
        }
        return true;
    }

    public virtual void SetUpNumberOfPartners()
    {
        partners = new List<EnemyPartnerScript>();
    }

    //When all the partners have been found, initialize the partners
    public virtual void StartUpPartners()
    {

    }

    public virtual bool HasPartner(EnemyPartnerScript eps)
    {
        return partners.Contains(eps);
    }

    //When a partner dies, delete it from the list
    public virtual void RemovePartner(EnemyPartnerScript eps)
    {
        partners.Remove(eps);
        if(partners.Count == 1)
        {
            RevertRemainingPartner();
        }
    }

    //If one partner is left, remove any effects the partner script is causing
    public virtual void RevertRemainingPartner()
    {
        if (partners.Count == 0 || emp) return;
        partners[0].Revert();
        partners[0].StopTransition();
        StopAllCoroutines();
        //Destroy(this);
    }

    public virtual void EMPStart()
    {
        emp = true;
    }

    public virtual void EMPStop()
    {
        emp = false;
        if(partners.Count == 0)
        {
            RevertRemainingPartner();
        }
    }
}
