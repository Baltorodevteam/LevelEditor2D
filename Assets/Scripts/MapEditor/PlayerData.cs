using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{ 
    public float hp;
    public float armor;
    public float weaponDamage;
    public float fireRate;
    public float moveSpeed;

    public float Calculate()
    {
        float dps = weaponDamage * fireRate;
        float survivability = hp * (1f + armor / 100f);

        return dps * 1.2f + survivability + moveSpeed;
    }
}
