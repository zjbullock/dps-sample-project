using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
public abstract class CharacterRoleSO : ScriptableObject
{
    [SerializeField]
    protected string roleName;

    public string RoleName { get => this.roleName; }
    
    public virtual void OnTurnStart(BattleManager battleController, PartySlot partySlot) {
        return;
    }

    public virtual void OnTurnEnd(BattleManager battleController, PartySlot partySlot) {
        return;
    }

    public virtual void OnSkillCast(BattleManager battleController, PartySlot partySlot, ActiveSkillSO activeSkillSO){
        return;
    }

    public virtual void OnDealingDamage(BattleManager battleController, PartySlot partySlot){
        return;
    }

    public virtual void OnTakingDamage(BattleManager battleController, PartySlot attackerPartySlot, PartySlot defenderPartySlot){
        return;
    }

    public virtual void OnInflictingStatusAilment(BattleManager battleController, BattleMember battleMember){
        return;
    }

    public virtual void OnCriticalHit(BattleManager battleController, PartySlot partySlot){
        return;
    }

    public virtual void OnHeal(BattleManager battleController, PartySlot partySlot){
        return;
    }

    public virtual void OnSavingHeal(BattleManager battleController, PartySlot partySlot) {
        return;
    }

    public virtual void OnRevive(BattleManager battleController, PartySlot partySlot){
        return;
    }

    public virtual void OnCureStatus(BattleManager battleController, PartySlot partySlot){
        return;
    }

    public virtual void OnApplyBuff(BattleManager battleController, PartySlot partySlot){
        return;
    }

    public virtual void OnBlock(BattleManager battleController, PartySlot partySlot){
        return;
    }

    public virtual void OnEvade(BattleManager battleController, PartySlot partySlot){
        return;
    }
}
}
