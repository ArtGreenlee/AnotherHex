using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XP : Attractable
{
    public float xpAmount;

    public override void attractableEffect(Attractor a)
    {
        a.GetComponentInParent<Player>().addXP(xpAmount);
    }
}
