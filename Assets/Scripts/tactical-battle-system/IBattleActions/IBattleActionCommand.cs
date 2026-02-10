using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat {
public interface IBattleActionCommand : IBattleActionDescription
{
#nullable enable
    string GetCostInfo();

    bool ConditionMet(IBattleEntity battleEntity);

    bool ShouldDistanceCheck();

    bool RequireLineOfSight();

    int GetCastTime(PartySlot partySlot);

    bool IsWithinVerticalRange(float heightDistanceToTarget);

    BattleActionAnimationController GetBattleActionAnimationController();

    bool CanBeDoneToTargetedTile(CombatTileController targetedTile, PartySlot caster, PartySlot? targetedEntity, System.Func<PartySlot, bool> isSameEntityCheck);
    
    ElementSO GetElement();

    TargetTypes GetTargetType();

    bool CanBeDoneOnTile(CombatTileController combatTileControllers);


    void ExecuteMultiTargetSkill(BattleManager battleController, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles);

    void ExecutePartnerTargetSkill(BattleManager battleController, PartySlot user, List<CombatTileController> combatTiles);

    void ExecuteEndAction(BattleManager battleManager, PartySlot user, List<PartySlot> targets, List<CombatTileController> combatTiles);

    // void ExecuteSingleTargetSkill(BattleManager battleController, PartySlot user, PartySlot? primaryAbilityTarget, List<CombatTileController> combatTiles);

    void PayCost(BattleManager battleController, PartySlot user);

    GenericDictionary<Vector3, CombatTileController> GetActionTilesByAreaOfEffect(CombatTileController combatTile, PartySlot partySlot, System.Action? processTiles = null);
    GenericDictionary<Vector3, CombatTileController> GetSelectActionTilesByAbilityRange(CombatTileController combatTile, float entityHeight, System.Action<GenericDictionary<Vector3, CombatTileController>>? processTiles, bool isFlying);

    string GetAbilityRangeText();

    string GetAbilityAreaText();

    ActionTypeEnums GetActionType();

    List<BattleActionEventSO> GetActionInstructions();

    BattleEventProcessor GetBattleEventProcessor(BattleManager battleController, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles);

    List<BattleActionEventSO> GetBattleActionEvents();

    StatusAilmentSO? GetStatusAilment();

    BattleCommand GenerateBattleCommand(PartySlot caster, System.Func<PartySlot, bool> isSameTypeEntity, CombatTileController targetedTile, List<CombatTileController> tiles, List<PartySlot> targetedSlots);

    BattleTileTargetBehaviorSO? GetBattleTileTargetBehavior();
#nullable disable
}

public interface IBattleStatusAilmentInflict
{
    void ProcessStatusAilment(PartySlot partySlot, IBattleActionCommand battleActionCommand);
}

public interface IBattleDamageEvent
{
    void PerformDamage(BattleManager battleManager, IBattleActionCommand battleActionCommand, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles);
}

[Serializable]
public class BattleEventProcessor
{

    [Header("Config")]
    [SerializeField]
    private BattleManager _battleManager;

    [SerializeField, SerializeReference]
    private PartySlot _user;

    [SerializeField, SerializeReference]
    private List<PartySlot> _affectedTargets;

    public List<PartySlot> AffectedTargets { get => this._affectedTargets.AsReadOnly().ToList(); }

    [SerializeField]
    private List<CombatTileController> _combatTiles;

    [SerializeField, SerializeReference]
    private IBattleActionCommand _battleActionCommand;


    [Header("Battle Events")]
    [SerializeField]
    private int _index;

    [SerializeField]
    private List<BattleActionEventSO> _battleActionEventSOs;

    private System.Action _onCompleteAction;

    public BattleEventProcessor(BattleManager battleManager, IBattleActionCommand battleActionCommand, PartySlot user, List<PartySlot> partySlots, List<CombatTileController> combatTiles)
    {
        this._index = 0;
        this._battleActionCommand = battleActionCommand;
        this._battleManager = battleManager;
        this._user = user;
        this._affectedTargets = new(partySlots);
        this._combatTiles = new(combatTiles);
        this._battleActionEventSOs = new(battleActionCommand.GetActionInstructions());
        this._onCompleteAction = () => battleActionCommand.ExecuteEndAction(this._battleManager, this._user, this._affectedTargets, this._combatTiles);
    }

    #nullable enable
    public BattleActionEventSO? ExecuteEvent()
    {
        if (this._index >= this._battleActionEventSOs.Count)
        {
            Debug.Log("Index reached end of battle action event list.  Ending Interactions");
            return null;
        }

        Debug.Log("EXECUTING BATTLE EVENT IN BATTLE EVENT PROCESSOR");
        BattleActionEventSO battleActionEvent = this._battleActionEventSOs[this._index];

        battleActionEvent.Execute(this._battleManager, this._battleActionCommand, this._user, this._affectedTargets, this._combatTiles);
        foreach (PartySlot partySlot in this._affectedTargets)
        {
            this.SpawnBattleAction(battleActionEvent, partySlot);
        }

        this._index++;

        return battleActionEvent;
    }
#nullable disable
    
    public void EndEvent()
    {
        this._onCompleteAction?.Invoke();
    }

    private void SpawnBattleAction(BattleActionEventSO battleActionEvent, PartySlot affectedSlot)
    {
        if (battleActionEvent.BattleActionController == null)
        {
            return;
        }

        this._battleManager.InstantiateBattleActionController(
            new BattleAnimationEventSpawn(
                animation: battleActionEvent.BattleActionController,
                caster: this._user.BattleEntityGO,
                parent: affectedSlot.BattleEntityGO.transform,
                transform: affectedSlot.BattleEntityGO.transform,
                lookAtPosition: this._user.BattleEntityGO!.transform.position,
                duringAnimationCallBack: null,
                onAnimationEndCallBack: null
            )
        );
    }
}

public interface IBattleActionDescription
{
    public string GetActionName();
    public string GetDescription();
}


// [System.Serializable]
// public class BattleActionInstructionExecutor : IBattleEvent
// {


//     [SerializeField]
//     List<BattleActionInstruction> _battleActionInstructions;

//     [SerializeField]
//     BattleManager _battleManger;

//     [SerializeField]
//     private bool _isDone;

//     public BattleActionInstructionExecutor(BattleManager battleManager, List<BattleActionInstruction> battleActionInstructions)
//     {
//         this._battleActionInstructions = battleActionInstructions;
//         this._battleManger = battleManager;
//         this._isDone = false;
//     }

//     public void Execute()
//     {
//         if (this._battleManger == null)
//         {
//             Debug.LogError("Battle Manager is NULL!");
//             return;
//         }

//         this._battleManger.StartCoroutine(ExecuteAction());
//     }

//     public IEnumerator ExecuteAction()
//     {
//         yield return null;
//         yield return null;
//         foreach (BattleActionInstruction battleActionInstruction in _battleActionInstructions)
//         {
//             yield return new WaitForSeconds(battleActionInstruction.WaitTime);
//             battleActionInstruction.Action?.Invoke();
//         }

//         this._isDone = true;
//     }

//     public bool IsDone()
//     {
//         return this._isDone;
//     }

//     public void End()
//     {
//         return;
//     }
// }

// [Serializable]
// public class BattleActionInstruction
// {
//     [SerializeField]
//     [Range(-10f, 10f)]
//     private float _waitTime;

//     public float WaitTime { get => this._waitTime; }

//     [SerializeField]
//     public System.Action _action;

//     public System.Action Action { get => this._action; set => this._action = value; }

//     public BattleActionInstruction(float waitTime, System.Action action)
//     {
//         this._waitTime = waitTime;
//         this._action = action;
//     }
// }
}