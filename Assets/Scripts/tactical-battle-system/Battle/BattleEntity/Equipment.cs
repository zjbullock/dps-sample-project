using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[Serializable]
public class EquipmentInfo {
    public EquipmentInfo(EquipmentInfo equipmentInfo) {
        this.level = equipmentInfo.level;
        this.rawStats = new RawStats(equipmentInfo.rawStats);
    }

    [Tooltip("Level requirement for the item")]
    [Range(1, 200)]
    public int level = 1;
    public RawStats rawStats;

    public StatusAilmentResistances statusAilmentResistances;

    public ActiveSkillSO activeSkillSO;
}

}






