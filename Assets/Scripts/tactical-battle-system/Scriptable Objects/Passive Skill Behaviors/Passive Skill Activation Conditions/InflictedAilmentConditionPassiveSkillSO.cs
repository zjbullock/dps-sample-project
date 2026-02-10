using System.Collections;
using System.Collections.Generic;
using DPS.TacticalCombat;
using UnityEngine;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Passive_Activation_Condition_Status_Ailment_Type", menuName = "ScriptableObjects/Passive Skill/Activation Conditions/Status Ailment Activation Condition")]
public class InflictedAilmentConditionPassiveSkillSO : PassiveSkillBehaviorActivationConditionSO
{

    [Tooltip("If true, checks if the status ailment prevents turns.")]
    [SerializeField]
    private bool preventsTurn = false;

    [Tooltip("If true, checks if the status ailment prevents skill actions")]
    [SerializeField]
    private bool preventsSkillAction = false;


    [Tooltip("IF true, checks if the status ailment prevents item actions")]
    [SerializeField]
    private bool preventsItemActions = false;

    [Tooltip("If true, checks if the status ailment can deal damage")]
    [SerializeField]
    private bool canDealDamage = false;

    public override bool ActivationConditionBattleEntity(IBattleEntity battleEntity, CombatTileController combatTileController)
    {
        InflictedAilment inflictedAilment = battleEntity.GetInflictedAilment();
        if (inflictedAilment != null && inflictedAilment.statusAilment != null) {
            if (preventsTurn && inflictedAilment.statusAilment.preventsTurn) {
                return false;
            }
            else if (preventsSkillAction &&  inflictedAilment.statusAilment.preventsSkillAction) {
                return false;
            }
            else if (preventsItemActions && inflictedAilment.statusAilment.preventsItemActions) {
                return false;
            }
            else if (canDealDamage && inflictedAilment.statusAilment.canDealDamage) {
                return false;
            }
        }
        return true;
    }

}
}