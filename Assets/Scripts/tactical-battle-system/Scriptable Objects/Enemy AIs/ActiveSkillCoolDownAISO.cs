using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat {
//Create skill if MP cost is met, and it's not on cooldown
[CreateAssetMenu(fileName = "Active Skill with Cooldown Enemy AI", menuName = "ScriptableObjects/Enemy AI/Active Skill with Cooldown Enemy AI")]
public class ActiveSkillCoolDownAISO : EnemyAISO {

    public ActiveSkillSO SpecialSkill;
    
    //Designates the amount of time that must wait before activating again
    [Range(0, 100)]
    public int turnCoolDown;

    public override ReadiedAbility DetermineActiveSkill(System.Random random, EnemyPartySlot enemy, List<PartySlot> playerParty, List<PartySlot> enemies, GenericDictionary<Vector3, CombatTileController> grid) {
        EnemyInfo enemyInfo = (EnemyInfo) enemy.BattleEntity;
        enemyInfo.AddLastUseActiveSkill(SpecialSkill);
        List<PartySlot> affectedParty = new List<PartySlot>();
        switch(SpecialSkill.GetTargetType()) {
            case TargetTypes.Single_Enemy:
                affectedParty.Add(playerParty[0]);
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
        return new ReadiedAbility(SpecialSkill, affectedParty, null, null);
    }

    public override bool CanBeActivated(EnemyPartySlot enemy, List<PartySlot> playerParty, List<PartySlot> enemies, uint turnCount, GenericDictionary<Vector3, CombatTileController> grid) {
        EnemyInfo enemyInfo = (EnemyInfo) enemy.BattleEntity;

        if(SpecialSkill == null || (SpecialSkill != null && !SpecialSkill.ConditionMet(enemyInfo))) {
            return false;
        }


        Debug.Log(enemyInfo.GetName() + " MP: "   + enemyInfo.BattleRawStats.mp);
        int lastUsedSkillIndex = enemyInfo.attemptedActiveSkills.LastIndexOf(SpecialSkill);
        int currentIndex = enemyInfo.attemptedActiveSkills.Count;
        Debug.Log("Last Used Skill Index: " + lastUsedSkillIndex);
        Debug.Log("Current Index: " + currentIndex);

        if(lastUsedSkillIndex == -1 || (lastUsedSkillIndex >= 0 && currentIndex - lastUsedSkillIndex > turnCoolDown)) {
            return true;
        }
        // if(enemyInfo.BattleRawStats.mp >= SpecialSkill.mpCost &&  ((enemyInfo.attemptedActiveSkills.Count == 0) || (enemyInfo.attemptedActiveSkills[enemyInfo.attemptedActiveSkills.Count-1] != SpecialSkill.skillName))) {
        //     return true;
        // }
        return false;
    }
}
}