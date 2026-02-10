using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Percentage Healing Item", menuName = "ScriptableObjects/Inventory/Percentage Healing Item")]
public class PercentageHealingItemSO : ConsumableInventoryItemSO
{
    //The % amount of hp that is recovered
    [RangeAttribute(0f, 1f)]
    public float hpAmountPercent;
    public override void ExecuteMultiTargetSkill(BattleManager battleController, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles) {
           foreach(PartySlot partySlot in primaryAbilityTargets) {

                if(partySlot.GetBattleMember()! == null || partySlot.BattleEntity!.IsDead()) {
                    Debug.Log("Target still dead");
                    if(partySlot.BattleEntityGO != null) {
                        partySlot.StatusMessage("UNAFFECTED");
                    }                    
                    continue;
                }
                RawStats rawStats = partySlot.BattleEntity!.GetRawStats();
                    Debug.Log("Attempting to heal");
                if(hpAmountPercent > 0f) {
                    int healedHp = (int) hpAmountPercent * rawStats.maxHp;
                    partySlot.HealHP(healedHp, null);
                }
            }
    }
}
    // public override void ExecuteSingleTargetSkill(BattleManager battleController, PartySlot user, PartySlot primaryAbilityTarget, List<CombatTileController> combatTiles) {
    //     if(primaryAbilityTarget.GetBattleMember()! == null || primaryAbilityTarget.GetBattleEntity!.IsDead()) {
    //         Debug.Log("Target still dead");
    //         if(primaryAbilityTarget.BattleEntityGO != null) {
    //             primaryAbilityTarget.StatusMessage("UNAFFECTED");
    //         }
    //         return;
    //     }
    //     RawStats rawStats = primaryAbilityTarget.GetBattleEntity!.GetRawStats();
    //         Debug.Log("Attempting to heal");
    //     if(hpAmountPercent > 0f) {
    //         int healedHp = (int) hpAmountPercent * rawStats.maxHp;
    //         primaryAbilityTarget.HealHP(healedHp, null);
    //     }
    // }

}
