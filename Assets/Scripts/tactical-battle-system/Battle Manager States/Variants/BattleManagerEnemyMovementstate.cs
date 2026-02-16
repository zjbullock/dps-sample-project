using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
public class BattleControllerEnemyMovementstate : BattleControllerBaseState
{
        public BattleControllerEnemyMovementstate(string stateName) : base(stateName)
        {
        }

        public override void EnterState(BattleManager battleController)
    {
        battleController.CurrentlyActingMember.GetCombatTileController()?.RemoveOccupantAndProcessEvent(battleController);
                base.EnterState(battleController);

        return;
    }

    public override void UpdateState(BattleManager battleController)
    {
        if (battleController.CurrentlyActingMember.canMove) {
            battleController.CurrentlyActingMember.canMove = false;
        }

    }

}
}