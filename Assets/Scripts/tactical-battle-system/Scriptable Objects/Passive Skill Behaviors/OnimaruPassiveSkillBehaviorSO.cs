using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Onimaru Passive Skill Behavior", menuName = "ScriptableObjects/Passive Skill/Behaviors/Onimaru Passive")]

public class OnimaruPassiveSkillBehaviorSO : StatusEffectPassiveSkillBehaviorSO
{
    [Range(0, 100)]
    public int soraHealingRate;

#nullable enable
    public override void ExecuteBeginPhasePassiveSkill(PartySlot partySlot, CombatTileController? combatTileController)
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
        base.ExecuteBeginPhasePassiveSkill(partySlot, combatTileController);

    }

    public override void ExecuteEndCombatPassiveSkill(PartySlot partySlot) {
        if (partySlot != null) {
            if (partySlot.GetBattleMember()! != null && partySlot.BattleEntity != null) {
                CharacterInfo? characterInfo = partySlot.BattleEntity as CharacterInfo;
                if (characterInfo != null) {
                    characterInfo.resolvePoints = 0;
                }
            }
            base.ExecuteEndCombatPassiveSkill(partySlot);

        } 
    }
    #nullable disable

    private void ReduceResolveGauge(CharacterInfo characterInfo) {
        characterInfo.resolvePoints = ( (characterInfo.resolvePoints - soraHealingRate) + (int) Math.Abs(characterInfo.resolvePoints - soraHealingRate)) / 2;
        return;
    }
}
}