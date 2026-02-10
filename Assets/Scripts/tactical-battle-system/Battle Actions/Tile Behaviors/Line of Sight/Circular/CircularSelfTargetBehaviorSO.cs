using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Circular Self Target Behavior", menuName = "ScriptableObjects/Active Skill/Behavior/LOS/Circular/Self Target")]

public class CircularSelfTargetBehaviorSO : CircularActionTileBehaviorSO
{
    #nullable enable

    public override bool CanBeDoneToTargetedTile(CombatTileController targetedTile, PartySlot caster,  PartySlot? targetedEntity, Func<PartySlot, bool> isSameEntityCheck)
    {
        return true;
    }

    public override void NavigateActiveSkillTargetingBehaviorForEnemy()
    {
        base.NavigateActiveSkillTargetingBehaviorForEnemy();
    }


    // protected override void SubmitPlayerActiveSkillAction(BattleMenuController battleMenuController)
    // {
    //     battleMenuController.audioController.PlayAudio(SoundEffectEnums.Confirm);
    //     List<CombatTileController> tiles = new();

    //     tiles.Add(battleMenuController.actionGridTargetCells[battleMenuController.targetedTile]);

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

    // public override bool CanBeDoneToTilePlayerActiveSkill(BattleMenuController battleMenuController)
    // {
    //     return battleMenuController.actionGridTargetCells.ContainsKey(battleMenuController.targetedTile) && battleMenuController.actionGridTargetCells[battleMenuController.targetedTile].HasPartyOccupant();
    // }
}
}