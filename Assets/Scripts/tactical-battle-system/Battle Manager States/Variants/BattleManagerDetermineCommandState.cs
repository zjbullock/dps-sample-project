using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace DPS.TacticalCombat {
[System.Serializable]
public class BattleControllerDetermineCommandState : BattleControllerBaseState
{


    private void GetNextPartyMember(BattleManager battleController) {
        // if (battleController.currentTurnOrder.Count == 0) {
        //     battleController.DetermineIfBattleOver();
        //     return;
        // }

        battleController.GetNextPartyMember();

        return;
    }

    public override void ProgressState(BattleManager battleController)
    {
        if (battleController.currentTurnOrder.Count == 0) {
            return;
        }

        battleController.CurrentlyActingMember = battleController.currentTurnOrder[0];
        if (battleController.CurrentlyActingMember == null) {
            return;
        }
        
        battleController.ToggleFreeCameraActiveOnEntity();
        battleController.PrioritizeCamera(battleController.BattleCameraManager.FreeLookCamera);

        battleController.CurrentlyActingMember.GetCombatTileController()?.PerformTurnStartInteraction(battleController);
        if (battleController.CurrentlyActingMember == null || (battleController.CurrentlyActingMember != null && battleController.CurrentlyActingMember.BattleEntity!.IsDead())) {
            Debug.Log("Currently acting member is now null or something");
            Debug.Log(battleController.CurrentlyActingMember);
            this.GetNextPartyMember(battleController);
            return;
        }
        
        if(battleController.IsEncounterOver()){
            return;
        }
        
        // battleController.CurrentlyActingMember.turnPortraitController.SelectPortrait(true);
        battleController.CurrentlyActingMember.BeginPhase(battleController);
        // battleController.currentlyActingMember.SetBattleObjectVector3(combatTileController);
        if(battleController.CurrentlyActingMember.BattleEntity!.IsDowned()) {
            switch(battleController.CurrentlyActingMember.BattleEntity!.GetPoiseState().PoiseState) {
                case PoiseState.Broken:
                    battleController.SetDialogueBoxText(battleController.CurrentlyActingMember.BattleEntity!.GetName() + " is poise broken!");
                    break;
                case PoiseState.Recovering:
                    battleController.SetDialogueBoxText(battleController.CurrentlyActingMember.BattleEntity!.GetName() + " has recovered from being poise broken!");
                    break;
            }
            battleController.StateManager.SwitchState(battleController.StateManager.DeclareActionPhaseState, battleController);
            return;
        } else if(battleController.CurrentlyActingMember.BattleEntity!.GetInflictedAilment() != null && battleController.CurrentlyActingMember.BattleEntity!.GetInflictedAilment().LoseTurn()) {
            battleController.SetDialogueBoxText(battleController.CurrentlyActingMember.BattleEntity!.GetName() + " " + battleController.CurrentlyActingMember.BattleEntity!.GetInflictedAilment().statusAilment.flavorText);
            battleController.StateManager.SwitchState(battleController.StateManager.DeclareActionPhaseState, battleController);
            return;
        } else if (battleController.CurrentlyActingMember.GetBattleMember().BattleCommand != null) {
            battleController.SetDialogueBoxText(battleController.CurrentlyActingMember.GetBattleMember().GetCommandMessage());
            battleController.StateManager.SwitchState(battleController.StateManager.DeclareActionPhaseState, battleController);
            return;
        }

        if (BattleProcessingStatic.PartySlotIsPlayerPartySlot(battleController.CurrentlyActingMember)) {
            // battleController.battlePrimaryMenuController.CurrentPartyMember = battleController.currentlyActingMember as PlayerPartySlot;
            // if (battleController.CurrentlyActingMember.enmityList.Count > 0 && battleController.CurrentlyActingMember.enmityList[0].isAggro && !battleController.IsDialogueBoxActive()) {
            //     battleController.SetDialogueBoxText(battleController.CurrentlyActingMember.BattleEntity!.GetName() + " has been aggroed by " + battleController.CurrentlyActingMember.enmityList[0].name + "!");
            //     battleController.SetDialogueBoxActive(true);
            // }   
            battleController.StateManager.SwitchState(battleController.StateManager.PlayerCommandstate, battleController);
        } else if (BattleProcessingStatic.PartySlotIsEnemyPartySlot(battleController.CurrentlyActingMember)) {
                battleController.StateManager.SwitchState(battleController.StateManager.EnemyCommandState, battleController);

        }
        base.ProgressState(battleController);
    }

    public override void UpdateState(BattleManager battleController)
    {
        this.ProgressState(battleController);

        //Next, proceed to ACTION_PHASE when the command has been determined.
    }
}
}