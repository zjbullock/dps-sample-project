using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Circular Single Enemy Member Target Behavior", menuName = "ScriptableObjects/Active Skill/Behavior/LOS/Circular/Single Enemy Member Target")]

public class CircularSingleEnemyMemberTargetBehaviorSO : CircularActionTileBehaviorSO
{
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
        
    //     if (battleMenuController.actionGridTargetCells.TryGetValue(battleMenuController.targetedTile, out CombatTileController targetedTile)) {
    //         return (battleMenuController.actionGridTargetCells.ContainsKey(battleMenuController.targetedTile) && 
    //                 battleMenuController.actionGridTargetCells[battleMenuController.targetedTile].GetPartyOccupants().Count > 0 && 
    //                 !BattleProcessingStatic.PartySlotIsPlayerPartySlot(battleMenuController.combatTileCursor.GetPartySlot()) &&
    //                 BattleProcessingStatic.TileIsTargetableActiveSkill(battleMenuController.queuedSkill, battleMenuController.currentPartyMember, battleMenuController.combatTileCursor.GetPartySlot(), targetedTile, battleMenuController: battleMenuController) &&
    //                 battleMenuController.queuedSkill.CanBeDoneOnTile(targetedTile));
    //     }
        

    //     return false;

    // }

    // public override bool CanBeDoneToTilePlayerItem(BattleMenuController battleMenuController)
    // {
    //     battleMenuController.actionGridTargetCells.TryGetValue(battleMenuController.targetedTile, out CombatTileController targetedTile);

    //     return (battleMenuController.actionGridTargetCells.ContainsKey(battleMenuController.targetedTile) && 
    //             battleMenuController.actionGridTargetCells[battleMenuController.targetedTile].GetPartyOccupants().Count > 0 && 
    //             !BattleProcessingStatic.PartySlotIsPlayerPartySlot(battleMenuController.combatTileCursor.GetPartySlot()) &&
    //             BattleProcessingStatic.TileIsTargetableItem(battleMenuController.queuedItem, battleMenuController.currentPartyMember, battleMenuController.combatTileCursor.GetPartySlot(), targetedTile, battleMenuController: battleMenuController) &&
    //             battleMenuController.queuedItem.CanBeDoneOnTile(targetedTile));
    // }
    #nullable enable

    public override bool CanBeDoneToTargetedTile(CombatTileController targetedTile, PartySlot caster,  PartySlot? targetedEntity, Func<PartySlot, bool> isSameEntityCheck)
    {
        if (targetedEntity == null || isSameEntityCheck(targetedEntity))
        {
            return false;
        }

        return true;
    }


    public override void NavigateItemTargetBehaviorForEnemy()
    {
        base.NavigateItemTargetBehaviorForEnemy();
    }


}
}
