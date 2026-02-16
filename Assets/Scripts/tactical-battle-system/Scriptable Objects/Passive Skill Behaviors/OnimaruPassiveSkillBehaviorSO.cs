using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Onimaru Passive Skill Behavior", menuName = "ScriptableObjects/Passive Skill/Behaviors/Onimaru Passive")]

public class OnimaruPassiveSkillBehaviorSO : StatusEffectPassiveSkillBehaviorSO
{
    [Range(0, 100)]
    public int soraHealingRate;

#nullable enable
    public override async Task ExecuteBeginPhasePassiveSkill(PartySlot partySlot, CombatTileController? combatTileController)
    {
        if (partySlot == null)
        {
            return;
        }

        if (partySlot.GetBattleMember() == null || partySlot.BattleEntity == null)
        {
            return;
        }

        CharacterInfo? characterInfo = partySlot.BattleEntity as CharacterInfo;
        if (characterInfo != null)
        {
            this.ReduceResolveGauge(characterInfo);
            partySlot.BattleEntityGO.UpdateGauges();
        }
        await base.ExecuteBeginPhasePassiveSkill(partySlot, combatTileController);

    }

    public override async Task ExecuteEndCombatPassiveSkill(PartySlot partySlot) {
        if (partySlot != null) {
            if (partySlot.GetBattleMember()! != null && partySlot.BattleEntity != null) {
                CharacterInfo? characterInfo = partySlot.BattleEntity as CharacterInfo;
                if (characterInfo != null) {
                    characterInfo.resolvePoints = 0;
                }
            }
            await base.ExecuteEndCombatPassiveSkill(partySlot);

        } 
    }
    #nullable disable

    private void ReduceResolveGauge(CharacterInfo characterInfo) {
        characterInfo.resolvePoints = ( (characterInfo.resolvePoints - soraHealingRate) + (int) Math.Abs(characterInfo.resolvePoints - soraHealingRate)) / 2;
        return;
    }
}
}