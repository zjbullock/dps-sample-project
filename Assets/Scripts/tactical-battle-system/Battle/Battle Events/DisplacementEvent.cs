using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat
{
   [System.Serializable]
public abstract class DisplacementEvent : IBattleEvent
{
    [Header("Config")]
    [SerializeField]
    protected BattleManager _battleManager;

    [SerializeField]
    protected AnimationCurve _animationCurve;

    #nullable enable
    [SerializeField, SerializeReference]
    protected PartySlot _partySlot;

    public PartySlot PartySlot { get => this._partySlot; }

    [SerializeField, SerializeReference]
    protected PartySlot? _collisionPartySlot = null;

    public PartySlot? CollisionPartySlot { get => this._collisionPartySlot; }

    [SerializeField]
    protected ElementSO _element;

    [Header("Runtime Values")]

    [SerializeField]
    protected bool _isDone;

    [SerializeField]
    private bool _isCollision;

    protected bool IsCollision { get => this._isCollision; set => this._isCollision = value; }


    [SerializeField]
    private float _elapsedTime;

    [SerializeField]
    private Vector3 _startPosition;

    public Vector3 StartPosition { get => this._startPosition; }

    [SerializeField]
    private Vector3 _endPosition;

    public Vector3 EndPosition { get => this._endPosition; }




    [Space]
    [Header("Combat Tiles")]

    [SerializeField]
    protected CombatTileController _startingTile;

    [SerializeField]
    protected CombatTileController? _destinationTile = null;

    [SerializeField]
    protected List<CombatTileController> _combatTiles = new();

    [SerializeField]
    protected List<CombatTileController> _transferrableTiles = new();


    public DisplacementEvent(PartySlot partySlot, List<CombatTileController> combatTiles, ElementSO element, BattleManager battleManager, AnimationCurve animationCurve)
    {
        this._partySlot = partySlot;
        this._combatTiles = combatTiles;
        this._battleManager = battleManager;
        this._animationCurve = animationCurve;
        this._element = element;
        this._destinationTile = null;
        this._isDone = false;
        this._elapsedTime = 0f;

        battleManager.BattleCameraManager.CameraTracker.SetTarget(this._partySlot.BattleEntityGO.TargetOffsetAnchorPoint.transform, false, true);

        this._startingTile = partySlot.GetCombatTileController();
        this.DetermineFinalTile();

        if (_destinationTile == null)
        {
            this._isDone = true;
            return;
        }

        if (BattleProcessingStatic.PartySlotIsSpawnablePartySlot(this._partySlot))
        {
            this.HandleInteractableObjectPartySlot();
        }
        else
        {
            this._partySlot.GetCombatTileController().RemoveOccupantAndProcessEvent(battleManager);
            this._destinationTile.SetPartyOccupant(battleManager, this._partySlot);
            this._startPosition = this._startingTile.GetGameObjectAnchor().transform.position;
            this._endPosition = this._destinationTile!.GetGameObjectAnchor().transform.position;
        }


    }

    private void HandleInteractableObjectPartySlot()
    {

        SpawnableObjectPartySlot? spawnableObjectPartySlot = this._partySlot.GetCombatTileController().RemoveSpawnableObject(this._battleManager);
        if (spawnableObjectPartySlot == null)
        {
            this._isDone = true;
            return;
        }

        PartySlot? partySlot = this._partySlot.GetCombatTileController().RemoveOccupantAndProcessEvent(this._battleManager);
        this._startPosition = this._startingTile.GetGameObjectAnchor().transform.position;
        this._endPosition = this._destinationTile!.GetGameObjectAnchor().transform.position;
        this._destinationTile.SetInteractableObjectPartySlot(this._battleManager, spawnableObjectPartySlot);
        if (partySlot != null)
        {
            this._destinationTile.SetPartyOccupant(this._battleManager,partySlot);
        }

    }

    public void Execute()
    {
        // if(this._startingTile == null || this._combatTiles.Count == 0) {
        //     Debug.Log("Can't displace");
        //     // this.partySlot?.GetCombatTileController().ToggleControlPosition(true);
        //     return;
        // }

        // if (this._partySlot == null) {
        //     return;
        // }

        // PartySlot? collisionPartySlot = this._combatTiles[0].GetPartyOccupant();
        // if (collisionPartySlot != null && collisionPartySlot != this._partySlot) {
        //     Debug.Log("Displacement Tile already filled");
        //     if (this._partySlot.GetBattleEntity!.CanFly() != collisionPartySlot!.GetBattleEntity!.CanFly()) {
        //         this.EndDisplacement();
        //         return;
        //     }
        //     this.CollisionDamage(this._partySlot);
        //     // this.partySlot.GetCombatTileController().ToggleControlPosition(true);

        //     this.CollisionDamage(collisionPartySlot);
        //     if (collisionPartySlot.GetBattleEntity!.CanBeDisplaced())
        //     {
        //         Debug.Log("Switching collision displacement target");
        //         Debug.Log("New collision target: " + collisionPartySlot.GetBattleEntity!.GetName());
        //         // this.partySlot = collisionPartySlot;
        //         this._startingTile = collisionPartySlot.GetCombatTileController();
        //         this._combatTiles.RemoveAt(0);     
        //         _battleManager.BattleEventController.AddBattleEvent(this.StartNewDisplacementEvent());
        //     }
        //     this.EndDisplacement();


        //     return;
        // }

        // // Debug.Log("this party slot's current tile coords" + this.partySlot.GetCombatTileController().Position);
        // // this.partySlot.GetCombatTileController().ToggleControlPosition(false);
        // if (!_combatTiles[0].CanAddOccupant(this._partySlot)) {
        //     this.EndDisplacement();
        //     return;
        // }

        // float distance = this._combatTiles[0].GetFloatHeightWithSpawnedObject() - this._startingTile.GetFloatHeightWithSpawnedObject();
        // if ((!this._partySlot.GetBattleEntity!.CanFly() &&  distance > 0f)) {
        //     this.CollisionDamage(this._partySlot);
        //     this.EndDisplacement();
        //     return;
        // }
        if (this._isDone)
        {
            return;
        }

        this.ProcessTime();
        this.DisplacementAction();
        this.DetermineState();
    }

    protected abstract void DetermineFinalTile();

    // protected void DetermineFinalTile()
    // {
    //     CombatTileController finalTile = this._startingTile;
    //     int finalIndex = 0;
    //     for (int i = 0; i < this._combatTiles.Count; i++)
    //     {
    //         CombatTileController combatTile = this._combatTiles[i];
    //         PartySlot? collidingSlot = combatTile.GetPartyOccupant();
    //         if (collidingSlot != null && collidingSlot != this._partySlot)
    //         {
    //             this._collisionPartySlot = collidingSlot;
    //             Debug.LogError($"Collision Detected between slot {_partySlot.BattleEntityGO} and {collidingSlot.BattleEntityGO}");
    //             break;
    //         }

    //         finalIndex = i;
    //         finalTile = combatTile;
    //     }

    //     this._destinationTile = finalTile;
    //     this._transferrableTiles = _combatTiles.GetRange(finalIndex, this._combatTiles.Count - finalIndex);
    // }

    protected void ProcessTime()
    {
        if (this._elapsedTime >= this._battleManager.BattleValues.DisplacementTime)
        {
            return;
        }

        this._elapsedTime += Time.deltaTime;
    }

    protected float GetTimeStep()
    {
        return this._elapsedTime / this._battleManager.BattleValues.DisplacementTime;
    }

    protected void DetermineState()
    {
        if (this._elapsedTime < this._battleManager.BattleValues.DisplacementTime)
        {
            return;
        }

        this._isDone = true;
    }

    public bool IsDone()
    {
        return this._isDone;
    }

    public void End() {
        Debug.Log("Ending Displacement");
        this.EndDisplacement();
        return;
    }

    protected abstract void DisplacementAction();

    protected abstract DisplacementEvent? StartNewDisplacementEvent();

    protected void EndDisplacement()
    {
        this.HandleCollision();
        if (BattleProcessingStatic.PartySlotIsSpawnablePartySlot(this._partySlot))
        {
            this._partySlot.GetCombatTileController().ResetSpawnableObjectPartyOccupantParent();
        }
        else
        {
            this._partySlot.GetCombatTileController().ResetPartyOccupantParent();
        }
    }

    private void HandleCollision()
    {
        if (!this._isCollision)
        {
            return;
        }
        this.CollisionDamage(this._partySlot);

        if (this._collisionPartySlot == null)
        {
            return;
        }

        if(BattleManager.instance == null) {
            return;
        }
        
        BattleManager.instance.BattleEventController.ReactingPartySlotHandler.AddReactingPartySlots(new(){this._collisionPartySlot}, this._element);

        this.CollisionDamage(this._collisionPartySlot);
        DisplacementEvent? displacementEvent = StartNewDisplacementEvent();
        if (displacementEvent == null)
        {
            return;
        }
        this._battleManager.BattleEventController.AddBattleEvent(displacementEvent);
    }
    
    

    private void CollisionDamage(PartySlot partySlot)
    {
        if (partySlot == null)
        {
            return;
        }
        int damage = BattleProcessingStatic.GetPercentDamage(partySlot.BattleEntity!.GetRawStats().maxHp, this._battleManager.BattleValues.DisplacementCollisionDamage);
        partySlot.ProcessDamage(this._battleManager, damage, _element, "Collision");
        return;
    }

    #nullable disable
}
 
}
