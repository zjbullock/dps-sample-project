using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DPS.Common;

namespace DPS.TacticalCombat{
[Serializable]
#nullable enable
public class ReadiedAbility {
    public ReadiedAbility(IBattleActionCommand activeSkill, List<PartySlot> primaryAbilityTargets, List<PartySlot> healTargets, List<PartySlot> secondaryAbilityTargets) {
        this.activeSkill = activeSkill;
        this.primaryAbilityTargets = primaryAbilityTargets;
        this.healTargets = healTargets;
        this.secondaryAbilityTargets = secondaryAbilityTargets;
    }

    public IBattleActionCommand activeSkill;

    [SerializeField, SerializeReference]
    public List<PartySlot> primaryAbilityTargets;

    [SerializeField, SerializeReference]
    public List<PartySlot> healTargets;

    [SerializeField, SerializeReference]
    public List<PartySlot> secondaryAbilityTargets;

    private BattleCommand? GetEnemyAbilityAtStartingTile(BattleManager battleController, CombatTileController startingPosition) {
        //Do check here for range.
        List<CombatTileController> primaryTargets = new List<CombatTileController>();

        if(!battleController.BattleFieldController.MovementGrid.ContainsKey(battleController.CurrentlyActingMember.GetCombatTileController().Position)) {
            battleController.BattleFieldController.MovementGrid.Add(battleController.CurrentlyActingMember.GetCombatTileController().Position, battleController.CurrentlyActingMember.GetCombatTileController());
        }

        bool shouldDistanceCheck = this.activeSkill.ShouldDistanceCheck();

        battleController.BattleFieldController.ActionGrid = this.activeSkill.GetActionTilesByAreaOfEffect(startingPosition, battleController.CurrentlyActingMember);

        int tileHitCount = 0;

        Action<int> setTileHitCount = (int newTileHitCount) => {
            tileHitCount = newTileHitCount;
        };

        bool startCanBeAdded = false;

        Action<bool> setStartCanBeAdded = (bool newStartCanBeAdded) => {
            startCanBeAdded = newStartCanBeAdded;
        };

        int startNewTileHitCount = 0;

        Action<int> setStartNewTileHitCount = (int newStartNewTileHitCount) => {
            startNewTileHitCount = newStartNewTileHitCount;
        };

        


        List<CombatTileController> startNewPrimaryTargets = new List<CombatTileController>();

        Action<List<CombatTileController>> setStartNewPrimaryTargets = (List<CombatTileController> newStartNewPrimaryTargets) => {
            startNewPrimaryTargets = newStartNewPrimaryTargets;
        };

        Action<CombatTileController> addStartNewPrimaryTarget = (CombatTileController newStartNewPrimaryTarget) => {
            startNewPrimaryTargets.Add(newStartNewPrimaryTarget);
        };

        CombatTileController? primaryTarget = startingPosition;
        Action<CombatTileController> setNewPrimaryTarget = (CombatTileController newStartNewPrimaryTarget) => {
            primaryTarget = newStartNewPrimaryTarget;
        };

        List<PartySlot> targetedSlots = new List<PartySlot>();
        Action<List<PartySlot>> setTargetedSlot = (List<PartySlot> partySlots) => {
            targetedSlots = new List<PartySlot>();
            foreach(PartySlot partySlot in partySlots) {
                targetedSlots.Add(partySlot);
            }
        };

        if (shouldDistanceCheck) {
            this.GetStartingActionableTilesDistanceCheck(startingPosition, tileHitCount, startNewTileHitCount, startCanBeAdded, setTileHitCount, setStartNewTileHitCount, setStartCanBeAdded, battleController, addStartNewPrimaryTarget, setStartNewPrimaryTargets, setTargetedSlot, setNewPrimaryTarget);
        } else {
            this.GetStartingActionableTiles(startingPosition, tileHitCount, startNewTileHitCount, startCanBeAdded, setTileHitCount, setStartNewTileHitCount, setStartCanBeAdded, battleController, addStartNewPrimaryTarget, setStartNewPrimaryTargets, targetedSlots, setTargetedSlot);
            primaryTarget = startingPosition;
        }
        // battleController.enemyActionGrid = startingPosition.GetPossibleActionTiles(this.activeSkill.abilityRange, this.activeSkill.heightRange, battleController.currentlyActingMember.battleObject.height, this.activeSkill.targetTypes, true);
        
        battleController.DestinationTile = startingPosition;

        if(!startCanBeAdded || startNewTileHitCount <= tileHitCount) {
            return null;
        }

        tileHitCount = startNewTileHitCount;
        primaryTargets = startNewPrimaryTargets;
        return new SkillCommand(this.activeSkill, battleController.CurrentlyActingMember, BattleProcessingStatic.PartySlotIsEnemyPartySlot, primaryTarget, primaryTargets, targetedSlots);
    }

    public BattleCommand? GetEnemyAbilityStructureTarget(BattleManager battleController, CombatTileController startingPosition) {
        //Do check here for range.
        Debug.Log("Starting to look into Get Enemy Ability Structure Target");
        List<CombatTileController> primaryTargets = new List<CombatTileController>();
        if(!battleController.BattleFieldController.MovementGrid.ContainsKey(battleController.CurrentlyActingMember.GetCombatTileController().Position)) {
            battleController.BattleFieldController.MovementGrid.Add(battleController.CurrentlyActingMember.GetCombatTileController().Position, battleController.CurrentlyActingMember.GetCombatTileController());
        }

        bool shouldDistanceCheck = this.activeSkill.ShouldDistanceCheck();

        battleController.BattleFieldController.ActionGrid = this.activeSkill.GetActionTilesByAreaOfEffect(startingPosition, battleController.CurrentlyActingMember);
        int tileHitCount = 0;

        Action<int> setTileHitCount = (int newTileHitCount) => {
            tileHitCount = newTileHitCount;
        };

        bool startCanBeAdded = false;

        Action<bool> setStartCanBeAdded = (bool newStartCanBeAdded) => {
            startCanBeAdded = newStartCanBeAdded;
        };

        int startNewTileHitCount = 0;

        Action<int> setStartNewTileHitCount = (int newStartNewTileHitCount) => {
            startNewTileHitCount = newStartNewTileHitCount;
        };

        


        List<CombatTileController> startNewPrimaryTargets = new List<CombatTileController>();
        Action<List<CombatTileController>> setStartNewPrimaryTargets = (List<CombatTileController> newStartNewPrimaryTargets) => {
            startNewPrimaryTargets = newStartNewPrimaryTargets;
        };

        Action<CombatTileController> addStartNewPrimaryTarget = (CombatTileController newStartNewPrimaryTarget) => {
            startNewPrimaryTargets.Add(newStartNewPrimaryTarget);
        };

        CombatTileController? primaryTarget = startingPosition;
        Action<CombatTileController> setNewPrimaryTarget = (CombatTileController newStartNewPrimaryTarget) => {
            primaryTarget = newStartNewPrimaryTarget;
        };

        List<PartySlot>? targetedSlots = new List<PartySlot>();
        Action<List<PartySlot>> setTargetedSlot = (List<PartySlot> partySlots) => {
            targetedSlots = new List<PartySlot>();
            foreach(PartySlot partySlot in partySlots) {
                targetedSlots.Add(partySlot);

            }
        };

        if (shouldDistanceCheck) {
            this.GetStartingActionableTilesDistanceCheck(startingPosition, tileHitCount, startNewTileHitCount, startCanBeAdded, setTileHitCount, setStartNewTileHitCount, setStartCanBeAdded, battleController, addStartNewPrimaryTarget, setStartNewPrimaryTargets, setTargetedSlot, setNewPrimaryTarget);
        } else {
            Debug.Log("Should attack structure tiles");
            this.GetAttackStructureTile(startingPosition, tileHitCount, startNewTileHitCount, startCanBeAdded, setTileHitCount, setStartNewTileHitCount, setStartCanBeAdded, battleController, addStartNewPrimaryTarget, setStartNewPrimaryTargets, setTargetedSlot);
            primaryTarget = startingPosition;
        }
        // battleController.enemyActionGrid = startingPosition.GetPossibleActionTiles(this.activeSkill.abilityRange, this.activeSkill.heightRange, battleController.currentlyActingMember.battleObject.height, this.activeSkill.targetTypes, true);
        
        if(startCanBeAdded && startNewTileHitCount > tileHitCount) {
            tileHitCount = startNewTileHitCount;
            primaryTargets = startNewPrimaryTargets;
            // battleController.currentlyActingMember.GetBattleMember()!.command.SetCommand(CommandType.Action, this.activeSkill, null, primaryTargets, targetedSlots: targetedSlots, targetedTile: primaryTarget);
            return new SkillCommand(this.activeSkill, battleController.CurrentlyActingMember, BattleProcessingStatic.PartySlotIsEnemyPartySlot, primaryTarget, primaryTargets, targetedSlots);
        }

        return null;
    }

    #nullable enable

    public BattleCommand? GetEnemyAbilityAndMovement(BattleManager battleController, CombatTileController startingPosition) {
        
        BattleCommand? battleCommand = this.GetEnemyAbilityAtStartingTile(battleController, startingPosition);
        bool shouldDistanceCheck = this.activeSkill.ShouldDistanceCheck();
        int tileHitCount = 0;
        List<CombatTileController> primaryTargets = new List<CombatTileController>();

        Action<int> setTileHitCount = (int newTileHitCount) => {
            tileHitCount = newTileHitCount;
        };
        
        CombatTileController targetedTile = startingPosition;

        foreach(KeyValuePair<Vector3, CombatTileController> movements in battleController.BattleFieldController.MovementGrid) {

            if (!this.distanceIsCloserThanStart(battleController.CurrentlyActingMember.GetCombatTileController().Position, movements.Key, this.primaryAbilityTargets[0].GetCombatTileController().Position)) {
                continue;
            }

            battleController.BattleFieldController.ActionGrid = this.activeSkill.GetActionTilesByAreaOfEffect(movements.Value, battleController.CurrentlyActingMember);
            // battleController.enemyActionGrid = movements.Value.GetPossibleActionTiles(this.activeSkill.abilityRange, this.activeSkill.heightRange, battleController.currentlyActingMember.battleObject.height, this.activeSkill.targetTypes, true);
            int newTileHitCount = 0;

            Action<int> setNewTileHitCount = (int tileHitCount) => {
                newTileHitCount = tileHitCount;
            };

            
            bool canBeAdded = false;
            
            Action<bool> setCanBeAdded = (bool newCanBeAdded) => {
                canBeAdded = newCanBeAdded;
            };
            

            List<CombatTileController> newPrimaryTargets = new List<CombatTileController>();

            Action<List<CombatTileController>> setNewPrimaryTargets = (List<CombatTileController> newPrimaryTargetList) => {
                newPrimaryTargets = newPrimaryTargetList;
            };

            Action<CombatTileController> addNewPrimaryTarget = (CombatTileController newPrimaryTarget) => {
                newPrimaryTargets.Add(newPrimaryTarget);
            };

            CombatTileController probableCenterTile = startingPosition;
            Action<CombatTileController> setNewPrimaryTarget = (CombatTileController newPrimaryTarget) => {
                probableCenterTile = newPrimaryTarget;
            };



            if (shouldDistanceCheck) {
                this.GetActionableTilesDistanceCheck(movements.Value, tileHitCount, newTileHitCount, canBeAdded, setTileHitCount, setNewTileHitCount, setCanBeAdded, battleController, addNewPrimaryTarget, setNewPrimaryTargets, setNewPrimaryTarget);

            } else {
                // this.GetActionableTiles(movements.Value, tileHitCount, newTileHitCount, canBeAdded, setTileHitCount, setNewTileHitCount, setCanBeAdded, battleController, addNewPrimaryTarget, setNewPrimaryTargets, targetedSlots, setTargetedSlot);
                this.GetActionableTiles(movements.Value, tileHitCount, newTileHitCount, canBeAdded, setTileHitCount, setNewTileHitCount, setCanBeAdded, battleController, addNewPrimaryTarget, setNewPrimaryTargets);
                // List<PartySlot> targetedSlots, Action<List<PartySlot>> setTargetedSlots
                targetedTile = movements.Value;
            }

            
            if(canBeAdded && newTileHitCount > tileHitCount && !movements.Value.HasPartyOccupant() && battleController.DestinationTile != null && this.distanceIsCloserThanOrEqualTo(battleController.DestinationTile!.Position, movements.Key, newPrimaryTargets[0].Position)) {
                tileHitCount = newTileHitCount;
                battleController.DestinationTile = movements.Value;
                targetedTile = probableCenterTile;
                primaryTargets = newPrimaryTargets;
            }

        }

        if (tileHitCount == 0) {
            return battleCommand;
        }

        return new SkillCommand(this.activeSkill, battleController.CurrentlyActingMember, BattleProcessingStatic.PartySlotIsEnemyPartySlot, targetedTile, primaryTargets, new());
    }

    private GenericDictionary<Vector3, CombatTileController> GetEnemyAbilityRangeGrid(ReadiedAbility enemyReadiedAbility, CombatTileController combatTileController, float height, System.Action<GenericDictionary<Vector3, CombatTileController>>? processActionTiles, bool isFlying) {
        return enemyReadiedAbility.activeSkill.GetSelectActionTilesByAbilityRange(combatTileController, height, processActionTiles, isFlying);
    }
    #nullable disable

    private bool distanceIsCloserThanStart(Vector3 currentCoordinates, Vector3 potentialCoordinates, Vector3 affectedMemberCoordinates) {
        float currentDistance = Vector3.Distance(currentCoordinates, affectedMemberCoordinates);
        float potentialDistance = Vector3.Distance(potentialCoordinates, affectedMemberCoordinates);
        return potentialDistance < currentDistance;
    }

    private bool distanceIsCloserThanOrEqualTo(Vector3 currentCoordinates, Vector3 potentialCoordinates, Vector3 affectedMemberCoordinates) {
        float currentDistance = Vector3.Distance(currentCoordinates, affectedMemberCoordinates);
        float potentialDistance = Vector3.Distance(potentialCoordinates, affectedMemberCoordinates);
        return potentialDistance <= currentDistance;
    }

    private void GetAttackStructureTile(CombatTileController startingPosition, int tileHitCount, int startNewTileHitCount, bool startCanBeAdded, Action<int> setTileHitCount,  Action<int> setStartNewTileHitCount, Action<bool> setStartCanBeAdded, BattleManager battleController, Action<CombatTileController> addNewPrimaryTarget, Action<List<CombatTileController>> setNewPrimaryTargets, Action<List<PartySlot>> setTargetedSlots) {
        switch(this.activeSkill.GetTargetType()) {
            case TargetTypes.Self:
                setTileHitCount(1);
                setStartCanBeAdded(true);
                break;
            case TargetTypes.Single_Party_Member:
                foreach(KeyValuePair<Vector3, CombatTileController> actions in battleController.BattleFieldController.ActionGrid) {
                    actions.Value.DisableActionTile();
                    if(actions.Value.HasPartyOccupant() && actions.Value.GetPartyOccupant().GetType() == typeof(EnemyPartySlot) && this.primaryAbilityTargets != null && this.primaryAbilityTargets[0] == actions.Value.GetPartyOccupant()) {
                        // startNewPrimaryTargets.Add(actions.Value);
                        addNewPrimaryTarget(actions.Value);
                        setStartNewTileHitCount(1);
                    }  
                }
                setStartCanBeAdded(true);
                break;
            case TargetTypes.All_Party_Members:
                foreach(KeyValuePair<Vector3, CombatTileController> actions in battleController.BattleFieldController.ActionGrid) {
                    actions.Value.DisableActionTile();
                    // startNewPrimaryTargets.Add(actions.Value);
                    addNewPrimaryTarget(actions.Value);

                    if(actions.Value.HasPartyOccupant() && actions.Value.GetPartyOccupant().GetType() == typeof(EnemyPartySlot) && this.primaryAbilityTargets != null) {
                        startNewTileHitCount++;
                        setStartNewTileHitCount(startNewTileHitCount);
                    }
                }
                setStartCanBeAdded(true);
                break;
            case TargetTypes.Single_Enemy:
                float distance = -1f;
                foreach(KeyValuePair<Vector3, CombatTileController> actions in battleController.BattleFieldController.ActionGrid) {
                    actions.Value.DisableActionTile();
                    if (actions.Value == startingPosition) {
                        //Skip this iteration, as it's the tile they're already on
                        continue;
                    }
                    List<PartySlot> tilePartySlots = actions.Value.GetPartyOccupants();
                    Vector3 actionableTile = actions.Key;
                    Vector3 currenttile = battleController.CurrentlyActingMember.GetCombatTileController().Position;
                    Vector3 targetCoordinates = battleController.CurrentlyActingMember.enmityList[0].partySlot.GetCombatTileController().Position;
                    bool structureIsInbetween = Vector3.Dot((targetCoordinates - currenttile).normalized, actions.Key.normalized) > 0;


                    float newDistance = Vector3.Distance(actions.Key, targetCoordinates);
                    if (distance == -1f) {
                        distance = newDistance;
                    }

                    if(structureIsInbetween && tilePartySlots.Count > 0 && tilePartySlots[0].GetType() == typeof(SpawnableObjectPartySlot)) {
                        if(newDistance <= distance && BattleProcessingStatic.TileIsTargetableActiveSkill(this.activeSkill, battleController.CurrentlyActingMember, tilePartySlots[0], actions.Value, battleController: battleController, currentEnemyTile: startingPosition)) {
                            // addNewPrimaryTarget(actions.Value);
                            setNewPrimaryTargets(new List<CombatTileController>(){actions.Value});
                            setStartNewTileHitCount(1);
                            setTargetedSlots(new List<PartySlot>(){tilePartySlots[0]});
                            distance = newDistance;
                        }
                    }      
                }
                setStartCanBeAdded(true);
                break;
            case TargetTypes.All_Enemies:
                foreach(KeyValuePair<Vector3, CombatTileController> actions in battleController.BattleFieldController.ActionGrid) {
                    actions.Value.DisableActionTile();
                    addNewPrimaryTarget(actions.Value);
                    // startNewPrimaryTargets.Add(actions.Value);
                    if(actions.Value.HasPartyOccupant() && 
                        actions.Value.GetPartyOccupant().GetType() != typeof(EnemyPartySlot) && 
                        !actions.Value.GetPartyOccupant().BattleEntity!.IsDead()) {
                        if (battleController.CurrentlyActingMember.enmityList[0].partySlot == actions.Value.GetPartyOccupant()) {
                            setStartCanBeAdded(true);
                        }
                        startNewTileHitCount++;
                        setStartNewTileHitCount(startNewTileHitCount);
                    }      
                }
                break;
        }
    }



    private void GetStartingActionableTiles(CombatTileController startingPosition, int tileHitCount, int startNewTileHitCount, bool startCanBeAdded, Action<int> setTileHitCount,  Action<int> setStartNewTileHitCount, Action<bool> setStartCanBeAdded, BattleManager battleController, Action<CombatTileController> addNewPrimaryTarget, Action<List<CombatTileController>> setNewPrimaryTargets, List<PartySlot> targetedSlots, Action<List<PartySlot>> setTargetedSlots) {
        switch(this.activeSkill.GetTargetType()) {
            case TargetTypes.Self:
                setTileHitCount(1);
                setStartCanBeAdded(true);
                break;
            case TargetTypes.Single_Party_Member:
                foreach(KeyValuePair<Vector3, CombatTileController> actions in battleController.BattleFieldController.ActionGrid) {
                    actions.Value.DisableActionTile();
                    if(actions.Value.HasPartyOccupant() && actions.Value.GetPartyOccupant().GetType() == typeof(EnemyPartySlot) && this.primaryAbilityTargets != null && this.primaryAbilityTargets[0] == actions.Value.GetPartyOccupant()) {
                        // startNewPrimaryTargets.Add(actions.Value);
                        addNewPrimaryTarget(actions.Value);
                        setStartNewTileHitCount(1);
                    }  
                }
                setStartCanBeAdded(true);
                break;
            case TargetTypes.All_Party_Members:
                foreach(KeyValuePair<Vector3, CombatTileController> actions in battleController.BattleFieldController.ActionGrid) {
                    actions.Value.DisableActionTile();
                    // startNewPrimaryTargets.Add(actions.Value);
                    addNewPrimaryTarget(actions.Value);

                    if(actions.Value.HasPartyOccupant() && actions.Value.GetPartyOccupant().GetType() == typeof(EnemyPartySlot) && this.primaryAbilityTargets != null) {
                        startNewTileHitCount++;
                        setStartNewTileHitCount(startNewTileHitCount);
                    }
                }
                setStartCanBeAdded(true);
                break;
            case TargetTypes.Single_Enemy:
                foreach(KeyValuePair<Vector3, CombatTileController> actions in battleController.BattleFieldController.ActionGrid) {
                    actions.Value.DisableActionTile();
                    List<PartySlot> tilePartySlots = actions.Value.GetPartyOccupants();

                    if(tilePartySlots.Count > 0 &&  tilePartySlots.Contains(battleController.CurrentlyActingMember.enmityList[0].partySlot)) {
                        // startNewPrimaryTargets.Add(actions.Value);
                        // foreach() {
                        //     if (BattleProcessingStatic.TileIsTargetableActiveSkill(battleController.currentlyActingMember, actions.Value, battleController: battleController)) {

                        //     }
                        // }
                        foreach(PartySlot partySlot in tilePartySlots) {
                            if(BattleProcessingStatic.TileIsTargetableActiveSkill(this.activeSkill, battleController.CurrentlyActingMember, partySlot, actions.Value, battleController: battleController, currentEnemyTile: startingPosition)) {
                                addNewPrimaryTarget(actions.Value);

                                setStartNewTileHitCount(1);
                                setTargetedSlots(new List<PartySlot>(){partySlot});
                                break;
                            }
                        }

                    }      
                }
                setStartCanBeAdded(true);
                break;
            case TargetTypes.All_Enemies:
                List<PartySlot> partySlots = new List<PartySlot>();
                partySlots.AddRange(targetedSlots);
                foreach(KeyValuePair<Vector3, CombatTileController> actions in battleController.BattleFieldController.ActionGrid) {
                    actions.Value.DisableActionTile();
                    addNewPrimaryTarget(actions.Value);
                    // startNewPrimaryTargets.Add(actions.Value);
                    if(actions.Value.GetPartyOccupants().Count > 0) {

                        foreach(PartySlot attackableSlot in actions.Value.GetPartyOccupants()) {
                            if(BattleProcessingStatic.PartySlotIsPlayerPartySlot(attackableSlot) && 
                                !attackableSlot.BattleEntity!.IsDead() &&
                                BattleProcessingStatic.TileIsTargetableActiveSkill(this.activeSkill, battleController.CurrentlyActingMember, attackableSlot, actions.Value, battleController: battleController, currentEnemyTile: startingPosition)) {
                                    if (battleController.CurrentlyActingMember.enmityList[0].partySlot == attackableSlot) {
                                        setStartCanBeAdded(true);
                                    }
                                    if(!partySlots.Contains(attackableSlot)){
                                        partySlots.Add(attackableSlot);
                                    }
                                    startNewTileHitCount++;
                                    setStartNewTileHitCount(startNewTileHitCount);
                            }

                        }    
                        // setTargetedSlots(partySlots);
                    }
  
                }
                break;
        }
    }

    private void GetStartingActionableTilesDistanceCheck(CombatTileController startingPosition, int tileHitCount, int startNewTileHitCount, bool startCanBeAdded, Action<int> setTileHitCount,  Action<int> setStartNewTileHitCount, Action<bool> setStartCanBeAdded, BattleManager battleController, Action<CombatTileController> addNewPrimaryTarget, Action<List<CombatTileController>> setNewPrimaryTargets, Action<List<PartySlot>> setTargetedSlots, Action<CombatTileController> setNewPrimaryTarget) {
        switch(this.activeSkill.GetTargetType()) {
            case TargetTypes.Self:
                setTileHitCount(1);
                setStartCanBeAdded(true);
                setNewPrimaryTarget(startingPosition);
                break;
            case TargetTypes.Single_Party_Member:
                foreach(KeyValuePair<Vector3, CombatTileController> actions in battleController.BattleFieldController.ActionGrid) {
                    actions.Value.DisableActionTile();
                    List<CombatTileController> newPrimaryTargets = new List<CombatTileController>();
                    int newTileHitCount = 0;

                    System.Action<GenericDictionary<Vector3, CombatTileController>> processTiles = (GenericDictionary<Vector3, CombatTileController> areaTargets) => {
                        foreach(KeyValuePair<Vector3, CombatTileController> areaTarget in areaTargets) {
                            if(areaTarget.Value.HasPartyOccupant() && areaTarget.Value.GetPartyOccupant().GetType() == typeof(EnemyPartySlot) && this.primaryAbilityTargets != null && this.primaryAbilityTargets[0] == areaTarget.Value.GetPartyOccupant()) {
                                newPrimaryTargets.Add(areaTarget.Value);
                                newTileHitCount = 1;
                            }           
                        }

                        if(newTileHitCount > startNewTileHitCount) {
                            setStartNewTileHitCount(newTileHitCount);
                            // startNewPrimaryTargets = newPrimaryTargets;
                            setNewPrimaryTargets(newPrimaryTargets);
                            setNewPrimaryTarget(actions.Value);
                        }
                    };

                    this.GetEnemyAbilityRangeGrid(this, actions.Value, battleController.CurrentlyActingMember.BattleEntityGO.height, processTiles, battleController.CurrentlyActingMember.BattleEntity!.CanFly());
                }
                setStartCanBeAdded(true);
                break;
            case TargetTypes.All_Party_Members:
                foreach(KeyValuePair<Vector3, CombatTileController> actions in battleController.BattleFieldController.ActionGrid) {
                    actions.Value.DisableActionTile();
                    List<CombatTileController> newPrimaryTargets = new List<CombatTileController>();
                    int newTileHitCount = 0;

                    System.Action<GenericDictionary<Vector3, CombatTileController>> processTiles = (GenericDictionary<Vector3, CombatTileController> areaTargets) => {
                        foreach(KeyValuePair<Vector3, CombatTileController> areaTarget in areaTargets) {
                            newPrimaryTargets.Add(areaTarget.Value);
                            if(areaTarget.Value.HasPartyOccupant() && areaTarget.Value.GetPartyOccupant().GetType() == typeof(EnemyPartySlot) && this.primaryAbilityTargets != null) {
                                newTileHitCount++;
                            }           
                        }   

                        if(newTileHitCount > startNewTileHitCount) {
                            setStartNewTileHitCount(newTileHitCount);
                            // startNewPrimaryTargets = newPrimaryTargets;
                            setNewPrimaryTargets(newPrimaryTargets);
                            setNewPrimaryTarget(actions.Value);
                        }
                    };
                    this.GetEnemyAbilityRangeGrid(this, actions.Value, battleController.CurrentlyActingMember.BattleEntityGO.height, processTiles, battleController.CurrentlyActingMember.BattleEntity!.CanFly());
                }
                setStartCanBeAdded(true);
                break;
            case TargetTypes.Single_Enemy:
                foreach(KeyValuePair<Vector3, CombatTileController> actions in battleController.BattleFieldController.ActionGrid) {
                    actions.Value.DisableActionTile();
                    List<CombatTileController> newPrimaryTargets = new List<CombatTileController>();
                    int newTileHitCount = 0;
                    System.Action<GenericDictionary<Vector3, CombatTileController>> processTiles = (GenericDictionary<Vector3, CombatTileController> areaTargets) => {
                        foreach(KeyValuePair<Vector3, CombatTileController> areaTarget in areaTargets) {
                            if(areaTarget.Value.GetPartyOccupant() == battleController.CurrentlyActingMember.enmityList[0].partySlot) {
                                newTileHitCount = 1;
                                newPrimaryTargets.Add(areaTarget.Value);
                            }           
                        }   

                        if(newTileHitCount > startNewTileHitCount) {
                            setStartNewTileHitCount(newTileHitCount);
                            // startNewPrimaryTargets = newPrimaryTargets;
                            setNewPrimaryTargets(newPrimaryTargets);
                            setNewPrimaryTarget(actions.Value);

                        }
                    };
                    this.GetEnemyAbilityRangeGrid(this, actions.Value, battleController.CurrentlyActingMember.BattleEntityGO.height, processTiles, battleController.CurrentlyActingMember.BattleEntity!.CanFly());  
                }
                setStartCanBeAdded(true);
                break;
            case TargetTypes.All_Enemies:
                List<PartySlot> partySlots = new List<PartySlot>();
                foreach(KeyValuePair<Vector3, CombatTileController> actions in battleController.BattleFieldController.ActionGrid) {
                    actions.Value.DisableActionTile();
                    bool areaStartCanBeAdded = false;
                    List<CombatTileController> newPrimaryTargets = new List<CombatTileController>();
                    int newTileHitCount = 0;
                    System.Action<GenericDictionary<Vector3, CombatTileController>> processTiles = (GenericDictionary<Vector3, CombatTileController> areaTargets) => {
                        List<PartySlot> newPartySlots = new List<PartySlot>();
                        foreach(KeyValuePair<Vector3, CombatTileController> areaTarget in areaTargets) {
                            newPrimaryTargets.Add(areaTarget.Value);
                            if(areaTarget.Value.HasPartyOccupant() && 
                                BattleProcessingStatic.PartySlotIsPlayerPartySlot(areaTarget.Value.GetPartyOccupant()) &&
                                !areaTarget.Value.GetPartyOccupant().BattleEntity!.IsDead()) {
                                if (battleController.CurrentlyActingMember.enmityList[0].partySlot == areaTarget.Value.GetPartyOccupant()) {
                                    areaStartCanBeAdded = true;
                                }
                                newPartySlots.Add(areaTarget.Value.GetPartyOccupant());
                                newTileHitCount++;
                            }           
                        }   

                        if(areaStartCanBeAdded && newTileHitCount > startNewTileHitCount && newPartySlots.Count > partySlots.Count) {
                            partySlots = newPartySlots;
                            setStartNewTileHitCount(newTileHitCount);
                            setStartCanBeAdded(true);
                            // startNewPrimaryTargets = newPrimaryTargets;
                            setNewPrimaryTargets(newPrimaryTargets);
                            setNewPrimaryTarget(actions.Value);
                        }
                    };
                    this.GetEnemyAbilityRangeGrid(this, actions.Value, battleController.CurrentlyActingMember.BattleEntityGO.height, processTiles, battleController.CurrentlyActingMember.BattleEntity!.CanFly()); 
                }
                break;
        }
    }


    private void GetActionableTiles(CombatTileController userPosition, int tileHitCount, int newTileHitCount, bool canBeAdded, Action<int> setTileHitCount,  Action<int> setNewTileHitCount, Action<bool> setCanBeAdded, BattleManager battleController, Action<CombatTileController> addNewPrimaryTarget, Action<List<CombatTileController>> setNewPrimaryTargets) {
        switch(this.activeSkill.GetTargetType()) {
                case TargetTypes.Self:
                    setTileHitCount(1);
                    setCanBeAdded(true);
                    break;
                case TargetTypes.Single_Party_Member:

                    foreach(KeyValuePair<Vector3, CombatTileController> actions in battleController.BattleFieldController.ActionGrid) {
                        actions.Value.DisableActionTile();
                        if(actions.Value.HasPartyOccupant() && actions.Value.GetPartyOccupant().GetType() == typeof(EnemyPartySlot) && this.primaryAbilityTargets != null && this.primaryAbilityTargets[0] == actions.Value.GetPartyOccupant()) {
                            addNewPrimaryTarget(actions.Value);
                            setNewTileHitCount(1);
                        }   

                    }
                    setCanBeAdded(true);
                    break;
                case TargetTypes.All_Party_Members:
                    foreach(KeyValuePair<Vector3, CombatTileController> actions in battleController.BattleFieldController.ActionGrid) {
                        actions.Value.DisableActionTile();
                        addNewPrimaryTarget(actions.Value);
                        if(actions.Value.HasPartyOccupant() && actions.Value.GetPartyOccupant().GetType() == typeof(EnemyPartySlot) && this.primaryAbilityTargets != null) {
                            newTileHitCount++;
                            setNewTileHitCount(newTileHitCount);
                        }

                    }
                    setCanBeAdded(true);
                    break;
                case TargetTypes.Single_Enemy:
                    foreach(KeyValuePair<Vector3, CombatTileController> actions in battleController.BattleFieldController.ActionGrid) {
                        actions.Value.DisableActionTile();
                         List<PartySlot> tilePartySlots = actions.Value.GetPartyOccupants();
                        if(tilePartySlots.Count > 0 && tilePartySlots.Contains(battleController.CurrentlyActingMember.enmityList[0].partySlot)) {
                            // startNewPrimaryTargets.Add(actions.Value);
                            // foreach() {
                            //     if (BattleProcessingStatic.TileIsTargetableActiveSkill(battleController.currentlyActingMember, actions.Value, battleController: battleController)) {

                            //     }
                            // }
                            foreach(PartySlot partySlot in tilePartySlots) {
                                Debug.Log("party Slot to attack: " + partySlot.BattleEntity.GetName());
                                if(BattleProcessingStatic.TileIsTargetableActiveSkill(this.activeSkill, battleController.CurrentlyActingMember, partySlot, actions.Value,battleController: battleController, currentEnemyTile: userPosition)) {
                                    setNewTileHitCount(1);
                                    addNewPrimaryTarget(actions.Value);
                                    // setTargetedSlots(new List<PartySlot>(){partySlot});
                                    break;
                                }
                            }

                        }     
                    }
                    setCanBeAdded(true);
                    break;
                case TargetTypes.All_Enemies:
                    List<PartySlot> partySlots = new List<PartySlot>();
                    // partySlots.AddRange(targetedSlots);
                    foreach(KeyValuePair<Vector3, CombatTileController> actions in battleController.BattleFieldController.ActionGrid) {
                        actions.Value.DisableActionTile();
                        addNewPrimaryTarget(actions.Value);
                        if(actions.Value.GetPartyOccupants().Count > 0) {

                            foreach(PartySlot attackableSlot in actions.Value.GetPartyOccupants()) {
                                if(BattleProcessingStatic.PartySlotIsPlayerPartySlot(attackableSlot) && 
                                    !attackableSlot.BattleEntity!.IsDead() &&
                                    BattleProcessingStatic.TileIsTargetableActiveSkill(this.activeSkill, battleController.CurrentlyActingMember, attackableSlot, actions.Value, battleController: battleController, currentEnemyTile: userPosition)) {                          
                                        if (battleController.CurrentlyActingMember.enmityList[0].partySlot == attackableSlot) {
                                            canBeAdded = true;
                                            setCanBeAdded(canBeAdded);
                                        }
                                        if(!partySlots.Contains(attackableSlot)){
                                            partySlots.Add(attackableSlot);
                                        }
                                        newTileHitCount++;
                                        setNewTileHitCount(newTileHitCount);
                                }

                            }    
                            // setTargetedSlots(partySlots);
                        }
                        // if(actions.Value.PartyOccupantExists() && 
                        //     actions.Value.GetPartyOccupant().GetType() != typeof(EnemyPartySlot) && 
                        //     !actions.Value.GetPartyOccupant().GetBattleEntity!.IsDead() &&
                        //     BattleProcessingStatic.TileIsTargetableActiveSkill(this.activeSkill, battleController.currentlyActingMember, actions.Value.GetPartyOccupant(), actions.Value, battleController: battleController, currentEnemyTile: userPosition)) {
                        //     if (battleController.currentlyActingMember.enmityList[0].partySlot == actions.Value.GetPartyOccupant()) {
                        //         canBeAdded = true;
                        //         setCanBeAdded(canBeAdded);
                        //     }
                        //     newTileHitCount++;
                        //     setNewTileHitCount(newTileHitCount);
                        // }
                    }
                    if (!canBeAdded) {
                        setNewTileHitCount(0);
                    }
                    break;
            }
    }

    private void GetActionableTilesDistanceCheck(CombatTileController userPosition, int tileHitCount, int newTileHitCount, bool canBeAdded, Action<int> setTileHitCount,  Action<int> setNewTileHitCount, Action<bool> setCanBeAdded, BattleManager battleController, Action<CombatTileController> addNewPrimaryTarget, Action<List<CombatTileController>> setNewPrimaryTargets, Action<CombatTileController> setNewPrimaryTarget) {
        switch(this.activeSkill.GetTargetType()) {
                case TargetTypes.Self:
                    setTileHitCount(1);
                    setCanBeAdded(true);
                    break;
                case TargetTypes.Single_Party_Member:
                    foreach(KeyValuePair<Vector3, CombatTileController> actions in battleController.BattleFieldController.ActionGrid) {
                        actions.Value.DisableActionTile();
                        List<CombatTileController> areaPrimaryTargets = new List<CombatTileController>();
                        int areaTargetcount = 0;
                        System.Action<GenericDictionary<Vector3, CombatTileController>> processTiles = (GenericDictionary<Vector3, CombatTileController> areaTargets) => {
                            foreach(KeyValuePair<Vector3, CombatTileController> areaTarget in areaTargets) {
                                if(areaTarget.Value.HasPartyOccupant() && areaTarget.Value.GetPartyOccupant().GetType() == typeof(EnemyPartySlot) && this.primaryAbilityTargets != null && this.primaryAbilityTargets[0] == areaTarget.Value.GetPartyOccupant()) {
                                        areaPrimaryTargets.Add(areaTarget.Value);
                                        areaTargetcount = 1;
                                }           
                            }
                            if(areaTargetcount > newTileHitCount) {
                                setNewTileHitCount(areaTargetcount);
                                setNewPrimaryTargets(areaPrimaryTargets);
                                setNewPrimaryTarget(actions.Value);
                            }
                        };
                        this.GetEnemyAbilityRangeGrid(this, actions.Value, battleController.CurrentlyActingMember.BattleEntityGO.height, processTiles, battleController.CurrentlyActingMember.BattleEntity!.CanFly());
                    }
                    setCanBeAdded(true);
                    break;
                case TargetTypes.All_Party_Members:
                    foreach(KeyValuePair<Vector3, CombatTileController> actions in battleController.BattleFieldController.ActionGrid) {
                        actions.Value.DisableActionTile();
                        List<CombatTileController> areaPrimaryTargets = new List<CombatTileController>();
                        int areaTargetcount = 0;
                        System.Action<GenericDictionary<Vector3, CombatTileController>> processTiles = (GenericDictionary<Vector3, CombatTileController> areaTargets) => {
                            foreach(KeyValuePair<Vector3, CombatTileController> areaTarget in areaTargets) {
                                areaPrimaryTargets.Add(actions.Value);

                                if(areaTarget.Value.HasPartyOccupant() && areaTarget.Value.GetPartyOccupant().GetType() == typeof(EnemyPartySlot) && this.primaryAbilityTargets != null) {
                                    areaTargetcount++;
                                }           
                            }   
                            if(areaTargetcount > newTileHitCount) {
                                setNewTileHitCount(areaTargetcount);
                                setNewPrimaryTargets(areaPrimaryTargets);
                                setNewPrimaryTarget(actions.Value);

                            }
                        };
                        this.GetEnemyAbilityRangeGrid(this, actions.Value, battleController.CurrentlyActingMember.BattleEntityGO.height, processTiles, battleController.CurrentlyActingMember.BattleEntity!.CanFly());
                    }
                    setCanBeAdded(true);
                    break;
                case TargetTypes.Single_Enemy:
                    foreach(KeyValuePair<Vector3, CombatTileController> actions in battleController.BattleFieldController.ActionGrid) {
                        actions.Value.DisableActionTile();
                        List<CombatTileController> areaPrimaryTargets = new List<CombatTileController>();
                        int areaTargetcount = 0;
                        System.Action<GenericDictionary<Vector3, CombatTileController>> processTiles = (GenericDictionary<Vector3, CombatTileController> areaTargets) => {
                            foreach(KeyValuePair<Vector3, CombatTileController> areaTarget in areaTargets) {
                                if(areaTarget.Value.GetPartyOccupant() == battleController.CurrentlyActingMember.enmityList[0].partySlot) {
                                    areaTargetcount = 1;
                                    areaPrimaryTargets.Add(areaTarget.Value);
                                }           
                            }  

                            if(areaTargetcount > newTileHitCount) {
                                setNewTileHitCount(areaTargetcount);
                                setNewPrimaryTargets(areaPrimaryTargets);
                                setNewPrimaryTarget(actions.Value);

                            } 
                        };
                        this.GetEnemyAbilityRangeGrid(this, actions.Value, battleController.CurrentlyActingMember.BattleEntityGO.height, processTiles, battleController.CurrentlyActingMember.BattleEntity!.CanFly());
                    }
                    setCanBeAdded(true);
                    break;
                case TargetTypes.All_Enemies:
                    List<PartySlot> partySlots = new List<PartySlot>();
                    foreach(KeyValuePair<Vector3, CombatTileController> actions in battleController.BattleFieldController.ActionGrid) {
                        actions.Value.DisableActionTile();
                        List<CombatTileController> areaPrimaryTargets = new List<CombatTileController>();
                        int areaTargetcount = 0;
                        bool areaTargetsCanBeAdded = false;
                        System.Action<GenericDictionary<Vector3, CombatTileController>> processTiles = (GenericDictionary<Vector3, CombatTileController> areaTargets) => {
                            List<PartySlot> newPartySlots = new List<PartySlot>();
                            foreach(KeyValuePair<Vector3, CombatTileController> areaTarget in areaTargets) {
                                areaPrimaryTargets.Add(areaTarget.Value);
                                if(areaTarget.Value.HasPartyOccupant() && 
                                    BattleProcessingStatic.PartySlotIsPlayerPartySlot(areaTarget.Value.GetPartyOccupant()) && 
                                    !areaTarget.Value.GetPartyOccupant().BattleEntity!.IsDead()) {
                                        //Does a check verifying that the highest aggro target is included in the attackable tiles
                                    if (battleController.CurrentlyActingMember.enmityList[0].partySlot == areaTarget.Value.GetPartyOccupant()) {
                                        areaTargetsCanBeAdded = true;
                                    }
                                    newPartySlots.Add(areaTarget.Value.GetPartyOccupant());
                                    areaTargetcount++;
                                }           
                            }   

                            if(areaTargetsCanBeAdded && areaTargetcount > newTileHitCount && newPartySlots.Count > partySlots.Count) {
                                partySlots = newPartySlots;
                                setNewTileHitCount(areaTargetcount);
                                setNewPrimaryTargets(areaPrimaryTargets);
                                canBeAdded = true;
                                setCanBeAdded(canBeAdded);
                                setNewPrimaryTarget(actions.Value);

                            } 
                        };
                        this.GetEnemyAbilityRangeGrid(this, actions.Value, battleController.CurrentlyActingMember.BattleEntityGO.height, processTiles, battleController.CurrentlyActingMember.BattleEntity!.CanFly());
                    }
                    if (!canBeAdded) {
                        setNewTileHitCount(0);
                    }
                    break;
            }
        return;
    }
}
}
