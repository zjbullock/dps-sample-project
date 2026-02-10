using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat{
[System.Serializable]
public class TileEvent : IBattleEvent
{
    protected TileEventAnimatorController combatTileAnimatorController;

    public TileEvent(TileEventAnimatorController combatTileAnimatorController) {
        this.combatTileAnimatorController = combatTileAnimatorController;
    }

    public void Execute()
    {
        return;
    }

    public bool IsDone()
    {
        return !this.combatTileAnimatorController.IsAnimating;
    }

    public void End() {
        return;
    }
}
}