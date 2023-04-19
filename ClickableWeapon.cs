using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableWeapon : Clickable       
{
    PlayerWeapons playerWeapons;
    Weapon weapon;

    public override void Start()
    {
        if (!TryGetComponent<Weapon>(out weapon))
        {
            Debug.LogWarning("ERROR: CLICKABLE WEAPON DOES NOT HAVE A WEAPON");
        }
        playerWeapons = PlayerWeapons.playerWeapons;
        base.Start();
    }

    public override void onAttachToPlayer(Slot slot)
    {
        playerWeapons.addWeapon(weapon);
        base.onAttachToPlayer(slot);
    }

    public override void onDetachFromPlayer()
    {
        playerWeapons.removeWeapon(weapon);
        base.onDetachFromPlayer();
    }
}
