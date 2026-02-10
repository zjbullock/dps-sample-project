using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Circular Single Party Member Target Behavior", menuName = "ScriptableObjects/Active Skill/Behavior/LOS/Circular/Single Party Member Target")]

public class CircularSinglePartyMemberTargetBehaviorSO : CircularActionTileBehaviorSO
{
        #nullable enable
    public override bool CanBeDoneToTargetedTile(CombatTileController targetedTile, PartySlot caster,  PartySlot? targetedEntity, Func<PartySlot, bool> isSameEntityCheck)
    {
        if (targetedEntity == null || !isSameEntityCheck(targetedEntity))
        {
            return false;
        }

        return true;
    }

    public override void NavigateActiveSkillTargetingBehaviorForEnemy()
    {
        base.NavigateActiveSkillTargetingBehaviorForEnemy();
    }

    // protected override void SubmitPlayerActiveSkillAction(BattleMenuController battleMenuController)
    // {
    //     battleMenuController.audioController.PlayAudio(SoundEffectEnums.Confirm);
    //     ActiveSkillSO activeSkill = battleMenuController.queuedSkill;
    //     List<CombatTileController> tiles = new List<CombatTileController>();
    //     tiles.Add(battleMenuController.actionGridTargetCells[battleMenuController.targetedTile]);
    //     List<PartySlot> targetedSlots = new();
    //     if (battleMenuController.combatTileCursor.GetPartySlot() != null)
    //     {
    //         targetedSlots.Add(battleMenuController.combatTileCursor.GetPartySlot());
    //     }
    //     battleMenuController.currentPartyMember.GetBattleMember().BattleCommand =
    //     new SkillCommand(
    //         activeSkill,
    //         battleMenuController.currentPartyMember,
    //         BattleProcessingStatic.PartySlotIsPlayerPartySlot,
    //         battleMenuController.combatTileCursor.combatTileController,
    //         tiles,
    //         targetedSlots
    //     );
    //     base.SubmitPlayerActiveSkillAction(battleMenuController);
    // }

    // protected override void SubmitPlayerItemAction(BattleMenuController battleMenuController)
    // {
    //     battleMenuController.audioController.PlayAudio(SoundEffectEnums.Confirm);             
    //     ConsumableInventoryItemSO itemToUse = (ConsumableInventoryItemSO) battleMenuController.battlePartyMembers.inventory.RemoveInventoryItem(battleMenuController.queuedItem, 1);

    //     List<CombatTileController> tiles = new List<CombatTileController>();
    //     tiles.Add(battleMenuController.actionGridTargetCells[battleMenuController.targetedTile]);

    //     battleMenuController.currentPartyMember.GetBattleMember().BattleCommand =
    //     new ItemCommand(
    //         itemToUse,
    //         battleMenuController.currentPartyMember,
    //         BattleProcessingStatic.PartySlotIsPlayerPartySlot,
    //         battleMenuController.combatTileCursor.combatTileController,
    //         tiles,
    //         new List<PartySlot>() { battleMenuController.combatTileCursor.GetPartySlot() }
    //     );
    //     base.SubmitPlayerItemAction(battleMenuController);
    // }

    // public override bool CanBeDoneToTilePlayerActiveSkill(BattleMenuController battleMenuController)
    // {
    //     return (battleMenuController.actionGridTargetCells.ContainsKey(battleMenuController.targetedTile) && 
    //             battleMenuController.actionGridTargetCells[battleMenuController.targetedTile].HasPartyOccupant() && 
    //             BattleProcessingStatic.PartySlotIsPlayerPartySlot(battleMenuController.actionGridTargetCells[battleMenuController.targetedTile].GetPartyOccupant()) &&
    //             battleMenuController.queuedSkill.CanBeDoneOnTile(battleMenuController.actionGridTargetCells[battleMenuController.targetedTile]));
    // }

    // public override bool CanBeDoneToTilePlayerItem(BattleMenuController battleMenuController)
    // {
    //     return (battleMenuController.actionGridTargetCells.ContainsKey(battleMenuController.targetedTile) &&  
    //             battleMenuController.actionGridTargetCells[battleMenuController.targetedTile].HasPartyOccupant() && 
    //             BattleProcessingStatic.PartySlotIsPlayerPartySlot(battleMenuController.actionGridTargetCells[battleMenuController.targetedTile].GetPartyOccupant()));
    // }
}
}