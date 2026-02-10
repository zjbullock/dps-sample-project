using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Mana Strike Passive Skill Behavior", menuName = "ScriptableObjects/Passive Skill/Behaviors/Mana Strike Passive")]
public class ManaStrikePassiveSkillBehaviorSO : PassiveSkillBehaviorSO
{
    public override void ExecutePostEquipmentStatAddPassiveSkill(CharacterInfo characterInfo) {   
        base.ExecutePostEquipmentStatAddPassiveSkill(characterInfo);
        characterInfo.RawStats = PerformManaStrikeCalculation(characterInfo);
    }

    private RawStats PerformManaStrikeCalculation(CharacterInfo characterInfo) {
        RawStats RawStats = new RawStats(characterInfo.RawStats);
        CharacterEquipment Equipment = characterInfo.Equipment;

    //     EquipmentSO mainHand = Equipment.EquipmentSlots.MainHand;
    //     if(mainHand != null && mainHand.equipmentInfo != null && mainHand.equipmentInfo.rawStats != null) {
    //         RawStats.magicalAttack += mainHand.equipmentInfo.rawStats.physicalAttack;
    //     }

    //    EquipmentSO offHand = Equipment.EquipmentSlots.OffHand;
    //     if(offHand != null && offHand.equipmentInfo != null && offHand.equipmentInfo.rawStats != null) {
    //         RawStats.magicalAttack += offHand.equipmentInfo.rawStats.physicalAttack;
    //     }

        BodyEquipmentSO armor = Equipment.EquipmentSlots.Body;
        if(armor != null && armor.equipmentInfo != null && armor.equipmentInfo.rawStats != null) {
            RawStats.magicalAttack += armor.equipmentInfo.rawStats.physicalAttack;
        }


        HelmEquipmentSO helm = Equipment.EquipmentSlots.Helm;
        if(helm != null && helm.equipmentInfo != null && helm.equipmentInfo.rawStats != null) {
            RawStats.magicalAttack += helm.equipmentInfo.rawStats.physicalAttack;
        }


        BootsEquipmentSO boots = Equipment.EquipmentSlots.Boots;
        if(boots != null && boots.equipmentInfo != null && boots.equipmentInfo.rawStats != null) {
            RawStats.magicalAttack += boots.equipmentInfo.rawStats.physicalAttack;
        }

        AccessoryEquipmentSO accessory_1 = Equipment.EquipmentSlots.Accessory_1;
        if(accessory_1 != null && accessory_1.equipmentInfo != null && accessory_1.equipmentInfo.rawStats != null) {
            RawStats.magicalAttack += accessory_1.equipmentInfo.rawStats.physicalAttack;
        }

        AccessoryEquipmentSO accessory_2 = Equipment.EquipmentSlots.Accessory_2;
        if(accessory_2 != null && accessory_2.equipmentInfo != null && accessory_2.equipmentInfo.rawStats != null) {
            RawStats.magicalAttack += accessory_2.equipmentInfo.rawStats.physicalAttack;
        }

        return RawStats;
    }
}
}