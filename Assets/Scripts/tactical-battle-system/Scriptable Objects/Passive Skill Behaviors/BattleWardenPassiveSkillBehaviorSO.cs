using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "BattleWarden", menuName = "ScriptableObjects/Passive Skill/Behaviors/Battle Warden")]
public class BattleWardenPassiveSkillBehaviorSO : PassiveSkillBehaviorSO
{

    [Range(0f, 1f)]
    public float battleWardenScale;

    public override void ExecutePostEquipmentStatAddPassiveSkill(CharacterInfo characterInfo) {   
        base.ExecutePostEquipmentStatAddPassiveSkill(characterInfo);
        characterInfo.RawStats = PerformBattleWardenCalculation(characterInfo);
    }


    private RawStats PerformBattleWardenCalculation(CharacterInfo characterInfo) {
        RawStats RawStats = new RawStats(characterInfo.RawStats);
        CharacterEquipment Equipment = characterInfo.Equipment;
        int defenseTotal = 0;

        // EquipmentSO mainHand = Equipment.EquipmentSlots.MainHand;
    //     if(mainHand != null && mainHand.equipmentInfo != null && mainHand.equipmentInfo.rawStats != null) {
    //         defenseTotal += mainHand.equipmentInfo.rawStats.defense;
    //     }

    //    EquipmentSO offHand = Equipment.EquipmentSlots.OffHand;
    //     if(offHand != null && offHand.equipmentInfo != null && offHand.equipmentInfo.rawStats != null) {
    //         defenseTotal += offHand.equipmentInfo.rawStats.defense;
    //     }

        BodyEquipmentSO armor = Equipment.EquipmentSlots.Body;
        if(armor != null && armor.equipmentInfo != null && armor.equipmentInfo.rawStats != null) {
            defenseTotal += armor.equipmentInfo.rawStats.defense;
        }


        HelmEquipmentSO helm = Equipment.EquipmentSlots.Helm;
        if(helm != null && helm.equipmentInfo != null && helm.equipmentInfo.rawStats != null) {
            defenseTotal += helm.equipmentInfo.rawStats.defense;
        }


        BootsEquipmentSO boots = Equipment.EquipmentSlots.Boots;
        if(boots != null && boots.equipmentInfo != null && boots.equipmentInfo.rawStats != null) {
            defenseTotal += boots.equipmentInfo.rawStats.defense;
        }

        AccessoryEquipmentSO accessory_1 = Equipment.EquipmentSlots.Accessory_1;
        if(accessory_1 != null && accessory_1.equipmentInfo != null && accessory_1.equipmentInfo.rawStats != null) {
            defenseTotal += accessory_1.equipmentInfo.rawStats.defense;
        }

        AccessoryEquipmentSO accessory_2 = Equipment.EquipmentSlots.Accessory_2;
        if(accessory_2 != null && accessory_2.equipmentInfo != null && accessory_2.equipmentInfo.rawStats != null) {
            defenseTotal += accessory_2.equipmentInfo.rawStats.defense;
        }


        defenseTotal = (int) ((float) defenseTotal * battleWardenScale);
        RawStats.physicalAttack += defenseTotal;
        return RawStats;
    }
}
}