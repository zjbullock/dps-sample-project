using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat{
[CreateAssetMenu(fileName = "Battle_Tile_Interaction_", menuName = "ScriptableObjects/Tile Interactions/Default")]

public class CombatTileInteractionSO: ScriptableObject  {

    [Header("Interaction Time Duration")]
    [Tooltip("Used by Combat Tile Controller to determine if this state will persist with no time limit")]
    public bool infiniteDuration = false;

    [Tooltip("Used by Combat Tile Controller State to control how long this specific interaction on the tile lasts")]
    [Range(0, 999)]
    public int duration;

    [Tooltip("Determines if this interaction can be triggered more than once")]
    public bool canBeReTriggered = false;

    [Tooltip("Game Object to spawn for the tile interaction")]
    public GameObject tileInteractionObject;

    [Tooltip("Game Object to spawn for objects within tiles")]
    public GameObject spawnableObject;

    [Tooltip("Sets the elemental trigger for the reaction")]
    public ElementSO ElementSO;

    [Tooltip("Map of potential combat tile state changes based on elemental interactions")]
    public GenericDictionary<ElementSO, CombatTileInteractionSO> combatTileStateChange;


    [Tooltip("List of potential combat tile conductions based on element")]
    public List<ElementSO> combatTileConductionList; 

    [Tooltip("List of potential combat tile interaction spreads based on element")]
    public List<ElementSO> combatTileSpreadList; 

    [Tooltip("List of Elements that end the combat tile interaction state")]
    public List<ElementSO> combatTileEndState; 

    [Tooltip("The movement cost of the tile")]
    [Range(1, 10)]
    public int movementCost = 1;

    [Tooltip("Denotes whether the spawned object can be walked upon or not")]
    [SerializeField]
    public bool walkable = true;   

    [Tooltip("Denotes whether this spawned object can be walked through")]
    public bool isSolid = false;

    [Tooltip("List of all possible combat tile interactions")]
    [SerializeField]
    private List<CombatTileInteractionHookSO> combatTileInteractions;



    public bool ElementInteractionPossible(ElementSO ElementSO) {
        if (combatTileStateChange != null && combatTileStateChange.Count > 0 && combatTileStateChange.ContainsKey(ElementSO)) {
            return true;
        } else if (combatTileConductionList != null && combatTileConductionList.Count > 0 && combatTileConductionList.Contains(ElementSO)) {
            return true;
        } else if (combatTileEndState != null && combatTileEndState.Count > 0 && combatTileEndState.Contains(ElementSO)) {
            return true;
        }
        return false;
    }

    public virtual void OnBattleAction(BattleManager battleManager, PartySlot user, BattleActionEventSO battleActionEventSO, IBattleActionCommand battleAction, CombatTileController combatTile, GenericDictionary<Vector3, CombatTileController> grid, List<CombatTileController> alreadyAffected, List<PartySlot> affectedTargets) {
        if (battleAction == null) {
            return;
        }


        if (combatTileEndState != null && combatTileEndState.Count > 0 && combatTileEndState.Contains(battleAction.GetElement())) {
            combatTile.ResetToDefaultState();
            return;
        }

        if (combatTileConductionList.Contains(battleAction.GetElement()) || combatTileSpreadList.Contains(battleAction.GetElement()))
        {
            #nullable enable
            List<CombatTileController> affectedGrid = this.ProcessSkillForTiles(user, battleAction, combatTile, grid, new List<CombatTileController>());
            Debug.Log($"Tile List: {affectedGrid.Count}");
            List<PartySlot> affectedPartyMembers = new();
            PartySlot? tileOccupant = combatTile.GetPartyOccupant();
            foreach (CombatTileController tile in affectedGrid)
            {
                if (!alreadyAffected.Contains(tile))
                {
                    alreadyAffected.Add(tile);
                }


                if (tile.currentstate.combatTileInteractionSO == null)
                {
                    continue;
                }

                if (
                    tileOccupant != null &&
                    !affectedTargets.Contains(tileOccupant) &&
                    !affectedPartyMembers.Contains(tileOccupant) &&
                    tile.currentstate.combatTileInteractionSO.combatTileConductionList.Contains(battleAction.GetElement()) &&
                    !tileOccupant.BattleEntity!.CanFly())
                {
                    affectedPartyMembers.Add(tileOccupant);
                }

                if (tile.currentstate.combatTileInteractionSO.combatTileEndState.Contains(battleAction.GetElement()) &&

                        combatTileEndState != null &&
                        combatTileEndState.Count > 0 &&
                        combatTileEndState.Contains(battleAction.GetElement()))
                {

                    combatTile.ResetToDefaultState();
                }

                if (tile.currentstate.combatTileInteractionSO.combatTileStateChange.ContainsKey(battleAction.GetElement()))
                {
                    tile.currentstate.combatTileInteractionSO.combatTileStateChange[battleAction.GetElement()].SetNewTileState(battleManager, tile);
                }
            }

            this.ProcessDamageTransfer(battleManager, user, affectedPartyMembers, battleActionEventSO, battleAction);
            affectedTargets.AddRange(affectedPartyMembers);
            #nullable disable
        }
        else if (combatTile.currentstate.combatTileInteractionSO.combatTileStateChange.ContainsKey(battleAction.GetElement()))
        {
            combatTile.currentstate.combatTileInteractionSO.combatTileStateChange[battleAction.GetElement()].SetNewTileState(battleManager, combatTile);
        }
        
        return;
    }

    private void ProcessDamageTransfer(BattleManager battleManager, PartySlot user, List<PartySlot> affectedPartyMembers, BattleActionEventSO battleActionEventSO, IBattleActionCommand battleActionCommand)
    {
        #nullable enable
        if (affectedPartyMembers.Count == 0)
        {
            return;
        }
        Debug.Log("DOING PROCESS DAMAGE TRANSFER");

        if (battleActionEventSO is not IBattleDamageEvent battleActionEventDamageSO)
        {
            return;
        }
        Debug.Log("DO DAMAGE ENVIRONMENTAL ACTION DETECTED!");
        this.DoDamageEnvironmentalAction(battleManager, user, battleActionCommand, battleActionEventDamageSO, affectedPartyMembers);
        return;
        #nullable disable

    }

    // public virtual void OnItemUsed(BattleController battleController, PartySlot user, StaticDamageItemSO staticDamageItem, CombatTileController combatTile, GenericDictionary<Vector3, CombatTileController> grid, List<CombatTileController> alreadyAffected) {
    //     if (staticDamageItem == null) {
    //         return;
    //     }

    //     if(combatTileEndState != null && combatTileEndState.Count > 0 && combatTileEndState.Contains(staticDamageItem.element)) {
    //         combatTile.ResetToDefaultState();
    //         return;
    //     }

    //     if (combatTileConductionList.Contains(staticDamageItem.element) || combatTileSpreadList.Contains(staticDamageItem.element) ) {
    //         List<CombatTileController> affectedGrid = this.ProcessItemsForTiles(staticDamageItem, combatTile, grid, new List<CombatTileController>());
    //         foreach(CombatTileController tile in affectedGrid) {
    //             if (alreadyAffected.Contains(tile)) {
    //                 continue;
    //             }
    //             alreadyAffected.Add(tile);
    //             if (tile.currentstate.combatTileInteractionSO != null) {
    //                 if (tile.currentstate.combatTileInteractionSO.combatTileConductionList.Contains(staticDamageItem.element) && tile.HasPartyOccupant() && !tile.GetPartyOccupant().GetBattleEntity!.CanFly()) {
    //                     tile.currentstate.combatTileInteractionSO.DoEnvironmentalItemDamage(battleController, user, staticDamageItem, tile.GetPartyOccupant());
    //                 }

    //                 if (tile.currentstate.combatTileInteractionSO.combatTileEndState.Contains(staticDamageItem.element) && 
    //                         combatTileEndState != null && 
    //                         combatTileEndState.Count > 0 && 
    //                         combatTileEndState.Contains(staticDamageItem.element)) {

    //                     combatTile.ResetToDefaultState();
    //                 }

    //                 if (tile.currentstate.combatTileInteractionSO.combatTileStateChange.ContainsKey(staticDamageItem.element)) {
    //                     tile.currentstate.combatTileInteractionSO.combatTileStateChange[staticDamageItem.element].SetNewTileState(tile);
    //                 }
    //             }
    //         }
    //     } else if (combatTile.currentstate.combatTileInteractionSO.combatTileStateChange.ContainsKey(staticDamageItem.element)) {
    //         combatTile.currentstate.combatTileInteractionSO.combatTileStateChange[staticDamageItem.element].SetNewTileState(combatTile);
    //     }

    //     return;
    // }

    protected virtual void SetNewTileState(BattleManager battleManager, CombatTileController combatTile)
    {
        combatTile.SwitchState(new CombatTileControllerAbnormalState(battleManager, combatTile, this));
        // combatTile.tileState = combatTile.tileState != CombatTileStateEnums.Ablaze ? CombatTileStateEnums.Ablaze : combatTile.tileState;
    }

    public virtual void OnTileEntered(BattleManager battlecontroller, PartySlot partySlot, CombatTileController combatTile) {
        foreach(CombatTileInteractionHookSO combatTileInteraction in this.combatTileInteractions) {
            combatTileInteraction.OnTileEntered(combatTile, battlecontroller, partySlot);
        }
        return;
    }

    public virtual void OnEntityTurnStart(BattleManager battlecontroller, PartySlot partySlot, CombatTileController combatTile) {
        return;
    }

    public virtual void OnEnteringState(CombatTileController combatTile, CombatTileControllerAbnormalState combatTileControllerAbnormalState) {
        if (this.tileInteractionObject != null) {
            combatTileControllerAbnormalState.tileEffect = (GameObject) Instantiate(this.tileInteractionObject);
        }

        foreach(CombatTileInteractionHookSO combatTileInteraction in this.combatTileInteractions) {
            combatTileInteraction.OnTileStateStart(combatTile);
        }
        return;
    }

    public virtual void OnExitingState(CombatTileController combatTile, CombatTileControllerAbnormalState combatTileControllerAbnormalState) {
        combatTileControllerAbnormalState.tileEffect = null;
        Debug.Log("Exiting Abnormal state");
        foreach (CombatTileInteractionHookSO combatTileInteraction in this.combatTileInteractions)
        {
            combatTileInteraction.OnTileStateEnd(combatTile);
        }
        return;
    }

    public virtual void OnTileExited(BattleManager battlecontroller, PartySlot partySlot, CombatTileController combatTile) {
        foreach(CombatTileInteractionHookSO combatTileInteraction in this.combatTileInteractions) {
            combatTileInteraction.OnTileExited(combatTile, battlecontroller, partySlot);
        }
        return;
    }

    public virtual bool CanBeSkillTriggered(BattleManager battlecontroller, IBattleActionCommand activeSkillSO, CombatTileController combatTile) {
        return false;
    }
    // public virtual bool CanBeItemTriggered(BattleManager battlecontroller, InventoryItemSO inventoryItemSO, CombatTileController combatTile) {
    //     return false;
    // }

    private List<CombatTileController> ProcessSkillForTiles(PartySlot user, IBattleActionCommand activeSkill, CombatTileController combatTileController, GenericDictionary<Vector3, CombatTileController> battleGrid, List<CombatTileController> tracker) {
        List<CombatTileController> processedTiles = new();
        processedTiles.AddRange(tracker);

        Vector3 position = combatTileController.Position;
        if (processedTiles.Contains(combatTileController)) {
            return processedTiles;
        } else {
            processedTiles.Add(combatTileController);
        }

        //First North
        Vector3 fNorthPosition = new(position.x, 0, position.z - 1);
        Vector3Int northPosition = Vector3Int.RoundToInt(fNorthPosition);
        if(battleGrid.ContainsKey(northPosition)) {
            CombatTileController northTile = battleGrid[northPosition];
            if (northTile != null && 
                combatTileController.GetFloatHeightWithSpawnedObject() == northTile.GetFloatHeightWithSpawnedObject() && 
                !processedTiles.Contains(northTile) && northTile.currentstate.combatTileInteractionSO != null && 
                combatTileController.currentstate.combatTileInteractionSO != null && 
                combatTileController.currentstate.combatTileInteractionSO ==  northTile.currentstate.combatTileInteractionSO) {
                List<CombatTileController> northTiles = northTile.currentstate.combatTileInteractionSO.ProcessSkillForTiles(user, activeSkill, northTile, battleGrid, processedTiles);
                foreach(CombatTileController newTile in northTiles) {
                    if (!processedTiles.Contains(newTile)) {
                        processedTiles.Add(newTile);
                    }
                }
            }
        }

        //Then South
        Vector3 fSouthPosition = new(position.x, 0, position.z + 1);
        Vector3Int southPosition = Vector3Int.FloorToInt(fSouthPosition);
        if(battleGrid.ContainsKey(southPosition)) {
            CombatTileController southTile = battleGrid[southPosition];

            if (southTile != null &&
                combatTileController.GetFloatHeightWithSpawnedObject() == southTile.GetFloatHeightWithSpawnedObject() && 
                southTile.currentstate.combatTileInteractionSO != null && 
                combatTileController.currentstate.combatTileInteractionSO != null && 
                combatTileController.currentstate.combatTileInteractionSO ==  southTile.currentstate.combatTileInteractionSO ) {
                List<CombatTileController> southTiles = southTile.currentstate.combatTileInteractionSO.ProcessSkillForTiles(user, activeSkill, southTile, battleGrid, processedTiles);
                foreach(CombatTileController newTile in southTiles) {
                    if (!processedTiles.Contains(newTile)) {
                        processedTiles.Add(newTile);
                    }
                }
            }
        }

        //Then West
        Vector3 fWestPosition = new Vector3(position.x - 1, position.y, position.z);
        Vector3Int westPosition = Vector3Int.RoundToInt(fWestPosition);
        if(battleGrid.ContainsKey(westPosition)) {
            CombatTileController westTile = battleGrid[westPosition];

            if (westTile != null && 
                combatTileController.GetFloatHeightWithSpawnedObject() == westTile.GetFloatHeightWithSpawnedObject() && 
                westTile.currentstate.combatTileInteractionSO != null && 
                combatTileController.currentstate.combatTileInteractionSO != null && 
                combatTileController.currentstate.combatTileInteractionSO ==  westTile.currentstate.combatTileInteractionSO) {
                List<CombatTileController> westTiles = westTile.currentstate.combatTileInteractionSO.ProcessSkillForTiles(user, activeSkill, westTile, battleGrid, processedTiles);
                foreach(CombatTileController newTile in westTiles) {
                    if (!processedTiles.Contains(newTile)) {
                        processedTiles.Add(newTile);
                    }
                }
            }
        }


        //Then East
        Vector3 fEastPosition = new Vector3(position.x + 1, position.y, position.z);
        Vector3Int eastPosition = Vector3Int.RoundToInt(fEastPosition);
        if(battleGrid.ContainsKey(eastPosition)) {
            CombatTileController eastTile = battleGrid[eastPosition];

            if (eastTile != null && 
                combatTileController.GetFloatHeightWithSpawnedObject() == eastTile.GetFloatHeightWithSpawnedObject() && 
                eastTile.currentstate.combatTileInteractionSO != null && 
                combatTileController.currentstate.combatTileInteractionSO != null && 
                combatTileController.currentstate.combatTileInteractionSO ==  eastTile.currentstate.combatTileInteractionSO) {
                List<CombatTileController> eastTiles = eastTile.currentstate.combatTileInteractionSO.ProcessSkillForTiles(user, activeSkill, eastTile, battleGrid, processedTiles);
                foreach(CombatTileController newTile in eastTiles) {
                    if (!processedTiles.Contains(newTile)) {
                        processedTiles.Add(newTile);
                    }
                }
                    
            }
        }

        return processedTiles;
    }

    //  private List<CombatTileController> ProcessItemsForTiles(StaticDamageItemSO damageItem, CombatTileController combatTileController, GenericDictionary<Vector3, CombatTileController> battleGrid, List<CombatTileController> tracker) {
    //     List<CombatTileController> processedTiles = new List<CombatTileController>();
    //     processedTiles.AddRange(tracker);

    //     Vector3 position = combatTileController.Position;
    //     if (processedTiles.Contains(combatTileController)) {
    //         return processedTiles;
    //     } else {
    //         processedTiles.Add(combatTileController);
    //     }

    //     //First North
    //     Vector3 fNorthPosition = new(position.x, 0, position.z - 1);
    //     Vector3Int northPosition = Vector3Int.RoundToInt(fNorthPosition);
    //     if(battleGrid.ContainsKey(northPosition)) {
    //         CombatTileController northTile = battleGrid[northPosition];
    //         if (northTile != null &&                 
    //             combatTileController.GetFloatHeightWithSpawnedObject() == northTile.GetFloatHeightWithSpawnedObject() && 
    //             northTile.currentstate.combatTileInteractionSO != null && 
    //             combatTileController.currentstate.combatTileInteractionSO != null && 
    //             combatTileController.currentstate.combatTileInteractionSO ==  northTile.currentstate.combatTileInteractionSO) {
    //             List<CombatTileController> northTiles = northTile.currentstate.combatTileInteractionSO.ProcessItemsForTiles( damageItem, northTile, battleGrid, processedTiles);
    //             foreach(CombatTileController newTile in northTiles) {
    //                 if (!processedTiles.Contains(newTile)) {
    //                     processedTiles.Add(newTile);
    //                 }
    //             }
    //         }
    //     }

    //     //Then South
    //     Vector3 fSouthPosition = new Vector3(position.x, position.y, position.z + 1);
    //     Vector3Int southPosition = Vector3Int.RoundToInt(fSouthPosition);
    //     if(battleGrid.ContainsKey(southPosition)) {
    //         CombatTileController southTile = battleGrid[southPosition];

    //         if (southTile != null && 
    //             combatTileController.GetFloatHeightWithSpawnedObject() == southTile.GetFloatHeightWithSpawnedObject() && 
    //             southTile.currentstate.combatTileInteractionSO != null && 
    //             combatTileController.currentstate.combatTileInteractionSO != null && 
    //             combatTileController.currentstate.combatTileInteractionSO ==  southTile.currentstate.combatTileInteractionSO) {
    //             List<CombatTileController> southTiles = southTile.currentstate.combatTileInteractionSO.ProcessItemsForTiles(damageItem, southTile, battleGrid, processedTiles);
    //             foreach(CombatTileController newTile in southTiles) {
    //                 if (!processedTiles.Contains(newTile)) {
    //                     processedTiles.Add(newTile);
    //                 }
    //             }
    //         }
    //     }

    //     //Then West
    //     Vector3 fWestPosition = new Vector3(position.x - 1, position.y, position.z);
    //     Vector3Int westPosition = Vector3Int.RoundToInt(fWestPosition);
    //     if(battleGrid.ContainsKey(westPosition)) {
    //         CombatTileController westTile = battleGrid[westPosition];

    //         if (westTile != null && 
    //             combatTileController.GetFloatHeightWithSpawnedObject() == westTile.GetFloatHeightWithSpawnedObject() && 
    //             westTile.currentstate.combatTileInteractionSO != null && 
    //             combatTileController.currentstate.combatTileInteractionSO != null && 
    //             combatTileController.currentstate.combatTileInteractionSO ==  westTile.currentstate.combatTileInteractionSO) {
    //             List<CombatTileController> westTiles = westTile.currentstate.combatTileInteractionSO.ProcessItemsForTiles(damageItem, westTile, battleGrid, processedTiles);
    //             foreach(CombatTileController newTile in westTiles) {
    //                 if (!processedTiles.Contains(newTile)) {
    //                     processedTiles.Add(newTile);
    //                 }
    //             }
    //         }
    //     }


    //     //Then East
    //     Vector3 fEastPosition = new Vector3(position.x + 1, position.y, position.z);
    //     Vector3Int eastPosition = Vector3Int.RoundToInt(fEastPosition);

    //     if(battleGrid.ContainsKey(eastPosition)) {
    //         CombatTileController eastTile = battleGrid[eastPosition];

    //         if (eastTile != null && 
    //             combatTileController.GetFloatHeightWithSpawnedObject() == eastTile.GetFloatHeightWithSpawnedObject() && 
    //             eastTile.currentstate.combatTileInteractionSO != null && 
    //             combatTileController.currentstate.combatTileInteractionSO != null && 
    //             combatTileController.currentstate.combatTileInteractionSO ==  eastTile.currentstate.combatTileInteractionSO) {
    //             List<CombatTileController> eastTiles = eastTile.currentstate.combatTileInteractionSO.ProcessItemsForTiles(damageItem, eastTile, battleGrid, processedTiles);
    //             foreach(CombatTileController newTile in eastTiles) {
    //                 if (!processedTiles.Contains(newTile)) {
    //                     processedTiles.Add(newTile);
    //                 }
    //             } 
    //         }
    //     }

    //     return processedTiles;
    // }

    public int GetMovementCost() {
        return this.movementCost;
    }

    private void DoDamageEnvironmentalAction(BattleManager battleController, PartySlot user, IBattleActionCommand battleAction, IBattleDamageEvent battleDamageEvent, List<PartySlot> targets)
    {
        // if (battleAction.GetTargetType() == TargetTypes.Single_Enemy || 
        //     battleAction.GetTargetType() == TargetTypes.All_Enemies || 
        //     battleAction.GetTargetType() == TargetTypes.All_Combatants || 
        //     battleAction.GetTargetType() == TargetTypes.All_Combatants_Except_Self) {
        // }
        battleDamageEvent.PerformDamage(battleController, battleAction, user, targets, new());
    }

    // private void DoEnvironmentalItemDamage(BattleManager battleController, PartySlot user, InventoryItemSO inventoryItem, PartySlot target) {
    //     StaticDamageItemSO staticDamageItem = inventoryItem as StaticDamageItemSO;
    //     if (staticDamageItem == null) {
    //         return;
    //     }
    //     staticDamageItem.ExecuteMultiTargetSkill(battleController, user, new() { target }, null);

    // }
}
}