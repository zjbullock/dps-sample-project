using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
public class PartySwapCommand : SwapCommand
{
    public PartySwapCommand(        
        PartySlot user, 
        CombatTileController targetedTile, 
        List<CombatTileController> tiles, 
        List<PartySlot> targetedSlots ):  base(user, targetedTile, tiles, targetedSlots) {}

    public override bool CanExecuteCommand()
    {
        return base.CanExecuteCommand();
    }
    #nullable enable
    public override void ExecuteCommand(BattleManager battleController, System.Action? callBack = null, IBattleCommand? commandOverride = null)
    {

        if (!BattleProcessingStatic.PartySlotIsPlayerPartySlot(caster))
        {
            Debug.LogError("Attempted a swap command, but failed!");
            return;
        }
        
        PlayerPartySlot? playerPartySlot = caster! as PlayerPartySlot;
        if (playerPartySlot == null)
        {
            Debug.LogError("Somehow not player party slot despite original check");
            return;    
        }
        
        // this.executeSwapOutSkill(playerPartySlot,  battleController, callBack);
        this.DoSwap(playerPartySlot, battleController, callBack);

        return;
    }

    // private void executeSwapOutSkill(PlayerPartySlot playerPartySlot, BattleManager battleController, System.Action? callBack = null) {
    //     SkillCommand? swapOutCommand = playerPartySlot!.useSwapOutSkill(battleController, playerPartySlot.GetCombatTileController()!);
    //     Debug.Log("Executing Swap Out Skill");
    //     if(swapOutCommand == null) {            //Check to see if a command was set by the swap out skill
    //         Debug.Log("HItting no swap out skill path");
    //         this.DoSwap(playerPartySlot, battleController, callBack);
    //         return;
    //     }
        
    //     // Debug.Log("Attempting swap out command");
    //     // playerPartySlot.ExecuteBattleCommand(battleController, () =>
    //     //     {
    //     //         callBack?.Invoke();
    //     //     }
    //     // );

    //     swapOutCommand.ExecuteCommand(battleController, () =>
    //     {
    //         this.DoSwap(playerPartySlot, battleController, callBack);
    //     });

    //     // playerPartySlot.ExecuteSwapBattleCommand(battleController, playerPartySlot.GetCombatTileController()!, () =>
    //     // {
    //     // }, commandOverride: swapOutCommand);      
    //     return;
    // }

    private void DoSwap(PlayerPartySlot playerPartySlot, BattleManager battleController,  System.Action? callBack = null) {
        playerPartySlot.SwapPartyMember(battleController);
        playerPartySlot.activateBeginCombatAbilities();
        battleController.BattleEventController.AddBattleEvent(new WaitEvent(0.1f, () => {
            SwapSkillSO? swapSkillSO = this.DetermineSwapSkill(battleController, callBack);
            this.ExecuteSwapSkill(battleController, swapSkillSO, callBack);
        }));
    }

    private SwapSkillSO? DetermineSwapSkill(BattleManager battleManager, System.Action? callBack = null) {
        
        CharacterInfo? characterInfo = caster.BattleEntity as CharacterInfo;
        if (characterInfo == null)
        {
            Debug.LogError("Swap Performer is not of type Character Info");
            return null;
        }

        SwapSkill? swapSkill = characterInfo.GetSkillInfo().SwapSkill;
        if(swapSkill == null)
        {
            Debug.LogError("Unit does not have the default Pass Turn Swap in Ability equipped.");
            return null;
        }

        SwapSkillSO? specialSwapSkillSO = swapSkill.SpecialSwapSkill;

        if (specialSwapSkillSO == null || !swapSkill.CanActivateSwapSkill()) {
            return swapSkill.DefaultSwapSkill;
        }

        swapSkill.SetSwapSkillCoolDown();

        return specialSwapSkillSO;


    }

    private void ExecuteSwapSkill(BattleManager battleManager,  SwapSkillSO? swapSkillSO, System.Action? callBack = null)
    {

        if (swapSkillSO == null)
        {
            Debug.LogError("No swap skill provided!");
            return;
        }
        
        caster.GetBattleMember().BattleCommand = new SkillCommand(
            swapSkillSO, 
            caster, 
            BattleProcessingStatic.PartySlotIsPlayerPartySlot, 
            targetedTile, 
            tiles, 
            targetedSlots
        );

        // battleManager.StateManager.SwitchState(battleManager.declareActionPhaseState);
        caster.ExecuteBattleCommand(battleManager, () => callBack?.Invoke());
        return;
    }

    public override void ReCalculateCommandTiles(PartySlot partySlot)
    {
        return;
    }
#nullable disable
}
}