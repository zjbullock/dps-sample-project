using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DPS.Common;

namespace DPS.TacticalCombat {
[Serializable]
public class BattleMember {
    #nullable enable
    public BattleMember(BattleEntitySlot battleEntitySlot) {
        this.ResetState();
        this.characterSlot = new BattleEntitySlot(battleEntitySlot);
        this.characterSlot.GenerateBattleRawStats();
        this.battleEntity = battleEntitySlot.GetFrontLineCharacter();
        this.battleCommand = null;
    }

    private void ResetState() {
        this.isDamaged = false;
        this.isUsingItem = false;
        this.isUsingAbility = false;
    }

    [SerializeField]
    private BattleEntitySlot characterSlot;


    #nullable disable

    [SerializeField, SerializeReference]
    protected IBattleEntity battleEntity;


    // [SerializeField, SerializeReference]
    // public int partyIndex;

    public bool isDamaged;

    public bool isUsingItem;

    public bool isUsingAbility;
    public BattleMemberType battleMemberType;

    #nullable enable
    [SerializeField, SerializeReference]
    private BattleCommand? battleCommand;

    public BattleCommand? BattleCommand { get => this.battleCommand; set => this.SetBattleCommand(value); }
    #nullable disable

    public GameObject targetingPosition;

    [Tooltip("Used to hold a ref to tiles that were acquired for a skill")]
    private GenericDictionary<Vector3, CombatTileController> tiles;

    public  GenericDictionary<Vector3, CombatTileController> Tiles { get => this.tiles; set => this.tiles = value; }
    public IBattleEntity GetBattleEntity { get => this.battleEntity; }


#nullable enable
    public void SwapPartyMembers()
    {
        IBattleEntity? currentFrontCharacterInfo = this.characterSlot.SwapPartners();
        if (currentFrontCharacterInfo == null)
        {
            return;
        }
        this.battleEntity = currentFrontCharacterInfo;

    }
    #nullable disable

    public void CombatEndStats()
    {
        if (this.characterSlot != null)
        {
            Debug.Log("Attempting to perform End Combat Stats for character slot");
            this.characterSlot.CombatEndStats();
        }
        return;
    }

    public bool LoseTurn() {
        return this.GetBattleEntity!.IsDowned() || 
                    (this.GetBattleEntity!.GetInflictedAilment() != null && 
                    this.GetBattleEntity!.GetInflictedAilment().LoseTurn());
    }

    public void EndPhase() {
        if (this.characterSlot != null) {
            this.characterSlot.GetFrontLineCharacter().EndPhase();
            IBattleEntity swapBattleEntity = this.characterSlot.GetSwapCharacter();
            if (swapBattleEntity != null) {
                swapBattleEntity.EndPhase(true);
            }
        }

        return;
    }

    #nullable enable
    // public Command? GetSwapSkill(BattleController battleController, CombatTileController combatTile, BattleObject battleObject, SwapSkillType swapSkillType) {
    //     Debug.Log("Attempting to get swap skill");
    //     SwapSkill swapSkill = this.battleEntity.GetSkillInfo().SwapSkill;
    //     if (swapSkill != null && swapSkill.CanActivateSwapSkill(swapSkillType)) {
    //         ActiveSkillSO activeSkill = swapSkill.activeSkill!;
    //         Debug.Log("Using swap skill");
    //         this.tiles = activeSkill.GetActionTilesByAreaOfEffect(combatTile, battleObject.height, this.battleEntity.CanFly());
    //         if (tiles != null && tiles.Count > 0) {
    //             List<CombatTileController> tileList = this.GetTilesAsList(this.tiles);
    //             swapSkill.SetSwapSkillCoolDown();
    //             Command swapCommand = new Command();
    //             swapCommand.SetCommand(CommandType.Action, activeSkill, null, tileList);
    //             return swapCommand;
    //         }
    //     }
    //     return null;
    // }

    public string? GetCommandMessage() {
        if (this.BattleCommand != null) {
            return this.BattleCommand.GetMessage(this);
        }
        return null;
    }

    #nullable disable

    // public void GetPartnerSwapSkill(BattleController battleController, CombatTileController combatTile, BattleObject battleObject, SwapSkillType swapSkillType) {
    //     Debug.Log("Attempting to get swap skill");
    //     if (this.characterSlot != null && this.characterSlot.GetSwapCharacter() != null) {
    //         SwapSkill swapSkill = this.characterSlot.GetSwapCharacter().GetSkillInfo().SwapSkill;
    //         if (swapSkill != null && swapSkill.CanActivateSwapSkill(swapSkillType)) {
    //             ActiveSkillSO activeSkill = swapSkill.activeSkill;
    //             Debug.Log("Using swap skill");
    //             this.tiles = activeSkill.GetActionTilesByAreaOfEffect(combatTile, battleObject.height, this.battleEntity.CanFly());
    //             if (tiles != null && tiles.Count > 0) {
    //                 List<CombatTileController> tileList = this.ClearTiles();
    //                 swapSkill.SetSwapSkillCoolDown();
    //                 this.command.SetCommand(CommandType.Action, activeSkill, null, tileList);
    //             }
    //         }
    //     }
    // }

    #nullable enable
    public BattleEntitySlot? GetCharacterSlot() {
        return this.characterSlot;
    }

    private void SetBattleCommand(BattleCommand? battleCommand) {
        if (battleCommand == null) {
            this.ClearCommand();
            return;
        }
        this.battleCommand = battleCommand;
    }

    private void ClearCommand()
    {
        if (this.battleCommand == null || !this.battleCommand.CanBeCleared(this))
        {
            return;
        }
        
        // Debug.Log("Clearing Command: " + this.battleCommand!.CommandName);
        if (this.BattleCommand!.Tiles != null) {
            foreach(CombatTileController combatTileController in this.BattleCommand!.Tiles) {
                combatTileController.RemoveProjectedTileBattleCommand(this);
            }
        }

        this.ClearTiles();
        this.battleCommand = null;
    }

    #nullable disable

    public List<CombatTileController> ClearTiles() {
        if (this.tiles == null) {
            return new List<CombatTileController>();
        }
        List<CombatTileController> tileList = this.tiles.ToValueList();
        if(this.tiles != null && this.tiles.Count > 0) {
            tileList.ForEach((tile) => tile.DisableActionTile());
        }
        return tileList;
    }
}
}

