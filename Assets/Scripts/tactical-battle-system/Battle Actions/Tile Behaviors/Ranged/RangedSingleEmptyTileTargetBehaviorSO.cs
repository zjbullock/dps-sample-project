using System;
using UnityEngine;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Ranged Empty Tile Behavior", menuName = "ScriptableObjects/Active Skill/Behavior/Ranged/Empty Tile")]

public class RangedSingleEmptyTileTargetBehaviorSO : RangedActionTileBehaviorSO
{
    public override bool CanBeDoneToTargetedTile(CombatTileController targetedTile, PartySlot caster, PartySlot targetedEntity, Func<PartySlot, bool> isSameEntityCheck)
    {
        if(targetedTile.GetPartyOccupant() != null)
        {
            return false;
        }

        return true;
    }
}
}