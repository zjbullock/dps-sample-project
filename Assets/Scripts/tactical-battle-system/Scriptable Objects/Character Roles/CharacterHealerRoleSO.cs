using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Role_Healer_#", menuName = "ScriptableObjects/Character Role/Heal")]
public class CharacterSupportRoleSO : CharacterRoleSO
{
    [SerializeField]
    private uint healPoints = 5;

    [SerializeField]
    private uint savingHealingPoints = 10;

    [SerializeField]
    private uint revivingPoints = 5;

    [SerializeField]
    private uint curingStatusAilmentPoints = 5;

    [SerializeField]
    private uint applyingBuffPoints = 5;
    
    public override void OnHeal(BattleManager battleController, PartySlot partySlot)
    {
        battleController.AddPartyPoints(this.healPoints);
        base.OnHeal(battleController, partySlot);
    }

    public override void OnSavingHeal(BattleManager battleController, PartySlot partySlot)
    {
        battleController.AddPartyPoints(this.savingHealingPoints);
        base.OnSavingHeal(battleController, partySlot);
    }

    public override void OnRevive(BattleManager battleController, PartySlot partySlot)
    {
        battleController.AddPartyPoints(this.revivingPoints);
        base.OnRevive(battleController, partySlot);
    }

    public override void OnCureStatus(BattleManager battleController, PartySlot partySlot)
    {
        battleController.AddPartyPoints(this.curingStatusAilmentPoints);
        base.OnCureStatus(battleController, partySlot);
    }

    public override void OnApplyBuff(BattleManager battleController, PartySlot partySlot)
    {
        battleController.AddPartyPoints(this.applyingBuffPoints);
        base.OnApplyBuff(battleController, partySlot);
    }
}
}