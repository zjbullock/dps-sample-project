using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Mana Strike Passive Skill Behavior", menuName = "ScriptableObjects/Passive Skill/Behaviors/Mana Strike Passive")]
public class ManaStrikePassiveSkillBehaviorSO : PassiveSkillBehaviorSO
{
    public override async Task ExecutePostEquipmentStatAddPassiveSkill(CharacterInfo characterInfo) {   
        await base.ExecutePostEquipmentStatAddPassiveSkill(characterInfo);
        characterInfo.RawStats = await PerformManaStrikeCalculation(characterInfo);
        return;
    }

    private async Task<RawStats> PerformManaStrikeCalculation(CharacterInfo characterInfo) {
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

        foreach(EquipmentSO equipmentSO in await Equipment.GetLoadedEquipmentList())
        {
            RawStats.magicalAttack += equipmentSO.equipmentInfo.rawStats.physicalAttack;
        }

        return RawStats;
    }
}
}