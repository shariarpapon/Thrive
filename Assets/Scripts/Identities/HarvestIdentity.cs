using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestIdentity : Identity
{
    public int HP;
    public Harvestable harvestable;

    public bool InflictDamage(int amount)
    {
        HP -= amount;
        if (HP <= 0)
        {
            HP = 0;
            return true;
        }
        return false;
    }
}
