using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Revival Healing Item", menuName = "ScriptableObjects/Inventory/Revival Healing Item")]
public class RevivalHealingItemSO : PercentageHealingItemSO {
    public override void ExecuteMultiTargetSkill(BattleManager battleController, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles) {
        foreach(PartySlot partySlot in primaryAbilityTargets) {
            if(partySlot.GetBattleMember()! == null || !partySlot.BattleEntity!.IsDead()) {
                Debug.Log("Target still alive");
                partySlot.StatusMessage("UNAFFECTED");
                continue;
            }
            RawStats rawStats = partySlot.BattleEntity!.GetRawStats();
                Debug.Log("Attempting to heal");
            if(hpAmountPercent > 0f) {
                int healedHp = (int) (hpAmountPercent * (float) rawStats.maxHp);
                partySlot.HealHP(healedHp, null);
            }
            // if(partySlot.GetBattleEntity!.IsDead()) {
            //     partySlot.GetBattleEntityGO()!.SetAnimationState(AnimationStates.Default);
            // }
        }
    }
    // public override void ExecuteSingleTargetSkill(BattleManager battleController, PartySlot user, PartySlot primaryAbilityTarget, List<CombatTileController> combatTiles) {
    //     if(primaryAbilityTarget.GetBattleMember()! == null || !primaryAbilityTarget.GetBattleEntity!.IsDead()) {
    //         Debug.Log("Target still alive");
    //         primaryAbilityTarget.StatusMessage("UNAFFECTED");
    //         return;
    //     }
    //     RawStats rawStats = primaryAbilityTarget.GetBattleEntity!.GetRawStats();
    //     Debug.Log("Attempting to heal");
    //     if(hpAmountPercent > 0f) {
    //         int healedHp = (int) (hpAmountPercent * (float) rawStats.maxHp);
    //         primaryAbilityTarget.HealHP(healedHp, null);
    //     }
    //     // if(partySlot.GetBattleEntity!.IsDead()) {
    //     //     partySlot.GetBattleEntityGO()!.spriteAnimatorController.SetAnimation(AnimationStates.Default);
    //     // }
    // }
}
}