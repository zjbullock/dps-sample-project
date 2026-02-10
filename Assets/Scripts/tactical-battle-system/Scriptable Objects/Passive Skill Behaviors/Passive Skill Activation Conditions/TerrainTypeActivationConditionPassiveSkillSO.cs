using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Passive_Activation_Condition_Terrain_", menuName = "ScriptableObjects/Passive Skill/Activation Conditions/Terrain Activation Condition")]
public class TerrainTypeActivationConditionPassiveSkillSO : PassiveSkillBehaviorActivationConditionSO
{
    [Tooltip("List of element types of the terrain that the passive ability can be activated on.")]
    [SerializeField]
    private List<ElementSO> ElementSOs = new List<ElementSO>();
    public override bool ActivationConditionBattleEntity(IBattleEntity battleEntity, CombatTileController combatTileController) {

        return this.ElementSOs.Count > 0 && combatTileController != null && combatTileController.currentstate != null && combatTileController.currentstate.combatTileInteractionSO != null  && this.ElementSOs.Contains(combatTileController.currentstate.combatTileInteractionSO.ElementSO);
    }
}
}