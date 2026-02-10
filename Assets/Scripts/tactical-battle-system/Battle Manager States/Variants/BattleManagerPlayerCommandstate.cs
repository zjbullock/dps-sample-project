using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
public class BattleControllerPlayerCommandstate : BattleControllerBaseState
{
    public override void EnterState(BattleManager battleController)
    {
        Debug.Log("Entering Player Command State");
        // battleController.BattleMenuController.ActivatePlayerMenu();
        base.EnterState(battleController);

        return;
    }

    public override void UpdateState(BattleManager battleController)
    {
        return;
    }
}
}