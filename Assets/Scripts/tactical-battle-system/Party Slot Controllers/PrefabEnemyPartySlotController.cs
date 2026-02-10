using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
public class PrefabEnemyPartySlotController : EnemyPartySlotController
{
    
    public override void PreparePartyMembers(BattleManager battleController)
    {
        this.GetEnemyGameObjects(battleController);
        base.PreparePartyMembers(battleController);

    }
    
    #nullable enable

    private void GetEnemyGameObjects(BattleManager battleController) {
        if (battleController.BattleFieldController == null) {
            throw new System.Exception("battle controller instance missing BattleFieldController instance!");
        }

        List<PrefabEnemyBattleObject> enemyInstances = new(battleController.BattleFieldController.GetComponentsInChildren<PrefabEnemyBattleObject>());


        foreach(PrefabEnemyBattleObject enemyPrefab in enemyInstances) {
            Vector3Int spawnPoint = Vector3Int.FloorToInt(enemyPrefab.transform.position);
            spawnPoint.y = 0;
            Vector3 gridKey = spawnPoint;

            if (!battleController.BattleFieldController.Grid.TryGetValue(gridKey, out CombatTileController combatTileController)) {
                throw new System.Exception("Location doesn't exist");
            }

            GiantEnemyPartySlot? enemyPartySlot = this.GenerateEnemyPartySlot(enemyPrefab);
            if (enemyPartySlot == null) {
                continue;
            }

            enemyPartySlot.SetBattleObject(enemyPrefab);

            combatTileController.SetPartyOccupant(battleController, enemyPartySlot);
            PartySlots.Add(enemyPartySlot);
        }

    }

    private GiantEnemyPartySlot? GenerateEnemyPartySlot(PrefabEnemyBattleObject enemyPrefab) {
        if (enemyPrefab.EnemyInfoSO == null) {
            return null;
        }

        EnemyInfo enemy = new(enemyPrefab)
        {
            letterDesignation = "BOSS"
        };

        BattleMember newEnemy = new(new BattleEntitySlot(enemy))
        {
            // newEnemy.partyIndex = i;
            battleMemberType = BattleMemberType.Enemy
        };
        
        return new(enemyPrefab, newEnemy);
    }
}
}