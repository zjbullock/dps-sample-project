using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[System.Serializable]
public class BattleControllerTurnStartState : BattleControllerBaseState
{

    public override void EnterState(BattleManager battleController)
    {
        battleController.SetTurnCount(battleController.TurnCount + 1);
        if (battleController.TurnCount > 1) {
            battleController.AddPartyPoints(5);
        }
        base.EnterState(battleController);

        return;
    }

    public override void UpdateState(BattleManager battleController)
    {
        this.ProgressState(battleController);
    }

    public override void ProgressState(BattleManager battleController)
    {
        Debug.Log("Progressing state!");
        battleController.StateManager.SwitchState(battleController.StateManager.DetermineCommandState, battleController);
        base.ProgressState(battleController);
    }
}
}