using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Role_Damage_#", menuName = "ScriptableObjects/Character Role/Damage")]
public class CharacterDamageRoleSO : CharacterRoleSO
{
    [SerializeField]
    private uint dealingDamagePoints = 5;

    [SerializeField]
    private uint inflictingStatusAilmentsPoints = 5;

    [SerializeField]
    private uint criticalHitsPoints = 10;

    public override void OnDealingDamage(BattleManager battleController, PartySlot partySlot)
    {
        battleController.AddPartyPoints(this.dealingDamagePoints);
        #nullable enable
        CharacterInfo? characterInfo = partySlot.BattleEntity! as CharacterInfo;
        if(characterInfo != null && characterInfo!.GetResolveGauge() != null) {
            characterInfo!.GetResolveGauge()!.OnDealingDamage(partySlot, battleController);
        }
        #nullable disable
        base.OnDealingDamage(battleController, partySlot);
    }

    public override void OnInflictingStatusAilment(BattleManager battleController, BattleMember battleMember)
    {
        battleController.AddPartyPoints(this.inflictingStatusAilmentsPoints);
        base.OnInflictingStatusAilment(battleController, battleMember);
    }

    public override void OnCriticalHit(BattleManager battleController, PartySlot partySlot)
    {
        battleController.AddPartyPoints(this.criticalHitsPoints);
        base.OnCriticalHit(battleController, partySlot);
    }
}
}