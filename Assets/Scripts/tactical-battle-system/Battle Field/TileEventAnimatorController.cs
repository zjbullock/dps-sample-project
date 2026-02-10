using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
public class TileEventAnimatorController : MonoBehaviour
{

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private bool isAnimating = false;

    public bool IsAnimating { get => this.isAnimating; set => this.isAnimating = value; }


    public void OnTurnEnd(BattleManager battleController) {
        this.AnimateOnTurnEnd(battleController);
    }

    private void AnimateOnTurnEnd(BattleManager battleController) {
        if (this.animator == null) {
            return;
        }
        this.animator.SetTrigger("progressState");
        this.isAnimating = true;
        battleController.BattleEventController.AddBattleEvent(new TileEvent(this));
    }

    private void StopAnimating() {
        this.isAnimating = false;
    }
}
}