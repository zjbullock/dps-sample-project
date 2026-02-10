using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DPS.Common;

namespace DPS.TacticalCombat {
    [Serializable]
public abstract class SpriteBattleObject : BattleObject {
    public SpriteAnimationController spriteAnimatorController;

    protected override void BattleObjectAwake()
    {
        base.BattleObjectAwake();
        if (this.spriteAnimatorController == null) {
            this.spriteAnimatorController = GetComponentInChildren<SpriteAnimationController>();
        }
    }

    private void Start() {
        if (this.spriteAnimatorController != null && this.spriteAnimatorController.ParentGO == null) {
            this.spriteAnimatorController.ParentGO = this.gameObject;
        }  
    }

    public override bool HasAnimator()
    {
        return this.spriteAnimatorController != null;
    }

    
    #nullable enable
    public override void TriggerAnimation(AnimationTrigger animationTrigger, string? specialAnimation = null) {
        if (this.spriteAnimatorController == null) {
            return;
        }
        if (specialAnimation != null) {
            this.spriteAnimatorController.TriggerSpecialAnimation(specialAnimation);
            return;
        }
        this.spriteAnimatorController.TriggerAnimation(animationTrigger);
    }
    #nullable disable

     public override void SetAnimationState(string animationState) {
        if (this.spriteAnimatorController == null) {
            return;
        }
        this.spriteAnimatorController.SetAnimation(animationState);

    }

    public override void SetAnimationState(AnimationStates animationState) {
        if (this.spriteAnimatorController == null) {
            return;
        }
        this.spriteAnimatorController.SetAnimation(animationState);
    }

    public override void TargetBattleObject (bool targetObject) {
        if (this.spriteAnimatorController == null) {
            return;
        }

        this.spriteAnimatorController.ToggleBackLight(targetObject);
    }



    public override void SetMovement(bool isRunning = false, bool isWalking = false)
    {
        if (this.spriteAnimatorController == null) {
            return;
        }

        this.spriteAnimatorController.SetMovement(isRunning, isWalking);
    }

    public override void ToggleEffect(AnimationEffect animationEffect, bool toggle)
    {
        if (this.spriteAnimatorController == null) {
            return;
        }

        this.spriteAnimatorController.ToggleEffect(animationEffect, toggle);
    }

    public override void EndMovement()
    {
        if (this.spriteAnimatorController == null) {
            return;
        }

        this.spriteAnimatorController.EndMovement();
    }
}

}
