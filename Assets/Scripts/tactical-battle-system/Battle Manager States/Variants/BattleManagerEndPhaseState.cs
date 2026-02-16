using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[System.Serializable]
public class BattleControllerEndPhaseState : BattleControllerBaseState
{
        public BattleControllerEndPhaseState(string stateName) : base(stateName)
        {
        }

        public override void EnterState(BattleManager battleController)
    {
        // if (battleController.IsDialogueBoxActive()) {
        //     battleController.ClearDialogueBoxText();
        //     battleController.SetDialogueBoxActive(false);
        // }
        

        // if(battleController.playerPartyController.inventory.itemsToRemove.Count > 0) {
        //     battleController.playerPartyController.inventory.PurgeQueuedItemList();
        // }


        Debug.Log("Currently in endphase!");
        // this.SortPartyMembersByTurnOrder();
        battleController.PrioritizeCamera(battleController.BattleCameraManager.FocusPointCamera);
        base.EnterState(battleController);
        this.ProcessOnTurnEndEvents(battleController);
        return;
    }

    public override void UpdateState(BattleManager battleController)
    {
        this.ProgressState(battleController);
    }

    private void ProcessOnTurnEndEvents(BattleManager battleController) {
        battleController.BattleFieldController.OnTurnEnd(battleController);
    }

    public override void ProgressState(BattleManager battleController)
    {
        battleController.BattleFieldController.GetGrid();
        battleController.RegenerateTurnOrder(battleController.futureTurnOrder);
        battleController.currentTurnOrder.AddRange(battleController.futureTurnOrder);
        battleController.futureTurnOrder = new List<PartySlot>();
        battleController.SortBattleMembersByInitiative();


        if (battleController.currentTurnOrder.Count > 0)
        {
            battleController.StateManager.SwitchState(battleController.StateManager.DetermineCommandState, battleController);
        }
        else
        {
            battleController.StateManager.SwitchState(battleController.StateManager.TurnStartState, battleController);
        }

        base.ProgressState(battleController);
    }
}
}