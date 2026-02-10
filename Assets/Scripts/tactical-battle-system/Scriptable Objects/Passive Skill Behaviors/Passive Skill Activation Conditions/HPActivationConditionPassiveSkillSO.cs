using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Passive_Activation_Condition_HP_", menuName = "ScriptableObjects/Passive Skill/Activation Conditions/HP Percent Activation Condition")]
public class HPActivationConditionPassiveSkillSO : PassiveSkillBehaviorActivationConditionSO
{

        // Determines what % the user's current hp must be of the max hp to activate the buff
    [Range(0f, 1f)]
    public float hpPercentActivation;

    [Tooltip("If true, the value must be below the threshold to activate.")]
    public bool belowThreshold;
    public override bool ActivationConditionBattleEntity(IBattleEntity battleEntity, CombatTileController combatTileController)
    {
        RawStats rawStats = battleEntity.GetRawStats();

        float currentHPPercent = (float) rawStats.hp / (float) rawStats.maxHp;
        return  ((this.belowThreshold && currentHPPercent <= hpPercentActivation) || (!this.belowThreshold && currentHPPercent >= hpPercentActivation));
    }
}
}