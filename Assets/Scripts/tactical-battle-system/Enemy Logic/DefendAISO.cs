using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat{
[CreateAssetMenu(fileName = "Defend AI", menuName = "ScriptableObjects/Enemy AI/Defend Enemy AI")]
public class DefendAISO : EnemyAISO {

    public ActiveSkillSO defendSkill;

    public override ReadiedAbility DetermineActiveSkill(System.Random random, EnemyPartySlot enemy, List<PartySlot> playerParty, List<PartySlot> enemies, GenericDictionary<Vector3, CombatTileController> grid) {
        EnemyInfo enemyInfo = (EnemyInfo) enemy.BattleEntity;
        enemyInfo.AddLastUseActiveSkill(defendSkill);
        List<PartySlot> targets = new List<PartySlot>();
        targets.Add(enemy);
        return new ReadiedAbility(defendSkill, targets, null, null);
    }

    // Update is called once per frame
    public override bool CanBeActivated(EnemyPartySlot enemy, List<PartySlot> playerParty, List<PartySlot> enemies, uint turnCount, GenericDictionary<Vector3, CombatTileController> grid) { 
        return true;
    }
}
}