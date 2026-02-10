using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Ranged All Any Target Behavior", menuName = "ScriptableObjects/Active Skill/Behavior/Ranged/All Any Target")]
public class RangedAllAnyTargetBehaviorSO : RangedActionTileBehaviorSO
{
    // protected override void SubmitPlayerActiveSkillAction(BattleMenuController battleMenuController)
    // {
    //     battleMenuController.audioController.PlayAudio(SoundEffectEnums.Confirm);
    //     List<PartySlot> targets = new List<PartySlot>();
    //     List<CombatTileController> tiles = new List<CombatTileController>();

    //     foreach(KeyValuePair<Vector3, CombatTileController> keyValuePair in battleMenuController.confirmActionGridTargetCells) {
    //         tiles.Add(keyValuePair.Value);
    //     }
    //     Debug.LogError("Confirmed Tiles!: " + tiles.Count);
    //     ActiveSkillSO activeSkill = battleMenuController.queuedSkill;
    //     battleMenuController.currentPartyMember.GetBattleMember().BattleCommand =
    //     new SkillCommand(
    //         activeSkill,
    //         battleMenuController.currentPartyMember,
    //         BattleProcessingStatic.PartySlotIsPlayerPartySlot,
    //         battleMenuController.combatTileCursor.combatTileController,
    //         tiles,
    //         new()
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
    //     return battleMenuController.actionGridTargetCells.ContainsKey(battleMenuController.targetedTile) && battleMenuController.queuedSkill.CanBeDoneOnTile(battleMenuController.actionGridTargetCells[battleMenuController.targetedTile]);
    // }

    // public override bool CanBeDoneToTilePlayerItem(BattleMenuController battleMenuController)
    // {
    //     return battleMenuController.actionGridTargetCells.ContainsKey(battleMenuController.targetedTile);
    // }
        #nullable enable

    public override bool CanBeDoneToTargetedTile(CombatTileController targetedTile, PartySlot caster,  PartySlot? targetedEntity, Func<PartySlot, bool> isSameEntityCheck)
    {
        return true;
    }
}
}