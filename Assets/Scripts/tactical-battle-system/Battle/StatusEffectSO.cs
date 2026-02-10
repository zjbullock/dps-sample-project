using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat{
public abstract class StatusEffectSO : ScriptableObject
{
    public enum StatusEffectTypes {
        Buff,
        Debuff
    }

    public string statusName;
    // A Description of the buff

    [TextArea(15, 20)]
    [SerializeField]
    private string description;
    public string Description { get => this.description; }
    // Determines if the buff is infinite turn count
    public bool infiniteTurns;

    //Turn Duration
    public int turnCount;

    //The sprite or icon for the buff
    public Sprite icon;

    [SerializeField]
    private StatusEffectTypes statusEffectType;

    public StatusEffectTypes StatusEffectType { get => this.statusEffectType;  }


    public virtual List<StatusEffect> PruneOverlappingStatusEffects(List<StatusEffect> statusEffects)
    {
        return this.RemoveStatusEffect(statusEffects);
    }


    public virtual void OnRemoveStatusEffect(IBattleEntity battleEntity)
    {
        return;
    }


    protected virtual List<StatusEffect> RemoveStatusEffect(List<StatusEffect> oldStatusEffects)
    {
        List<StatusEffect> newStatusEffects = new();

        foreach (StatusEffect statusEffect in oldStatusEffects)
        {
            if (statusEffect.statusEffect.statusName != this.name)
            {
                newStatusEffects.Add(statusEffect);
            }
        }

        return newStatusEffects;
    }

    public virtual void PreStatMultiplierBuffs(IBattleEntity battleEntity) {
        return;
    }

    public virtual void ProcessBuffList(IBattleEntity battleEntity) {
        return;
    }

    /**
        Should be called when something special should happen upon taking damage.
    */
    public virtual bool OnTakeDamage(ElementSO ElementSO, PartySlot partySlot, CombatTileController tileController) {
        return true;
    }
    

    /**
        Should be called when an attacker is dealing damage to another entity
    */
    public virtual bool OnDealDamage(ElementSO ElementSO, PartySlot attackingPartySlot, PartySlot hitPartySlot) {
        return true;
    }

}
}