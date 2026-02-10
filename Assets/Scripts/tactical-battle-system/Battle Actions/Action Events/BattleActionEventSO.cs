using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace DPS.TacticalCombat {
public abstract class BattleActionEventSO : ScriptableObject
{
    [SerializeField]
    private BattleActionController _battleActionController;

    public BattleActionController BattleActionController { get => this._battleActionController; }

    public void Execute(BattleManager battleManager, IBattleActionCommand battleActionCommand, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles)
    {
        if (battleActionCommand.GetTargetType() == TargetTypes.Partner)
        {
            this.ExecutePartnerAction(battleManager, battleActionCommand, user, primaryAbilityTargets, combatTiles);
            return;
        }
            
        this.ExecuteAction(battleManager, battleActionCommand, user, primaryAbilityTargets, combatTiles);
    }

    protected abstract void ExecuteAction(BattleManager battleManager, IBattleActionCommand battleActionCommand, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles);

    protected abstract void ExecutePartnerAction(BattleManager battleManager, IBattleActionCommand battleActionCommand, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles);


    public abstract bool CanBeDoneOnTile(List<CombatTileController> combatTileControllers);

}
}