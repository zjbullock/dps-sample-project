using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DPS.Common;

namespace DPS.TacticalCombat{
[Serializable]
public class CombatTileController : MonoBehaviour {

    [Header("Component References")]
    [SerializeField]
    private TileGlowController actionGlow;

    [SerializeField]
    private TileGlowController confirmActionGlow;

    [SerializeField]
    private TileGlowController moveGlow;

    [SerializeField]
    private TileGlowController projectedGlow;

    [SerializeField]
    private TileGlowController occupiedGlow;

    [SerializeField]
    private Renderer tile;

    public Renderer Tile { get => this.tile; }

    [SerializeReference]
    [Tooltip("The anchor point for the tile, where inhabitants will stay upon")]
    private GameObject anchorPoint;

    [Header("Run Time Values")]

    [SerializeField, SerializeReference]
    [Tooltip("Party Slots that are currently keeping this party slot alive")]
    private List<BattleMember> projectedGlowBattleMembers;


    //This controls the logic for Combat Tile Interactions.
    // [SerializeField]
    // private List<CombatTileInteractionSO> combatTileInteractions;

    // public Vector3 tileOccupantOffset;

    [Tooltip("Retrieved at runtime.  Helps with node traversal of the grid")]
    private Vector3Int position;

    public Vector3Int Position {set => this.position = value; get => this.position; }

    [Tooltip("Retrieved at runtime.  Helps with getting height of the tile")]
    [SerializeField]
    private float height;

    public float Height { get => this.height; }

    public List<CombatTileController> movementTiles;


    [SerializeField, SerializeReference]
    public CombatTileControllerBaseState currentstate;

    [Tooltip("Keep track of what states have already been entered")]
    [SerializeField, SerializeReference]
    public GenericDictionary<CombatTileInteractionSO, CombatTileControllerBaseState> previousStates = new GenericDictionary<CombatTileInteractionSO, CombatTileControllerBaseState>();

    [Tooltip("Determines the default state of the tile")]
    [SerializeField]    
    private CombatTileControllerDefaultState defaultState;


    [Header("Control the Starting Spawnable Battle Object")]
    [Tooltip("Determines the starting state of the tile.  This can be different from the default state.  This state is only applied during the Awake")]
    [SerializeReference]
    private SpawnableBattleObject startingSpawnableBattleObject;

    #nullable enable
    [Header("All Party Slots")]
    [Tooltip("PartySlot Occupant.  The main body of the party occupant, or just a body occupant if normal sized")]
    [SerializeField, SerializeReference]
    private PartySlot? _partyOccupant;


    [SerializeField, SerializeReference]
    private SpawnableObjectPartySlot? _spawnableObjectPartySlot;

    public SpawnableObjectPartySlot? SpawnableObjectPartySlot { get => this._spawnableObjectPartySlot; }

    #nullable disable




    // [Tooltip("The height that a flying occupant will have added while on this tile")]
    // [SerializeField]
    // [Range(2f, 10f)]
    // private float flyHeight = 2;

    // [SerializeField]
    // private bool controlPosition = true;


    [SerializeField]
    private bool isWalkable = true;

    private float minimalOffset = 0.005f;

    [SerializeField]
    private BattleManager battleController;


    void Awake()
    {
        if (this.tile != null)
        {
            this.height = this.tile.bounds.size.y;
        }

        this.SwitchState(defaultState);

        this._partyOccupant = null;
        this.projectedGlowBattleMembers = new List<BattleMember>();
    }


    

    private void SetGlows() {
        this.SetTileGlowControllerCombatTileController(actionGlow);

        this.SetTileGlowControllerCombatTileController(confirmActionGlow);

        this.SetTileGlowControllerCombatTileController(moveGlow);

        this.SetTileGlowControllerCombatTileController(projectedGlow);

        this.SetTileGlowControllerCombatTileController(occupiedGlow);
    }

    private void SetTileGlowControllerCombatTileController(TileGlowController tileGlowController) {
        if (tileGlowController == null) {
            return;
        }

        tileGlowController.CombatTileController = this;
    }

    void Start() {
        this.SetGlows();
    }

    public void SetBattleController(BattleManager battleManager)
    {
        this.battleController = battleManager;
        this.GetStartingSpawnableObject();
    }

    private void SetStartingSpawnObjectPosition(SpawnableBattleObject spawnableBattleObject) {
        if (spawnableBattleObject.ShouldCenter) {
            spawnableBattleObject.gameObject.transform.position = this.GetTileBasePosition(false);
        }
        return;
    }

    private void GetStartingSpawnableObject() {
        #nullable enable
        SpawnableBattleObject? spawnableBattleObject = this.startingSpawnableBattleObject != null ? this.startingSpawnableBattleObject :  GetComponentInChildren<SpawnableBattleObject>();
        if (spawnableBattleObject == null) {
            return;
        }
        this.SetStartingSpawnObjectPosition(spawnableBattleObject);
        this.SetInteractableBattleObject(battleController, spawnableBattleObject);
        #nullable disable
    }

    void Update() {
        this.GetNewTileHeight();
        this.currentstate.UpdateTile(this);
        this.DetermineProjectedGlowState();

        // if (this.controlPosition) {
        //     this.SetOccupantPosition();
        // }

        // this.setSpawnedObjectHeight();

        this.ManagePartyOccupantSelector();
        this.SetPartyOccupantStatusBar();
    }

    private void SetPartyOccupantStatusBar() {
        if (!this.HasPartyOccupant()) {
            return;
        }


        bool toggleStatusBar = this.actionGlow.gameObject.activeSelf || this.confirmActionGlow.gameObject.activeSelf;

        this.DisablePartyOccupantsStatusBar(toggleStatusBar);

    }

    private void GetNewTileHeight() {
        if (this.height != this.tile.transform.localScale.y) {
            this.height = this.tile.transform.localScale.y;
        }

    }

    private void ManagePartyOccupantSelector() {
        if (this._partyOccupant == null) {
            return;
        }

        bool activateGlow = this.confirmActionGlow.gameObject.activeSelf || this.actionGlow.gameObject.activeSelf;

        this._partyOccupant.ToggleTargetIndicator(activateGlow);

    }

    // private void SetOccupantPosition() {
    //     if (this.partyOccupant == null || this.partyOccupant.BattleEntityGO == null || this.partyOccupant.GetBattleEntity == null) {
    //         return;
    //     }
    //     this.partyOccupant.BattleEntityGO.transform.position = this.GetHeightForSpriteVector3Offset(true);
    // }

    private void DetermineProjectedGlowState() {
        if (this.projectedGlowBattleMembers == null) {
            return;
        }
        List<BattleMember> newBattleMembers = new List<BattleMember>();
        foreach(BattleMember battleMember in this.projectedGlowBattleMembers) {
            if(battleMember != null && !battleMember.GetBattleEntity.IsDead()) {
                newBattleMembers.Add(battleMember);
            }
        }
        this.projectedGlowBattleMembers = newBattleMembers;
        if(this.projectedGlowBattleMembers.Count > 0 && !this.projectedGlow.gameObject.activeSelf) {
            this.projectedGlow.gameObject.SetActive(true);
            return;
        } else if (this.projectedGlowBattleMembers.Count <= 0 && this.projectedGlow.gameObject.activeSelf) {
            this.projectedGlow.gameObject.SetActive(false);
            return;
        }
    }

    private void HandleDefaultState(CombatTileControllerBaseState controllerState) {
        if (this.currentstate != null && this.currentstate != this.defaultState) {
            this.currentstate.EndState(this);
        }
        this.currentstate = controllerState;
        this.currentstate.EnterState(this);
    }

    private void HandleAbnormalState(CombatTileControllerBaseState controllerState) {
        if (controllerState is not CombatTileControllerAbnormalState combatTileControllerAbnormalState)
        {
            return;
        }

        if (this.previousStates.ContainsKey(combatTileControllerAbnormalState.combatTileInteractionSO) && combatTileControllerAbnormalState.combatTileInteractionSO.canBeReTriggered) {
            this.currentstate.EndState(this);
            this.currentstate = combatTileControllerAbnormalState;
            this.currentstate.EnterState(this);
            this.previousStates[combatTileControllerAbnormalState.combatTileInteractionSO] = combatTileControllerAbnormalState;
        }else if (this.currentstate == null) {
            this.currentstate = combatTileControllerAbnormalState;
            this.currentstate.EnterState(this);
            this.previousStates[combatTileControllerAbnormalState.combatTileInteractionSO] = combatTileControllerAbnormalState;
        } else {
            this.currentstate.EndState(this);
            this.currentstate = combatTileControllerAbnormalState;
            this.currentstate.EnterState(this);
            this.previousStates[combatTileControllerAbnormalState.combatTileInteractionSO] = combatTileControllerAbnormalState;
        }
    }

    public void SwitchState(CombatTileControllerBaseState controllerState) {
        if(controllerState == this.defaultState) {
            this.HandleDefaultState(controllerState);
            return;
        }

        this.HandleAbnormalState(controllerState);
    }

    public virtual void OnTurnEnd(BattleManager battleController) {
        this.currentstate.UpdateTileTurnState(battleController, this);
    }

    public void PerformTurnStartInteraction(BattleManager battleController) {
        currentstate.PerformTurnStartInteraction(battleController, this);
    }
    
    public void OnActionCommand(BattleManager battleController, PartySlot user, BattleActionEventSO battleActionEventSO, IBattleActionCommand action, GenericDictionary<Vector3, CombatTileController> grid, List<CombatTileController> alreadyAffected, List<PartySlot> affectedTargets) {
        if (this.currentstate == null || this.currentstate.combatTileInteractionSO == null) {
            return;
        }

                    
        if (action != null && this.currentstate.combatTileInteractionSO.ElementInteractionPossible(action.GetElement())) {
            this.currentstate.combatTileInteractionSO.OnBattleAction(battleController, user, battleActionEventSO, action, this, grid, new(alreadyAffected), affectedTargets);
        }

        return;
    }

    // public void OnItemUsed(BattleController battleController, PartySlot user, ConsumableInventoryItemSO inventoryItem, GenericDictionary<Vector3, CombatTileController> grid, List<CombatTileController> alreadyAffected) {
    //     // foreach(CombatTileInteractionSO combatTileInteractionSO in this.combatTileInteractions) {
    //     //     combatTileInteractionSO.OnItemUsed(inventoryItem, this);
    //     // }   
    //     if (this.currentstate != null && this.currentstate.combatTileInteractionSO != null) {
            
    //         StaticDamageItemSO staticDamageItem = inventoryItem as StaticDamageItemSO;
    //         if (staticDamageItem != null && this.currentstate.combatTileInteractionSO.ElementInteractionPossible(staticDamageItem.element)) {
    //             Debug.Log("Attempting to overwrite current tile status");
    //             this.currentstate.combatTileInteractionSO.OnItemUsed(battleController, user, staticDamageItem, this, grid, alreadyAffected);
    //         }
    //     }     
    //     return;
    // }

    public void ResetTileState() {
        if(actionGlow.gameObject.activeSelf)
            actionGlow.gameObject.SetActive(false);

        if(moveGlow.gameObject.activeSelf)
            moveGlow.gameObject.SetActive(false);

        this.DisableActionConfirmTile();

        // this.actionWasPreviouslyActive = false;
        // this.movementWasPreviouslyActive = false;
    }

    public void ActivateActionTile() {
        if(!actionGlow.gameObject.activeSelf)
            actionGlow.gameObject.SetActive(true);
    }

    public void ActivateMovementTile() {
        if(!moveGlow.gameObject.activeSelf) {
            moveGlow.gameObject.SetActive(true);
        }
    }

    public void AddProjectedTileBattleCommand(BattleMember battleMember) {
        if(battleMember == null) {
            return;
        }
        if(this.projectedGlowBattleMembers != null && !this.projectedGlowBattleMembers.Contains(battleMember)) {
            this.projectedGlowBattleMembers.Add(battleMember);
        }
    }

    private void ToggleOccupiedTile(bool toggle) {
        if (this.occupiedGlow == null) {
            return;
        }

        this.occupiedGlow.gameObject.SetActive(toggle);
    }

    #nullable enable
    public void RemoveProjectedTileBattleCommand(BattleMember? battleMember) {
        if(battleMember == null) {
            return;
        }
        if(this.projectedGlowBattleMembers != null && this.projectedGlowBattleMembers.Contains(battleMember)) {
            this.projectedGlowBattleMembers.Remove(battleMember);
        }
    }
    #nullable disable


    public void ConfirmMovementSelectTile() {
        moveGlow.gameObject.SetActive(false);
        confirmActionGlow.gameObject.SetActive(true); 
        if (!this.HasPartyOccupant()) {
            return;
        }
        
    }

    public void DisableActionTile() {
        if(actionGlow.gameObject.activeSelf)
            actionGlow.gameObject.SetActive(false);

        if (!this.HasPartyOccupant()) {
            return;
        }

        this.DisablePartyOccupantsStatusBar(false);
    }

    public void DisableActionConfirmTile() {
        if (!this.confirmActionGlow.gameObject.activeSelf) {
            return;
        }

        this.confirmActionGlow.gameObject.SetActive(false);
        if (!this.HasPartyOccupant()) {
            return;
        }

        this.DisablePartyOccupantsStatusBar(false);
    }

    private void DisablePartyOccupantsStatusBar(bool toggle) {
        if (this.HasPartyOccupant()) {
            this._partyOccupant.BattleEntityGO.ToggleStatusBar(toggle);
        }

    }

    public void ConfirmActionSelectTile() {
        actionGlow.gameObject.SetActive(false);
        this.confirmActionGlow.gameObject.SetActive(true);
        if (!this.HasPartyOccupant()) {
            return;
        }

        
        // this.actionWasPreviouslyActive = false;

    }

    public void ActivateConfirmActionTile() {
        if (this.confirmActionGlow.gameObject.activeSelf) {
            return;
        }
        this.confirmActionGlow.gameObject.SetActive(true);
        if (!this.HasPartyOccupant()) {
            return;
        }
    }

    public void DisableMovementTile(bool removeMovementTiles) {
        if(moveGlow.gameObject.activeSelf)
            moveGlow.gameObject.SetActive(false);
        // this.DisableActionConfirmTile();
        // if (removeMovementTiles) {
        //     this.movementTiles = new ();
        // }
        // this.movementWasPreviouslyActive = false;
        this.movementTiles = new ();

    }

    public int GetMovementCost(GenericDictionary<ElementSO, int> terrainMovementSpeedOverride) {
        int subtractedCost = 0;
        if (this.currentstate.combatTileInteractionSO != null) {
            if (terrainMovementSpeedOverride != null && terrainMovementSpeedOverride.ContainsKey(this.currentstate.combatTileInteractionSO.ElementSO)) {
                subtractedCost = terrainMovementSpeedOverride[this.currentstate.combatTileInteractionSO.ElementSO];
            }
            return this.currentstate.combatTileInteractionSO.movementCost - subtractedCost;
        }
        return 1;
    }

    

    /**
        Gets all possible moves via recursive node traversal.
    */
    


    public float GetHeight() {
        return this.Height;
    }

    public float GetFloatHeightWithSpawnedObject(bool useAnchorPoint = false) {
        float height = this.GetHeight();
        if (this._spawnableObjectPartySlot == null) {
            return height;
        }

        if (!useAnchorPoint) {
            height += this._spawnableObjectPartySlot.Controller.GetFloatHeightForSpawnedObject();
            return height;
        }

        Vector3 anchorPointCasted;
        Vector3? anchorPoint = this._spawnableObjectPartySlot.Controller.GetOccupantAnchorPoint().transform.position;
        if (anchorPoint == null) {
           return height;
        }


        
        anchorPointCasted = (Vector3) anchorPoint;
        return anchorPointCasted.y;
    }
 
    private Vector3 GetVector3HeightOffsetWithSpawnedObject(bool useAnchorPoint = false) {
        Vector3 newPosition = this.GetTileBasePosition(true);
        if (this._spawnableObjectPartySlot == null ||
            !this._spawnableObjectPartySlot.Controller.ShouldCenter ) {
           return newPosition; 
        }

        if (!useAnchorPoint) {
            newPosition += this._spawnableObjectPartySlot.Controller.GetVector3HeightForSpawnedObject();
            return newPosition;
        }

        Vector3? anchorPoint = this._spawnableObjectPartySlot.Controller.GetOccupantAnchorPoint().transform.position;
        if (anchorPoint == null) {
            return newPosition;
        }

        return (Vector3) anchorPoint;
    }

    public GameObject GetGameObjectAnchor() {

        if (this._spawnableObjectPartySlot != null ) {
           return this._spawnableObjectPartySlot.Controller.GetOccupantAnchorPoint(); 
        }

        if (this.anchorPoint != null) {
            return this.anchorPoint;
        }

        return this.gameObject;
    }

    public Vector3 GetHeightForSpriteVector3Offset(bool useAnchorPoint = true) {
        Vector3 newPosition = this.GetVector3HeightOffsetWithSpawnedObject(useAnchorPoint);
        return newPosition; 
    }

    public bool HasPartyOccupant() {
        return this._partyOccupant != null;
    }
   
    #nullable enable

    public PartySlot? GetPartyOccupant() {
        return this._partyOccupant;
    }

    #nullable disable 
    
    private float GetPartyOccupantHeight (float height) {
        return height;
    }

    public List<PartySlot> GetPartyOccupants() {
        List<PartySlot> partySlots = new List<PartySlot>();
        if (this.HasPartyOccupant()) {
            partySlots.Add(this._partyOccupant);
        }

        if (this._spawnableObjectPartySlot != null) {
            partySlots.Add(this._spawnableObjectPartySlot);
        }
        return partySlots;
    }


    private float GetHeightWithSpawnedObject() {
        float height = this.Position.y + this.GetTileBasePosition(false).y;
        if (this._spawnableObjectPartySlot != null) {
            height += this._spawnableObjectPartySlot.Controller.GetFloatHeightForSpawnedObject();
        }
        return height;
    }

    public float GetHeightWithSpawnedObjectAndPresentEntity() {
        float height = this.GetHeightWithSpawnedObject();
        if (this.HasPartyOccupant()) {
            height = this.GetPartyOccupantHeight(height);
        }
        return height;
    }

    public float GetHeightWithSpawnedObjectAndProvidedEntityFlight() {
        float height = this.Position.y + this.GetTileBasePosition(false).y;
        if (this._spawnableObjectPartySlot != null) {
            height +=this._spawnableObjectPartySlot.Controller.GetFloatHeightForSpawnedObject();
        }
        height = this.GetPartyOccupantHeight(height);
        return height;
    }

    public void ResetPartyOccupantParent()
    {
        if (this._partyOccupant == null || this._partyOccupant.BattleEntityGO == null)
        {
            return;
        }
        this.SetPartyOccupantParent(this.GetGameObjectAnchor().transform);
        // this._partyOccupant.BattleEntityGO.transform.position = this.GetGameObjectAnchor().transform.position;
    }

    public void ResetSpawnableObjectPartyOccupantParent()
    {
        if (this._spawnableObjectPartySlot == null || this._spawnableObjectPartySlot.BattleEntityGO == null)
        {
            return;
        }
        this.SetSpawnableObjectOccupantParent();
    }
    
    public void SetSpawnedObjectFromCombatTileInteractionSO(BattleManager battleManager, CombatTileInteractionSO combatTileInteractionSO)
    {
        if (combatTileInteractionSO == null)
        {
            return;
        }

        GameObject spawnedObject = (GameObject)Instantiate(combatTileInteractionSO.spawnableObject, this.GetTileBasePosition(false), transform.rotation);
        Debug.Log("Spawned Object:  " + spawnedObject);
        if (spawnedObject == null)
        {
            Debug.Log("No spawnable object");
            return;
        }

        spawnedObject.transform.SetParent(this.tile.transform);

        Quaternion rotation = new Quaternion();

        spawnedObject.transform.rotation = rotation;

        Debug.Log("Getting combat tile interaction for spawned object");
        SpawnableBattleObject spawnableBattleObject = spawnedObject.GetComponent<SpawnableBattleObject>();
        this.SetInteractableBattleObject(battleManager, spawnableBattleObject);
    }

    #nullable enable

    public void SetAndInstantiateSpawnedObject(BattleManager battleManager, SpawnableBattleObject spawnableBattleObject) {
        GameObject? spawnedObject = (GameObject)  Instantiate( spawnableBattleObject.gameObject, this.GetTileBasePosition(false), transform.rotation);
        if (spawnedObject == null) {
            Debug.Log("No spawned object to spawn");
            return;
        }
        spawnedObject.transform.SetParent(this.tile.transform);
        Quaternion rotation = new Quaternion();        
        spawnedObject.transform.rotation = rotation;

        spawnableBattleObject = spawnedObject.GetComponent<SpawnableBattleObject>();

        this.SetInteractableBattleObject(battleManager, spawnableBattleObject);
    }

    public void SetInteractableBattleObject(BattleManager battleManager, SpawnableBattleObject spawnableBattleObject)
    {
        if (spawnableBattleObject == null)
        {
            Debug.Log("Spawnable Battle Object not found");
            return;
        }

        CombatTileInteractionSO? combatTileInteractionSO = spawnableBattleObject.CombatTileInteraction;

        if (combatTileInteractionSO == null)
        {
            return;
        }

        this.ResetToDefaultState();
        // this.SwitchState(combatTileControllerAbnormalState ?? 
        //     new CombatTileControllerAbnormalState( battleManager,
        //                                             this, 
        //                                             combatTileInteractionSO, 
        //                                             spawnableBattleObject));
        BattleEntitySlot battleEntitySlot = new(new SpawnedObjectInfo(spawnableBattleObject.GetSpawnableObjectInfo()));

        BattleMember battleMember = new(battleEntitySlot);

        SpawnableObjectPartySlot spawnableObjectPartySlot = new (battleMember, spawnableBattleObject);

        this.SetInteractableObjectPartySlot(battleManager,spawnableObjectPartySlot);

    }

    public void SetInteractableObjectPartySlot(BattleManager battleManager, SpawnableObjectPartySlot spawnableObjectPartySlot)
    {
        if (!this.CanAddSpawnedObject())
        {
            Debug.Log($"Cannot add Spawned Object at: {this.position}");
            return;
        }

        this._spawnableObjectPartySlot = spawnableObjectPartySlot;
        this._spawnableObjectPartySlot.SetCombatTileController(battleManager, this);
        this.SetSpawnableObjectOccupantParent();
        this.SetPartyOccupantParent(this.GetGameObjectAnchor().transform);
    }
    
    public void SetPartyOccupant(BattleManager battleController, PartySlot partySlot)
    {
        if (!this.CanAddPartyOccupant(partySlot))
        {
            return;
        }

        this._partyOccupant = partySlot;

        this._partyOccupant.SetCombatTileController(battleController, this);

        if (this._partyOccupant.BattleEntityGO != null)
        {
            this.SetPartyOccupantParent(this.GetGameObjectAnchor().transform);
        }

        this.SetTileOccupied();
    }

    public void SetAdditionalPartyOccupant(BattleManager battleManager, PartySlot partySlot)
    {
        if (!this.CanAddPartyOccupant(partySlot))
        {
            return;
        }

        this._partyOccupant = partySlot;

        this.SetTileOccupied();
    }

    private void SetTileOccupied()
    {
        this.ToggleOccupiedTile(true);
        this.OnTileEntered(battleController, this._partyOccupant); 
    }

    private bool CanAddPartyOccupant(PartySlot partySlot)
    {
        if (BattleProcessingStatic.PartySlotIsSpawnablePartySlot(partySlot))
        {
            return false;
        }

        if (BattleProcessingStatic.IsGiantEntity(this._partyOccupant) && this.SpawnableObjectPartySlot != null)
        {
            return false;
        }

        return true;
    }

    // public void ToggleControlPosition(bool controlPosition) {
    //     this.controlPosition = controlPosition;
    // }

    public PartySlot? RemoveOccupantAndProcessEvent(BattleManager battleManager) {
        PartySlot? partySlot = this.RemoveOccupant();
        this.OnExitingTile(battleManager, partySlot);
        // this.controlPosition = false;
        return partySlot;
    }

    public SpawnableObjectPartySlot? RemoveSpawnableObject(BattleManager battleManager)
    {
        SpawnableObjectPartySlot? spawnableObjectPartySlot = this._spawnableObjectPartySlot;

        this._spawnableObjectPartySlot = null;

        return spawnableObjectPartySlot;
    }


    public PartySlot? RemoveOccupant()
    {
        PartySlot? partySlot = this._partyOccupant;
        this._partyOccupant = null;
        this.ToggleOccupiedTile(false);
        // this.controlPosition = false;
        return partySlot;
    }


    public bool CanAddOccupant(PartySlot partySlot)
    {
        if (!this.CanBeOccupied())
        {
            return false;
        }

        if (BattleProcessingStatic.IsGiantEntity(partySlot) && this._spawnableObjectPartySlot != null)
        {
            return false;
        }

        if (!this.HasPartyOccupant())
        {
            return true;
        }

        #nullable enable
        PartySlot? currentPartySlot = this.GetPartyOccupant();

        if (currentPartySlot == null)
        {
            return true;
        }

        if (!currentPartySlot.BattleEntity!.CanFly() && partySlot.BattleEntity!.CanFly())
        {
            return true;
        }

        if ((BattleProcessingStatic.PartySlotIsPlayerPartySlot(currentPartySlot) && BattleProcessingStatic.PartySlotIsPlayerPartySlot(partySlot)) ||
            (BattleProcessingStatic.PartySlotIsEnemyPartySlot(currentPartySlot) && BattleProcessingStatic.PartySlotIsEnemyPartySlot(partySlot)))
        {
            return true;
        }

#nullable disable
        return false;
    }

    #nullable enable
    private bool CanBeOccupied() {
        
        if (this._spawnableObjectPartySlot != null) {
            return this._spawnableObjectPartySlot.Controller.IsWalkable;
        }

        return this.isWalkable;

    }
    #nullable disable
    //useMinimalOffset adds 0.001f to the returned offset.  Can be used to prevent clipping.
    public Vector3 GetTileBasePosition(bool useMinimalOffset) {
        // return tileOccupantOffset;
        //Aids in getting offset.

        if (this.anchorPoint != null) {
            return this.anchorPoint.transform.position;
        }
        Vector3 position = this.transform.position;
        position.y = this.Height + (useMinimalOffset ? this.minimalOffset : 0f);
        return position;
    }


    public void OnTileEntered(BattleManager battleController, PartySlot partySlot) {
        if (this.currentstate != null && this.currentstate.combatTileInteractionSO != null) {
            this.currentstate.combatTileInteractionSO.OnTileEntered(battleController, partySlot, this);
        }
        return;
    }

    public void OnExitingTile(BattleManager battleController, PartySlot partySlot) {
        if (this.currentstate != null && this.currentstate.combatTileInteractionSO != null) {
            this.currentstate.combatTileInteractionSO.OnTileExited(battleController, partySlot, this);
        }
        return;
    }

    
    #nullable disable

    public bool CanAddSpawnedObject()
    {
        if (BattleProcessingStatic.IsGiantEntity(this._partyOccupant))
        {
            return false;
        }

        if (this._spawnableObjectPartySlot == null)
        {
            return true;
        }
        return false;
    }

    public void TurnChange(BattleManager battleController) {
        if (this._spawnableObjectPartySlot != null)
        {
            this._spawnableObjectPartySlot.Controller.DecreaseCounter();
        }
    }

    public void CleanupSpawnedObject() {
        this.ClearSpawnedObjectInfo();
    }

    private void ClearSpawnedObjectInfo () {
        if (this._spawnableObjectPartySlot == null) {
            return;
        }
        Debug.Log("Cleaning up Spawned Objects");

        this.SetPartyOccupantParent(this.anchorPoint.transform);

        this._spawnableObjectPartySlot = null;
    }

    private void SetPartyOccupantParent(Transform anchor) {
        if (this._partyOccupant == null || this._partyOccupant.GetCombatTileController() != this) {
            return;
        }

        this._partyOccupant.BattleEntityGO.transform.position = anchor.position;
        this._partyOccupant.BattleEntityGO.transform.SetParent(anchor);
    }

    private void SetSpawnableObjectOccupantParent()
    {
        if (this._spawnableObjectPartySlot == null || this._spawnableObjectPartySlot.GetCombatTileController() != this) {
            return;
        }

        this._spawnableObjectPartySlot.BattleEntityGO.transform.position = this.anchorPoint.transform.position;
        this._spawnableObjectPartySlot.BattleEntityGO.transform.SetParent(this.anchorPoint.transform);
    }

    public void ResetToDefaultState()
    {
        this.SwitchState(this.defaultState);
    }

    public void OverrideDefaultStateTileInteraction(CombatTileInteractionSO combatTileInteraction) {
        if (combatTileInteraction == null) {
            Debug.LogError("Override Combat Tile interaction is missing!");
            return;
        }

        this.defaultState.combatTileInteractionSO = combatTileInteraction;
    }

}
}

