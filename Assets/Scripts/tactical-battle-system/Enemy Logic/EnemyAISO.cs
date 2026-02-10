using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat {
public abstract class EnemyAISO : ScriptableObject, IEnemyAI {
    public virtual ReadiedAbility DetermineActiveSkill(System.Random random, EnemyPartySlot enemy, List<PartySlot> playerParty, List<PartySlot> enemies, GenericDictionary<Vector3, CombatTileController> grid) {
        return null;
    }

    public virtual bool CanBeActivated(EnemyPartySlot enemy, List<PartySlot> playerParty, List<PartySlot> enemies, uint turnCount, GenericDictionary<Vector3, CombatTileController> grid) {
        return true;
    }
}
}