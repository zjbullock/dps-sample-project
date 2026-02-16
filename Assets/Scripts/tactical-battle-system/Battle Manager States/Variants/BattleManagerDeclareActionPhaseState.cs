using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Playables;
using DPS.Common;

namespace DPS.TacticalCombat{
[System.Serializable]
public class BattleControllerDeclareActionPhaseState : BattleControllerBaseState
{
        public BattleControllerDeclareActionPhaseState(string stateName) : base(stateName)
        {
        }

        // [SerializeField]
        // private SignalAsset battleCamToSideCamSignal;

        public override void EnterState(BattleManager battleController)
    {

        this.CheckEnemyTiles(battleController);
        Debug.Log("***DECLARE_ACTION PHASE***");
        battleController.ToggleSideCameraActiveOnEntity();

        if(battleController.CurrentlyActingMember.GetBattleMember().BattleCommand != null) {
            battleController.SetDialogueBoxText(battleController.CurrentlyActingMember.GetBattleMember().GetCommandMessage());
        }

        // if(!battleController.IsDialogueBoxActive()) {
        //     battleController.SetDialogueBoxActive(true);
        // }
        // battleController.cameraBlends[battleCamToSideCamSignal].Play();
        base.EnterState(battleController);
        battleController.ProgressState();
        return;
    }

    public override void UpdateState(BattleManager battleController)
    {
       // if (battleController.actionPhaseControllerState == ActionPhaseState.GetNextAction) {
            //     currentlyActingMember = battleController.turnOrder[actionCursor];
            //     battleController.actionPhaseControllerState = ActionPhaseState.DeclareAction;
            // } 
        // this.ProgressState(battleController);
    }

    private void CheckEnemyTiles(BattleManager battleController) {
        if (!this.ShouldCheckEnemyTiles(battleController)) {
            return;
        }
        SkillCommand enemySkillCommand = battleController.CurrentlyActingMember.GetBattleMember().BattleCommand as SkillCommand;
        battleController.BattleFieldController.ActionGrid = enemySkillCommand.GetActionTilesByAreaOfEffect(battleController.DestinationTile != null ? battleController.DestinationTile : battleController.CurrentlyActingMember.GetCombatTileController(),  battleController.CurrentlyActingMember);
        if(enemySkillCommand.ShouldDistanceCheck()) {
            battleController.BattleFieldController.ConfirmActionGrid = new GenericDictionary<Vector3, CombatTileController>();
            foreach (CombatTileController tile in enemySkillCommand!.Tiles) {
                tile.ActivateConfirmActionTile();
                battleController.BattleFieldController.AddConfirmActionTile(tile);
            }
        }
        // foreach (CombatTileController tile in enemySkillCommand!.Tiles) {
        //     tile.AddProjectedTileBattleCommand(battleController.currentlyActingMember);
        // }
        battleController.DestinationTile = null;
    }

    private bool ShouldCheckEnemyTiles(BattleManager battleController) {
           return battleController.CurrentlyActingMember.GetBattleMember()!.battleMemberType == BattleMemberType.Enemy &&
                    !battleController.CurrentlyActingMember.GetBattleMember().LoseTurn();
    }

    // private void AddSideCameraUpdateStateEvent(BattleController battleController) {
    //     if (battleController.IsCurrentCamera(battleController.SideCamera)) {
    //         return;
    //     }
    //     battleController.AddUpdateStateEvent(new BattleFocusCameraEvent(battleController.SideCamera));
    // }

    public override void ProgressState(BattleManager battleController)
    {
        Debug.Log("-------Let's go!!!---------");
        battleController.StateManager.SwitchState(battleController.StateManager.ResultActionPhaseState, battleController);
        base.ProgressState(battleController);
    }
}
}