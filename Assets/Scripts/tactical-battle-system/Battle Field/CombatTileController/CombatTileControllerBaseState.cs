using System;
using UnityEngine;

namespace DPS.TacticalCombat{
[Serializable]
public abstract class CombatTileControllerBaseState 
{   
    [SerializeField]
    public CombatTileInteractionSO combatTileInteractionSO;

    public virtual void EnterState(CombatTileController combatTileController) {
        return;
    }

    public virtual void UpdateTileState(CombatTileController combatTileController) {
        return;
    }

    public virtual void UpdateTile(CombatTileController combatTileController) {
        return;
    }

    public virtual void PerformTurnStartInteraction(BattleManager battleController, CombatTileController combatTileController) {
        if (combatTileInteractionSO == null) {
            return;
        }

        if(combatTileController.HasPartyOccupant() ) {
            combatTileInteractionSO.OnEntityTurnStart(battleController, combatTileController.GetPartyOccupant(), combatTileController);
        }
    }

    public virtual void EndState(CombatTileController combatTileController) {
        return;
    }

    public virtual void UpdateTileTurnState(BattleManager battleController, CombatTileController combatTileController) {
        combatTileController.TurnChange(battleController);
        return;
    }

}
}
