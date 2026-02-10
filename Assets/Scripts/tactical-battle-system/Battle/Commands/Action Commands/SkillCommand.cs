using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat {
[System.Serializable]
public class SkillCommand : BattleActionCommand
{
    [Header("SKILL COMMAND")]
    #nullable enable

    [SerializeField]
    
    protected int castTime;

    [SerializeField]
    protected int maxCastTime = 0;
    public SkillCommand(
                    IBattleActionCommand action,
                    PartySlot caster,
                    Func<PartySlot, bool> isSameTypeEntity,
                    CombatTileController targetedTile,
                    List<CombatTileController> tiles,
                    List<PartySlot> targetedSlots
                   ): base(action, caster, isSameTypeEntity, tiles, targetedSlots, targetedTile) {
        try
        {
                        if(targetedTile == null)
            {
                Debug.LogError("Targeted Tile is null in SkillCommand");
            }
        }
        catch(Exception e)
        {
            Debug.LogError("Error getting action name in SkillCommand: " + e.Message);
            Debug.LogError(e.StackTrace.ToString());
        }

        this.castTime = action.GetCastTime(caster);
        this.maxCastTime = this.castTime;
    }
    public override string GetMessage(BattleMember battleMember)
    {
        string commandMessage = "";
        if (this.CanExecuteCommand())
        {
            commandMessage = battleMember.GetBattleEntity!.GetName() + " uses " + this.CommandName + "!";
        }
        else if (this.castTime == this.maxCastTime)
        {
            commandMessage = battleMember.GetBattleEntity!.GetName() + " begins channeling " + this.CommandName + "!";
        }
        else if (this.castTime < this.maxCastTime)
        {
            commandMessage = battleMember.GetBattleEntity!.GetName() + " continues to channel " + this.CommandName + "...";
        }
        return commandMessage;
    }

    public override void ExecuteCommand(BattleManager battleController, Action? callBack = null, IBattleCommand? commandOverride = null)
    {
        if(!this.CanExecuteCommand()) {
            if (this.castTime == this.maxCastTime ){
                caster.BattleEntityGO.ToggleEffect(AnimationEffect.Casting, true);
            }
            this.ReduceCastTime();
            callBack?.Invoke();
            return;
        }

        caster.BattleEntityGO?.ToggleEffect(AnimationEffect.Casting, false);
        base.ExecuteCommand(battleController, callBack, commandOverride);
    }

    public override bool CanExecuteCommand()
    {
        return this.castTime <= 0;
    }

    protected void ReduceCastTime() {
        this.castTime--;
    }



    

#nullable disable
}
}