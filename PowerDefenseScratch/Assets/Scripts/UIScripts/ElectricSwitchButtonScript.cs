using UnityEngine;
using System.Collections;

public class ElectricSwitchButtonScript : RadialButtonScript
{
    public ElectricPathNode source;


    public override void DisableIfNeeded()
    {
        if (source == null) DisableButton();
    }

    public ElectricPathNode GetSource()
    {
        return source;
    }

}
