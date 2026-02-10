using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat {
//Given a list of active skills, it will select from a list of usable skills
[CreateAssetMenu(fileName = "Skilled Enemy AI", menuName = "ScriptableObjects/Enemy AI/Random Skill Enemy AI")]
public class RandomUsableSkillAISO : EnemyAISO {

    public List<ActiveSkillSO> ActiveSkills;

    public override ReadiedAbility DetermineActiveSkill(System.Random random, EnemyPartySlot enemy, List<PartySlot> playerParty, List<PartySlot> enemies, GenericDictionary<Vector3, CombatTileController> grid) {
        EnemyInfo enemyInfo = (EnemyInfo) enemy.BattleEntity;
        List<ActiveSkillSO> usableActiveSkills = new List<ActiveSkillSO>();
        //Adds all usable skills based on MP
        foreach(ActiveSkillSO activeSkill in ActiveSkills) {
            if (activeSkill.ConditionMet(enemyInfo)) {
                usableActiveSkills.Add(activeSkill);
            }
        }
        int activeSkillIndex = random.Next(0, usableActiveSkills.Count);
        ActiveSkillSO selectedSkill = usableActiveSkills[activeSkillIndex];
        // if(activeSkills.Count > 0 && enemyInfo.attemptedActiveSkills != "Attack") {

        // }
        List<PartySlot> affectedParty = new List<PartySlot>();
        switch(selectedSkill.GetTargetType()) {

            case TargetTypes.Single_Enemy:
                int targetedPartySlotIndex = random.Next(0, playerParty.Count);
                affectedParty.Add(playerParty[targetedPartySlotIndex]);
                break;
            case TargetTypes.All_Enemies:
                affectedParty = playerParty;
                break;
            case TargetTypes.Single_Party_Member:
                int targetedEnemySlotIndex = random.Next(0, enemies.Count);
                affectedParty.Add(enemies[targetedEnemySlotIndex]);
                break;
            case TargetTypes.All_Party_Members:
                affectedParty = enemies;
                break;
        }
        enemyInfo.AddLastUseActiveSkill(selectedSkill);
        return new ReadiedAbility(selectedSkill, affectedParty, null, null);  //Set Muscling Drop skill if MP requirement
    }

    public override bool CanBeActivated(EnemyPartySlot enemy, List<PartySlot> playerParty, List<PartySlot> enemies, uint turnCount, GenericDictionary<Vector3, CombatTileController> grid) {
        //Adds all usable skills based on MP
        EnemyInfo enemyInfo = (EnemyInfo) enemy.BattleEntity;
        foreach(ActiveSkillSO activeSkill in ActiveSkills) {
            if (activeSkill.ConditionMet(enemyInfo)) {
                return true;
            }
        }
        return false;
    }

}
}
