using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "BattleWarden", menuName = "ScriptableObjects/Passive Skill/Behaviors/Battle Warden")]
public class BattleWardenPassiveSkillBehaviorSO : PassiveSkillBehaviorSO
{

    [Range(0f, 1f)]
    public float battleWardenScale;

    public override async Task ExecutePostEquipmentStatAddPassiveSkill(CharacterInfo characterInfo) {   
        await base.ExecutePostEquipmentStatAddPassiveSkill(characterInfo);
        characterInfo.RawStats = await PerformBattleWardenCalculation(characterInfo);
    }


    private async Task<RawStats> PerformBattleWardenCalculation(CharacterInfo characterInfo) {
        RawStats RawStats = new RawStats(characterInfo.RawStats);
        CharacterEquipment Equipment = characterInfo.Equipment;
        int defenseTotal = 0;


        foreach(EquipmentSO equipmentSO in await Equipment.GetLoadedEquipmentList())
        {
            defenseTotal += equipmentSO.equipmentInfo.rawStats.defense;   
        }


        defenseTotal = (int) (defenseTotal * battleWardenScale);
        RawStats.physicalAttack += defenseTotal;
        return RawStats;
    }
}
}