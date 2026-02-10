using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
    [Serializable]
public class CombatTileControllerDefaultState : CombatTileControllerBaseState
{
    // public CombatTileControllerDefaultState(): base() {

    // }
    public override void UpdateTileTurnState(BattleManager battleController, CombatTileController combatTileController)
    {
        base.UpdateTileTurnState(battleController, combatTileController);
        return;
    }

}
}