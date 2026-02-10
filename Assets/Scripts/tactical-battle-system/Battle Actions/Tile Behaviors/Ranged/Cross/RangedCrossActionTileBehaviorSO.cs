using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat
{
    

public abstract class RangedCrossActionTileBehaviorSO : BattleTileTargetBehaviorSO
{

    // public override bool GetActionableTilesForPlayer(BattleMenuController battleMenuController)
    // {
    //     if (battleMenuController.actionGridTargetCells.Count == 0) {
    //         battleMenuController.SetCommandMenuVisibility(false);
    //         battleMenuController.currentPartyMember.BattleEntityGO.playerCursor.SetActive(false);
    //         Vector3 tileCoordinates = (Vector3) battleMenuController.currentPartyMember.GetCombatTileController().Position;
    //         base.GetActionsByQueueTypeForPlayer(battleMenuController, tileCoordinates, true, false);
    //         base.GetActionsByQueueTypeForPlayer(battleMenuController, tileCoordinates, false, true);
    //         battleMenuController.tileTargetingObject.SetActive(true);
    //         battleMenuController.ActivateFreeCamWithNewTarget(battleMenuController.combatTileCursor.GetFollowTarget());
    //         return true;
    //     } 
    //     //Gets the Actionable Tiles based on the cursor location
    //     else if (battleMenuController.combatTileCursor != null && 
    //         battleMenuController.combatTileCursor.combatTileController != null && 
    //         battleMenuController.targetedTileSpeculative != battleMenuController.combatTileCursor.combatTileController.Position) {
    //         battleMenuController.targetedTileSpeculative = battleMenuController.combatTileCursor.combatTileController.Position;
    //         battleMenuController.ClearCursorOptions();
    //         if (battleMenuController.actionGridTargetCells.ContainsKey(battleMenuController.targetedTileSpeculative)) {
    //             base.GetActionsByQueueTypeForPlayer(battleMenuController, battleMenuController.targetedTileSpeculative, false, true);
    //         }
    //     } 
    //     return false;
    // }

    public override GenericDictionary<Vector3, CombatTileController> GetSelectActionTilesByAbilityRange(CombatTileController combatTile, int abilityRange, float abilityVerticalRange, float entityHeight, bool isFlying)
    {
        GenericDictionary<Vector3, CombatTileController> possibleActionTiles = new GenericDictionary<Vector3, CombatTileController>();
        possibleActionTiles.Add(combatTile.Position, combatTile);

        GenericDictionary<CombatTileController, int> combatTileRayCounter = new GenericDictionary<CombatTileController, int>();
        float totalAngle = 360;
        float delta = totalAngle / 4;
        //Perform a raycast from the center of the current tile, and get all tiles hit by it.  If entityHeight > raycasted object's height, add the tile's coordinate to the possibleActionTiles
        for (int i = 0; i < 4; i++) {
            
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
                        if (totalHeight >= combatTileController.GetFloatHeightWithSpawnedObject()) {
                            bool added = this.AddPossibleTile(possibleActionTiles, new KeyValuePair<Vector3, CombatTileController>(combatTileController.Position, combatTileController));

                        }
                    }
                }
            }
        }
        Debug.Log("possibleActionTiles: " + possibleActionTiles.ToString());
        return possibleActionTiles;
    }

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

                        if (combatTileRayCounter[combatTileController] >= this.rayCastNecessary && (totalHeight >= combatTileController.GetFloatHeightWithSpawnedObject())) {
                            bool added = this.AddPossibleTile(possibleActionTiles, new KeyValuePair<Vector3, CombatTileController>(combatTileController.Position, combatTileController));
                        }
                    }
                }
            }
        }
        return possibleActionTiles;
    }
    

    #nullable enable

    public override string GetAbilityAreaOfEffectText(int abilityRange) {
        return abilityRange + "m";
    }
    #nullable disable


}
}
