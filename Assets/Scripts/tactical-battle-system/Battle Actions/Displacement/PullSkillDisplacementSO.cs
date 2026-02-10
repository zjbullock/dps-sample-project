using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat{

[CreateAssetMenu(fileName = "Displacement_Pull_#", menuName = "ScriptableObjects/Active Skill/Displacement/Pull")]
public class PullSkillDisplacementSO : SkillDisplacementSO
{
    public class PullDisplacementEvent : DisplacementEvent
    {
        #nullable enable

        public PullDisplacementEvent(PartySlot partySlot, List<CombatTileController> combatTiles, ElementSO element, BattleManager battleController, AnimationCurve animationCurve) : base(partySlot, combatTiles, element, battleController, animationCurve) { }

        protected override void DisplacementAction()
        {
            // if (this._partySlot == null || this._partySlot.BattleEntityGO == null)
            // {
            //     this._combatTiles = null;
            //     return;
            // }

            //  If the pull target would encounter a tile that's too high, or some form of obstacle, the pull should be stopped just before and return the list.
            // if (this._combatTiles == null || this._combatTiles.Count == 0)
            // {
            //     Debug.Log("Pull Event has no combat tiles");
            //     return;
            // }

            // Vector3 tilePosition = this._combatTiles[0].GetHeightForSpriteVector3Offset(this._partySlot.GetBattleEntity!.CanFly());
            // Debug.Log(tilePosition);

            // this._partySlot.BattleEntityGO.transform.position = Vector3.MoveTowards(this._partySlot.BattleEntityGO.transform.position, tilePosition, base._displacementSpeed * Time.deltaTime);
            // Vector3 startingPosition = this._startingTile.GetGameObjectAnchor().transform.position;
            // Vector3 endingPosition = this._destinationTile!.GetGameObjectAnchor().transform.position;

            // float maxHeight = (startingPosition.y + endingPosition.y + 1f) * 2f;

            // this._partySlot.BattleEntityGO.transform.position = BattleProcessingStatic.GetVerticalArc(startingPosition, endingPosition, maxHeight, this.GetTimeStep());

            // Vector3 startingPosition = this._startingTile.GetGameObjectAnchor().transform.position;
            // Vector3 endingPosition = this._destinationTile!.GetGameObjectAnchor().transform.position;


            // this._partySlot.BattleEntityGO.transform.position = BattleProcessingStatic.GetVerticalArc(startingPosition, endingPosition, maxHeight, this.GetTimeStep());
            // this._partySlot.BattleEntityGO.transform.position = Vector3.Lerp(startingPosition, endingPosition, this.GetTimeStep());
            // this._partySlot.BattleEntityGO.transform.position = Vector3.Lerp(startingPosition, endingPosition, base._animationCurve.Evaluate(this.GetTimeStep()));
            this._partySlot.BattleEntityGO.transform.position = Vector3.Lerp(base.StartPosition, base.EndPosition, base._animationCurve.Evaluate(this.GetTimeStep()));

            // if (this._partySlot.BattleEntityGO.transform.position != tilePosition)
            // {
            //     return;
            // }

            // if (this._startingTile != null && this._startingTile.GetPartyOccupant() == this._partySlot)
            // {
            //     this._startingTile.RemoveOccupantAndProcessEvent(this._battleManager);
            // }
            // this._startingTile = this._combatTiles[0];
            // this._startingTile.SetPartyOccupant(this._battleManager, this._partySlot);
            // this._partySlot.ReCalculateCommandTiles();
            // this._combatTiles.RemoveAt(0);

            // if (this.combatTiles.Count == 0 && this.startingTile != null) {
            //     this.startingTile.SetOccupant(this.partyslot);
            //     this.partyslot.ReCalculateCommandTiles();
            // }
        }
        protected override void DetermineFinalTile()
        {
            CombatTileController finalTile = this._startingTile;

            int finalIndex = 0;

            for (int i = 0; i < this._combatTiles.Count; i++)
            {
                CombatTileController combatTile = this._combatTiles[i];
                List<PartySlot> collidingSlots = combatTile.GetPartyOccupants();
                if (collidingSlots.Count > 0 && !collidingSlots.Contains(this._partySlot))
                {
                    // this._collisionPartySlot = collidingSlot;
                    // this.IsCollision = true;
                    continue;
                }

                finalIndex = i;
                finalTile = combatTile;
                break;
            }

            this._destinationTile = finalTile;
            this._transferrableTiles = _combatTiles.GetRange(finalIndex, this._combatTiles.Count - finalIndex);
        }
        protected override DisplacementEvent? StartNewDisplacementEvent()
        {
            // if (base._collisionPartySlot == null)
            // {
            //     return null;
            // }
            // return new PullDisplacementEvent(base._collisionPartySlot, base._transferrableTiles, base._element, _battleManager, base._animationCurve);
            return null;
        }
        #nullable disable

    }

    #nullable enable
    public override void Execute(BattleManager battleController, IBattleActionCommand activeSkill, GenericDictionary<Vector3, CombatTileController> grid, PartySlot user, List<PartySlot> affectedPartySlots)
    {

        affectedPartySlots.Sort((a, b) =>
        {
            float AToUser = Vector3.Distance(a.GetCombatTileController().Position, user.GetCombatTileController().Position);
            float BToUser = Vector3.Distance(b.GetCombatTileController().Position, user.GetCombatTileController().Position);

            if (AToUser < BToUser)
            {
                return -1;
            }

            if (AToUser > BToUser)
            {
                return 1;
            }

            return 0;
        });

        //List of Party Slots to be used for pull
        List<PullDisplacementEvent> pullPartySlots = new List<PullDisplacementEvent>();

        foreach (PartySlot affectedPartySlot in affectedPartySlots)
        {
            Debug.Log("affected party slot member: " + affectedPartySlot.BattleEntity!.GetName());
            if (!this.CanExecute(grid, user, affectedPartySlot))
            {
                Debug.Log("Cannot Execute Pull");
                continue;
            }

            CombatTileController? userTile = grid[user.GetCombatTileController().Position];
            CombatTileController? targetTile = grid[affectedPartySlot.GetCombatTileController().Position];
            if (userTile == null || targetTile == null)
            {
                Debug.Log("No Tile hits");
                continue;
            }

            List<CombatTileController> combatTiles = this.CanBePulled(userTile, targetTile);
            if (combatTiles == null || combatTiles.Count == 0)
            {
                Debug.Log("targeted tile entity cannot be pulled: " + affectedPartySlot.BattleEntity!.GetName());
                continue;
            }
            pullPartySlots.Add(new PullDisplacementEvent(affectedPartySlot, combatTiles, base._element, battleController, base._animationCurve));
        }

        // this.PrintTargetOrder(pullPartySlots);


        foreach (PullDisplacementEvent pullEvent in pullPartySlots)
        {
            battleController.BattleEventController.AddBattleEvent(pullEvent);
        }

        return;
    }

    protected bool CanExecute(GenericDictionary<Vector3, CombatTileController>? grid, PartySlot? user, PartySlot affectedPartySlot) {
        if (grid == null || user == null || affectedPartySlot == null) {
            return false;
        }

        return affectedPartySlot.BattleEntity!.CanBeDisplaced();
    }

    //Returns pull path if it exists.
    private List<CombatTileController> CanBePulled(CombatTileController userTile, CombatTileController targetTile) {
        List<RaycastHit> raycastHits = new();

        Vector3 fromPosition = userTile.transform.position;

        Vector3 toPosition = targetTile.transform.position;

        Vector3 direction = (toPosition - fromPosition).normalized;

        float distance = Vector3.Distance(fromPosition, toPosition);

        //First, ray cast from user to target to get all raycast hits
        raycastHits.AddRange(Physics.RaycastAll(fromPosition, direction, distance));
        // Debug.DrawRay(fromPosition, direction * distance, Color.magenta, 60);
        if(raycastHits.Count == 0) {
            Debug.Log("Ray Cast from caster to target had no hits");
            return new();
        }

        raycastHits.Sort( (a, b) => a.distance.CompareTo(b.distance));



        List<PartySlot> affectedPartySlots = new ();

        //Next, get list of all combat tile controllers on the path.

        List<CombatTileController> combatTileControllers = this.GetCombatTileControllersByRayCasts(raycastHits);

        if (combatTileControllers.Count == 0) {
            Debug.Log("No hits for combat tile controllers along ray cast path");
            return combatTileControllers;
        }

        foreach(CombatTileController combatTileController in combatTileControllers) {
            affectedPartySlots.AddRange(combatTileController.GetPartyOccupants());
        }


        if (affectedPartySlots.Count == 0) {
            Debug.Log("No affected party slots");
            return combatTileControllers;
        }

        //Next, Sort the Party slots along the path to get the closest target.
        affectedPartySlots = sortPartySlotsByDistanceFromUser(userTile.Position, affectedPartySlots);
        if (affectedPartySlots[0] != targetTile.GetPartyOccupant()) {
            return combatTileControllers;
        }

        PartySlot pullTarget = affectedPartySlots[0];


        //  Next, use ray casts to get path of the push.
        Vector3 pullTargetPosition = pullTarget.GetCombatTileController().Position;
        direction = (fromPosition - pullTargetPosition).normalized;
        raycastHits.Clear();

        float pullDistance = distance < this.displaceDistance ? distance : this.displaceDistance;

        raycastHits.AddRange(Physics.RaycastAll(pullTargetPosition, direction, pullDistance));
        Debug.DrawRay(pullTargetPosition, direction * pullDistance, Color.magenta, 60);
        
        if (raycastHits.Count == 0)
        {
            return combatTileControllers;
        }
        raycastHits.Sort( (a, b) => b.distance.CompareTo(a.distance));
        

        return this.GetCombatTileControllersByRayCasts(raycastHits);
    }

    private List<CombatTileController> GetCombatTileControllersByRayCasts(List<RaycastHit> raycastHits) {

        List<CombatTileController> combatTileControllers = new List<CombatTileController>();
        foreach(RaycastHit raycastHit in raycastHits) {
            GameObject? tileObject = raycastHit.transform.gameObject;
            if (tileObject == null || !tileObject.TryGetComponent<CombatTileController>(out CombatTileController combatTile)) {
                continue;
            }

            if (combatTileControllers.Contains(combatTile))
            {
                continue;
            }
            combatTileControllers.Add(combatTile);
        }
        return combatTileControllers;
    }

    private List<PartySlot> sortPartySlotsByDistanceFromUser(Vector3 userCoordinates, List<PartySlot> affectedPartySlots) {
        affectedPartySlots.Sort((a, b) => {
            float AToUser = Vector3.Distance(a.GetCombatTileController().Position, userCoordinates);
            float BTouser = Vector3.Distance(b.GetCombatTileController().Position, userCoordinates);
            
            if (AToUser < BTouser) {
                return -1;
            }

            if (AToUser > BTouser) {
                return 1;
            }

            return 0;
        });

        return affectedPartySlots;
    }

    private void PrintTargetOrder(List<PartySlot> affectedPartySlots) {
        if (affectedPartySlots.Count == 0) {
            return;
        }

        Debug.Log("Party slot Order: ");
        foreach(PartySlot partySlot in affectedPartySlots) {
            Debug.Log("Name: " + partySlot.BattleEntity!.GetName());
        }
    }

    
}
}