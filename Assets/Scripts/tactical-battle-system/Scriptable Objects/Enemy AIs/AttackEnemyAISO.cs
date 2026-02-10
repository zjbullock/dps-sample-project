using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Basic Attack Enemy AI", menuName = "ScriptableObjects/Enemy AI/Basic Attack Enemy Behavior")]

public class AttackEnemyAISO : EnemyAISO {
    public ActiveSkillSO AttackSkill;

    public override ReadiedAbility DetermineActiveSkill(System.Random random, EnemyPartySlot enemy, List<PartySlot> playerParty, List<PartySlot> enemies, GenericDictionary<Vector3, CombatTileController> grid) {
        if (playerParty == null || playerParty.Count == 0) {
            return null;
        } 

        EnemyInfo enemyInfo = (EnemyInfo) enemy.BattleEntity;
        if(enemyInfo.IsDead()) {
            return null;
        }

        List<PartySlot> primaryAbilityTargets = new List<PartySlot>();

        ActiveSkillSO activeSkill = AttackSkill;

        primaryAbilityTargets.Add(playerParty[0]);
        enemyInfo.AddLastUseActiveSkill(activeSkill);
        return new ReadiedAbility(activeSkill, primaryAbilityTargets, null, null);
    }

    public override bool CanBeActivated(EnemyPartySlot enemy, List<PartySlot> playerParty, List<PartySlot> enemies, uint turnCount, GenericDictionary<Vector3, CombatTileController> grid) {
        return true;
    }
}
}