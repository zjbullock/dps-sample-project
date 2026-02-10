using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Terrain Movement Override Skill Behavior", menuName = "ScriptableObjects/Passive Skill/Behaviors/Terrain Movement Override")]

public class TerrainMovementOverrideSkillBehaviorSO : PassiveSkillBehaviorSO
{
    public GenericDictionary<ElementSO, int> elementMovementOverrides = new GenericDictionary<ElementSO, int>();

    public override void ExecutePostEquipmentStatAddPassiveSkill(CharacterInfo characterInfo)
    {
        characterInfo.SetTerrainMovementOverride(this.elementMovementOverrides);
    }

}
}