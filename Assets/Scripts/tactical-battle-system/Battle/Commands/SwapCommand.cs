using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[System.Serializable]
public abstract class SwapCommand : BattleCommand
{


    #nullable enable
    public SwapCommand(
        PartySlot user, 
        CombatTileController targetedTile, 
        List<CombatTileController> tiles, 
        List<PartySlot> targetedSlots ): base("Swap", user, targetedTile, tiles, targetedSlots) {

    }
    
    public override bool CanExecuteCommand()
    {
        return true;
    }

}
}
