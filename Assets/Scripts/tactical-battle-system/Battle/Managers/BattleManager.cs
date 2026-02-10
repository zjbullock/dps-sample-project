using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using TMPro;
using Unity.Cinemachine;
using System;
using UnityEngine.Playables;
using System.Runtime.Remoting.Messaging;
using System.Collections.ObjectModel;
using DPS.TacticalCombat;
using DPS.Common;

namespace DPS.TacticalCombat {
public class BattleManager : MonoBehaviour
{   
    #nullable enable
    public static BattleManager? instance;
    #nullable disable

    [Header("Component Refs")]
    // Start is called before the first frame update
    [SerializeField]
    private PartySlotController _playerPartyController;

    [SerializeField]
    public PartySlotController PlayerPartyController { get => this._playerPartyController; }

    [SerializeField]
    private PartySlotController _enemyPartyController;

    [SerializeField]
    public PartySlotController EnemyPartyController { get => this._enemyPartyController; }

    [SerializeField]
    private BattleEventController _battleEventController;

    public BattleEventController BattleEventController { get => this._battleEventController; }

    [SerializeField] private BattleFieldController battleFieldController;

    [SerializeField] private BattleActionQueueController _battleActionQueueController;

    public BattleActionQueueController BattleActionQueueController { get => this._battleActionQueueController; }

    // [Tooltip("The current Cinemachine Camera event Controller that has priority")]
    // [SerializeField]
    // private CinemachineCameraEventController currentCamera;

    // public CinemachineCameraEventController CurrentCamera { get => this.currentCamera; }

    [SerializeField]
    private BattleCameraManager _battleCameraManager;


    public BattleCameraManager BattleCameraManager { get => this._battleCameraManager; }

    // [Tooltip("The primary focus point on the map")]
    // [SerializeField]
    // private CinemachineCameraEventController focusPointCamera;


    // public CinemachineCameraEventController FocusPointCamera { get => this.focusPointCamera; }



    // [Tooltip("The free look Cinemachine Camera event Controller ref")]
    // [SerializeField]
    // private CinemachineCameraEventController freeLookCamera;

    // public CinemachineCameraEventController FreeLookCamera { get => this.freeLookCamera; }

    // [Tooltip("The side Cinemachine Camera event Controller ref")]
    // [SerializeField]
    // private CinemachineCameraEventController sideCamera;

    // public CinemachineCameraEventController SideCamera { get => this.sideCamera; }



    #nullable enable
    public BattleFieldController BattleFieldController { get => this.battleFieldController; }
    #nullable disable
    public AudioController audioController;

    private BGMController bgmController;

    public BGMController BGMController { get => this.bgmController; }

    // [SerializeField]
    // private UIBattleMenuController _battleMenuController;

    // public UIBattleMenuController BattleMenuController { get => this._battleMenuController; }

    // #nullable enable
    // private SceneTransitionService? sceneTransitionService;

    // public SceneTransitionService? SceneTransitionService { get => this.sceneTransitionService; }

    [SerializeField]
    private EncounterConditionController _encounterConditionController;

    public EncounterConditionController EncounterConditionController { get => this._encounterConditionController; }

    #nullable disable

    [SerializeField]
    private PopUpController popUpController;
    

    [Header("Battle Value Configurations")]

    [SerializeField]
    private BattleValueSO battleValues;

    public BattleValueSO BattleValues { get => this.battleValues; }


    [SerializeField]
    private GaugeValues currentPoints;

    public GaugeValues CurrentPoints { get => this.currentPoints; }


    [Header("Turn Battle Values")]
    private uint _turnCount;

    public uint TurnCount { get => this._turnCount; }

    [SerializeField, SerializeReference]
    public List<PartySlot> currentTurnOrder;


    [SerializeField, SerializeReference]
    public List<PartySlot> futureTurnOrder;

    public List<GameObject> turnPortrait;

    [SerializeField, SerializeReference]
    private PartySlot _currentlyActingMember;

    public PartySlot CurrentlyActingMember { get => this._currentlyActingMember; set => this._currentlyActingMember = value; }

    [Header("Combat Grid Info")]

    [SerializeField]
    [Tooltip("Enemy destination tile for movement.")]
    #nullable enable
    public CombatTileController? DestinationTile;
    #nullable disable

    public GenericDictionary<Vector3, CombatTileController> Grid { get => this.battleFieldController.Grid; }


    [Header("Combat Events")]
    #region Combat Events
    System.Action<uint> _onTurnChangeEvent = delegate { };

    public void AddOnTurnChangeEventListener(System.Action<uint> onTurnChangeAction)
    {
        this._onTurnChangeEvent += onTurnChangeAction;
    }

    public void RemoveOnTurnChangeEventListener(System.Action<uint> onTurnChangeAction)
    {
        this._onTurnChangeEvent -= onTurnChangeAction;
    }

    public void ClearOnTurnChangeEventListeners()
    {
        this._onTurnChangeEvent = delegate {};
    }

    System.Action<List<PartySlot>, List<PartySlot>> _onTurnOrderChangeEvent = delegate { };

    public void AddOnTurnOrderChangeEventListener(System.Action<List<PartySlot>, List<PartySlot>> onTurnOrderChangeAction)
    {
        this._onTurnOrderChangeEvent += onTurnOrderChangeAction;
    }

    public void RemoveOnTurnOrderChangeEventListener(System.Action<List<PartySlot>, List<PartySlot>> onTurnOrderChangeAction)
    {
        this._onTurnOrderChangeEvent -= onTurnOrderChangeAction;
    }

    public void ClearOnTurnOrderChangeEventListeners()
    {
        this._onTurnOrderChangeEvent = delegate { };
    }


    #endregion Combat Events
    // public GenericDictionary<SignalAsset, PlayableDirector> cameraBlends;

    [Header("Battle Statistics")]
    [SerializeField]
    private GenericDictionary<InventoryItemSO, int> obtainedDrops;

    public GenericDictionary<InventoryItemSO, int> ObtainedDrops { get => this.getObtainedDrops(); set => this.setObtainedDrops(value); }


    private GenericDictionary<InventoryItemSO, int> getObtainedDrops()
    {
        return this.obtainedDrops;
    }

    private void setObtainedDrops(GenericDictionary<InventoryItemSO, int> newValue)
    {
        this.obtainedDrops = newValue;
    }


    [SerializeField]
    private uint obtainedGp;
    public uint ObtainedGP { get => this.getObtainedGP(); set => this.setObtainedGP(value); }

    private uint getObtainedGP()
    {
        return this.obtainedGp;
    }

    private void setObtainedGP(uint newValue)
    {
        this.obtainedGp = newValue;
    }



    public void AddPartyPoints(uint partyPoints)
    {
        if (this.CurrentPoints.PartyMoraleGauge + partyPoints > this.BattleValues.MaxPartyMoraleGauge)
        {
            this.CurrentPoints.PartyMoraleGauge = this.BattleValues.MaxPartyMoraleGauge;
            return;
        }
        this.CurrentPoints.PartyMoraleGauge += partyPoints;
        this.SetPartyMoraleGauge();
    }

    public void SubtractPartyPoints(uint partyPoints)
    {
        this.CurrentPoints.PartyMoraleGauge -= partyPoints;
        this.SetPartyMoraleGauge();
    }

    public void ResetPartyMoraleGauge()
    {
        this.CurrentPoints.PartyMoraleGauge = 0;
        this.SetPartyMoraleGauge();
    }

    private void SetPartyMoraleGauge()
    {
        // this.BattleMenuController.MoraleGaugeController.SetCurrentValue((int)this.CurrentPoints.PartyMoraleGauge);
    }



    public void OnKOed()
    {
        this.SubtractPartyPoints(this.BattleValues.PartyMemberKOPoints);
        return;
    }

    public void OnEnemyKO()
    {
        this.AddPartyPoints(this.BattleValues.EnemyKOPoints);
        return;
    }


    public void OnInflictPoiseBreak()
    {
        this.AddPartyPoints(this.BattleValues.InflictPoiseBreakPoints);
        return;
    }

    public void OnPartyMemberDebuffed()
    {
        this.SubtractPartyPoints(this.BattleValues.PartyMemberDebuffedPoints);
        return;
    }


    public void OnPartyMemberPoiseBreak()
    {
        this.SubtractPartyPoints(this.BattleValues.PartyMemberPoiseBrokenPoints);
        return;
    }

    public void OnSwap()
    {
        this.AddPartyPoints(this.BattleValues.SwapPoints);
        return;
    }

    private BattleAnimationSpawner _battleAnimationSpawner;

    public BattleAnimationSpawner BattleAnimationSpawner { get => this._battleAnimationSpawner; }

    [SerializeField]
    private BattleManagerStates _stateManager;

    public BattleManagerStates StateManager { get => this._stateManager; }

    
 

    public void PrioritizeCamera(CinemachineCameraEventController cinemachineCamera)
    {
        // if (this.currentCamera != null) {
        //     Debug.Log("this.currentCamera: " + this.currentCamera.name);
        //     this.currentCamera.SetCameraPriority(false);
        // }

        // this.currentCamera = cinemachineCamera;
        // Debug.Log("this.currentCamera: " + this.currentCamera.name);

        cinemachineCamera.SetCameraPriority(true);
    }

    public void ProgressState()
    {
        Debug.Log("Calling Progress State Signal");
        this.StateManager.CurrentState?.ProgressState(this);
    }

    
    public PopUpController SpawnPopup(Vector3 position, Quaternion rotation)
    {
        return Instantiate(this.popUpController, position, rotation);
    }


    [SerializeField]
    private GUIStyle textStyle;

    void Awake()
    {
        this.SetInstance();
        this._stateManager = new();
        this._battleAnimationSpawner = new();
        // GameObject test = this.grid[new Vector3(4f, 0f, 5f)];
        // if (test != null) {
        //     test.GetComponent<CombatTileController>().PerformMovementTargetingStateChange();
        // }
        this.ObtainedDrops = new GenericDictionary<InventoryItemSO, int>();
        this.ObtainedGP = 0;
    }

    void SetInstance()
    {
        // If there is an instance, and it's not me, delete myself.
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            Debug.Log("Setting BattleManager instance");
            instance = this;
        }
    }

    void ClearInstance()
    {
        instance = null;
    }

    void OnDestroy()
    {
        ClearInstance();
    }

    void Start()
    {
        // actionCursor = 0;
        this.SetFocusPoint();
        
        // if (this.battleCameraManager != null) {
        //     this.cameraFollow = this.battleCameraManager;
        // }
        // this.dialogueBox = GameObject.Find("BattleDialogue");
        // this.dialogueBoxBackground = GameObject.Find("BattleDialogueBackground");
        // this.dialogueText = dialogueBox.GetComponent<TextMeshProUGUI>();
        // this.dialogueBox.SetActive(false);
        // this.dialogueBoxBackground.SetActive(false);
        // this.battlePrimaryMenuController.BattleDialogueController.SetActive(false);
        // this.BattleMenuController.MoraleGaugeController.SetGaugeMaxValueAndCurrentValue((int)this.BattleValues.MaxPartyMoraleGauge, 0);
        // this.battlePrimaryMenuController.MoraleGaugeController.SetActive(true);
        // this.battlePrimaryMenuController.BattleMenuControllerOLD.baseMovementSpeed = this.BattleValues.BaseMovementSpeed;
        #nullable enable
        GameObject? bgmAudioObject = GameObject.FindGameObjectWithTag("BGMAudioSource");
        if (bgmAudioObject != null)
        {
            bgmController = bgmAudioObject.GetComponent<BGMController>();
        }
        #nullable disable


        // if (bgmAudioObject == null)
        // {
        //     bgmAudioObject = (GameObject)Instantiate(Resources.Load(Constants.Controllers.ResourcePaths.BGMAudioSource));
        //     if (bgmAudioObject != null)
        //     {
        //         bgmController = bgmAudioObject.GetComponent<BGMController>();
        //     }
        // }
        // else
        // {
        // }

        // this.StopTimeController();
    }

    public void PreparePartyMembers()
    {
        this.PlayerPartyController.PreparePartyMembers(this);
        this.EnemyPartyController.PreparePartyMembers(this);
    }

    public void TogglePartyMemberObjects(bool active)
        {
            this.PlayerPartyController.TogglePartyMemberObjects(active);
            this.EnemyPartyController.TogglePartyMemberObjects(active);
        }

    // private void StopTimeController()
    // {
    //     if (TimeController.instance == null)
    //     {
    //         return;
    //     }

    //     TimeController.instance.ShouldProcessTime = false;
    // }

    private void SetFocusPoint()
    {
#nullable enable
        try
        {
            this.BattleCameraManager.CameraTracker.SetTarget(this.BattleCameraManager.FocusPointCamera.transform, false, false);
            this.PrioritizeCamera(this.BattleCameraManager.FocusPointCamera);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Debug.LogError("Unable to get the focus point cinemachine camera event controller");
        }

#nullable disable
    }




    public void SetTurnCount(uint turnCount)
    {
        this._turnCount = turnCount;
        // if (turnCounter.TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI textMeshPro))
        // {
        //     textMeshPro.text = "Turn: " + turnCount;
        // }
        _onTurnChangeEvent?.Invoke(this._turnCount);
    }



    // Update is called once per frame
    void Update()
    {
        // bool? sceneTransitioning = sceneTransitionService?.IsTransitioning();
        // if (sceneTransitioning == null || sceneTransitioning == true)
        // {
        //     return;
        // }

        if (!this.BattleEventController.CanProgress())
        {
            return;
        }
        this.StateManager.UpdateState(this);
    }

    // private void OnGUI()
    // {
    //     string displayedText =
    //         "Gained GP: " + this.ObtainedGP + "\n" +
    //         "Current FPS: " + Mathf.Ceil(1 / Time.unscaledDeltaTime);
    //     GUI.Label(new Rect(10, 100, 200, 100), displayedText, textStyle);
    // }

    public void GetNewEnemyEnmityTarget(PartySlot enemy)
    {
        PartySlot startingAggro = null;
        float distance = 0;
        foreach (PartySlot partySlot in this.PlayerPartyController.PartySlots)
        {
            Vector3 partyMemberCoords = (Vector3)partySlot.GetCombatTileController().Position;
            Vector3 enemyCoords = (Vector3)enemy.GetCombatTileController().Position;
            if (startingAggro == null)
            {

                if (!partySlot.BattleEntity!.IsDead() && partyMemberCoords != null && enemyCoords != null)
                {
                    distance = Vector3.Distance(enemyCoords, partyMemberCoords);
                    startingAggro = partySlot;
                }

            }
            else
            {

                if (!partySlot.BattleEntity!.IsDead() && partyMemberCoords != null && enemyCoords != null)
                {
                    float newDistance = Vector3.Distance(enemyCoords, partyMemberCoords);
                    if (newDistance < distance)
                    {
                        // Debug.Log("Old Enmity Distance: " + distance + " for " + startingAggro.GetBattleEntity!.GetName());
                        // Debug.Log("New Enmity Distance: " + newDistance + " for " + partySlot.GetBattleEntity!.GetName());
                        startingAggro = partySlot;
                        distance = newDistance;
                    }
                }
            }
        }
        // Debug.Log(enemy.GetBattleEntity!.GetName() + " is targeting " + startingAggro.GetBattleEntity!.GetName());
        enemy.AddEnmityTarget(startingAggro, true, false, 0, 0);
    }

    //Returns true if should return from update.  Returns false otherwise
    public bool IsEncounterOver()
    {

        if (this.PlayerPartyController.AliveCount == 0)
        {
            //this.cameraFollow.ResetBattleTargetPosition();
            this.StateManager.SwitchState(this.StateManager.LoseState, this);
            return true;
        }

        if (this.EnemyPartyController.AliveCount == 0)
        {
            //this.cameraFollow.ResetBattleTargetPosition();
            // PlayerProfileData.Instance.gp += this.ObtainedGP;
            // foreach (var item in this.ObtainedDrops)
            // {
            //     PlayerProfileData.Instance.inventory.AddInventoryItem(item.Key, item.Value);
            // }
            foreach (PartySlot partySlot in this.PlayerPartyController.PartySlots)
            {
                partySlot.CombatEnd();
            }
            this.StateManager.SwitchState(this.StateManager.WinState, this);

            return true;
        }

        return false;
    }

    public void SortBattleMembersByInitiative()
    {
        if (this.currentTurnOrder == null)
        {
            return;
        }
        this.RegenerateTurnOrder(this.currentTurnOrder);
        this.RegenerateTurnOrder(this.futureTurnOrder);
        this.currentTurnOrder.Sort((battleEntity1, battleEntity2) =>
        {
            if (battleEntity2.BattleEntity!.GetRawStats().initiative > battleEntity1.BattleEntity!.GetRawStats().initiative)
            {
                return 1;
            }
            if (battleEntity2.BattleEntity!.GetRawStats().initiative < battleEntity1.BattleEntity!.GetRawStats().initiative)
            {
                return -1;
            }
            return 0;
            // return battleEntity2.battleEntity.GetRawStats().initiative.CompareTo(battleEntity1.battleEntity.GetRawStats().initiative);
        });
        this.futureTurnOrder.Sort((battleEntity1, battleEntity2) =>
        {
            if (battleEntity2.BattleEntity!.GetRawStats().initiative > battleEntity1.BattleEntity!.GetRawStats().initiative)
            {
                return 1;
            }
            if (battleEntity2.BattleEntity!.GetRawStats().initiative < battleEntity1.BattleEntity!.GetRawStats().initiative)
            {
                return -1;
            }
            return 0;
            // return battleEntity2.battleEntity.GetRawStats().initiative.CompareTo(battleEntity1.battleEntity.GetRawStats().initiative);
        });
        // for(int i = 0; i < this.turnOrder.Count; i++) {
        //     this.turnOrder[i].battleMember.turnOrder = i;
        // }
        // this.BattleMenuController.TurnOrderController.SetTurnPortraits(this.currentTurnOrder, this.futureTurnOrder);
        this._onTurnOrderChangeEvent?.Invoke(this.currentTurnOrder, this.futureTurnOrder);
    }

    public void GetNextPartyMember()
    {
        this.CurrentlyActingMember?.EndPhase(this);
        // this.CurrentlyActingMember?.turnPortraitController.SelectPortrait(false);
        // actionCursor++;

        //First, get the turn member that just acted, then add them to the future Turn Order.
        // PartySlot partySlot = this.turnOrder[0];

        //Next, remove the first index from the turnOrder.
        this.currentTurnOrder.RemoveAt(0);

        bool nextPersonAlive = false;
        while (nextPersonAlive == false && this.currentTurnOrder.Count > 0)
        {
            if (this.currentTurnOrder[0].BattleEntity!.IsDead())
            {
                this.currentTurnOrder.RemoveAt(0);
                continue;
            }

            nextPersonAlive = true;
        }

        this.futureTurnOrder = new List<PartySlot>();

        foreach (PartySlot partyMemberSlot in this.PlayerPartyController.PartySlots)
        {
            if (!partyMemberSlot.BattleEntity!.IsDead() && !this.currentTurnOrder.Contains(partyMemberSlot))
            {
                this.futureTurnOrder.Add(partyMemberSlot);
            }
        }
        
        foreach (EnemyPartySlot enemyMemberSlot in this.EnemyPartyController.PartySlots)
        {
            EnemyInfo enemyInfo = enemyMemberSlot.BattleEntity as EnemyInfo;
            if (enemyInfo == null || enemyInfo.IsDead())
            {
                continue;
            }
            if (!this.currentTurnOrder.Contains(enemyMemberSlot))
            {
                this.futureTurnOrder.Add(enemyMemberSlot);
            }

        }

        // this.futureTurnOrder.Add(partySlot);
        // this.RegenerateTurnOrder(this.futureTurnOrder);
        this.SortBattleMembersByInitiative();

        // this.BattleMenuController.TurnOrderController.SetTurnPortraits(this.currentTurnOrder, this.futureTurnOrder);
    }

    public void RegenerateTurnOrder(List<PartySlot> turnOrder)
    {
        List<PartySlot> filteredList = new List<PartySlot>();
        foreach (PartySlot partySlot in turnOrder)
        {
            if (!partySlot.BattleEntity!.IsDead())
            {
                filteredList.Add(partySlot);
            }
        }
        turnOrder = filteredList;
        this.RecalculateCastedTiles(turnOrder);
    }

    public void RecalculateCastedTiles(List<PartySlot> turnOrder)
    {
        foreach (PartySlot partySlot in turnOrder)
        {
            partySlot.ReCalculateCommandTiles();
        }
    }

    public void AddToCurrentTurnOrder(PartySlot partySlot)
    {
        this.currentTurnOrder.Add(partySlot);

        Debug.Log("Adding a new party slot to the current turn");
        this.SortBattleMembersByInitiative();
        // this.BattleMenuController.TurnOrderController.SetTurnPortraits(this.currentTurnOrder, this.futureTurnOrder);

    }

    public void AddToFutureTurnOrder(PartySlot partySlot)
    {
        this.futureTurnOrder.Add(partySlot);

        Debug.Log("adding a new party slot to the future turn");
        this.SortBattleMembersByInitiative();
        // this.BattleMenuController.TurnOrderController.SetTurnPortraits(this.currentTurnOrder, this.futureTurnOrder);
    }

    public void SetDialogueBoxText(String text)
    {
        // this.BattleMenuController.BattleDialogueController.SetDialogueBoxText(text);
    }

    // public bool IsDialogueBoxTextEmpty()
    // {
    //     return this.BattleMenuController.BattleDialogueController.IsEmpty();
    // }

    // public bool IsDialogueBoxActive()
    // {
    //     return this.BattleMenuController.BattleDialogueController.IsActive();
    // }

    // public void SetDialogueBoxActive(bool active)
    // {
    //     this.BattleMenuController.BattleDialogueController.SetActive(active);
    // }

    // public void ClearDialogueBoxText()
    // {
    //     this.BattleMenuController.BattleDialogueController.SetDialogueBoxText("");
    // }

    // public void ActivateResultScreen()
    // {
    //     this.BattleMenuController.ActivateResultScreen();
    // }

#nullable enable
    // public void ToggleSideCameraActiveOnEntity() {
    //     // Debug.Log("setting camera target");

    //     this.SetCameraTargetToActingEntity(true, true);
    //     try {
    //         // this.cameraTrackerController.transform.rotation = this.currentlyActingMember.BattleEntityGO.transform.rotation;
    //     } catch (Exception e) {
    //         Debug.LogError(e);
    //         Debug.LogError("Somehow, battle entity GO not present");
    //     }
    // }
    public void ToggleSideCameraActiveOnEntity()
    {
        // Debug.Log("setting camera target");

        this.SetCameraTargetToActingEntity(true);
        try
        {
            // this.cameraTrackerController.transform.rotation = this.currentlyActingMember.BattleEntityGO.transform.rotation;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Debug.LogError("Somehow, battle entity GO not present");
        }
    }

    public void ToggleFreeCameraActiveOnEntity()
    {
        // Debug.Log("setting camera target");


        this.SetCameraTargetToActingEntity(false);
    }
#nullable disable

    public void ToggleFreeCameraActive(Transform transform)
    {
        this.SetCameraTarget(transform, false);
    }



    private void SetCameraTarget(Transform transform, bool shouldTrackRotation)
    {
        this.BattleCameraManager.CameraTracker.SetTarget(transform, shouldTrackRotation, false);
    }

    private void SetCameraTargetToActingEntity(bool shouldTrackRotation)
    {
        if (this.CurrentlyActingMember == null || this.CurrentlyActingMember.BattleEntityGO == null)
        {
            return;
        }

        this.BattleCameraManager.CameraTracker.SetTarget(this.CurrentlyActingMember.BattleEntityGO.TargetOffsetAnchorPoint.transform, shouldTrackRotation, false);
    }


    // public bool IsCurrentCamera(CinemachineCameraEventController cinemachineCameraEventController) {
    //     return this.currentCamera = cinemachineCameraEventController;
    // }
        #nullable enable

    public BattleActionController? InstantiateBattleActionController(BattleAnimationEventSpawn animationEvent)
    {
        if (animationEvent.AnimationPrefab == null)
        {
            Debug.LogWarning("Animation Event prefab is null");
            return null;
        }

        BattleActionController? instantiatedObject = Instantiate(animationEvent.AnimationPrefab, animationEvent.Position, animationEvent.Rotation);

        if (instantiatedObject == null)
        {
            throw new Exception("BattleActionController failed to instantiate.");
        }

        if (animationEvent.animationParent != null)
        {
            instantiatedObject.transform.SetParent(animationEvent.animationParent);
        }


        Debug.LogWarning("Setting animation event struct in battle action controller");
        instantiatedObject.SetBattleActionControllerContent(animationEvent);
        // instantiatedObject.transform.LookAt(2 * instantiatedObject.transform.position - animationEvent.lookAtTargetPosition);
        return instantiatedObject;
    }
    #nullable disable


}


[Serializable]
public class GaugeValues
{

    [SerializeField, Range(0, 500)]
    private uint _partyMoraleGauge = 0;
    public uint PartyMoraleGauge { get => this._partyMoraleGauge; set => this._partyMoraleGauge = value; }

}

[Serializable]
public class BattleAnimationSpawner
{
#nullable enable
    public void HandleAndProcessUserBattleCommandAnimations(BattleManager battleController, BattleActionAnimationController? battleActionAnimationController, BattleCommand battleCommand, PartySlot caster, Action? onAnimationProgressAction, Action? onAnimationEndAction)
    {
        //Create a list of animation Objects
        List<GameObject> animationObjects = new();
        //Get the Battle Action Animation Controller
        try
        {
            if (battleActionAnimationController == null)
            {
                Debug.LogError("No Battle Action Animation Controller found!");

                return;
            }


            BattleActionController? animation = null;

            if (battleActionAnimationController.UserAnimation != null)
            {
                animation = battleActionAnimationController.UserAnimation;
            }
            else
            {
                animation = caster.BattleEntityGO.GetGenericUserAnimation(battleActionAnimationController.animationTrigger);
            }

            if (animation == null)
            {
                Debug.Log("Adding Wait Event!");
                battleController.BattleEventController.AddBattleEvent(new WaitEvent(battleController.BattleValues.NoAnimationWaitTimer,  () => { onAnimationProgressAction?.Invoke();  onAnimationEndAction?.Invoke(); }));
                return;
            }

            Debug.Log("---Adding User Animation step");
            BattleAnimationEventSpawn animationEvent = new(animation, caster.BattleEntityGO, caster.GetCombatTileController().GetGameObjectAnchor().transform, caster.BattleEntityGO.transform, caster.BattleEntityGO.transform.position, onAnimationProgressAction, onAnimationEndAction);
            battleController.BattleEventController.AddBattleEvent(new BattleAnimationEvent(new List<BattleAnimationEventSpawn> { animationEvent }, battleController, false));
            return;
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
            onAnimationProgressAction?.Invoke();
        }


    }




    public void HandleTargetLocationAnimationEvent(BattleManager battleController, BattleActionAnimationController battleActionAnimationController, PartySlot caster, BattleCommand battleCommand, System.Action duringAnimationCallBack, System.Action onAnimationEndCallBack, Vector3 lookAtTargetPosition, List<PartySlot> targetAnimationList, CombatTileController targetedTile)
    {
        try
        {

            if (battleActionAnimationController.TargetLocationAnimation == null)
            {
                duringAnimationCallBack?.Invoke();
                onAnimationEndCallBack?.Invoke();
                return;
            }


            List<BattleAnimationEventSpawn> animationEvents = new List<BattleAnimationEventSpawn>();
            if (targetedTile != null)
            {
                // targetAnimationList.Add(targetedTile.GetGameObjectAnchor());
                Transform parentTransform = targetedTile.GetGameObjectAnchor().transform;
                Quaternion rotation = Quaternion.Euler(Vector3.zero);
                animationEvents.Add(new BattleAnimationEventSpawn(battleActionAnimationController.TargetLocationAnimation, caster.BattleEntityGO, parentTransform.position, rotation, lookAtTargetPosition, duringAnimationCallBack, onAnimationEndCallBack));
            }
            else
            {
                Debug.LogError("Target tiled somehow null!");
            }

            animationEvents.AddRange(this.GetTargetAnimationStructs(battleActionAnimationController, caster, targetAnimationList, lookAtTargetPosition));

            if (animationEvents.Count != 0)
            {
                battleController.BattleEventController.AddBattleEvent(new BattleAnimationEvent(animationEvents, battleController));
                return;
            }

            duringAnimationCallBack?.Invoke();
            onAnimationEndCallBack?.Invoke();

        }
        catch (Exception e)
        {
            Debug.LogWarning("Error During Target Animations: " + e.Message);
            duringAnimationCallBack?.Invoke();
            onAnimationEndCallBack?.Invoke();
        }

        return;
    }

    private List<BattleAnimationEventSpawn> GetTargetAnimationStructs(BattleActionAnimationController battleActionAnimationController, PartySlot caster, List<PartySlot> targetAnimationList, Vector3 lookAtTargetPosition)
    {
        List<BattleAnimationEventSpawn> battleAnimationEventSpawnStructs = new();

        if (battleActionAnimationController.TargetAnimation == null)
        {
            return battleAnimationEventSpawnStructs;
        }

        List<BattleObject> targets = new();

        foreach (PartySlot partySlot in targetAnimationList)
        {
            if (targets.Contains(partySlot.BattleEntityGO))
            {
                continue;
            }

            targets.Add(partySlot.BattleEntityGO);
        }


        foreach (BattleObject battleObject in targets)
        {
            if (battleObject == null)
            {
                continue;
            }
            Transform parentTransform = battleObject.transform;
            Quaternion rotation = Quaternion.Euler(Vector3.zero);
            battleAnimationEventSpawnStructs.Add(new BattleAnimationEventSpawn(battleActionAnimationController.TargetAnimation, caster.BattleEntityGO, parentTransform.position, rotation, lookAtTargetPosition, null, null));
        }
        Debug.LogWarning("Finished adding target animations!");

        return battleAnimationEventSpawnStructs;
    }
    #nullable disable
}
}