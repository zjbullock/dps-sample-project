using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
public class BattleControllerActionPhaseEndState : BattleControllerBaseState
{
    public override void EnterState(BattleManager battleController)
    {
        base.EnterState(battleController);
        return;
    }

    public override void UpdateState(BattleManager battleController)
    {
        battleController.StateManager.SwitchState(battleController.StateManager.EndPhaseState, battleController);
    }
}
}