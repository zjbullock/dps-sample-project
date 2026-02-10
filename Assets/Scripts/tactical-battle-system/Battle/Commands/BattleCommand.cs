using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace DPS.TacticalCombat{

[System.Serializable]
public abstract class BattleCommand : IBattleCommand
{   
    [Header("Config")]
    [SerializeField, SerializeReference]
    protected string commandName;

    public string CommandName { get => this.commandName; }

    [SerializeField, SerializeReference]
    protected List<PartySlot> targetedSlots = new();

    public List<PartySlot> TargetedSlots { get => this.targetedSlots; }


    [SerializeField, SerializeReference]
    protected List<PartySlot> partyMembers;

    public List<PartySlot> PartyMembers { get => this.partyMembers; }

    [SerializeField, SerializeReference]
    protected List<PartySlot> enemyMembers;

    public List<PartySlot> EnemyMembers { get => this.enemyMembers; }

    [SerializeField, SerializeReference]
    protected List<CombatTileController> tiles;

    public List<CombatTileController> Tiles { get => this.tiles; }


    [SerializeField, SerializeReference]
    protected CombatTileController targetedTile;

    public CombatTileController TargetedTile { get => this.targetedTile; }
    

    [SerializeField, SerializeReference]
    protected PartySlot caster;

    public PartySlot Caster { get => this.caster; }


    [Header("Runtime Values")]
    
    protected bool _wasCast = false;

    public bool WasCast { get => this._wasCast; }

    #nullable enable
    public BattleCommand(
            string commandName,
            PartySlot caster,
            CombatTileController targetedTile,
            List<CombatTileController> tiles,
            List<PartySlot> targetedSlots
        )
    {
        this.commandName = commandName;
        this.caster = caster;
        this.tiles = tiles;
        this.targetedSlots = targetedSlots;
        this.targetedTile = targetedTile;
    }


    public virtual string GetMessage(BattleMember battleMember)
    {
        return battleMember.GetBattleEntity!.GetName() + " uses " + this.CommandName + "!";
    }

    public virtual bool CanBeCleared(BattleMember battleMember)
    {
        return true;
    }

    public abstract void ReCalculateCommandTiles(PartySlot partySlot);

    public abstract void ExecuteCommand(BattleManager battleController, System.Action? callBack = null, IBattleCommand? commandOverride = null);
    public abstract bool CanExecuteCommand();

    

}
}