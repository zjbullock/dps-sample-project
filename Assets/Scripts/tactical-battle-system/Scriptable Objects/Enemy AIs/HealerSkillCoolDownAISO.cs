using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Healer Skill Enemy AI", menuName = "ScriptableObjects/Enemy AI/Healer Skill Enemy AI")]
public class HealerSkillCoolDownAISO : EnemyAISO {
    private ActiveSkillSO HealSkill;

    [Range(0, 100)]
    public int turnCoolDown;

    [Range(0f, 1f)]
    public float percentageHPThreshold;

    public override ReadiedAbility DetermineActiveSkill(System.Random random, EnemyPartySlot enemy, List<PartySlot> playerParty, List<PartySlot> enemies, GenericDictionary<Vector3, CombatTileController> grid) {
        EnemyInfo enemyInfo = (EnemyInfo) enemy.BattleEntity;
        enemyInfo.AddLastUseActiveSkill(HealSkill);
        List<PartySlot> targets = new List<PartySlot>();
        switch(HealSkill.GetTargetType()) {
            case TargetTypes.Single_Party_Member:
                float startingPercent = 1f;
                PartySlot targetedSlot = null;
                foreach(PartySlot partySlot in enemies) {
                    if (!partySlot.BattleEntity!.IsDead()) {
                        if(targetedSlot == null) {
                            targetedSlot = partySlot;
                        }
                        float percentageHP = (float) partySlot.BattleEntity!.GetRawStats().hp / (float) partySlot.BattleEntity!.GetRawStats().maxHp;
                        if (percentageHP <= percentageHPThreshold && percentageHP < startingPercent) {
                            startingPercent = percentageHP;
                            targetedSlot = partySlot;
                        }
                    }
                }
                if(targetedSlot != null) {
                    targets.Add(targetedSlot);
                }
                break;
            case TargetTypes.All_Party_Members:
                targets = enemies;
                break;
        }
        return new ReadiedAbility(HealSkill, targets, null, null);  //Set Muscling Drop skill if MP requirement
    }

    public override bool CanBeActivated(EnemyPartySlot enemy,List<PartySlot> playerParty, List<PartySlot> enemies, uint turnCount, GenericDictionary<Vector3, CombatTileController> grid) {
        EnemyInfo enemyInfo = (EnemyInfo) enemy.BattleEntity;

        if(HealSkill == null || !HealSkill.ConditionMet(enemyInfo)) {
            return false;
        }

        // int lastUsedSkillIndex = enemyInfo.attemptedActiveSkills.LastIndexOf(HealSkill);
        // int lastIndex = enemyInfo.attemptedActiveSkills.Count - 1;
        
        foreach (PartySlot partySlot in enemies)
        {
            if (partySlot.BattleEntity.IsDead())
            {
                continue;
            }

            float percentageHP = ((float)partySlot.BattleEntity!.GetRawStats().hp) / ((float)partySlot.BattleEntity!.GetRawStats().maxHp);
            if (percentageHP < percentageHPThreshold)
            {
                Debug.Log("Party slot: " + partySlot.BattleEntity!.GetName());
                Debug.Log("Percentage Threshold: " + percentageHP);
                return true;
            }
        }
        // if(enemyInfo.BattleRawStats.mp >= SpecialSkill.mpCost &&  ((enemyInfo.attemptedActiveSkills.Count == 0) || (enemyInfo.attemptedActiveSkills[enemyInfo.attemptedActiveSkills.Count-1] != SpecialSkill.skillName))) {
        //     return true;
        // }
        return false;
    }

}
}