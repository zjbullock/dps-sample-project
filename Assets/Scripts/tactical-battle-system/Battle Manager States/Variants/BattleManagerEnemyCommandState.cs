using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
public class BattleControllerEnemyCommandState : BattleControllerBaseState
{
        public BattleControllerEnemyCommandState(string stateName) : base(stateName)
        {
        }

        public override void EnterState(BattleManager battleController)
    {
        battleController.ToggleFreeCameraActiveOnEntity();
        // battleController.SetCameraTarget(battleController.currentlyActingMember.BattleEntityGO.transform);
        
        if (!BattleProcessingStatic.PartySlotIsEnemyPartySlot(battleController.CurrentlyActingMember)) {
            throw new System.Exception("Calling enemy command event without enemy party slot");
        }

        Debug.Log("Getting enemy command event");

        EnemyPartySlot enemyPartySlot = battleController.CurrentlyActingMember as EnemyPartySlot;

        battleController.BattleEventController.AddBattleEvent(enemyPartySlot.GetEnemyCommandEvent(battleController));
        base.EnterState(battleController);
        return;
    }

    public override void UpdateState(BattleManager battleController)
    {


    }
}
}