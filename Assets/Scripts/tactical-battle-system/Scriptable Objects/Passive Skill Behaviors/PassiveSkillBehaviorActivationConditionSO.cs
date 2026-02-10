using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Passive_Activation_Condition_", menuName = "ScriptableObjects/Passive Skill/Activation Conditions/default")]

public class PassiveSkillBehaviorActivationConditionSO : ScriptableObject
{
    public virtual bool ActivationConditionBattleEntity(IBattleEntity battleEntity, CombatTileController combatTileController) {

        return true;
    }
}
}