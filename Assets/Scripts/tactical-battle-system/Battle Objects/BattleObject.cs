using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DPS.Common;

namespace DPS.TacticalCombat
{
    [Serializable]
public abstract class BattleObject : MonoBehaviour {    
    public bool targeted;

    protected BattleMember battleMember;

    protected BattleManager battleController;

    public GameObject playerCursor;

    public GameObject healTextBox;

    // public TextMeshPro healText;

    // [SerializeField]
    // protected GameObject statusBar;

    // public GaugeController healthBar;

    // public PoiseUIController poiseUI;

    private float healthBarTimer = 3f;

    private float maxHealthBarTimer = 3f;

    // private float healthBarRevealSpeed = 1f;
    private float currentProcessPopupTimer = 0f;

    [SerializeField]
    [Range(0.2f, 1f)]
    private float processPopupTimer = 0.2f;

    public bool timerIsRunning;

    public float height;

    protected List<PopupEvent> popupEvents;

    [Tooltip("The anchorpoint for status messages of a Battle Object")]
    [SerializeField]
    private GameObject targetOffsetAnchorPoint;

    public GameObject TargetOffsetAnchorPoint { get => this.targetOffsetAnchorPoint; }

    [Tooltip("Denotes whether this battle object is ready to be used or fill out different gauges.")]
    [SerializeField]
    protected bool isReady;

    [Tooltip("Hold a reference to the Side Focus Camera")]
    [SerializeField]
    protected CinemachineCameraEventController sideFocusCam;

    public CinemachineCameraEventController SideFocusCam { get => this.sideFocusCam; }

    [Tooltip("Hold a reference to the Free Look Focus Camera")]
    [SerializeField]
    protected CinemachineCameraEventController freeLookCam;

    public CinemachineCameraEventController FreeLookCam { get => this.freeLookCam; }

    [Header("Generic Battle Animations")]
    [SerializeField]
    [Tooltip("Placeholder for generic animations")]
    private GenericDictionary<AnimationTrigger, BattleActionController> genericAnimations;




    void Awake() {
        this.BattleObjectAwake();
        this.popupEvents = new List<PopupEvent>();
    }

    public Vector3 GetTargetAnchorPointPosition() {
        if (this.targetOffsetAnchorPoint == null) {
            return Vector3.zero;
        }

        return this.targetOffsetAnchorPoint.transform.position;
    }


    protected virtual void BattleObjectAwake() {
        this.targeted = false;
        this.playerCursor = null;
        // this.statusBar = null;
        this.timerIsRunning = false;
        this.maxHealthBarTimer = healthBarTimer;
        this.isReady = false;
        this.height = 1.5f;
    }

    void Update() {
        this.UpdateBattleObject();
        this.PopupProcess();
        //Controls the StatusBar when not currently active.
        if (timerIsRunning) {
            if(healthBarTimer > 0) {
                healthBarTimer -= Time.deltaTime;
            } else {
                this.ToggleStatusBar(false);
            }
        }
    }

    public void TriggerPopup(bool trigger) {
        this.timerIsRunning = trigger;
        healthBarTimer = maxHealthBarTimer;
    }

    protected virtual void UpdateBattleObject() {
        return;
    }
    
    public virtual bool IsCommandMenuVisible() {
        return false;
    }

    public void UpdateGauges() {
        // if (poiseUI != null) {
        //     poiseUI.SetDownedStatus(battleMember.GetBattleEntity.GetPoiseBreakState().poisePoints);
        // }
    }

    public void ToggleStatusBar(bool toggle, bool popup = true) {
        // if (this.statusBar != null && this.statusBar.activeSelf != toggle) {
        //     this.statusBar.SetActive(toggle);

        // }
        if (popup) {
            this.TriggerPopup(toggle);
        }
    }
    
    public BattleMember GetBattleMember() {
        return this.battleMember;
    }

    public IBattleEntity GetBattleEntity {
        get => this.battleMember.GetBattleEntity;
    }

    public virtual void SetBattleMember(BattleMember battleMember) {
        this.battleMember = battleMember;
    }

    private void PopupProcess() {
        this.ProcessPopUpEventTimer();
        if (this.popupEvents.Count == 0 || this.currentProcessPopupTimer > 0) {
            return;
        }


        PopupEvent popupEvent = this.popupEvents[0];

        Vector3 floatingBox = new Vector3(transform.position.x, transform.position.y + (this.targetOffsetAnchorPoint != null ? this.targetOffsetAnchorPoint.transform.position.y : 0f), transform.position.z);

        PopUpController popUpController = BattleManager.instance.SpawnPopup(floatingBox, transform.rotation);


        popUpController.TriggerPopup(popupEvent.value, floatingBox, popupEvent.color);

        this.ResetProcessPopUpEventTimer();
        this.popupEvents.RemoveAt(0);
    }

    private void ProcessPopUpEventTimer() {
        this.currentProcessPopupTimer -= Time.deltaTime;
    }

    private void ResetProcessPopUpEventTimer() {
        this.currentProcessPopupTimer = this.processPopupTimer;
    }

    public struct PopupEvent {
        public string value;
        public Color color;

        public PopupEvent(string value, Color color) {
            this.value = value;
            this.color = color;
        }
    }


    public virtual void TakeDamage(string damage) {
        // GameObject damageTextBox = (GameObject) Instantiate(Resources.Load(StringConstants.UI.BattlePopUp), this.GetTargetAnchorPoint(), transform.rotation);
        // Vector3 floatingBox = new Vector3(transform.position.x, transform.position.y + (this.targetOffsetAnchorPoint != null ? this.targetOffsetAnchorPoint.transform.position.y : 0f), transform.position.z);
        // PopUpController popUpController = damageTextBox.GetComponent<PopUpController>();
        // popUpController.TriggerPopup(damage, floatingBox, Color.red);

        this.popupEvents.Add(new PopupEvent(damage, Color.red));
        // if(healthBar != null) {
        //     healthBar.SetCurrentValue(battleMember.GetBattleEntity.GetRawStats().hp);
        //     statusBar.SetActive(true);
        //     TriggerPopup(true);
        // }

    }

    public virtual void HealHP(string healAmount) {
        // Debug.Log(transform.position);
        // GameObject healTextBox = (GameObject) Instantiate(Resources.Load(StringConstants.UI.BattlePopUp), this.GetTargetAnchorPoint(), transform.rotation);
        // Vector3 floatingBox = new Vector3(transform.position.x, transform.position.y + (this.targetOffsetAnchorPoint != null ? this.targetOffsetAnchorPoint.transform.position.y : 0f), transform.position.z);
        // PopUpController popUpController = healTextBox.GetComponent<PopUpController>();
        // popUpController.TriggerPopup(healAmount, floatingBox, Color.green);
        this.popupEvents.Add(new PopupEvent(healAmount, Color.green));
        // if(healthBar != null) {
        //     healthBar.SetCurrentValue(battleMember.GetBattleEntity.GetRawStats().hp);
        //     statusBar.SetActive(true);
        //     TriggerPopup(true);
        // }
    }
    

    public void StatusMessage(string message) {
        // GameObject damageTextBox = (GameObject) Instantiate(Resources.Load(StringConstants.UI.BattlePopUp), this.GetTargetAnchorPoint(), transform.rotation);
        // PopUpController popUpController = damageTextBox.GetComponent<PopUpController>();
        // Vector3 floatingBox = new Vector3(transform.position.x, this.GetTargetAnchorPoint().y, transform.position.z);
        // popUpController.TriggerPopup(message, floatingBox, Color.white);
        this.popupEvents.Add(new PopupEvent(message, Color.white));

    }


    public abstract bool HasAnimator();

    #nullable enable
    public abstract void TriggerAnimation(AnimationTrigger animationTrigger, string? specialAnimation = null);

    public BattleActionController? GetGenericUserAnimation(AnimationTrigger animationTrigger) {

        if(this.genericAnimations.TryGetValue(animationTrigger, out BattleActionController battleActionController)) {

            return battleActionController;
        }

        return null;
    }

    public abstract void SetAnimationState(string animationState);

    public abstract void SetAnimationState(AnimationStates animationState);

    public abstract void ToggleEffect(AnimationEffect animationEffect, bool toggle);

    public abstract void TargetBattleObject (bool targetObject);

    public abstract void SetMovement(bool isRunning = false, bool isWalking = false);

    public abstract void EndMovement();

    public virtual void OnSetCombatTileController(BattleManager battleManager, PartySlot partySlot) {
        return;
    }

    public virtual void DestroyBattleObject() {
        return;
    }

    public virtual bool CanMoveToTile(CombatTileController combatTileController, PartySlot partySlot) {
        return true;
    }
}

}
