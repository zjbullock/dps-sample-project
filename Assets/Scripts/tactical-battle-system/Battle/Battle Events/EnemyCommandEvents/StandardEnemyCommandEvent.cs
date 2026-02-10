using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
public class StandardEnemyCommandEvent : EnemyCommandEvent
{
    private bool attackingStructure = false;

    public StandardEnemyCommandEvent(BattleManager battleController, EnemyPartySlot enemyPartySlot) : base(battleController, enemyPartySlot)
    {
    }

    protected override void ExtraAILogic()
    {        
        if (!this.attackingStructure) {
            this.GetBasicAttackStructureAI(this.battleController);
            return;
        }
        base.ExtraAILogic();
    }

    protected override void GetAIList()
    {
        if (this.enemyInfo.enemyAIs != null && this.enemyInfo.enemyAIs.Count > 0) {
            this.enemyAIs.AddRange(this.enemyInfo.enemyAIs);
        }

        this.enemyAIs.Add(this.enemyInfo.basicAttackAI);
    }

    protected override void GetExtraAggroAIList()
    {
        return;
    }

    #nullable enable
    private void GetBasicAttackStructureAI(BattleManager battleController) {
        Vector3Int enmityTargetCoordinates = this.enemyPartySlot.enmityList[0].partySlot.GetCombatTileController().Position;

        if (enmityTargetCoordinates == null) {
            Debug.LogError("Something happened getting enmity Target coordinates");
            return;
        }

        float distance = Vector3.Distance(this.enemyPartySlot.GetCombatTileController().Position, enmityTargetCoordinates);

        foreach(KeyValuePair<Vector3, CombatTileController> keyValuePair in battleController.BattleFieldController.MovementGrid) {

            float newDistance = Vector3.Distance(keyValuePair.Key, (Vector3) enmityTargetCoordinates);
            if (newDistance < distance && !keyValuePair.Value.HasPartyOccupant()) {
                battleController.DestinationTile = keyValuePair.Value;
                distance = newDistance;
            }

        }

        this.attackingStructure = true;

        this.enemyPartySlot.GetBattleMember()!.BattleCommand = this.GetBasicAttackStructureAI(battleController, enemyInfo, random, enmityTargets, battleController.DestinationTile!);
    }

    private BattleCommand? GetBasicAttackStructureAI(BattleManager battleController, EnemyInfo enemy, System.Random random, List<PartySlot> enmityTargets, CombatTileController startingPosition) {
        if (enmityTargets == null || enmityTargets.Count == 0) {
            return null;
        }

        EnemyAISO enemyAISO = enemy.basicAttackAI;

        if(!enemyAISO.CanBeActivated(this.enemyPartySlot, enmityTargets, battleController.EnemyPartyController.PartySlots, battleController.TurnCount, battleController.Grid)) {

            return null;
        }
    
         /**
            Forecast all hittable tiles with the preferred ability from each movement grid option.
            Based upon forecasted tiles, choose the movement position that hits the most targets.
            If 0 hits, just move closer to the aggro target, and pass turn.
        */
        ReadiedAbility enemyReadiedAbility = enemyAISO.DetermineActiveSkill(random, this.enemyPartySlot, enmityTargets, battleController.EnemyPartyController.PartySlots, battleController.Grid);

        if(enemyReadiedAbility == null) {
            Debug.Log("No Readied Ability made: " + enemyAISO.name);
            return null;
        }

        Debug.Log("Making enemy attack structure command");
        

        return enemyReadiedAbility.GetEnemyAbilityStructureTarget(battleController, startingPosition);
    }
}
}