using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat{
public static class BattleProcessingStatic
{
    public static string GetTargetType(TargetTypes targetTypes)
    {
        string targetType = targetTypes switch
        {
            TargetTypes.None => "None",
            TargetTypes.Self => "User",
            TargetTypes.Single_Party_Member => "Single Party Member",
            TargetTypes.All_Party_Members => "All Party Members",
            TargetTypes.Single_Enemy => "Single Enemy",
            TargetTypes.All_Enemies => "All Enemies",
            TargetTypes.All_Combatants => "All Combatants",
            TargetTypes.All_Combatants_Except_Self => "All Combatants Except User",
            TargetTypes.Single_Any => "Any Combatant",
            TargetTypes.Partner => "Swap Partner",
            _ => "",
        };
        return targetType;
    }

    public static int GetPercentDamage(int hp, float percent)
    {
        int damageOverTimeHP = (int)(hp * percent);
        return damageOverTimeHP;
    }

    public static bool PartySlotIsPlayerPartySlot(PartySlot partySlot)
    {
        if (partySlot == null)
        {
            return false;
        }

        return typeof(PlayerPartySlot).IsAssignableFrom(partySlot.GetType());
    }


    public static bool PartySlotIsEnemyPartySlot(PartySlot partySlot)
    {
        if (partySlot == null)
        {
            return false;
        }

        return typeof(EnemyPartySlot).IsAssignableFrom(partySlot.GetType());
    }


    public static bool PartySlotIsSpawnablePartySlot(PartySlot partySlot)
    {
        if (partySlot == null)
        {
            return false;
        }

        return typeof(SpawnableObjectPartySlot).IsAssignableFrom(partySlot.GetType());
    }

    public static bool CalculateEntityEvasion(BattleManager battleManager, PartySlot attacker, PartySlot defender, ActionTypeEnums attackType, System.Random random)
    {

        int evasion = defender.BattleEntity!.GetRawStats().evasion;
        if (defender.BattleEntity.CanFly() && attackType == ActionTypeEnums.Physical_Melee)
        {
            Debug.Log(string.Format("Evasion increased by {0} due to physical melee on flying enemy.", battleManager.BattleValues.FlyingEntityEvasionMultiplier * 100));
            evasion += (int)(evasion * battleManager.BattleValues.FlyingEntityEvasionMultiplier);
            Debug.Log("evasion: " + evasion);
        }

        bool evaded = EntityEvaded(random, attacker.BattleEntity!.GetRawStats().accuracy, evasion);
        if (evaded)
        {
            defender.OnEvade(battleManager);
        }
        return evaded;
    }

    public static bool CalculateEntityBlock(BattleManager battleManager, PartySlot attacker, PartySlot defender, System.Random random)
    {
        int blockChance = random.Next(0, 100);

        bool hit = blockChance < defender.BattleEntity!.GetRawStats().blockChance;

        if (hit)
        {
            defender.OnBlock(battleManager);
        }
        return hit;
    }

    #nullable enable
    public static DamageInfoDTO? GetEntityToEntityDamageInfo(BattleManager battleController, PartySlot attacker, RawStats attackerStats, PartySlot defender, System.Random random, ActionTypeEnums attackType, ElementSO ElementSO)
    {
        int damage;
        int defensiveStat;

        // DamageResult postDamage = new DamageResult()
        // {
        //     hit = false,
        //     postDamageStatus = PostActionEnums.None
        // };

        if (defender.BattleEntity!.IsDead())
        {
            return null;
        }

        if (attackType == ActionTypeEnums.Magical)
        {
            damage = attackerStats.magicalAttack;
            defensiveStat = defender.BattleEntity!.GetRawStats() != null ? defender.BattleEntity!.GetRawStats().magicalResistance : 0;
        }
        else
        {
            damage = attackerStats.physicalAttack;
            defensiveStat = defender.BattleEntity!.GetRawStats() != null ? defender.BattleEntity!.GetRawStats().defense : 0;
        }

        if (CalculateEntityEvasion(battleController, attacker, defender, attackType, random))
        {
            return null;
        }


        if (CalculateEntityBlock(battleController, attacker, defender, random))
        {
            return null;
        }

        //Always do at least 1 damage
        int finalDamage = ((damage - defensiveStat + (int)System.Math.Abs(damage - defensiveStat)) / 2) + 1;    

        bool isCritical = random.Next(0, 100) < attackerStats.criticalRate;

        if (isCritical)
        {
            finalDamage += (int)((float)finalDamage * (float)((float)attackerStats.criticalDamage / 100f));
        }

        if (defender.BattleEntity!.IsDowned())
        {
            finalDamage = (int)((float)finalDamage * 1.2f);
        }

        if (defender.BattleEntity!.IsDefending())
        {
            finalDamage /= 2;
        }

        // defender.ProcessDamage(battleController, finalDamage, ElementSO, additionalText: damageString);
        // OnTakingDamage(battleController, attacker, defender);
        // EntityDealingDamage(battleController, attacker);
        return new DamageInfoDTO(finalDamage, attacker, defender, isCritical, ElementSO);
        // primaryAbilityTarget.battleObject.poiseUI.SetDownedStatus(primaryAbilityTarget.GetBattleEntity!.GetDowned().downPoints);
    }
    #nullable disable



    public static void PerformDamageToEntity(BattleManager battleManager, DamageInfoDTO damageInfo)
    {
        if(damageInfo.IsCritical) {
            OnEntityCriticalHit(battleManager, damageInfo.Attacker);
        }

        damageInfo.Defender.ProcessDamage(battleManager, damageInfo.Damage, damageInfo.Element, additionalText: damageInfo.GetDamageString());
        OnTakingDamage(battleManager, damageInfo.Attacker, damageInfo.Defender);
        OnEntityDealingDamage(battleManager, damageInfo.Attacker);
    }

    public static void StaticDamageToEntity(BattleManager battleManager, int damage, PartySlot defender, ActionTypeEnums attackType, ElementSO ElementSO)
    {
        defender.ProcessDamage(battleManager, damage, ElementSO);
    }

    private static bool CanAddTile(CombatTileController endTile, CombatTileController startTile)
    {
        return endTile.movementTiles.Count == 0 || (endTile.movementTiles.Count > 0 && endTile.movementTiles.Count > startTile.movementTiles.Count + 1);
    }

    private static GenericDictionary<Vector3, CombatTileController> GetPossibleMoveLogic(CombatTileController combatTile, int movementSpeed, PartySlot partySlot, GenericDictionary<Vector3, CombatTileController> battleGrid, List<CombatTileController> previousTiles)
    {
        if (!partySlot.CanMoveToTile(combatTile))
        {
            return new GenericDictionary<Vector3, CombatTileController>();
        }

        if (partySlot.BattleEntity.CanFly())
        {
            return GetFlyingMoves(combatTile, partySlot);
        }




        GenericDictionary<Vector3, CombatTileController> possibleMoves = new GenericDictionary<Vector3, CombatTileController>
        {
            { combatTile.Position, combatTile }
        };

        List<CombatTileController> tilesToMove = new List<CombatTileController>();
        if (combatTile.movementTiles.Count == 0 || (combatTile.movementTiles.Count > 0 && (previousTiles.Count + 1 < combatTile.movementTiles.Count)))
        {
            tilesToMove.AddRange(previousTiles);
            tilesToMove.Add(combatTile);
            combatTile.movementTiles = tilesToMove;
        }


        if (!combatTile.HasPartyOccupant())
            combatTile.ActivateMovementTile();


        //First North
        Vector3Int northPosition = new(combatTile.Position.x, 0, combatTile.Position.z - 1);
        if (battleGrid.ContainsKey(northPosition))
        {
            CombatTileController northTile = battleGrid[northPosition];

            int northSpeed = movementSpeed;
            int northTileMovementCost = northTile.GetMovementCost(partySlot.BattleEntity.GetBattleTerrainMovementOverride());
            if (CanAddTile(northTile, combatTile, partySlot, northSpeed, northTileMovementCost))
            {
                northSpeed -= northTileMovementCost;
                List<CombatTileController> newTilesToMove = new();
                newTilesToMove.AddRange(combatTile.movementTiles);
                GenericDictionary<Vector3, CombatTileController> northMoves = GetPossibleMoveLogic(northTile, northSpeed, partySlot, battleGrid, newTilesToMove);
                foreach (KeyValuePair<Vector3, CombatTileController> keyValuePair in northMoves)
                {
                    AddPossibleWalkableTile(possibleMoves, keyValuePair);
                }
            }
        }

        //Then South
        Vector3Int southPosition = new(combatTile.Position.x, 0, combatTile.Position.z + 1);
        if (battleGrid.ContainsKey(southPosition))
        {
            CombatTileController southTile = battleGrid[southPosition];

            int southSpeed = movementSpeed;
            int southTileMovementCost = southTile.GetMovementCost(partySlot.BattleEntity.GetBattleTerrainMovementOverride());
            if (CanAddTile(southTile, combatTile, partySlot, southSpeed, southTileMovementCost))
            {
                southSpeed -= southTileMovementCost;
                List<CombatTileController> newTilesToMove = new();
                newTilesToMove.AddRange(combatTile.movementTiles);
                GenericDictionary<Vector3, CombatTileController> southMoves = GetPossibleMoveLogic(southTile, southSpeed, partySlot, battleGrid, newTilesToMove);
                foreach (KeyValuePair<Vector3, CombatTileController> keyValuePair in southMoves)
                {
                    AddPossibleWalkableTile(possibleMoves, keyValuePair);
                }
            }
        }

        //Then West
        Vector3Int westPosition = new(combatTile.Position.x - 1, 0, combatTile.Position.z);
        if (battleGrid.ContainsKey(westPosition))
        {
            CombatTileController westTile = battleGrid[westPosition];


            int westSpeed = movementSpeed;
            int westTileMovementCost = westTile.GetMovementCost(partySlot.BattleEntity.GetBattleTerrainMovementOverride());
            if (CanAddTile(westTile, combatTile, partySlot, westSpeed, westTileMovementCost))
            {
                westSpeed -= westTileMovementCost;
                List<CombatTileController> newTilesToMove = new();
                newTilesToMove.AddRange(combatTile.movementTiles);
                GenericDictionary<Vector3, CombatTileController> westMoves = GetPossibleMoveLogic(westTile, westSpeed, partySlot, battleGrid, newTilesToMove);
                foreach (KeyValuePair<Vector3, CombatTileController> keyValuePair in westMoves)
                {
                    AddPossibleWalkableTile(possibleMoves, keyValuePair);
                }
            }
        }


        //Then East

        Vector3Int eastPosition = new(combatTile.Position.x + 1, 0, combatTile.Position.z);
        if (battleGrid.ContainsKey(eastPosition))
        {
            CombatTileController eastTile = battleGrid[eastPosition];

            int eastSpeed = movementSpeed;
            int eastTileMovementCost = eastTile.GetMovementCost(partySlot.BattleEntity.GetBattleTerrainMovementOverride());
            if (CanAddTile(eastTile, combatTile, partySlot, eastSpeed, eastTileMovementCost))
            {
                eastSpeed -= eastTileMovementCost;
                List<CombatTileController> newTilesToMove = new();
                newTilesToMove.AddRange(combatTile.movementTiles);
                GenericDictionary<Vector3, CombatTileController> eastMoves = GetPossibleMoveLogic(eastTile, eastSpeed, partySlot, battleGrid, newTilesToMove);
                foreach (KeyValuePair<Vector3, CombatTileController> keyValuePair in eastMoves)
                {
                    AddPossibleWalkableTile(possibleMoves, keyValuePair);
                }
            }
        }

        return possibleMoves;
    }

    private static bool CanAddTile(CombatTileController newTile, CombatTileController startTile, PartySlot partySlot, int movementSpeed, int tileMovementCost)
    {
        return CanAddTile(newTile, startTile) && newTile.CanAddOccupant(partySlot) && movementSpeed > 0 && movementSpeed >= tileMovementCost && IsWalkableHeightDifference(startTile, newTile.GetFloatHeightWithSpawnedObject(), partySlot.BattleEntity.GetMovement().JumpHeight, partySlot.BattleEntity.CanFly());
    }

    private static bool AddPossibleWalkableTile(GenericDictionary<Vector3, CombatTileController> possibleTile, KeyValuePair<Vector3, CombatTileController> keyValuePair)
    {
        if (!possibleTile.ContainsKey(keyValuePair.Key))
        {
            possibleTile.Add(keyValuePair.Key, keyValuePair.Value);
            return true;
        }
        return false;
    }

    // private static bool GetPossibleTile(PartySlot partySlot, CombatTileController startCombatTile, CombatTileController desiredCombatTile, int movementSpeed) {
    //     return partySlot.CanMoveToTile(desiredCombatTile) &&
    //         desiredCombatTile.CanAddOccupant(partySlot) && 
    //         movementSpeed > 0 && 
    //         movementSpeed >= desiredCombatTile.GetMovementCost(partySlot.GetBattleEntity.GetBattleTerrainMovementOverride()) && 
    //         IsWalkableHeightDifference(startCombatTile, desiredCombatTile.GetFloatHeightWithSpawnedObject(), partySlot.GetBattleEntity.GetMovement().JumpHeight, partySlot.GetBattleEntity.CanFly());
    // }

    private static KeyValuePair<Vector3, CombatTileController> GeneratePossibleTile(CombatTileController combatTileController)
    {
        if (!combatTileController.HasPartyOccupant())
            combatTileController.ActivateMovementTile();

        return new(combatTileController.Position, combatTileController);
    }

    private struct TileMovementDetails
    {
        public int MovementSpeed;
        public List<CombatTileController> MovementTiles;

        public TileMovementDetails(int movement, List<CombatTileController> movementTiles)
        {
            this.MovementSpeed = movement;
            this.MovementTiles = movementTiles;
        }
    }

    public static GenericDictionary<Vector3, CombatTileController> GetPossibleMoves(CombatTileController combatTile, PartySlot partySlot, GenericDictionary<Vector3, CombatTileController> battleGrid)
    {
        return GetPossibleMoveLogic(combatTile, partySlot.BattleEntity.GetMovement().Speed, partySlot, battleGrid, new());
    }

    // private static GenericDictionary<Vector3, CombatTileController> GetPossibleMoveLogic(
    //     CombatTileController combatTile, 
    //     PartySlot partySlot, 
    //     GenericDictionary<Vector3, CombatTileController> battleGrid) {

    //     if (partySlot.GetBattleEntity.CanFly()) {
    //         return GetFlyingMoves(combatTile, partySlot);
    //     }


    //     GenericDictionary<Vector3, CombatTileController> possibleMoves = new GenericDictionary<Vector3, CombatTileController>
    //     {
    //         { combatTile.Position, combatTile }
    //     };




    //     Stack<KeyValuePair<CombatTileController, TileMovementDetails>> tileExplorationStack = new();
    //     tileExplorationStack.Push(new KeyValuePair<CombatTileController, TileMovementDetails>(combatTile, new(partySlot.GetBattleEntity.GetMovement().Speed, new() ) ));

    //     while(tileExplorationStack.Count > 0) {
    //         //Get all North movements based on 
    //         KeyValuePair<CombatTileController, TileMovementDetails> keyValuePair = tileExplorationStack.Pop();

    //         CombatTileController currentTile = keyValuePair.Key;

    //         int northMoveSpeed, eastMoveSpeed, southMoveSpeed, westMoveSpeed;
    //         northMoveSpeed = eastMoveSpeed = southMoveSpeed = westMoveSpeed = keyValuePair.Value.MovementSpeed;

    //         Vector3Int northPosition = new (currentTile.Position.x, 0, currentTile.Position.z - 1);
    //         if (battleGrid.TryGetValue(northPosition, out CombatTileController northTile ) && GetPossibleTile(partySlot, currentTile, northTile, northMoveSpeed)) {
    //             int speed = northMoveSpeed - northTile.GetMovementCost(partySlot.GetBattleEntity!.GetBattleTerrainMovementOverride());


    //             List<CombatTileController> newMovementTiles = new();
    //             newMovementTiles.AddRange(keyValuePair.Value.MovementTiles);
    //             newMovementTiles.Add(currentTile);

    //             if (!possibleMoves.ContainsKey(northTile.Position)) {
    //                 possibleMoves.Add(GeneratePossibleTile(northTile));
    //                 northTile.movementTiles = newMovementTiles;

    //             }

    //             if (possibleMoves.ContainsKey(northTile.Position) && northTile.movementTiles.Count > 0 && northTile.movementTiles.Count > newMovementTiles.Count) {
    //                 northTile.movementTiles = newMovementTiles;
    //             }
    //             tileExplorationStack.Push(new(northTile, new(speed, newMovementTiles) ));

    //         }

    //         Vector3Int southPosition = new(currentTile.Position.x, 0, currentTile.Position.z + 1);
    //         if (battleGrid.TryGetValue(southPosition, out CombatTileController southTile) && GetPossibleTile(partySlot, currentTile, southTile, northMoveSpeed)) {
    //             int speed = northMoveSpeed - southTile.GetMovementCost(partySlot.GetBattleEntity!.GetBattleTerrainMovementOverride());


    //             List<CombatTileController> newMovementTiles = new();
    //             newMovementTiles.AddRange(keyValuePair.Value.MovementTiles);
    //             newMovementTiles.Add(currentTile);

    //             if (!possibleMoves.ContainsKey(southTile.Position)) {
    //                 possibleMoves.Add(GeneratePossibleTile(southTile));
    //                 southTile.movementTiles = newMovementTiles;
    //             }

    //             if (possibleMoves.ContainsKey(southTile.Position) && southTile.movementTiles.Count > 0 && southTile.movementTiles.Count > newMovementTiles.Count) {
    //                 southTile.movementTiles = newMovementTiles;
    //             }

    //             tileExplorationStack.Push(new(southTile, new(speed, newMovementTiles) ));

    //         }

    //         Vector3Int westPosition = new(currentTile.Position.x - 1, 0, currentTile.Position.z);
    //         if (battleGrid.TryGetValue(westPosition, out CombatTileController westTile) && GetPossibleTile(partySlot, currentTile, westTile, northMoveSpeed)) {
    //             int speed = northMoveSpeed - westTile.GetMovementCost(partySlot.GetBattleEntity!.GetBattleTerrainMovementOverride());

    //             List<CombatTileController> newMovementTiles = new();
    //             newMovementTiles.AddRange(keyValuePair.Value.MovementTiles);
    //             newMovementTiles.Add(currentTile);

    //             if (!possibleMoves.ContainsKey(westTile.Position)) {
    //                 possibleMoves.Add(GeneratePossibleTile(westTile));
    //                 westTile.movementTiles = newMovementTiles;

    //             }

    //             if (possibleMoves.ContainsKey(westTile.Position) && westTile.movementTiles.Count > 0 && westTile.movementTiles.Count > newMovementTiles.Count) {
    //                 westTile.movementTiles = newMovementTiles;
    //             }

    //             tileExplorationStack.Push(new(westTile, new(speed, newMovementTiles) ));

    //         }

    //         Vector3Int eastPosition = new(currentTile.Position.x + 1, 0, currentTile.Position.z);
    //         if (battleGrid.TryGetValue(eastPosition, out CombatTileController eastTile) && GetPossibleTile(partySlot, currentTile, eastTile, northMoveSpeed)) {
    //             int speed = northMoveSpeed - eastTile.GetMovementCost(partySlot.GetBattleEntity!.GetBattleTerrainMovementOverride());


    //             List<CombatTileController> newMovementTiles = new();
    //             newMovementTiles.AddRange(keyValuePair.Value.MovementTiles);
    //             newMovementTiles.Add(currentTile);

    //             if (!possibleMoves.ContainsKey(eastTile.Position)) {
    //                 possibleMoves.Add(GeneratePossibleTile(eastTile));
    //                 eastTile.movementTiles = newMovementTiles;
    //             }

    //             if (possibleMoves.ContainsKey(eastTile.Position) && eastTile.movementTiles.Count > 0 && eastTile.movementTiles.Count > newMovementTiles.Count) {
    //                 eastTile.movementTiles = newMovementTiles;
    //             }

    //             tileExplorationStack.Push(new(eastTile, new(speed, newMovementTiles ) ));
    //         }

    //     }

    //     return possibleMoves;
    // }

    public static GenericDictionary<Vector3, CombatTileController> GetFlyingMoves(CombatTileController startTile, PartySlot partySlot)
    {

        GenericDictionary<Vector3, CombatTileController> possibleMovementTiles = new GenericDictionary<Vector3, CombatTileController>();
        GenericDictionary<CombatTileController, int> combatTileRayCounter = new GenericDictionary<CombatTileController, int>();

        possibleMovementTiles.Add(startTile.Position, startTile);

        float totalAngle = 360;
        float delta = totalAngle / 180;
        //Perform a raycast from the center of the current tile, and get all tiles hit by it.  If entityHeight > raycasted object's height, add the tile's coordinate to the possibleActionTiles
        for (int i = 0; i < 180; i++)
        {

            Vector3 dir = Quaternion.Euler(0, i * delta, 0) * startTile.transform.right;

            List<RaycastHit> raycastHits = new(Physics.RaycastAll(startTile.Position, dir, partySlot.BattleEntity.GetMovement().Speed));

            foreach (RaycastHit raycast in raycastHits)
            {
                if (!raycast.transform.gameObject.TryGetComponent<CombatTileController>(out CombatTileController combatTileController))
                {
                    continue;
                }
                //Converts the ability range to units relevant to the game's standard tile height of 0.25f.

                if (!combatTileRayCounter.ContainsKey(combatTileController))
                {
                    combatTileRayCounter[combatTileController] = 0;
                }
                else
                {
                    combatTileRayCounter[combatTileController]++;
                }

                if (combatTileRayCounter[combatTileController] < 4 || possibleMovementTiles.ContainsKey(combatTileController.Position) || !combatTileController.CanAddOccupant(partySlot))
                {
                    continue;
                }

                possibleMovementTiles.Add(combatTileController.Position, combatTileController);
                combatTileController.ActivateMovementTile();
                GetFlyingMovementTiles(startTile, combatTileController);

            }
        }

        return possibleMovementTiles;
    }

    public static void GetFlyingMovementTiles(CombatTileController start, CombatTileController end)
    {
        if (start == null)
        {
            return;
        }

        // List<RaycastHit> raycastHits = new List<RaycastHit>();

        // Vector3 fromPosition = start.Position;
        // Vector3 toPosition = end.Position;

        // Vector3 direction = (toPosition - fromPosition).normalized;

        // float distance = Vector3.Distance(fromPosition, toPosition);

        // //First, ray cast from user to target to get all raycast hits
        // raycastHits.AddRange(Physics.RaycastAll(fromPosition, direction, distance));
        // Debug.DrawRay(fromPosition, direction * distance, Color.red, 60);

        // raycastHits.Sort( (a, b) => a.distance.CompareTo(b.distance));
        // List<CombatTileController> combatTileControllers = new List<CombatTileController>();

        // foreach(RaycastHit raycastHit in raycastHits) {
        //     #nullable enable
        //     GameObject? tileObject = raycastHit.transform.gameObject;
        //     if (tileObject == null || !tileObject.TryGetComponent<CombatTileController>(out CombatTileController combatTile)) {
        //         continue;
        //     }
        //     #nullable disable
        //     combatTileControllers.Add(combatTile);
        // }

        // end.movementTiles = combatTileControllers;
        end.movementTiles = new(){
            end
        };
    }

    private static bool IsWalkableHeightDifference(CombatTileController combatTile, float newTileHeight, float verticalMovement, bool isFlying)
    {
        if (isFlying)
        {
            return true;
        }
        newTileHeight = Mathf.Round(newTileHeight * 10.0f) * 0.1f;
        float heightWithSpawnedObject = Mathf.Round(combatTile.GetFloatHeightWithSpawnedObject() * 10.0f) * 0.1f;
        return newTileHeight - heightWithSpawnedObject <= verticalMovement;
    }



#nullable enable
    public static bool TileIsTargetableActiveSkill(IBattleActionCommand activeSkill, PartySlot userPartySlot, PartySlot affectedPartySlot, CombatTileController targetTile, BattleManager? battleController = null, CombatTileController? currentEnemyTile = null)
    {



        if (PartySlotIsEnemyPartySlot(userPartySlot) && battleController != null && currentEnemyTile != null)
        {
            bool isFlying = battleController.CurrentlyActingMember.BattleEntity!.CanFly();
            float totalHeight = currentEnemyTile.GetHeightWithSpawnedObjectAndProvidedEntityFlight();

            if (affectedPartySlot.GetType() == typeof(SpawnableObjectPartySlot))
            {
                float minimumHeightOfObjectDifference = Mathf.Abs(totalHeight -battleController.BattleFieldController.ActionGrid[targetTile.Position].GetHeight());

                float maxHeightOfObjectDifference = Mathf.Abs(totalHeight - battleController.BattleFieldController.ActionGrid[targetTile.Position].GetFloatHeightWithSpawnedObject());

                return ((isFlying) || (activeSkill.IsWithinVerticalRange(maxHeightOfObjectDifference) || activeSkill.IsWithinVerticalRange(minimumHeightOfObjectDifference)));
            }
            else
            {
                float heightDistanceToTarget = Mathf.Abs(totalHeight - battleController.BattleFieldController.ActionGrid[targetTile.Position].GetHeightWithSpawnedObjectAndPresentEntity());


                return IsHeightDistanceToTargetEnemy(isFlying, heightDistanceToTarget, activeSkill: activeSkill);
            }
        }

        return false;
    }


    private static bool IsHeightDistanceToTargetEnemy(bool isFlying, float heightDistanceToTarget, IBattleActionCommand? activeSkill = null)
    {
        return (isFlying || activeSkill != null && activeSkill.IsWithinVerticalRange(heightDistanceToTarget));
    }

    private static bool IsHeightDistanceToTargetEnemy(bool isFlying, float heightDistanceToTarget, ConsumableInventoryItemSO? inventoryItem = null)
    {
        return true;
    }

    public static void OnTakingDamage(BattleManager battleController, PartySlot attacker, PartySlot defender)
    {
        CharacterInfo? characterInfo = defender.BattleEntity as CharacterInfo;
        if (characterInfo == null)
        {
            return;
        }
        CharacterRoleSO? characterRoleSO = characterInfo.GetCharacterRole();
        if (characterRoleSO == null)
        {
            return;
        }
        characterRoleSO.OnTakingDamage(battleController, attacker, defender);
    }

    public static void OnEntityUseAbility(BattleManager battleController, PartySlot attacker, ActiveSkillSO activeSkill)
    {
        CharacterInfo? characterInfo = attacker.BattleEntity as CharacterInfo;
        if (characterInfo == null)
        {
            return;
        }
        CharacterRoleSO? characterRoleSO = characterInfo.GetCharacterRole();
        if (characterRoleSO == null)
        {
            return;
        }
        characterRoleSO.OnSkillCast(battleController, attacker, activeSkill);
    }

    public static void OnEntityBuffSkill(BattleManager battleController, PartySlot user)
    {
        CharacterInfo? characterInfo = user.BattleEntity as CharacterInfo;
        if (characterInfo == null)
        {
            return;
        }
        CharacterRoleSO? characterRoleSO = characterInfo.GetCharacterRole();
        if (characterRoleSO == null)
        {
            return;
        }
        characterRoleSO.OnApplyBuff(battleController, user);
    }

    public static void OnEntityDealingDamage(BattleManager battleController, PartySlot attacker)
    {
        if (attacker == null) {
            return;
        }
        if (attacker.BattleEntity is not CharacterInfo characterInfo)
        {
            return;
        }
        CharacterRoleSO? characterRoleSO = characterInfo.GetCharacterRole();
        if (characterRoleSO == null)
        {
            return;
        }
        characterRoleSO.OnDealingDamage(battleController, attacker);
    }

    public static void OnEntityCriticalHit(BattleManager battleController, PartySlot attacker)
    {
        if (attacker == null) {
            return;
        }

        if (attacker.BattleEntity is not CharacterInfo characterInfo)
        {
            return;
        }
        CharacterRoleSO? characterRoleSO = characterInfo.GetCharacterRole();
        if (characterRoleSO == null)
        {
            return;
        }
        characterRoleSO.OnCriticalHit(battleController, attacker);
    }


    public static void EntityHeal(BattleManager battleController, PartySlot healer, PartySlot healedSlot)
    {
        CharacterInfo? healerCharacterInfo = healer.BattleEntity as CharacterInfo;
        BattleEntityInfo? healedBattleEntityInfo = healedSlot.BattleEntity as BattleEntityInfo;
        if (healerCharacterInfo == null || healedBattleEntityInfo == null)
        {
            return;
        }
        CharacterRoleSO? characterRoleSO = healerCharacterInfo.GetCharacterRole();
        if (characterRoleSO == null)
        {
            return;
        }

        RawStats healedRawStats = healedBattleEntityInfo.GetRawStats();
        float savingHealPercent = (float)((float)healedRawStats.hp / (float)healedRawStats.maxHp);

        if (savingHealPercent <= 0.3f)
        {
            characterRoleSO.OnSavingHeal(battleController, healer);
            return;
        }

        characterRoleSO.OnHeal(battleController, healer);
    }

    public static bool EntityEvaded(System.Random random, int userAccuracy, int targetEvasion)
    {
        int evasionChance = random.Next(0, 100);
        // Debug.LogError(string.Format("User Accuracy: {0}, Target Evasion: {1}, Rolled Evasion Chance: {2}", userAccuracy, targetEvasion, evasionChance));
        return evasionChance > userAccuracy - targetEvasion;
    }

    public static bool IsGiantEntity(PartySlot? partySlot)
    {
        if (partySlot == null)
        {
            return false;
        }

        return partySlot.Width > 0;
    }
#nullable disable

    public static IEnumerator PerformMovement(BattleManager battleController, PartySlot partySlot, CombatTileController destinationTile, System.Action action)
    {
        Debug.Log("Performing movement coroutine");
        partySlot.BattleEntityGO.SetMovement(isRunning: true);
        CombatTileController previousTile = partySlot.GetCombatTileController();
        while (destinationTile.movementTiles.Count > 0)
        {

            CombatTileController currentTile = destinationTile.movementTiles[0];

            Vector3 endPosition = battleController.Grid[currentTile.Position].GetHeightForSpriteVector3Offset(true);
            Vector3 startPosition = previousTile.GetHeightForSpriteVector3Offset(true);
            Vector3 lookAtPosition = new(partySlot.BattleEntityGO.transform.forward.x, partySlot.BattleEntityGO.transform.position.y, partySlot.BattleEntityGO.transform.forward.z);
            partySlot.LookAt(lookAtPosition);

            if (!battleController.Grid.ContainsKey(currentTile.Position))
            {
                yield return null;
            }

            lookAtPosition = new Vector3(endPosition.x, partySlot.BattleEntityGO.transform.position.y, endPosition.z);
            partySlot.LookAt(lookAtPosition);
            partySlot.BattleEntityGO.transform.position = GetNewPosition(partySlot, startPosition, endPosition, battleController.BattleValues.BaseMovementSpeed * Time.deltaTime);

            if (partySlot.BattleEntityGO.transform.position == endPosition)
            {
                previousTile = currentTile;
                // battleState.currentPartyMember.GetBattleEntityGO().transform.LookAt(battleState.currentPartyMember.GetBattleEntityGO().transform.forward);
                destinationTile.movementTiles.RemoveAt(0);
            }
            yield return null;
        }

        partySlot.BattleEntityGO.EndMovement();

        // battleMenuController.currentPartyMember.SetTileCoordinates(new Vector3(battleMenuController.targetedTile.x, battleMenuController.targetedTile.y, battleMenuController.targetedTile.z));
        destinationTile.SetPartyOccupant(battleController, partySlot);

        partySlot.PostMovePhase(battleController);

        action?.Invoke();
    }

    private static Vector3 GetNewPosition(PartySlot partySlot, Vector3 startPosition, Vector3 endPosition, float movementSpeed)
    {

        Vector3 trueEndPos = endPosition;
        Vector3 flattenedCurrentPos = partySlot.BattleEntityGO.transform.position;
        Vector3 flattenedStart = startPosition;
        Vector3 flattenedEnd = endPosition;

        flattenedStart.y = 0;
        flattenedEnd.y = 0;
        flattenedCurrentPos.y = 0;

        float distanceToStart = Vector3.Distance(flattenedCurrentPos, flattenedStart);
        float distanceToEnd = Vector3.Distance(flattenedCurrentPos, flattenedEnd);

        Vector3 entityPos = partySlot.BattleEntityGO.transform.position;
        if (distanceToStart < distanceToEnd)
        {
            entityPos.y = startPosition.y;
            trueEndPos.y = startPosition.y;

        }
        else
        {
            trueEndPos.y = endPosition.y;
            entityPos.y = endPosition.y;
        }

        partySlot.BattleEntityGO.transform.position = entityPos;
        return Vector3.MoveTowards(partySlot.BattleEntityGO.transform.position, trueEndPos, movementSpeed);
    }

    // private static Vector3 GetNewJumpPosition(PartySlot partySlot, Vector3 start, Vector3 end, float movementSpeed) {
    //     return DPS.Common.GeneralUtilsStatic.GetVerticalArc(start, end, end.y + 1, 0, 1);
    // }

    public static Vector3 GetVerticalArc(Vector3 start, Vector3 end, float height, float currentStep, float maxSteps)
    {
        return GetVerticalArc(start, end, height, currentStep / maxSteps);
    }

    public static Vector3 GetVerticalArc(Vector3 start, Vector3 end, float height, float fracComplete)
    {
        //Gets the center of the arc.
        Vector3 center = (start + end) / 2f;

        //Increases the height of the center
        center += new Vector3(0, height, 0);

        //The fraction of animationTime that has elapsed since the startTime
        Vector3 startToCenter = Vector3.Lerp(start, center, fracComplete);
        Vector3 centerToEnd = Vector3.Lerp(center, end, fracComplete);

        //Current slerped position
        return Vector3.Lerp(startToCenter, centerToEnd, fracComplete);
    }

    public static void DisplaceTargets(SkillDisplacementSO skillDisplacementSO, IBattleActionCommand battleAction, BattleManager battleController, PartySlot user, List<PartySlot> affectedPartySlots)
    {
#nullable enable
        if (skillDisplacementSO == null)
        {
            return;
        }

        GenericDictionary<Vector3, CombatTileController>? grid = battleController.Grid;

        if (grid == null)
        {
            return;
        }

        skillDisplacementSO.Execute(battleController, battleAction, grid, user, affectedPartySlots);

#nullable disable
    }

}
}

