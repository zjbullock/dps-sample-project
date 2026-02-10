using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DPS.TacticalCombat {
public class BattleControllerResultActionPhaseState : BattleControllerBaseState
{

    public override void EnterState(BattleManager battleController)
    {
        if(battleController.CurrentlyActingMember.GetBattleMember()!.LoseTurn()) {
            // this.OnActionCompletion(battleController);
            return;
        }
        // foreach(KeyValuePair<Vector3, CombatTileController> keyValuePair in battleController.Grid) {
        //     keyValuePair.Value.DisableActionTile();
        //     keyValuePair.Value.DisableActionConfirmTile();
        // }
        // if (battleController.currentlyActingMember.GetBattleMember()!.battleMemberType == BattleMemberType.Enemy) {
        //     battleController.enemyActionGrid.Clear();
        //     battleController.confirmEnemyActionGrid.Clear();
        // }

        // #nullable enable
        
        this.ExecuteCommand(battleController);


        base.EnterState(battleController);

        return;
    }

    public override void UpdateState(BattleManager battleManager)
    {

        // switch (battleResultState) {
        //     case BattleTurnResultStateEnums.None:
        //         this.processNormalActionEndResult(actionPhaseState, battleController, combatTileController);
        //         break;
        //     case BattleTurnResultStateEnums.Swap:
        //         this.processSwapActionEndResult(actionPhaseState, battleController, combatTileController);
        //         break;
        // }

        // switch (battleResultState) {
        //     case BattleTurnResultStateEnums.None:
        //         this.processNormalActionEndResult(actionPhaseState, battleController, combatTileController);
        //         break;
        //     case BattleTurnResultStateEnums.Swap:
        //         this.processSwapActionEndResult(actionPhaseState, battleController, combatTileController);
        //         break;
        // }
        // if (battleController.IsWaitList()) {
        //     battleController.ProcessWaitList();
        //     return;
        // }
        this.OnActionCompletion(battleManager);
        if (battleManager.IsEncounterOver())
        {
            return;
        }

        battleManager.GetNextPartyMember();
        if (battleManager.currentTurnOrder.Count == 0)
        {
            battleManager.StateManager.SwitchState(battleManager.StateManager.EndPhaseState, battleManager);
        }
        else
        {
            battleManager.StateManager.SwitchState(battleManager.StateManager.DetermineCommandState, battleManager);
        }
    }

    private void ExecuteCommand(BattleManager battleManager) {

        PartySlot currentActionMember = battleManager.CurrentlyActingMember;
        BattleFirstActionEvent battleFirstActionEvent = new(battleManager);
        battleManager.BattleEventController.AddBattleEvent(battleFirstActionEvent);
        battleManager.CurrentlyActingMember.ExecuteBattleCommand(battleManager, () => { battleFirstActionEvent.CompleteFirstEvent(); });

        // currentActionMember.HandleAndProcessUserAnimations(battleController, callBack: () =>
        // {
        //     // this.OnActionCompletion( battleController);
        // });

        return;
    }

    private void OnActionCompletion(BattleManager battleManager) {
        if(battleManager.CurrentlyActingMember.BattleEntity!.GetInflictedAilment() != null 
            && battleManager.CurrentlyActingMember.BattleEntity!.GetInflictedAilment().CanDealDamage()) {
                battleManager.CurrentlyActingMember.BattleEntity!.GetInflictedAilment().DealDamage(battleManager, battleManager.CurrentlyActingMember);
        }
        
        if (battleManager.CurrentlyActingMember.BattleEntity!.IsDowned()) {
            battleManager.CurrentlyActingMember.ProgressPoiseBrokenState();
        }


        foreach (PartySlot partySlot in battleManager.PlayerPartyController.PartySlots)
        {
            partySlot.RemoveDeadEnmityTargets();
        }
        
        foreach(EnemyPartySlot enemy in battleManager.EnemyPartyController.PartySlots) {            
            enemy.RemoveDeadEnmityTargets();
            if(enemy.enmityList.Count == 0) {
                battleManager.GetNewEnemyEnmityTarget(enemy);
            }
        }

        return;
    }

}
}
