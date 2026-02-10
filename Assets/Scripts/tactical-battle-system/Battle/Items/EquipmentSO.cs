using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Equipment", menuName = "ScriptableObjects/Inventory/Equipment")]
[Serializable]
public class EquipmentSO : InventoryItemSO
{

    [Range(1, 100)]
    public int level = 1;
    public EquipmentInfo equipmentInfo;

    #nullable enable
    public ActiveSkillSO? ActiveSkill
    {
        get  {
            return equipmentInfo.activeSkillSO;
        }
    }
    #nullable disable
}
}