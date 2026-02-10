using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat {
[System.Serializable]
public abstract class EnemyCommandEvent : IBattleEvent
{
    protected bool isDone = false;

    protected EnemyInfo enemyInfo;

    protected BattleManager battleController;

    protected List<PartySlot> enmityTargets;

    protected System.Random random;

    protected CombatTileController startingPosition;

    [SerializeField]
    protected List<IEnemyAI> enemyAIs = new List<IEnemyAI>();


    protected bool isAggroed = false;

    #nullable enable
    protected ReadiedAbility? readiedAbility = null;

    protected EnemyPartySlot enemyPartySlot;
    #nullable disable
    

    public EnemyCommandEvent(BattleManager battleController, EnemyPartySlot enemyPartySlot) {
        this.enemyPartySlot = enemyPartySlot;

        if ( this.enemyPartySlot.GetCombatTileController() == null) {
            this.isDone = true;
            Debug.LogError("Not currently inhabiting a tile");
            return;
        }

        this.battleController = battleController;
        this.enemyInfo = (EnemyInfo) this.enemyPartySlot.BattleEntity;

        this.enmityTargets = new List<PartySlot>();
        foreach(EnmityTarget enmityTarget in this.enemyPartySlot.enmityList) {
            this.enmityTargets.Add(enmityTarget.partySlot);
        }

        this.random = new System.Random();
        this.startingPosition = this.enemyPartySlot.GetCombatTileController();
        this.battleController.DestinationTile =  this.startingPosition;

        battleController.BattleFieldController.MovementGrid =  BattleProcessingStatic.GetPossibleMoves(this.enemyPartySlot.GetCombatTileController(), this.enemyPartySlot, this.battleController.Grid);        

        this.DetermineBattleCommand();
    }

    private void DetermineBattleCommand() {
        try {
            if(this.enemyPartySlot.enmityList[0].isAggro) {
                this.GetAggroAIList();
            } else {
                this.GetAIList();
            }
        } finally {
            this.enemyPartySlot.BattleEntityGO.ToggleEffect(AnimationEffect.Thinking, true);
            battleController.StartCoroutine(this.GetDefaultAI());
        }
    }

    private void GetAggroAIList() {
        this.enemyAIs.Add(this.enemyInfo.basicAttackAI);
        this.GetExtraAggroAIList();
        return;
    }

    protected abstract void GetExtraAggroAIList();

    protected abstract void GetAIList();


    private IEnumerator GetDefaultAI() {
        while (this.enemyPartySlot.GetBattleMember()!.BattleCommand == null) {
            this.DefaultAILogic();
            yield return new WaitForSeconds(0.5f);
        }

        this.enemyPartySlot.BattleEntityGO.ToggleEffect(AnimationEffect.Thinking, false);
        this.DoEnemyMovementAndFinish();
    }

    private void DefaultAILogic() {
        Debug.Log("Determining which AI to use");
        if (this.enemyAIs.Count > 0) {
            this.GetHighestValueAI(this.battleController);
            return;
        }

        this.ExtraAILogic();
    }

    protected virtual void ExtraAILogic() {
        this.SelectNoActionAI(battleController, enemyInfo, random, enmityTargets);
        return;
    }

    #nullable enable
    protected void GetHighestValueAI(BattleManager battleController) {
        try {
            if (!this.enemyAIs[0].CanBeActivated(this.enemyPartySlot, enmityTargets, battleController.EnemyPartyController.PartySlots, battleController.TurnCount, battleController.Grid)) {
                return;
            }

            this.readiedAbility = this.GetReadiedAbilityFromEnemyAI(this.enemyAIs[0], enmityTargets, random, battleController, startingPosition);

            if(this.readiedAbility == null) {
                return;
            }

            this.enemyPartySlot.GetBattleMember()!.BattleCommand = this.readiedAbility.GetEnemyAbilityAndMovement(battleController, startingPosition);

            return;
        } finally {
            this.enemyAIs.RemoveAt(0);
        }
    }

    private ReadiedAbility? GetReadiedAbilityFromEnemyAI(IEnemyAI enemyAI, List<PartySlot> attackableSlots, System.Random random, BattleManager battleController, CombatTileController startingPosition) {                
        if(enemyAI == null || !enemyAI.CanBeActivated(this.enemyPartySlot, attackableSlots, battleController.EnemyPartyController.PartySlots, battleController.TurnCount, battleController.Grid)) {
            return null;
        } 
        ReadiedAbility enemyReadiedAbility = enemyAI.DetermineActiveSkill(random, this.enemyPartySlot, attackableSlots, battleController.EnemyPartyController.PartySlots, battleController.Grid);
        return enemyReadiedAbility;
    }

    private void SelectNoActionAI(BattleManager battleController, EnemyInfo enemy, System.Random random, List<PartySlot> enmityTargets) {
        ReadiedAbility enemyReadiedAbility = enemy.noActionAI.DetermineActiveSkill(random, this.enemyPartySlot, enmityTargets, battleController.EnemyPartyController.PartySlots, battleController.Grid);

        List<CombatTileController> tiles = new List<CombatTileController>();
        tiles.Add(this.enemyPartySlot.GetCombatTileController());

        // this.enemyPartySlot.GetBattleMember()!.command.SetCommand(CommandType.Action, enemyReadiedAbility.activeSkill, null, tiles);
        this.enemyPartySlot.GetBattleMember()!.BattleCommand = new SkillCommand(enemyReadiedAbility.activeSkill, this.enemyPartySlot, BattleProcessingStatic.PartySlotIsEnemyPartySlot, this.enemyPartySlot.GetCombatTileController(), tiles, new());

    }

    //Move to the destination tile
    protected void DoEnemyMovementAndFinish() {
        if (battleController == null) {
            return;
        }

        Debug.Log("Starting enemy movement");
        this.enemyPartySlot.GetCombatTileController().RemoveOccupantAndProcessEvent(battleController);

        battleController.StartCoroutine(BattleProcessingStatic.PerformMovement(
            battleController, 
            this.enemyPartySlot, 
            this.battleController.DestinationTile,
            () => {
                this.FinishMovement(battleController);
                // callBack();
                this.isDone = true;
            }));
    }

    private void FinishMovement(BattleManager battleController) {
        foreach(KeyValuePair<Vector3, CombatTileController> keyValuePair in battleController.BattleFieldController.MovementGrid) {
            keyValuePair.Value.DisableMovementTile(true);
        }
        battleController.DestinationTile!.SetPartyOccupant(this.battleController, this.enemyPartySlot);
        battleController.BattleFieldController.ClearMovementGrid();
        battleController.BattleFieldController.ClearActionGrid();
    }

    public void Execute()
    {
        return;
    }

    public void End() {
        this.battleController.StateManager.SwitchState(this.battleController.StateManager.DeclareActionPhaseState, battleController);
        return;
    }

    public bool IsDone()
    {
        return this.isDone;
    }
}
}