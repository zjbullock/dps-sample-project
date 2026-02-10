using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat {
public abstract class CascadingActionTileBehaviorSO : BattleTileTargetBehaviorSO
{
    public override bool GetActionableTilesForEnemy()
    {
        return base.GetActionableTilesForEnemy();
    }

    // public override bool GetActionableTilesForPlayer(BattleMenuController battleMenuController)
    // {
    //     if (battleMenuController.actionGridTargetCells.Count == 0) {
    //         battleMenuController.SetCommandMenuVisibility(false);
    //         battleMenuController.currentPartyMember.BattleEntityGO.playerCursor.SetActive(false);
    //         Vector3 tileCoordinates = (Vector3) battleMenuController.currentPartyMember.GetCombatTileController().Position;
    //         base.GetActionsByQueueTypeForPlayer(battleMenuController, tileCoordinates, true, false);
    //         battleMenuController.tileTargetingObject.SetActive(true);
    //         // battleMenuController.SetCameraTarget(battleMenuController.combatTileCursor.GetFollowTarget());
    //         return true;
    //     }
    //     return false;
    // }



    public override GenericDictionary<Vector3, CombatTileController> GetActionTilesByAreaOfEffect(CombatTileController combatTile, int abilityRange, float abilityVerticalRange, float entityHeight, bool isFlying)
    {
        GenericDictionary<Vector3, CombatTileController> possibleActionTiles = new GenericDictionary<Vector3, CombatTileController>();
        possibleActionTiles.Add(combatTile.Position, combatTile);

        GenericDictionary<CombatTileController, int> combatTileRayCounter = new GenericDictionary<CombatTileController, int>();
        float totalAngle = 360;
        float delta = totalAngle / this.rayCount;
        //Perform a raycast from the center of the current tile, and get all tiles hit by it.  If entityHeight > raycasted object's height, add the tile's coordinate to the possibleActionTiles
        for (int i = 0; i < this.rayCount; i++) {
            
            Vector3 dir = Quaternion.Euler(0, i * delta, 0) * combatTile.transform.right;
            List<RaycastHit> raycastHits = new List<RaycastHit>();
            RaycastHit hit;
            Vector3 truePosition = new Vector3(combatTile.Position.x, combatTile.Position.y, combatTile.Position.z);
            truePosition.y = entityHeight + combatTile.GetHeightForSpriteVector3Offset(false).y;
            //Gets the initial ray casts hits to get the maximum distance of the rayCastAll.
            if (Physics.Raycast(truePosition, dir, out hit, abilityRange, ~this.ignore)) {
                // Debug.Log(hit.distance);
                raycastHits.AddRange(Physics.RaycastAll(combatTile.Position, dir, hit.distance));
                Debug.DrawRay(combatTile.Position, dir * hit.distance, Color.red, 5);
            } else {
                raycastHits.AddRange(Physics.RaycastAll(combatTile.Position, dir, abilityRange));
                Debug.DrawRay(combatTile.Position, dir * abilityRange, Color.red, 5);

            }

            float totalHeight = combatTile.GetHeightWithSpawnedObjectAndProvidedEntityFlight();

            raycastHits.Sort( (a, b) => a.distance.CompareTo(b.distance));

            float height = Mathf.Round(combatTile.GetFloatHeightWithSpawnedObject() * 10.0f) * 0.1f;
            Vector2 coordinates = new Vector2(combatTile.Position.x, combatTile.Position.z);
            CombatTileController startingTile = combatTile;
            foreach (RaycastHit raycast in raycastHits) {
                GameObject tileObject = raycast.transform.gameObject;
                if (tileObject != null) {
                    CombatTileController combatTileController = tileObject.GetComponent<CombatTileController>();
                    //Converts the ability range to units relevant to the game's standard tile height of 0.25f.
                    if (combatTileController != null) {
                        if (!combatTileRayCounter.ContainsKey(combatTileController)) {
                            combatTileRayCounter[combatTileController] = 0;
                        } else {
                            combatTileRayCounter[combatTileController]++;
                        }

                        
                        float distance = Mathf.Abs(totalHeight - combatTileController.GetHeightWithSpawnedObjectAndPresentEntity());
                        float newHeight = Mathf.Round(combatTileController.GetFloatHeightWithSpawnedObject() * 10.0f) * 0.1f;
                        if (newHeight > height || !this.combatTileInRange(startingTile, combatTileController)) {
                            break;
                        }
                        startingTile = combatTileController;
                        if (combatTileRayCounter[combatTileController] >= this.rayCastNecessary && newHeight <= height ) {
                        // if (combatTileRayCounter[combatTileController] >= this.rayCastNecessary ) {
                            height = newHeight;
                            bool added = this.AddPossibleTile(possibleActionTiles, new KeyValuePair<Vector3, CombatTileController>(combatTileController.Position, combatTileController));
                        }
                    }
                }
            }
        }
        return possibleActionTiles;
    }

    private bool combatTileInRange(CombatTileController start, CombatTileController destination) {
        if (start == null || destination == null) {
            return false;
        }
        return Mathf.Sqrt(Mathf.Pow(start.Position.x - destination.Position.x, 2) + Mathf.Pow(start.Position.z - destination.Position.z, 2)) < 1.5f;
    }

    public override GenericDictionary<Vector3, CombatTileController> GetSelectActionTilesByAbilityRange(CombatTileController combatTile, int abilityRange, float abilityVerticalRange, float entityHeight, bool isFlying)
    {
        GenericDictionary<Vector3, CombatTileController> possibleActionTiles = new GenericDictionary<Vector3, CombatTileController>();
        possibleActionTiles.Add(combatTile.Position, combatTile);
        GenericDictionary<CombatTileController, int> combatTileRayCounter = new GenericDictionary<CombatTileController, int>();
        float totalAngle = 360;
        float delta = totalAngle / this.rayCount;
        //Perform a raycast from the center of the current tile, and get all tiles hit by it.  If entityHeight > raycasted object's height, add the tile's coordinate to the possibleActionTiles
        for (int i = 0; i < this.rayCount; i++) {
            
            Vector3 dir = Quaternion.Euler(0, i * delta, 0) * combatTile.transform.right;
            List<RaycastHit> raycastHits = new List<RaycastHit>();
            RaycastHit hit;
            Vector3 truePosition = new Vector3(combatTile.Position.x, combatTile.Position.y, combatTile.Position.z);
            truePosition.y = entityHeight + combatTile.GetHeightForSpriteVector3Offset(false).y;
            //Gets the initial ray casts hits to get the maximum distance of the rayCastAll.
            if (Physics.Raycast(truePosition, dir, out hit, abilityRange, ~this.ignore)) {
                // Debug.Log(hit.distance);
                raycastHits.AddRange(Physics.RaycastAll(combatTile.Position, dir, hit.distance));
                Debug.DrawRay(combatTile.Position, dir * hit.distance, Color.red, 5);
            } else {
                raycastHits.AddRange(Physics.RaycastAll(combatTile.Position, dir, abilityRange));
                Debug.DrawRay(combatTile.Position, dir * abilityRange, Color.red, 5);

            }


            foreach (RaycastHit raycast in raycastHits) {
                GameObject tileObject = raycast.transform.gameObject;
                if (tileObject != null) {
                    CombatTileController combatTileController = tileObject.GetComponent<CombatTileController>();
                    float totalHeight = entityHeight + combatTile.GetFloatHeightWithSpawnedObject();
                    //Converts the ability range to units relevant to the game's standard tile height of 0.25f.
                    if (combatTileController != null) {
                        if (!combatTileRayCounter.ContainsKey(combatTileController)) {
                            combatTileRayCounter[combatTileController] = 0;
                        } else {
                            combatTileRayCounter[combatTileController]++;
                        }
                        if (combatTileRayCounter[combatTileController] >= this.rayCastNecessary) {
                            bool added = this.AddPossibleTile(possibleActionTiles, new KeyValuePair<Vector3, CombatTileController>(combatTileController.Position, combatTileController));
                        }
                    }
                }
            }
        }
        return possibleActionTiles;
    }

    public override bool RequireLineOfSight() {
        return true;
    }
}
}
