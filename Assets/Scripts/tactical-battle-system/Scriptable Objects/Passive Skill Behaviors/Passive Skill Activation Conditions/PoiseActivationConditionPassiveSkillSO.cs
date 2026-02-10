using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {

[CreateAssetMenu(fileName = "Passive_Activation_Condition_Poise_", menuName = "ScriptableObjects/Passive Skill/Activation Conditions/Poise Activation Condition")]
public class PoiseActivationConditionPassiveSkillSO : PassiveSkillBehaviorActivationConditionSO
{
    [Tooltip("List of valid poise states for a passive to be activated on")]
    [SerializeField]
    private List<PoiseState> validPoiseStates;

    public override bool ActivationConditionBattleEntity(IBattleEntity battleEntity, CombatTileController combatTileController)
    {
        Poise poise = battleEntity.GetPoiseState();

        if (validPoiseStates.Contains(poise.PoiseState)) {
            return true;
        }
        return false;
    }
}
}