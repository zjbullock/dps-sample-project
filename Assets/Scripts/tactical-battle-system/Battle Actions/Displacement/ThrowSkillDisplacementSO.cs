using System.Collections.Generic;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Displacement_Throw_#", menuName = "ScriptableObjects/Active Skill/Displacement/Throw")]
public class ThrowSkillDisplacementSO : SkillDisplacementSO
{
    [SerializeField]
    private AnimationCurve _heightAnimationCurve;
    public class ThrowDisplacementEvent : DisplacementEvent
    {



        [SerializeField]
        private float _maxHeight;

        [SerializeField]
        private float _minimumHeight;

        public ThrowDisplacementEvent(PartySlot partySlot, List<CombatTileController> combatTiles, ElementSO element, BattleManager battleController, AnimationCurve animationCurve, AnimationCurve heightAnimationCurve) : base(partySlot, combatTiles, element, battleController, animationCurve)
        {
        }

#nullable enable
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

            this._partySlot.BattleEntityGO.transform.position = BattleProcessingStatic.GetVerticalArc(base.StartPosition, base.EndPosition, this._maxHeight, this.GetTimeStep());
            // this._partySlot.BattleEntityGO.transform.position = Vector3.MoveTowards(this._partySlot.BattleEntityGO.transform.position, tileHeightOffset, this._displacementSpeed * Time.deltaTime);

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


            this._destinationTile = this.GetFinalTile();
            this._maxHeight = this.GetMaxHeight();
            this._transferrableTiles = new();
        }

        private CombatTileController GetFinalTile()
        {
            CombatTileController finalTile = this._startingTile;

            bool isSpawnableObject = BattleProcessingStatic.PartySlotIsSpawnablePartySlot(base._partySlot);

            for (int i = this._combatTiles.Count - 1; i > 0; i--)
            {
                CombatTileController combatTile = this._combatTiles[i];
                if ((!isSpawnableObject && !combatTile.HasPartyOccupant()) || (isSpawnableObject && combatTile.GetPartyOccupants().Count == 0))
                {
                    finalTile = combatTile;
                    break;
                }

            }

            return finalTile;
        }

        private float GetMaxHeight()
        {
            float maxHeight = this._startingTile.GetGameObjectAnchor().transform.position.y;

            for (int i = this._combatTiles.Count - 1; i > 0; i--)
            {
                if (maxHeight < this._combatTiles[i].GetGameObjectAnchor().transform.position.y)
                {
                    maxHeight = this._combatTiles[i].GetGameObjectAnchor().transform.position.y;
                }

                if (this._combatTiles[i] == this._destinationTile)
                {
                    break;
                }
            }

            return (maxHeight + 3f) * 2;
        }

        protected override DisplacementEvent? StartNewDisplacementEvent()
        {
            return null;
        }

    }


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
        List<ThrowDisplacementEvent> thrownPartySlots = new();

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

            List<CombatTileController> combatTiles = this.CanBeThrown(userTile, targetTile, activeSkill);
            if (combatTiles.Count == 0)
            {
                Debug.LogWarning("CANNOT DISPLACE");
                continue;
            }
            thrownPartySlots.Add(new ThrowDisplacementEvent(affectedPartySlot, combatTiles, base._element, battleController, base._animationCurve, this._heightAnimationCurve));
        }

        // this.PrintTargetOrder(pushPartySlots);


        foreach (ThrowDisplacementEvent pushEvent in thrownPartySlots)
        {
            battleController.BattleEventController.AddBattleEvent(pushEvent);
        }

        return;
    }

    //Returns push path if it exists.
    private List<CombatTileController> CanBeThrown(CombatTileController userTile, CombatTileController targetTile, IBattleActionCommand activeSkill)
    {
        List<RaycastHit> raycastHits = new();
        Vector3 fromPosition = userTile.transform.position;

        Vector3 toPosition = targetTile.transform.position;

        Vector3 direction = (toPosition - fromPosition).normalized;

        float distance = Vector3.Distance(fromPosition, toPosition);

        //First, ray cast from user to target to get all raycast hits
        raycastHits.AddRange(Physics.RaycastAll(fromPosition, direction, distance));
        Debug.DrawRay(fromPosition, direction * distance, Color.red, 60);
        if (raycastHits.Count == 0)
        {
            Debug.Log("Ray Cast from caster to target had no hits");
            return new();
        }
        raycastHits.Sort((a, b) => a.distance.CompareTo(b.distance));



        List<PartySlot> affectedPartySlots = new();

        //Next, get list of all combat tile controllers on the path.

        List<CombatTileController> combatTileControllers = this.GetCombatTileControllersByRayCasts(raycastHits);

        if (combatTileControllers.Count == 0)
        {
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


        if (affectedPartySlots.Count == 0)
        {
            return new();
        }

        //Next, Sort the Party slots along the path to get the closest target.
        affectedPartySlots = sortPartySlotsByDistanceFromUser(userTile.Position, affectedPartySlots);
        if (!targetTile.GetPartyOccupants().Contains(affectedPartySlots[0]))
        {
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
        raycastHits.Sort((a, b) => a.distance.CompareTo(b.distance));


        return this.GetCombatTileControllersByRayCasts(raycastHits);
    }

    private List<PartySlot> sortPartySlotsByDistanceFromUser(Vector3 userCoordinates, List<PartySlot> affectedPartySlots)
    {

        List<PartySlot> newAffectedPartySlots = new(affectedPartySlots);
        newAffectedPartySlots.Sort((a, b) =>
        {
            float AToUser = Vector3.Distance(a.GetCombatTileController().Position, userCoordinates);
            float BTouser = Vector3.Distance(b.GetCombatTileController().Position, userCoordinates);

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

        return newAffectedPartySlots;
    }


    private List<CombatTileController> GetCombatTileControllersByRayCasts(List<RaycastHit> raycastHits)
    {

        List<CombatTileController> combatTileControllers = new();
        foreach (RaycastHit raycastHit in raycastHits)
        {
            GameObject? tileObject = raycastHit.transform.gameObject;
            if (tileObject == null || !tileObject.TryGetComponent<CombatTileController>(out CombatTileController combatTile))
            {
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

    protected bool CanExecute(GenericDictionary<Vector3, CombatTileController>? grid, PartySlot? user, PartySlot affectedPartySlot)
    {
        if (grid == null || user == null || affectedPartySlot == null)
        {
            return false;
        }

        return affectedPartySlot.BattleEntity!.CanBeDisplaced();
    }
            #nullable disable

}
}