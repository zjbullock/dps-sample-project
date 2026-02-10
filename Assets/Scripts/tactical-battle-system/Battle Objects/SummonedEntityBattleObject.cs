using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
public class SummonedEntityBattleObject : PlayerBattleObject
{
    public override void DestroyBattleObject()
    {
        Destroy(this.gameObject);
    }
}
}