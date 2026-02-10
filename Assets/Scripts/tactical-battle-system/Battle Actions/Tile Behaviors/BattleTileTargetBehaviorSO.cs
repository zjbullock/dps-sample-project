using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat{
[System.Serializable]
public abstract class BattleTileTargetBehaviorSO : ScriptableObject
{
    [Header("Component References")]
    [Tooltip("The sprite for the targeting behavior")]
    public Sprite targetingImage;

    [Header("Raycount Logic")]
    [Tooltip("Useful for determining the number of rays to shoot when calculating Action distance, as it's based on line of sight")]
    [Range(36, 360)]
    public int rayCount = 180;

    [Tooltip("Specified the number of raycasts necessary for a hit to be detected")]
    [Range(1, 20)]
    public int rayCastNecessary = 4;
    
    [Tooltip("Used to ignore the specified layers to avoid for sight calculations.\n  In general, the following should be unchecked:\n\n  Walls,\n Cut Out Wall,\n Physical UI,\n Glow FX\n\n  Everything else should be checked.")]
    public LayerMask ignore;

    [Header("Runtime Configurations")]

    [Tooltip("Denotes whether the ability requires line of sight or not")]
    public bool requireLineOfSight = true;

    [Tooltip("Denotes whether the ability comes from above")]
    public bool isOverheadAbility = false;



    // public void OnSubmitPressed(BattleMenuController battleMenuController, ActiveSkillSO activeSkill) {
    //     if (!this.CanBeDoneToTilePlayerActiveSkill(battleMenuController)) {
    //         battleMenuController.audioController.PlayAudio(SoundEffectEnums.Cancel);
    //         return;
    //     }
    //     this.SubmitPlayerActiveSkillAction(battleMenuController);
    // }

    // public void OnSubmitPressed(BattleMenuController battleMenuController, InventoryItemSO inventoryItem) {
    //     if (!this.CanBeDoneToTilePlayerItem(battleMenuController)) {
    //         battleMenuController.audioController.PlayAudio(SoundEffectEnums.Cancel);
    //         return;
    //     }
    //     this.SubmitPlayerItemAction(battleMenuController);
    // }

    protected virtual void SubmitPlayerActiveSkillAction(BattleManager battleManaer) {
        // this.FinishSubmittingAction(battleMenuController);
        return;
    }

    
    protected virtual void SubmitPlayerItemAction(BattleManager battleManager) {
        // this.FinishSubmittingAction(battleMenuController);
        return;
    }

    // private void FinishSubmittingAction(BattleMenuController battleMenuController) {
    //     battleMenuController.ClearActionOptions();
    //     battleMenuController.ClearCursorOptions();
    //     battleMenuController.tileTargetingObject.SetActive(false);
    //     battleMenuController.SwitchState(battleMenuController.defaultState);
    //     battleMenuController.ToActionPhase();
    // }

    public virtual void NavigateActiveSkillTargetingBehaviorForEnemy () {
        return;
    }

    public virtual void NavigateItemTargetBehaviorForEnemy() {
        return;
    }

    public virtual bool GetActionableTilesForEnemy() {
        return false;
    }

    // public virtual bool CanBeDoneToTilePlayerActiveSkill(BattleMenuController battleMenuController) {
    //     return true;
    // }
    
    #nullable enable
    public abstract bool CanBeDoneToTargetedTile(CombatTileController targetedTile, PartySlot caster, PartySlot? targetedEntity, System.Func<PartySlot, bool> isSameEntityCheck);
    #nullable disable

    // public virtual bool CanBeDoneToTilePlayerItem(BattleMenuController battleMenuController) {
    //     return true;
    // }



    public virtual GenericDictionary<Vector3, CombatTileController> GetActionTilesByAreaOfEffect(CombatTileController combatTile, int abilityRange, float abilityVerticalRange, float entityHeight, bool isFlying) {
        return new GenericDictionary<Vector3, CombatTileController>();
    }

    public virtual GenericDictionary<Vector3, CombatTileController> GetSelectActionTilesByAbilityRange(CombatTileController combatTile, int abilityRange, float abilityVerticalRange, float entityHeight, bool isFlying) {
        return new GenericDictionary<Vector3, CombatTileController>();
    }

    public virtual bool ShouldDistanceCheck() {
        return false;
    }


    public virtual bool AddPossibleTile( GenericDictionary<Vector3, CombatTileController> possibleTile, KeyValuePair<Vector3, CombatTileController> keyValuePair) {
        if(!possibleTile.ContainsKey(keyValuePair.Key)) {
            possibleTile.Add(keyValuePair.Key, keyValuePair.Value);
            return true;
        }
        return false;
    }

    // public virtual bool GetActionableTilesForPlayer(BattleMenuController battleMenuController) {        
        
    //     return false;
    
    // }
    public GenericDictionary<Vector3, CombatTileController> GetActionableTiles(BattleManager battleManager, CombatTileController combatTileController)
    {
        return battleManager.BattleActionQueueController.BattleActionCommand.GetActionTilesByAreaOfEffect(combatTileController, battleManager.CurrentlyActingMember);
    }
    
    // public void GetActionsByQueueTypeForPlayer(BattleMenuController battleState, Vector3 tileCoordinates, bool setCursorPosition, bool confirmAction) {
    //     if (battleState.queuedSkill != null && battleState.queuedSkill.GetActionName().Length > 0) {
    //         this.GetActiveSkillActionOptions(battleState, tileCoordinates, setCursorPosition, confirmAction);
    //     } else if (battleState.queuedItem != null && battleState.queuedItem.name.Length > 0) {
    //         this.GetItemActionOptions(battleState, tileCoordinates, setCursorPosition, confirmAction);
    //     }
    // }


    
    // public void GetActiveSkillActionOptions(BattleMenuController battleMenuController, Vector3 tileCoordinates, bool setCursorPosition, bool confirmAction) {
    //     float entityHeight = battleMenuController.currentPartyMember.BattleEntityGO.height;
    //     CombatTileController combatTileController = battleMenuController.battleController.Grid[tileCoordinates];

    //     if (confirmAction) {
    //         battleMenuController.confirmActionGridTargetCells = battleMenuController.queuedSkill.GetSelectActionTilesByAbilityRange(combatTileController, entityHeight, null, battleMenuController.currentPartyMember.BattleEntity!.CanFly());
    //     } else {
    //         battleMenuController.actionGridTargetCells = battleMenuController.queuedSkill.GetActionTilesByAreaOfEffect(combatTileController,battleMenuController.currentPartyMember);
    //     }
    //     // this.gridTargetCells = combatTileController.GetPossibleActionTiles(attackRange, currentPartyMember, grid);
        
    //     if (setCursorPosition) {
    //         battleMenuController.targetedTile = new Vector3 (tileCoordinates.x, tileCoordinates.y, tileCoordinates.z);
    //         // tileTargetingObject.transform.position = targetedTile + gridTargetCells[targetedTile].GetTileOffset();
    //         // tileTarget.SetCursorHeight(gridTargetCells[targetedTile].GetTileOffset());
    //         battleMenuController.combatTileCursor.SetCursorPosition(battleMenuController.actionGridTargetCells[battleMenuController.targetedTile].GetHeightForSpriteVector3Offset(false));
    //     }

    // }

    // public void GetItemActionOptions(BattleMenuController battleMenuController, Vector3 tileCoordinates, bool setCursorPosition, bool confirmAction) {
    //     float entityHeight = battleMenuController.currentPartyMember.BattleEntityGO.height;
    //     CombatTileController combatTileController = battleMenuController.battleController.Grid[tileCoordinates];

    //     if (confirmAction) {
    //         battleMenuController.confirmActionGridTargetCells = battleMenuController.queuedItem.GetSelectActionTilesByAbilityRange(combatTileController, entityHeight, null, battleMenuController.currentPartyMember.BattleEntity!.CanFly());
    //     } else {
    //         battleMenuController.actionGridTargetCells = battleMenuController.queuedItem.GetActionTilesByAreaOfEffect(combatTileController, battleMenuController.currentPartyMember);
    //     }
    //     // this.gridTargetCells = combatTileController.GetPossibleActionTiles(attackRange, currentPartyMember, grid);
        
    //     if (setCursorPosition) {
    //         battleMenuController.targetedTile = new Vector3 (tileCoordinates.x, tileCoordinates.y, tileCoordinates.z);
    //         // tileTargetingObject.transform.position = targetedTile + gridTargetCells[targetedTile].GetTileOffset();
    //         // tileTarget.SetCursorHeight(gridTargetCells[targetedTile].GetTileOffset());
    //         battleMenuController.combatTileCursor.SetCursorPosition(battleMenuController.actionGridTargetCells[battleMenuController.targetedTile].GetHeightForSpriteVector3Offset(false));
    //     }

    // }

    #nullable enable
    public string GetAbilityRangeText(int abilityArea) {
       return abilityArea + "m";
    }

    public virtual string GetAbilityAreaOfEffectText(int abilityRange) {
        return "0m";
    }
    #nullable disable

    public virtual bool RequireLineOfSight() {
        return this.requireLineOfSight;
    }
}
}

    // public enum AbilityPattern {
    //     Default,
    //     Cross,
    //     Diagonals,
    //     Line,
    //     Aoe,
    //     PerimeterAoe,
    //     MultipleSquares,
    //     Cone
    // }