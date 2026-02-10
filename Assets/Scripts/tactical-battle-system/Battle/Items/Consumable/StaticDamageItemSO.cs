using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Static_Damage_Item", menuName = "ScriptableObjects/Inventory/Static Damage Item")]
public class StaticDamageItemSO : ConsumableInventoryItemSO {
    [Range(10, 10000)]
    public int damageAmount = 10;
    //Element of the item if it does damage
    public ActionTypeEnums attackType;
    //Flag for if this item inflicts debuffs on enemies.

    public override void ExecuteMultiTargetSkill(BattleManager battleController, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles) {
        foreach(PartySlot partySlot in primaryAbilityTargets) {
            if(partySlot.GetBattleMember()! == null || partySlot.BattleEntity!.IsDead()) {
                continue;
            }
            performDamageCalculation(battleController, partySlot, combatTiles);
        }
    }

    // public override void ExecuteSingleTargetSkill(BattleManager battleController, PartySlot user, PartySlot primaryAbilityTarget, List<CombatTileController> combatTiles) {
    //     if(primaryAbilityTarget.GetBattleMember()! == null || primaryAbilityTarget.GetBattleEntity!.IsDead()) {
    //         return;
    //     }
    //     performDamageCalculation(battleController, primaryAbilityTarget, combatTiles);
    // }

    private void performDamageCalculation(BattleManager battleController, PartySlot primaryAbilityTarget, List<CombatTileController> selectedTiles) {
        int damageAmount = this.damageAmount;

        primaryAbilityTarget.ProcessDamage(battleController, damageAmount, base.element);

    }
}
}