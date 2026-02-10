using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DPS.Common;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Buff_Terrain_Movement_", menuName = "ScriptableObjects/Status Effect/Terrain Movement")]
public class TerrainMovementBuffSO : StatusEffectSO
{
    public GenericDictionary<ElementSO, int> terrainMovementOverrides = new GenericDictionary<ElementSO, int>();

    // public override GenericDictionary<string, StatusEffect> PruneOverlappingStatusEffects(GenericDictionary<string, StatusEffect> buffs)
    // {
    //     GenericDictionary<string, StatusEffect> newBuffs = new GenericDictionary<string, StatusEffect>();
    //     foreach (KeyValuePair<string, StatusEffect> buff in buffs)
    //     {
    //         if (buff.Value == null || buff.Value.statusEffect == null)
    //         {
    //             continue;
    //         }


    //         if (buff.Value.statusEffect.GetType() != typeof(TerrainMovementBuffSO))
    //         {

    //             newBuffs.Add(buff);
    //             continue;
    //         }
            

    //         TerrainMovementBuffSO terrainMovementBuff = buff.Value.statusEffect as TerrainMovementBuffSO;
    //         foreach(KeyValuePair<ElementSO, int> thisTerrainMovementOverride in this.terrainMovementOverrides) {
    //             if(terrainMovementBuff.terrainMovementOverrides != null && terrainMovementBuff.terrainMovementOverrides.Count > 0 && !terrainMovementBuff.terrainMovementOverrides.ContainsKey(thisTerrainMovementOverride.Key)) {
    //                 newBuffs.Add(buff);
    //             }
    //         }

    //     }
    //     return newBuffs;
    // }

    public override void PreStatMultiplierBuffs(IBattleEntity battleEntity)
    {
        GenericDictionary<ElementSO, int> terrainMovement = battleEntity.GetBattleTerrainMovementOverride();

        foreach(KeyValuePair<ElementSO, int> terrainMovementOverride in this.terrainMovementOverrides) {
            terrainMovement[terrainMovementOverride.Key] = terrainMovementOverride.Value;
        }
        base.PreStatMultiplierBuffs(battleEntity);
    }
}
}