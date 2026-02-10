using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Displacement_Push_#", menuName = "ScriptableObjects/Active Skill/Displacement/Push")]
public class PushSkillDisplacementSO : SkillDisplacementSO
{
    public class PushDisplacementEvent : DisplacementEvent
    {
#nullable enable
        public PushDisplacementEvent(PartySlot partySlot, List<CombatTileController> combatTiles, ElementSO element, BattleManager battleController, AnimationCurve animationCurve) : base(partySlot, combatTiles, element, battleController, animationCurve) { }

        protected override void DisplacementAction()
        {
            // if (this._partySlot == null || this._partySlot.BattleEntityGO == null) {
            //     this._combatTiles = null;
            //     return;
            // }

            //  If the push target would encounter a tile that's too high, or some form of obstacle, the push should be stopped just before and return the list.
            // if (this._combatTiles == null || this._combatTiles.Count == 0) {
            //     return;
            // }

            // Vector3 tileStep = this._combatTiles[0].Position;
            // Vector3 startingPosition = this._combatTiles[0].GetHeightForSpriteVector3Offset(this._partySlot.GetBattleEntity!.CanFly());



            // this._partySlot.BattleEntityGO.transform.position = BattleProcessingStatic.GetVerticalArc(startingPosition, endingPosition, maxHeight, this.GetTimeStep());
            // this._partySlot.BattleEntityGO.transform.position = Vector3.Lerp(startingPosition, endingPosition, this.GetTimeStep());
            this._partySlot.BattleEntityGO.transform.position = Vector3.Lerp(base.StartPosition, base.EndPosition, base._animationCurve.Evaluate(this.GetTimeStep()));

            // if (this._partySlot.BattleEntityGO.transform.position == tileHeightOffset) {
            //     if (this._startingTile != null && this._startingTile.GetPartyOccupant() == this._partySlot) {
            //         this._startingTile.RemoveOccupantAndProcessEvent(this._battleManager);
            //     }
            //     this._startingTile = this._combatTiles[0];
            //     this._startingTile.SetPartyOccupant(this._battleManager, this._partySlot);
            //     this._partySlot.ReCalculateCommandTiles();
            //     this._combatTiles.RemoveAt(0);              
            //     Debug.Log("Pushing");  
            // }

            // if (this.combatTiles.Count == 0 && this.startingTile != null) {
            //     this.startingTile.SetOccupant(this.partyslot);
            //     this.partyslot.ReCalculateCommandTiles();
            // }
        }

        protected override void DetermineFinalTile()
        {
            CombatTileController finalTile = this._startingTile;
            base._transferrableTiles = new();

            int finalIndex = 0;

            for (int i = 0; i < this._combatTiles.Count; i++)
            {
                CombatTileController combatTile = this._combatTiles[i];
                // PartySlot? collidingSlot = combatTile.GetPartyOccupant();
                List<PartySlot> collidingSlots = combatTile.GetPartyOccupants();
                if (collidingSlots.Count > 0 && !collidingSlots.Contains(this._partySlot))
                {
                    PartySlot collidingSlot = collidingSlots[^1];
                    this._collisionPartySlot = collidingSlot;
                    Debug.Log($"Collision Detected between slot {_partySlot.BattleEntityGO} and {collidingSlot.BattleEntityGO}");
                    this.IsCollision = true;
                    finalIndex++;
                    break;
                }

                float currentTileHeight = finalTile.GetFloatHeightWithSpawnedObject();
                float nextTileHeight = combatTile.GetFloatHeightWithSpawnedObject();

                if (currentTileHeight < nextTileHeight)
                {
                    this.IsCollision = true;
                    break;
                }

                finalIndex = i;
                finalTile = combatTile;
            }

            if (this.IsCollision && finalIndex < _combatTiles.Count)
            {
                this._transferrableTiles = _combatTiles.GetRange(finalIndex, this._combatTiles.Count - finalIndex);
            }

            this._destinationTile = finalTile;

        }

        protected override DisplacementEvent? StartNewDisplacementEvent()
        {
            if (base._collisionPartySlot == null || !base._collisionPartySlot.BattleEntity!.CanBeDisplaced())
            {
                return null;
            }
            return new PushDisplacementEvent(base._collisionPartySlot, base._transferrableTiles, base._element, _battleManager, base._animationCurve);
        }
        #nullable disable

    }

    #nullable enable
    public override void Execute(BattleManager battleController, IBattleActionCommand activeSkill, GenericDictionary<Vector3, CombatTileController> grid, PartySlot user, List<PartySlot> affectedPartySlots)
    {

        affectedPartySlots.Sort((a, b) =>
        {
            float AToUser = Vector3.Distance(a.GetCombatTileController().Position, user.GetCombatTileController().Position);
            float BTouser = Vector3.Distance(b.GetCombatTileController().Position, user.GetCombatTileController().Position);

            if (AToUser < BTouser)
            {
                return -1;
            }

            if (AToUser > BTouser)
            {
                return 1;
            }

            return 0;
        });

        //List of Party Slots to be used for push
        List<PushDisplacementEvent> pushPartySlots = new List<PushDisplacementEvent>();

        foreach (PartySlot affectedPartySlot in affectedPartySlots)
        {
            // if (BattleProcessingStatic.PartySlotIsSpawnablePartySlot(affectedPartySlot))
            // {
            //     continue;
            // }

            Debug.Log("affected PUSH party slot member: " + affectedPartySlot.BattleEntity!.GetName());
            if (!this.CanExecute(grid, user, affectedPartySlot))
            {
                Debug.Log("Cannot Execute Push");
                continue;
            }

            CombatTileController? userTile = grid[user.GetCombatTileController().Position];
            CombatTileController? targetTile = grid[affectedPartySlot.GetCombatTileController().Position];
            if (userTile == null || targetTile == null)
            {
                Debug.Log("No Tile hits");
                continue;
            }

            List<CombatTileController> combatTiles = this.CanbePushed(userTile, targetTile, activeSkill);
            if (combatTiles.Count == 0)
            {
                Debug.LogWarning("CANNOT DISPLACE");
                continue;
            }
            pushPartySlots.Add(new PushDisplacementEvent(affectedPartySlot, combatTiles, base._element, battleController, base._animationCurve));
        }

        // this.PrintTargetOrder(pushPartySlots);


        foreach (PushDisplacementEvent pushEvent in pushPartySlots)
        {
            battleController.BattleEventController.AddBattleEvent(pushEvent);
        }

        return;
    }

    protected bool CanExecute(GenericDictionary<Vector3, CombatTileController>? grid, PartySlot? user, PartySlot affectedPartySlot) {
        if (grid == null || user == null || affectedPartySlot == null) {
            return false;
        }

        return affectedPartySlot.BattleEntity!.CanBeDisplaced();
    }

    //Returns push path if it exists.
    private List<CombatTileController> CanbePushed(CombatTileController userTile, CombatTileController targetTile, IBattleActionCommand activeSkill) {
        List<RaycastHit> raycastHits = new ();
        Vector3 fromPosition = userTile.transform.position;

        Vector3 toPosition = targetTile.transform.position;

        Vector3 direction = (toPosition - fromPosition).normalized;

        float distance = Vector3.Distance(fromPosition, toPosition);

        //First, ray cast from user to target to get all raycast hits
        raycastHits.AddRange(Physics.RaycastAll(fromPosition, direction, distance));
        Debug.DrawRay(fromPosition, direction * distance, Color.red, 60);
        if(raycastHits.Count == 0) {
            Debug.Log("Ray Cast from caster to target had no hits");
            return new();
        }
        raycastHits.Sort( (a, b) => a.distance.CompareTo(b.distance));



        List<PartySlot> affectedPartySlots = new();

        //Next, get list of all combat tile controllers on the path.

        List<CombatTileController> combatTileControllers = this.GetCombatTileControllersByRayCasts(raycastHits);

        if (combatTileControllers.Count == 0) {
            Debug.Log("No hits for combat tile controllers along ray cast path");
            return new();
        }

        foreach (CombatTileController combatTileController in combatTileControllers)
        {
            // if (combatTileController.GetPartyOccupant() == null)
            // {
            //     continue;
            // }
            // affectedPartySlots.Add(combatTileController.GetPartyOccupant()!);
            affectedPartySlots.AddRange(combatTileController.GetPartyOccupants());
        }


        if (affectedPartySlots.Count == 0) {
            return new();
        }

        //Next, Sort the Party slots along the path to get the closest target.
        affectedPartySlots = sortPartySlotsByDistanceFromUser(userTile.Position, affectedPartySlots);
        if (!targetTile.GetPartyOccupants().Contains(affectedPartySlots[0])) {
            return new();
        }

        PartySlot pushTarget = affectedPartySlots[0];


        //  Next, use ray casts to get path of the push.  
        raycastHits.Clear();

        fromPosition = pushTarget.GetCombatTileController().Position;
        
        raycastHits.AddRange(Physics.RaycastAll(fromPosition, direction, this.displaceDistance));

        Debug.DrawRay(fromPosition, direction * this.displaceDistance, Color.red, 60);
        if (raycastHits.Count == 0)
        {
            return new();
            
        }
        raycastHits.Sort( (a, b) => a.distance.CompareTo(b.distance));
        

        return this.GetCombatTileControllersByRayCasts(raycastHits);
    }

    private List<CombatTileController> GetCombatTileControllersByRayCasts(List<RaycastHit> raycastHits) {

        List<CombatTileController> combatTileControllers = new List<CombatTileController>();
        foreach(RaycastHit raycastHit in raycastHits) {
            GameObject? tileObject = raycastHit.transform.gameObject;
            if (tileObject == null || !tileObject.TryGetComponent<CombatTileController>(out CombatTileController combatTile)) {
                continue;
            }

            if (combatTileControllers.Contains(combatTile)) {
                continue;
            }

            combatTileControllers.Add(combatTile);
        }
        return combatTileControllers;
    }

    private List<PartySlot> sortPartySlotsByDistanceFromUser(Vector3 userCoordinates, List<PartySlot> affectedPartySlots) {

        List<PartySlot> newAffectedPartySlots = new(affectedPartySlots);
        newAffectedPartySlots.Sort((a, b) => {
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

        return newAffectedPartySlots;
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