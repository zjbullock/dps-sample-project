using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
public class BattleControllerLoseState : BattleControllerBaseState
{
    private enum LoseState {
        Result,
        End
    }

    // private LoseState loseState;

    public BattleControllerLoseState(string name): base(name) {
        // this.loseState = LoseState.Result;
    }

    public override void EnterState(BattleManager battleController)
    {
        base.EnterState(battleController);

        return;
    }

    public override void UpdateState(BattleManager battleController)
    {
        // switch (this.loseState) {
            // case LoseState.Result: 
            //     if(!battleController.IsDialogueBoxActive()) {
            //         battleController.SetDialogueBoxText("There are no remaining party members left alive on the field.  Game Over.");
            //         battleController.SetDialogueBoxActive(true);

            //     }
            //     if (battleController.playerInputActions.BattleUI.Submit.WasPressedThisFrame()) {
            //         this.loseState = LoseState.End;
            //         return;
            //     }
            //     break;
            // case LoseState.End:
            //     battleController.SceneTransitionService?.FadeToTitleScene();                
            //     break;
        // }
        Debug.Log("PLAYER LOSES");
    }
}
}