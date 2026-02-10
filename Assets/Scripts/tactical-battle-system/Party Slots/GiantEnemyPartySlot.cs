using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
public class GiantEnemyPartySlot : EnemyPartySlot
{
    [SerializeField]
    private int width;


    public GiantEnemyPartySlot(PrefabEnemyBattleObject prefabEnemyBattleObject, BattleMember battleMember) : base(battleMember)
    {
        this.width = prefabEnemyBattleObject.Width + prefabEnemyBattleObject.EnemyHitBoxGraceValue;
    }

    protected override int GetWidth()
    {
        return this.width;
    }
}
}