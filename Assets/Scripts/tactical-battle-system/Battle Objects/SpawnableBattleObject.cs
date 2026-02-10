using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat{
public class SpawnableBattleObject : BattleObject
{

    [Tooltip("Spawnable Object Info for the combat tile interaction.  Relevant if it has behaviors associated with it")]
    [SerializeField]
    private SpawnableObjectInfoSO primarySpawnableObjectInfo;

#nullable enable
    [Tooltip("Combat tile interactions if any")]
    [SerializeField]
    private CombatTileInteractionSO? combatTileInteraction;

    public CombatTileInteractionSO? CombatTileInteraction { get => this.combatTileInteraction; }
#nullable disable

    [Tooltip("The collider to be used when setting occupant height in real time")]
    [SerializeField]
    private GameObject occupantAnchorPoint;


    [SerializeField]
    private bool shouldCenter = true;

    public bool ShouldCenter { get => this.shouldCenter; }

    [SerializeField]
    private bool spreadShouldCenter = false;

    public bool SpreadShouldCenter { get => this.spreadShouldCenter; }

    [Tooltip("The Combat Tile controller this will apply to")]
    [SerializeField]
    private CombatTileController primaryCombatTileController;


    [Tooltip("The combat tile controllers aside from the primary.")]
    [SerializeField]
    private GenericDictionary<Vector3, CombatTileController> spreadCombatTileControllers;

    // [Tooltip("Overrides the combat tile interaction's walkable rule for the spread combat tiles")]
    // [SerializeField]
    // private bool isSpreadWalkable = true;

    // [Tooltip("Overrides the combat tile interaction's solid rule for the spread combat tiles")]
    // [SerializeField]
    // private bool isSpreadSolid = false;


    [Tooltip("Runtime animator controller attached to the game object.")]
    [SerializeField]
    private Animator animator;

    private bool isAnimating = false;

    // [Tooltip("Controls the spread value of the Spawnable Battle Object")]
    // [SerializeField]
    // [Range(0, 10)]
    // private int startingSpreadValue = 0;

    [Tooltip("The time inbetween state changes")]
    [SerializeField]
    [Range(0, 10)]
    private int stateChangeDelay = 0;

    [Tooltip("The current value of the state change cooldown")]
    [SerializeField]
    private int currentStateChangeCooldown = 0;

    private GenericDictionary<Vector3, CombatTileController> grid = new();

    public GenericDictionary<Vector3, CombatTileController> Grid { set => this.SetGrid(value); get => this.grid; }

    public bool IsAnimating { get => this.isAnimating; set => this.isAnimating = value; }

    // #nullable enable
    // private SpawnableObjectPartySlot? spawnableObjectPartySlot = null;
    // #nullable disable

    protected override void BattleObjectAwake()
    {
        base.BattleObjectAwake();
        this.height = 0f;
    }


    // private void CreatePartySlot(BattleManager battleManager) {
    //     if (this.primaryCombatTileController == null) {
    //         return;
    //     }
    //     SpawnedObjectController spawnedObjectController = new(this.gameObject, this.combatTileInteraction.duration, this.combatTileInteraction.walkable, this.combatTileInteraction.isSolid, this.combatTileInteraction.infiniteDuration, this.shouldCenter, null);
    //     BattleEntitySlot battleEntitySlot = new(new SpawnedObjectInfo(this.GetSpawnableObjectInfo()));
    //     BattleMember battleMember = new(battleEntitySlot);
    //     this.spawnableObjectPartySlot = new(battleMember);

    //     this.primaryCombatTileController.SetInteractableObjectPartySlot(battleManager, this, spawnedObjectController);
    // }


    protected override void UpdateBattleObject()
    {
        if (!base.isReady && base.battleMember != null)
        {
            // base.statusBar = (GameObject)Instantiate(Resources.Load(Constants.UI.EnemyHP), base.GetTargetAnchorPointPosition(), transform.rotation);
            // base.statusBar.transform.SetParent(transform, true);
            // base.statusBar.SetActive(false);
            base.isReady = true;
        }
    }

    public SpawnableObjectInfoSO GetSpawnableObjectInfo()
    {
        return this.primarySpawnableObjectInfo;
    }

#nullable enable


#nullable enable
    public GameObject? GetOccupantAnchorPoint()
    {
        if (this.occupantAnchorPoint == null)
        {
            return null;
        }
        return this.occupantAnchorPoint;
    }


    public void SetSpawnedObject(BattleManager battleManager)
    {


        if (this.combatTileInteraction == null || this.primaryCombatTileController == null)
        {
            return;
        }


        if (this.spreadCombatTileControllers.Count == 0)
        {
            return;
        }


        // SpawnedObjectController secondaryObjectController = new(this.gameObject, this.combatTileInteraction.duration, this.isSpreadWalkable, this.isSpreadSolid, this.combatTileInteraction.infiniteDuration, this.spreadShouldCenter, null);
        // // foreach (var combatTile in this.spreadCombatTileControllers)
        // // {
        // //     SpawnedObjectController? existingSpawnedObjectController = combatTile.Value.SpawnedObjectController;
        // //     if (existingSpawnedObjectController != null && existingSpawnedObjectController.SpawnedObject != this.gameObject)
        // //     {
        // //         combatTile.Value.CleanupSpawnedObject(true);
        // //     }
        // //     combatTile.Value.SetInteractableObjectPartySlot(battleManager, this, secondaryObjectController);
        // // }
    }

    public override bool HasAnimator()
    {
        return this.animator != null;
    }


    private bool CanProgressObjectState()
    {
        return this.currentStateChangeCooldown == 0;
    }

    private void SetCurrentStateChangeCooldownToDelay()
    {
        this.currentStateChangeCooldown = this.stateChangeDelay;
    }

    private void ProgressAnimatorState(BattleManager? battleController)
    {
        if (!this.HasAnimator())
        {
            return;
        }

        if (!CanProgressObjectState())
        {
            this.currentStateChangeCooldown--;
            return;
        }

        this.SetCurrentStateChangeCooldownToDelay();
        Debug.Log("Triggering Progress State animator in spawned battle object");
        this.TriggerAnimation("progressState", battleController);
    }

    private void TriggerAnimation(string animationName, BattleManager? battleController)
    {
        if (!DPS.Common.GeneralUtilsStatic.AnimationParameterExists(animationName, new List<AnimatorControllerParameter>(this.animator.parameters)))
        {
            return;
        }

        this.animator.SetTrigger(animationName);
        if (battleController == null)
        {
            return;
        }
        battleController.BattleEventController.AddBattleEvent(new SpawnedObjectEvent(this));
    }

    public void SetAnimatorState(string animationState, bool toggle, BattleManager? battleController)
    {
        if (!this.HasAnimator())
        {
            return;
        }

        if (!DPS.Common.GeneralUtilsStatic.AnimationParameterExists(animationState, new List<AnimatorControllerParameter>(this.animator.parameters)))
        {
            return;
        }
        if (battleController == null)
        {
            return;
        }
        // this.TriggerAnimation(animationTrigger, battleController);
    }
    public void OnTurnEnd(BattleManager battleController)
    {
        this.ProgressAnimatorState(battleController);

        return;
    }



    public override void SetAnimationState(string animationState)
    {
        return;
    }

    public override void SetAnimationState(AnimationStates animationState)
    {
        return;
    }

    public override void TargetBattleObject(bool targetObject)
    {
        return;
    }

    public override void SetMovement(bool isRunning = false, bool isWalking = false)
    {
        return;
    }

    public override void TriggerAnimation(AnimationTrigger animationTrigger, string? specialAnimation = null)
    {
        return;
    }

    public override void ToggleEffect(AnimationEffect animationEffect, bool toggle)
    {
        return;
    }

    public override void EndMovement()
    {
        return;
    }

    public override void DestroyBattleObject()
    {
        // if(base.battleMember == null || !base.GetBattleEntity!.IsDead()) {
        //     return;
        // }
        Destroy(this.gameObject);
    }

    // private void SetNeighboringCombatTilesByDistance(BattleManager battleManager, int distance)
    // {

    //     //  Cannot raycast properly if there are no surrounding tiles.
    //     if (this.primaryCombatTileController == null)
    //     {
    //         return;
    //     }

    //     //First, reset existing combat tile list
    //     // this.ResetCombatTileControllers();

    //     //Create new list to set
    //     this.GetSurroundingTiles(battleManager, distance);

    //     //Finally, spread properties to the new tile list
    //     this.SetSpawnedObject(battleManager);
    //     return;
    // }

    // public void ResetCombatTileControllers()
    // {
    //     if (this.spreadCombatTileControllers.Count == 0)
    //     {
    //         return;
    //     }

    //     //Reset all surrounding tiles to default state
    //     foreach (var combatTile in this.spreadCombatTileControllers)
    //     {
    //         SpawnedObjectController? existingSpawnedObjectController = combatTile.Value.SpawnedObjectController;
    //         if (existingSpawnedObjectController != null && existingSpawnedObjectController.SpawnedObject == this.gameObject)
    //         {
    //             combatTile.Value.CleanupSpawnedObject(false);
    //         }
    //         // CombatTile.Value.ResetTileState();
    //     }

    //     this.spreadCombatTileControllers.Clear();
    // }

    // public void ResetSpreadProperties() {
    //     //Next, populate new tile list with new spread distance
    //     this.SetNeighboringCombatTilesByDistance(this.startingSpreadValue);

    // }

    private void GetSurroundingTiles(BattleManager battleManager, int distance)
    {
        // this.CreatePartySlot(battleManager);

        if (this.primaryCombatTileController == null)
        {
            return;
        }



        List<RaycastHit> sphereRayCastHits = new List<RaycastHit>();
        Vector3 sphereDir = Quaternion.Euler(0, 0, 0) * primaryCombatTileController.transform.right;

        sphereRayCastHits.AddRange(Physics.SphereCastAll(primaryCombatTileController.Position, distance, sphereDir, 0));


        GenericDictionary<Vector3, CombatTileController> combatTileControllers = new();
        foreach (RaycastHit raycast in sphereRayCastHits)
        {
            GameObject tileObject = raycast.transform.gameObject;
            if (tileObject == null || !tileObject.TryGetComponent<CombatTileController>(out CombatTileController combatTileController) || this.primaryCombatTileController == combatTileController)
            {
                continue;
            }

            combatTileControllers.Add(combatTileController.Position, combatTileController);
            // combatTileController.ActivateConfirmActionTile();
        }

        this.spreadCombatTileControllers = combatTileControllers;
    }


    private void SetGrid(GenericDictionary<Vector3, CombatTileController> grid)
    {
        this.grid = grid;

        this.GetStartingTile();
    }

    private Vector3 GetCurrentPosition()
    {
        return new(Mathf.RoundToInt(this.transform.position.x), 0, Mathf.RoundToInt(this.transform.position.z));
    }

    private void GetStartingTile()
    {
        //Get new center for the object
        this.primaryCombatTileController = null;

        Vector3 key = this.GetCurrentPosition();

        if (!this.Grid.TryGetValue(key, out CombatTileController startingTile))
        {
            return;
        }

        this.primaryCombatTileController = startingTile;
    }


#nullable disable

}
}
