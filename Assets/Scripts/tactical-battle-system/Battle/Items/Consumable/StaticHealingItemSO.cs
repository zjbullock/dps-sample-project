using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Static Healing Item", menuName = "ScriptableObjects/Inventory/Static Healing Item")]
public class StaticHealingItemSO : ConsumableInventoryItemSO {

    [Range(1, 9999)]
    public int hpAmount = 1;
    public override void ExecuteMultiTargetSkill(BattleManager battleController, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles) {
        foreach(PartySlot partySlot in primaryAbilityTargets) {
                if(partySlot.GetBattleMember()! == null || partySlot.BattleEntity!.IsDead()) {
                    Debug.Log("Target still dead");
                    partySlot.StatusMessage("UNAFFECTED");
                    continue;
                }
                RawStats rawStats = partySlot.BattleEntity!.GetRawStats();
                if(hpAmount > 0) {
                    partySlot.HealHP(hpAmount, null);
                }
        }
    }

    // public override void ExecuteSingleTargetSkill(BattleManager battleController, PartySlot user, PartySlot primaryAbilityTarget, List<CombatTileController> combatTiles) {
    //     if(primaryAbilityTarget.GetBattleMember()! == null || primaryAbilityTarget.GetBattleEntity!.IsDead()) {
    //         Debug.Log("Target still dead");
    //         primaryAbilityTarget.StatusMessage("UNAFFECTED");
    //         return;
    //     }
    //     RawStats rawStats = primaryAbilityTarget.GetBattleEntity!.GetRawStats();
    //     if(hpAmount > 0) {
    //         primaryAbilityTarget.HealHP(hpAmount, null);
    //     }
    // }

}
}