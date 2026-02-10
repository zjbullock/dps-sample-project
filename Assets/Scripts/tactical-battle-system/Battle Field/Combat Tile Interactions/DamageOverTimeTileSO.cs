using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Damage Over Time Tile", menuName = "ScriptableObjects/Tile Interactions/Damage Over Time")]

public class DamageOverTimeTileSO : CombatTileInteractionSO
{
    [Tooltip("Determines the factor of damage done.  I.e. 0.5 would be half the target's max HP.")]
    [Range(0, 1)]
    public float damageScaleFactor = 0.5f;

    [Tooltip("The Damage status text to display for the damage over time tile")]
    [SerializeField]
    private string damageStatusText;

    [SerializeField]
    private ElementSO elementalTrigger;
    
    public override void OnEntityTurnStart(BattleManager battleController, PartySlot partySlot, CombatTileController combatTile) {
        if (!partySlot.BattleEntity!.IsDead() && !partySlot.BattleEntity!.CanFly()) {
            int hpDamage = BattleProcessingStatic.GetPercentDamage(partySlot.BattleEntity!.GetRawStats().maxHp, this.damageScaleFactor);
            partySlot.ProcessDamage(battleController, hpDamage, this.ElementSO, additionalText: this.damageStatusText.ToUpper());
        }
        return;
    }


    public override bool CanBeSkillTriggered(BattleManager battleController, IBattleActionCommand action, CombatTileController combatTile) {
        List<BattleActionEventSO> battleEvents = action.GetBattleActionEvents();
        foreach (BattleActionEventSO battleActionEventSO in battleEvents)
        {
            if (battleActionEventSO is IBattleDamageEvent battleDamageEvent)
            {
               return action.GetElement() == elementalTrigger; 
            }
        }
        return base.CanBeSkillTriggered(battleController, action, combatTile);
    }
}
}