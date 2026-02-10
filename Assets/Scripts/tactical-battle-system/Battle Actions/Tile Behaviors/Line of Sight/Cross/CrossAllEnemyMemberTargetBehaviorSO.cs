using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat{
[CreateAssetMenu(fileName = "Cross All Enemy Member Target Behavior", menuName = "ScriptableObjects/Active Skill/Behavior/LOS/Cross/All Enemy Member Target")]
public class CrossAllEnemyMemberTargetBehaviorSO : CrossActionTileBehaviorSO
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
    //     List<PartySlot> targets = new List<PartySlot>();
    //     List<CombatTileController> tiles = new List<CombatTileController>();
        
    //     foreach(KeyValuePair<Vector3, CombatTileController> keyValuePair in battleMenuController.actionGridTargetCells) {
    //         tiles.Add(keyValuePair.Value);
    //     }

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

    public override void NavigateItemTargetBehaviorForEnemy()
    {
        base.NavigateItemTargetBehaviorForEnemy();
    }

}
}