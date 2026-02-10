using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Passive_Activation_Condition_RP_", menuName = "ScriptableObjects/Passive Skill/Activation Conditions/RP Percent Activation Condition")]

public class RPActivationConditionPassiveSkillSO : PassiveSkillBehaviorActivationConditionSO
{
        // Determines what % the user's current hp must be of the max hp to activate the buff
    [Range(0f, 1f)]
    public float rpPercentageActivation;

    [Tooltip("If true, the value must be below the threshold to activate.")]
    public bool belowThreshold;
    #nullable enable
    public override bool ActivationConditionBattleEntity(IBattleEntity battleEntity, CombatTileController combatTileController)
    {
        CharacterInfo? characterInfo = battleEntity as CharacterInfo;
        if (characterInfo == null) {
            return false;
        }
        if (characterInfo.GetResolveGauge() != null) {
            if (characterInfo.ResolveGaugeSO != null) {
                float currentRPPercent = (float) characterInfo.resolvePoints / (float) characterInfo.ResolveGaugeSO.maxResolveGauge;
                return  ((this.belowThreshold && currentRPPercent <= rpPercentageActivation) || (!this.belowThreshold && currentRPPercent >= rpPercentageActivation));
            } else {
                return false;
            }
        }
        return false;
    }
}
}