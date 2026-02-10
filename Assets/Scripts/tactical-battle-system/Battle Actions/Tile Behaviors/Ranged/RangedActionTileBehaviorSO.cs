using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat{
public abstract class RangedActionTileBehaviorSO : BattleTileTargetBehaviorSO
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

    #nullable enable

    public override GenericDictionary<Vector3, CombatTileController> GetActionTilesByAreaOfEffect(CombatTileController? combatTile, int abilityRange, float abilityVerticalRange, float entityHeight, bool isFlying)
    {
        if (combatTile == null) {
            return new GenericDictionary<Vector3, CombatTileController>();
        }
        GenericDictionary<Vector3, CombatTileController> possibleActionTiles = new GenericDictionary<Vector3, CombatTileController>();
        possibleActionTiles.Add(combatTile.Position, combatTile);
        //Testing Activation of Tile for cursor
        GenericDictionary<CombatTileController, int> combatTileRayCounter = new GenericDictionary<CombatTileController, int>();
        float totalAngle = 360;
        float delta = totalAngle /  this.rayCount;
        //Perform a raycast from the center of the current tile, and get all tiles hit by it.  If entityHeight > raycasted object's height, add the tile's coordinate to the possibleActionTiles
        for (int i = 0; i < this.rayCount; i++) {
            
            Vector3 dir = Quaternion.Euler(0, i * delta, 0) * combatTile.transform.right;
            List<RaycastHit> raycastHits = new List<RaycastHit>();
            RaycastHit hit;
            Vector3 truePosition = new Vector3(combatTile.Position.x, combatTile.Position.y, combatTile.Position.z);
            truePosition.y = 999f;
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
                    float totalHeight = 999f;
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

    public override GenericDictionary<Vector3, CombatTileController> GetSelectActionTilesByAbilityRange(CombatTileController combatTile, int abilityRange, float abilityVerticalRange, float entityHeight,bool isFlying)
    {
        GenericDictionary<Vector3, CombatTileController> possibleActionTiles = new GenericDictionary<Vector3, CombatTileController>();
        possibleActionTiles.Add(combatTile.Position, combatTile);
        //Testing Activation of Tile for cursor


        List<RaycastHit> sphereRayCastHits = new List<RaycastHit>();
        Vector3 sphereDir = Quaternion.Euler(0, 0, 0) * combatTile.transform.right;

        sphereRayCastHits.AddRange(Physics.SphereCastAll(combatTile.Position, abilityRange, sphereDir, 0));
        foreach (RaycastHit raycast in sphereRayCastHits) {
            GameObject tileObject = raycast.transform.gameObject;
            if (tileObject != null) {
                if(tileObject.TryGetComponent<CombatTileController>(out CombatTileController combatTileController)) {
                    bool added = this.AddPossibleTile(possibleActionTiles, new KeyValuePair<Vector3, CombatTileController>(combatTileController.Position, combatTileController));

                }
                //Converts the ability range to units relevant to the game's standard tile height of 0.25f.
            }
        }
        return possibleActionTiles;
    }

    public override bool ShouldDistanceCheck()
    {
        return true;
    }

    #nullable enable


    public override string GetAbilityAreaOfEffectText(int abilityRange) {
        return abilityRange + "m";
    }
    #nullable disable

}
}