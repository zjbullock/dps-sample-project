using System.Collections;
using System.Collections.Generic;
using DPS.TacticalCombat;
using UnityEngine;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Passive_Activation_Condition_MP_", menuName = "ScriptableObjects/Passive Skill/Activation Conditions/MP Percent Activation Condition")]
public class MPActivationConditionPassiveSkillSO : PassiveSkillBehaviorActivationConditionSO
{
    // Start is called before the first frame update
        // Determines what % the user's current hp must be of the max hp to activate the buff
    [Range(0f, 1f)]
    public float mpPercentActivation;

    [Tooltip("If true, the value must be below the threshold to activate.")]
    public bool belowThreshold;
    public override bool ActivationConditionBattleEntity(IBattleEntity battleEntity, CombatTileController combatTileController)
    {
        if (battleEntity == null) {
            return false;
        }
        #nullable enable
        RawStats? rawStats = battleEntity.GetRawStats();
        if (rawStats == null) {
            return false;
        }
        float currentMPPercent = (float) rawStats.mp / (float) rawStats.maxMp;
        #nullable disable
        return  ((this.belowThreshold && currentMPPercent <= this.mpPercentActivation) || (!this.belowThreshold && currentMPPercent >= this.mpPercentActivation));
    }
}
}