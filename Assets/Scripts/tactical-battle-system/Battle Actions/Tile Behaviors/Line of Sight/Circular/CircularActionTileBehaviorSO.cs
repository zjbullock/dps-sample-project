using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat{
public abstract class CircularActionTileBehaviorSO : BattleTileTargetBehaviorSO
{
    public override bool GetActionableTilesForEnemy()
    {
        return base.GetActionableTilesForEnemy();
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

            foreach (RaycastHit raycast in raycastHits)
            {
                GameObject tileObject = raycast.transform.gameObject;
                if (tileObject == null)
                {
                    //Converts the ability range to units relevant to the game's standard tile height of 0.25f.
                    continue;
                }
                
                CombatTileController combatTileController = tileObject.GetComponent<CombatTileController>();

                if (combatTileController == null)
                {
                    continue;
                }
            
                if (!combatTileRayCounter.ContainsKey(combatTileController)) {
                    combatTileRayCounter[combatTileController] = 0;
                } else {
                    combatTileRayCounter[combatTileController]++;
                }

                
                // float distanceToBattleEntity = Mathf.Abs(totalHeight - combatTileController.GetHeightWithSpawnedObjectAndPresentEntity());

                // bool canHitTarget = distanceToBattleEntity <= abilityVerticalRange;
                // float distanceToStructure = Mathf.Abs(totalHeight - combatTileController.GetHeightWithSpawnedObject());

                // bool canHitObject = distanceToStructure <= abilityVerticalRange;



                // if (combatTileRayCounter[combatTileController] >= this.rayCastNecessary && (!this.requireLineOfSight || (isFlying || canHitTarget || canHitObject)) ) {
                if (combatTileRayCounter[combatTileController] >= this.rayCastNecessary ) {

                    bool added = this.AddPossibleTile(possibleActionTiles, new KeyValuePair<Vector3, CombatTileController>(combatTileController.Position, combatTileController));
                }
            }
        }
        return possibleActionTiles;
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
