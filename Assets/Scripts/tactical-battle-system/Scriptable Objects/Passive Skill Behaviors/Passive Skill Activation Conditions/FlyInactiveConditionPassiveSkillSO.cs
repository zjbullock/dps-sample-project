using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Passive_Activation_Condition_Fly_Inactive", menuName = "ScriptableObjects/Passive Skill/Activation Conditions/Flying Activation Condition")]

public class FlyInactiveConditionPassiveSkillSO : PassiveSkillBehaviorActivationConditionSO
{
    public override bool ActivationConditionBattleEntity(IBattleEntity battleEntity, CombatTileController combatTileController)
    {
        return !battleEntity.CanFly();
    }
}
}