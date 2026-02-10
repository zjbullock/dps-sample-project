using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Status Ailment", menuName = "ScriptableObjects/Status Ailment")]
[Serializable]
public class StatusAilmentSO : ScriptableObject {
    // Start is called before the first frame update
    public string statusAilmentName;
    public string description;

    [SerializeField]
    [Range(0, 100)]
    private int _statusAilmentActivationRate = 10;

    public int StatusAilmentActivationRate { get => this._statusAilmentActivationRate; }

    //DoT flags
    //If true, percentage damage comes from max HP.  If false, uses current HP  
    public bool affectsMaxHP;
    [RangeAttribute(0f, 1f)]
    public float percentageDamage;

    public string flavorText;

    public bool isInfiniteTurnCount;
    public int minTurnCount;
    public int maxTurnCount;

    [SerializeField]
    private ElementSO _element;

    public Sprite statusAilmentIcon;

    [Tooltip("Determines if a turn can been taken on this status ailment type")]
    [SerializeField]
    public bool preventsTurn = false;

    [Tooltip("Determines if a skill action can be taken on this status ailment type")]
    [SerializeField]
    public bool preventsSkillAction = false;


    [Tooltip("Determines if an item action can be taken on this status ailment type")]
    [SerializeField]
    public bool preventsItemActions = false;

    [Tooltip("Determines if damage will be done by the status ailment type")]
    [SerializeField]
    public bool canDealDamage = false;

    [Tooltip("The status ailment type for use in adding status ailment resistances")]
    [SerializeField]
    private ResistableStatusAilments statusAilmentType;

    public void InflictStatusAilment(PartySlot partySlot, StatusAilmentActivation statusAilmentActivation) {
        IBattleEntity battleEntity = partySlot.BattleEntity;
        if (battleEntity == null || !this.CanInflictStatusAilment(battleEntity, statusAilmentActivation)) {
            return;
        }


        System.Random random = new System.Random();

        if(this.StatusAilmentCalculation(random, battleEntity, statusAilmentActivation)) {
            Debug.Log(battleEntity.GetName() + " has been inflicted with " + statusAilmentActivation.statusAilment.statusAilmentName);
            battleEntity.SetInflictedAilment(statusAilmentActivation.statusAilment);
            if(preventsTurn) {
                partySlot.BattleEntityGO?.ToggleEffect(AnimationEffect.Casting, false);

                partySlot.GetBattleMember().BattleCommand = null;
            }
        }
    }

    public void InflictStatusAilment(IBattleEntity battleEntity, StatusAilmentActivation statusAilmentActivation) {
        if (battleEntity == null || !this.CanInflictStatusAilment(battleEntity, statusAilmentActivation)) {
            return;
        }


        System.Random random = new System.Random();

        if(this.StatusAilmentCalculation(random, battleEntity, statusAilmentActivation)) {
            Debug.Log(battleEntity.GetName() + " has been inflicted with " + statusAilmentActivation.statusAilment.statusAilmentName);
            battleEntity.SetInflictedAilment(statusAilmentActivation.statusAilment);
        }
    }

    /** 
        A virtual method that can be overridden to perform a different StatusAilmentCalculation
    */
    private bool StatusAilmentCalculation(System.Random random, IBattleEntity battleEntity, StatusAilmentActivation statusAilmentActivation) {
        int resistance = 0;
        #nullable enable
        StatusAilmentResistances? statusAilmentResistances = battleEntity.GetStatusAilmentResistances();
        if (statusAilmentResistances == null) {
            return statusAilmentActivation.activationRate > (random.Next(0,100));
        }
        #nullable disable

        switch (this.statusAilmentType) {
            case ResistableStatusAilments.Sleep:
                resistance = statusAilmentResistances.sleepResistance;
                break;
            case ResistableStatusAilments.Poison:
                resistance = statusAilmentResistances.poisonResistance;
                break;
            case ResistableStatusAilments.Freeze:
                resistance = statusAilmentResistances.freezeResistance;
                break;
            case ResistableStatusAilments.Burn:
                resistance = statusAilmentResistances.burnResistance;
                break;
            case ResistableStatusAilments.Paralyze:
                resistance = statusAilmentResistances.paralysisResistance;
                break;
        }
        return (statusAilmentActivation.activationRate - resistance) > (random.Next(0, 100));
    }

    private bool CanInflictStatusAilment(IBattleEntity battleEntity, StatusAilmentActivation statusAilmentActivation) {
        if(battleEntity.IsDowned() || statusAilmentActivation == null || (statusAilmentActivation != null && statusAilmentActivation.statusAilment == null)) {
            Debug.Log("No Status Ailment");
            return false;
        }
        Debug.Log("Attempting Status Ailment activation");
        if(battleEntity.GetInflictedAilment() != null && battleEntity.GetInflictedAilment().statusAilment != null) {
            Debug.Log("Status Ailment already exists");
            return false;
        }
        return true;
    }

    #nullable enable
    public void DealDamage(BattleManager battleController, PartySlot partySlot) {
        IBattleEntity? battleEntity = partySlot.BattleEntity;
        if (battleEntity == null) {
            return;
        }
        RawStats rawStats = partySlot.BattleEntity!.GetRawStats();

        int damageOverTimeHP = BattleProcessingStatic.GetPercentDamage(this.affectsMaxHP ? rawStats.maxHp : rawStats.hp, this.percentageDamage);

        if (damageOverTimeHP > 0)
        {
            partySlot.ProcessDamage(battleController, damageOverTimeHP, _element, additionalText: this.statusAilmentName);
        }
        
    }
    
    #nullable disable
    // public bool PreventsAction() {

    // }

    private enum ResistableStatusAilments
    {
        None,
        Poison,
        Freeze,
        Burn,
        Sleep,
        Paralyze
    }
}
}
