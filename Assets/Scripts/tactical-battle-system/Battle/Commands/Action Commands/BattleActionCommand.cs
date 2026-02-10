using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat{
[System.Serializable]
public abstract class BattleActionCommand : BattleCommand
{
    [SerializeField]
    protected IBattleActionCommand action;

    [SerializeField, SerializeReference]
    protected TargetTypes targetTypes;

    public TargetTypes TargetTypes { get => this.targetTypes; }

    protected Func<PartySlot, bool> isSameTypeEntity;

    protected BattleActionCommand(
        IBattleActionCommand action,
        PartySlot caster,
        Func<PartySlot, bool> isSameTypeEntity,
        List<CombatTileController> tiles,
        List<PartySlot> targetedSlots,
        CombatTileController targetedTile) : base(action.GetActionName(), caster, targetedTile, tiles, targetedSlots)
    {
        this.action = action;
        this.targetTypes = action.GetTargetType();
        this.isSameTypeEntity = isSameTypeEntity;
    }

    public GenericDictionary<Vector3, CombatTileController> GetActionTilesByAreaOfEffect(CombatTileController combatTileController, PartySlot caster)
    {
        return action.GetActionTilesByAreaOfEffect(combatTileController, caster);
    }
#nullable enable

    public override void ExecuteCommand(BattleManager battleController, Action? callBack = null, IBattleCommand? commandOverride = null)
    {
        Debug.Log($"Executing Battle Action Command: {this.CommandName} by {this.caster}");
        Debug.Log($"Targeted Tile: {this.targetedTile}");
        this.caster.LookAt(new Vector3(targetedTile.Position.x, this.caster.BattleEntityGO.transform.position.y, targetedTile.Position.z));

        CommandTargets commandTargets = this.GetCommandExecution(battleController);
        base._wasCast = true;
        commandTargets.Print();
        battleController.BattleEventController.ReactingPartySlotHandler.AddReactingPartySlots(commandTargets.HitTargets.ToList(), this.action.GetElement());
        this.ExecuteAnimations(battleController, commandTargets, callBack);
        this.action?.PayCost(battleController, caster);
        caster.OnBattleStatsChange();
        Debug.Log("Clearing command");
        // caster.GetBattleMember().BattleCommand = null;
        return;
    }
#nullable disable

    public BattleActionAnimationController GetBattleActionAnimationController()
    {
        return this.action.GetBattleActionAnimationController();
    }

    public bool ShouldDistanceCheck()
    {
        return this.action.ShouldDistanceCheck();
    }

#nullable enable
    public override bool CanBeCleared(BattleMember? battleMember)
    {
        return (battleMember != null && battleMember.LoseTurn()) || base._wasCast;
    }

    protected CommandTargets GetCommandExecution(BattleManager battleController)
    {
        List<PartySlot> partySlots = new List<PartySlot>();
        List<CombatTileController> actionableTiles = new();
        if (this.tiles.Count > 0)
        {
            actionableTiles.AddRange(this.tiles);
        }

        System.Action? targetAnimationActionCallback = null;
        // System.Action? targetAnimationOnEndCallback = null;

        switch (action.GetTargetType())
        {
            case TargetTypes.Partner:
                targetAnimationActionCallback = () =>
                {
                    action.ExecutePartnerTargetSkill(battleController, caster, tiles);
                };
                break;
            case TargetTypes.Self:
                partySlots = new() { caster };
                break;
            case TargetTypes.Single_Party_Member:
                if (tiles![0].HasPartyOccupant() && !isSameTypeEntity(tiles[0].GetPartyOccupant()!))
                {
                    break;
                }
                partySlots.Add(tiles[0].GetPartyOccupant()!);
                break;
            case TargetTypes.All_Party_Members:
                foreach (CombatTileController tile in tiles!)
                {
                    if (tile.HasPartyOccupant() && isSameTypeEntity(tile.GetPartyOccupant()!))
                    {
                        partySlots.Add(tile.GetPartyOccupant()!);
                    }
                }
                break;
            case TargetTypes.Single_Enemy:
                Debug.Log("Actionable Tiles:" + tiles.Count);

                if (this.targetedSlots != null && this.targetedSlots.Count > 0)
                {
                    partySlots = new() { this.targetedSlots[0] };
                    break;
                }

                if (tiles![0].GetPartyOccupants().Count > 0 && !isSameTypeEntity(tiles[0].GetPartyOccupants()[0]))
                {
                    partySlots = new() { tiles[0].GetPartyOccupants()[0] };
                    break;
                }

                partySlots = new();
                break;
            case TargetTypes.All_Enemies:

                foreach (CombatTileController tile in tiles!)
                {
                    partySlots.AddRange(tile.GetPartyOccupants());
                }

                partySlots = partySlots.FindAll((partySlot) => !isSameTypeEntity(partySlot));
                break;
            case TargetTypes.All_Combatants:

                foreach (CombatTileController tile in tiles!)
                {
                    partySlots.AddRange(tile.GetPartyOccupants());
                }
                break;
            case TargetTypes.All_Combatants_Except_Self:
                foreach (CombatTileController tile in tiles!)
                {
                    partySlots.AddRange(tile.GetPartyOccupants());

                }

                partySlots.Remove(caster);
                break;
            case TargetTypes.Single_Any:

                if (tiles![0].GetPartyOccupants().Count == 0)
                {
                    partySlots = new();
                    break;
                }

                if (this.targetedSlots != null && this.targetedSlots.Count > 0)
                {
                    partySlots = new() { this.targetedSlots[0] };
                    break;
                }

                partySlots = new() { tiles[0].GetPartyOccupant()! };

                break;
            default:
                break;
        }

        // targetAnimationActionCallback = () =>
        // {
        //     // action.ExecuteSingleTargetSkill(battleController, caster, partySlots[0], tiles);
        //     action.ExecuteMultiTargetSkill(battleController, caster, partySlots, tiles);
        // };

        // targetAnimationOnEndCallback = () =>
        // {
        //     action.ExecuteEndAction(battleController, caster, partySlots, tiles);
        // };

        // return new CommandTargets(partySlots, actionableTiles, targetAnimationActionCallback, targetAnimationOnEndCallback);
        return new CommandTargets(partySlots, actionableTiles);
    }

    public override void ReCalculateCommandTiles(PartySlot? partySlot)
    {
        if (partySlot == null)
        {
            return;
        }

        if (this.action == null)
        {
            return;
        }

        CombatTileController? startingPosition = partySlot.GetCombatTileController();
        if (startingPosition == null)
        {
            return;
        }
        GenericDictionary<Vector3, CombatTileController> actionGrid = this.action.GetActionTilesByAreaOfEffect(startingPosition, partySlot);

        if (this.tiles.Count == 0)
        {
            return;
        }

        foreach (CombatTileController tile in this.tiles)
        {
            tile.RemoveProjectedTileBattleCommand(partySlot.GetBattleMember());
        }

        this.tiles = actionGrid.ToValueList();

        foreach (CombatTileController tile in this.tiles)
        {
            tile.AddProjectedTileBattleCommand(partySlot.GetBattleMember());
        }


        if (!this.action.ShouldDistanceCheck() || this.targetedTile == null)
        {
            return;
        }

        GenericDictionary<Vector3, CombatTileController> areaOfEffectGrid = this.action.GetSelectActionTilesByAbilityRange(this.targetedTile, partySlot.BattleEntityGO.height, null, partySlot.BattleEntity!.CanFly());


        foreach (CombatTileController tile in this.tiles)
        {
            tile.RemoveProjectedTileBattleCommand(partySlot.GetBattleMember());
        }

        this.tiles = areaOfEffectGrid.ToValueList();

        foreach (CombatTileController tile in this.tiles)
        {
            tile.AddProjectedTileBattleCommand(partySlot.GetBattleMember());
        }


    }

    private void ExecuteAnimations(BattleManager battleController, CommandTargets commandTargets, Action? callBack)
    {
        BattleActionAnimationController battleActionAnimationController = this!.GetBattleActionAnimationController();

        BattleEventProcessor battleEventProcessor = this.action.GetBattleEventProcessor(battleController, this.caster, commandTargets.HitTargets.ToList(), commandTargets.CombatTileControllers.ToList());
        List<PartySlot> affectedTargets = battleEventProcessor.AffectedTargets;

        System.Action onActionEndCallBack = () =>
        {

            Debug.Log($"{base.caster} On Action End CALLBACK");

            // caster.ResetCommand();
            // caster.ResetCommand();
            // foreach (CombatTileController tileController in commandTargets.CombatTileControllers)
            // {
            //     tileController.OnActionCommand(battleController, caster, this.action, battleController.Grid, commandTargets.CombatTileControllers.ToList());
            // }
            caster.PostCommandAbilities();

            battleEventProcessor.EndEvent();
            
            callBack?.Invoke();
        };

        System.Action onActionProgressCallBack = () =>
        {
            Debug.Log($"{base.caster} On Action Progress CALLBACK");
            BattleActionEventSO? battleActionEventSO = battleEventProcessor.ExecuteEvent();
            if (battleActionEventSO == null)
            {
                return;
            }
            
            foreach (CombatTileController tileController in commandTargets.CombatTileControllers)
            {
                tileController.OnActionCommand(battleController, caster, battleActionEventSO, this.action, battleController.Grid, commandTargets.CombatTileControllers.ToList(), affectedTargets );
            }
        };

        System.Action? onUserActionEndCallBack = null;
        System.Action? onUserActionProgressCallBack = null;
        System.Action? onTargetLocationEndCallBack = null;
        System.Action? onTargetLocationProgressCallBack = null;

        if (battleActionAnimationController.TargetLocationAnimation != null)
        {
            onTargetLocationEndCallBack = onActionEndCallBack;
            onTargetLocationProgressCallBack = onActionProgressCallBack;
            onUserActionEndCallBack = () =>
            {
                Debug.Log("--Spawning Handle Target Location Animation Event");
                battleController.BattleAnimationSpawner.HandleTargetLocationAnimationEvent(
                    battleController,
                    this.GetBattleActionAnimationController(),
                    caster,
                    this,
                    lookAtTargetPosition: caster.BattleEntityGO!.transform.position,
                    duringAnimationCallBack: onTargetLocationProgressCallBack,
                    onAnimationEndCallBack: onTargetLocationEndCallBack,
                    targetedTile: this.targetedTile,
                    targetAnimationList: commandTargets.HitTargets.ToList()
                );

            };
        }
        else
        {
            onUserActionEndCallBack = onActionEndCallBack;
            onUserActionProgressCallBack = onActionProgressCallBack;
        }

        // List<BattleActionInstruction> battleActionInstructions = this.action.GetActionInstructions();
        // foreach (BattleActionInstruction battleActionInstruction in battleActionInstructions)
        // {
        //     battleActionInstruction.Action = onActionProgressCallBack;
        // }
        // battleActionInstructions.Add(new(0.1f, onActionEndCallBack));


        // battleController.BattleAnimationSpawner.HandleAndProcessUserBattleCommandAnimations(battleController, this, caster, onAnimationEndAction: onUserActionEndCallBack, onAnimationProgressAction: onUserActionProgressCallBack);


        battleController.BattleAnimationSpawner.HandleAndProcessUserBattleCommandAnimations(battleController, this.GetBattleActionAnimationController(), this, caster, onAnimationEndAction: onUserActionEndCallBack, onAnimationProgressAction: onUserActionProgressCallBack);
        // battleController.BattleEventController.AddBattleEvent(new BattleActionInstructionExecutor(battleController, battleActionInstructions));


    }
}

public struct CommandTargets
{

    [SerializeField]
    private readonly List<PartySlot> hitTargets;

    public ReadOnlyCollection<PartySlot> HitTargets { get => this.hitTargets.AsReadOnly(); }

    [SerializeField]
    private readonly List<CombatTileController> combatTileControllers;

    public ReadOnlyCollection<CombatTileController> CombatTileControllers { get => this.combatTileControllers.AsReadOnly(); }

    // [SerializeField]
    // private readonly System.Action? _duringTargetAnimationEvent;

    // public System.Action? DuringTargetAnimationEvent { get => this._duringTargetAnimationEvent; }

    // [SerializeField]
    // private readonly System.Action? _onTargetAnimationEventEnd;

    // public System.Action? OnTargetAnimationEventEnd { get => this._onTargetAnimationEventEnd; }

    // public CommandTargets(List<PartySlot> hitTargets, List<CombatTileController> combatTileControllers, System.Action? duringActionCallBack, System.Action? onEndCallBack)
    // {
    //     this.hitTargets = hitTargets;
    //     this.combatTileControllers = combatTileControllers;
    //     this._duringTargetAnimationEvent = duringActionCallBack;
    //     this._onTargetAnimationEventEnd = onEndCallBack;
    // }

    public CommandTargets(List<PartySlot> hitTargets, List<CombatTileController> combatTileControllers)
    {
        this.hitTargets = hitTargets;
        this.combatTileControllers = combatTileControllers;
    }

    public void Print()
    {
        Debug.Log("Hit Targets: " + this.hitTargets.Count);
    }

}
}