using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat{
[CreateAssetMenu(fileName = "Target_Behavior_Ranged_Cross_Single_Any", menuName = "ScriptableObjects/Active Skill/Behavior/Ranged/Cross/Single Any Member Target")]

public class RangedCrossSingleAnyTargetBehaviorSO : RangedCrossActionTileBehaviorSO
{
    // public override bool CanBeDoneToTilePlayerActiveSkill(BattleMenuController battleMenuController)
    // {
    //     return (battleMenuController.actionGridTargetCells.ContainsKey(battleMenuController.targetedTile) && battleMenuController.queuedSkill.CanBeDoneOnTile(battleMenuController.actionGridTargetCells[battleMenuController.targetedTile]));
    // }

    // protected override void SubmitPlayerActiveSkillAction(BattleMenuController battleMenuController)
    // {
    //     battleMenuController.audioController.PlayAudio(SoundEffectEnums.Confirm);
    //     List<PartySlot> targets = new List<PartySlot>();
    //     List<CombatTileController> tiles = new List<CombatTileController>();
        
    //     foreach(KeyValuePair<Vector3, CombatTileController> keyValuePair in battleMenuController.confirmActionGridTargetCells) {
    //         tiles.Add(keyValuePair.Value);
    //     }

    //     IBattleActionCommand activeSkill = battleMenuController.queuedSkill;
    //     battleMenuController.currentPartyMember.GetBattleMember().BattleCommand =
    //     new SkillCommand(
    //         activeSkill,
    //         battleMenuController.currentPartyMember,
    //         BattleProcessingStatic.PartySlotIsPlayerPartySlot,
    //         battleMenuController.combatTileCursor.combatTileController,
    //         tiles,
    //         new List<PartySlot>() { battleMenuController.combatTileCursor.GetPartySlot() }
    //     );
    //     base.SubmitPlayerActiveSkillAction(battleMenuController);
    // }
    #nullable enable

    public override bool CanBeDoneToTargetedTile(CombatTileController targetedTile, PartySlot caster,  PartySlot? targetedEntity, Func<PartySlot, bool> isSameEntityCheck)
    {
        return true;
    }
}
}