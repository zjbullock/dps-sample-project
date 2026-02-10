using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DPS.TacticalCombat {

[CreateAssetMenu(fileName = "Resolve Gauge", menuName = "ScriptableObjects/Resolve Gauge/Standard")]

public class ResolveGaugeSO : ScriptableObject {
    public string gaugeName;
    public string description;
    public int maxResolveGauge = 3;

    public ResolveGaugeType gaugeType;

    #nullable enable
    public AudioClip? gaugeAudio;

    public Color listItemColorOverride;

    [SerializeField]
    private ResolveGaugePartyMoraleTypes partyMoralePointsCastSkillType;

    [SerializeField]
    [Range(0, 100)]
    private uint partyMoralePointCastSkill = 20;

    private int resolvePointGain = 1;

    [SerializeField]
    private List<BattleEvent> battleEvents = new List<BattleEvent>();
    public enum BattleEvent {
        BeginPhase,
        EndPhase,
        TakingDamage,
        SuccessfulBlock,
        SuccessfulEvade,
        DealingDamage,
        CastingSkill
    }
    // public virtual void AddResolvePoints(CharacterInfo characterInfo, ActiveSkillSO activeSkillSO){}
    // public virtual void SubtractResolvePoints(CharacterInfo characterInfo, ActiveSkillSO activeSkillSO){}

    // public  void AddResolvePoints(CharacterInfo characterInfo, OldActiveSkillSO activeSkillSO){
    //     characterInfo.resolvePoints = characterInfo.resolvePoints + activeSkillSO.resolveGaugeEffects.increaseResolveGaugePoints > maxResolveGauge ? maxResolveGauge : characterInfo.resolvePoints + activeSkillSO.resolveGaugeEffects.increaseResolveGaugePoints;
    // }

    // public void SubtractResolvePoints(CharacterInfo characterInfo, OldActiveSkillSO activeSkillSO){
    //     characterInfo.resolvePoints = ((characterInfo.resolvePoints - activeSkillSO.resolveGaugeEffects.decreaseResolveGaugePoints) + (int) Math.Abs(characterInfo.resolvePoints - activeSkillSO.resolveGaugeEffects.decreaseResolveGaugePoints)) / 2;
    // }

    //Performs turn begin actions.
    public virtual void OnBeginPhase(PartySlot partySlot, BattleManager battleController) {
        PlayerPartySlot? playerPartySlot = (PlayerPartySlot) partySlot;
        if (playerPartySlot == null) {
            return;
        }
        if(this.battleEvents.Contains(BattleEvent.BeginPhase)) {
            this.IncreasePoints(playerPartySlot);
        }
        return;
    }

    public virtual void OnEndPhase(PartySlot partySlot, BattleManager battleController) {
        PlayerPartySlot? playerPartySlot = (PlayerPartySlot) partySlot;
        if (playerPartySlot == null) {
            return;
        }
        if(this.battleEvents.Contains(BattleEvent.EndPhase)) {
            this.IncreasePoints(playerPartySlot);
        }
        return;
    }

    public virtual void OnTakingDamage(PartySlot partySlot, BattleManager battleController) {
        PlayerPartySlot? playerPartySlot = (PlayerPartySlot) partySlot;
        if (playerPartySlot == null) {
            return;
        }
        if(this.battleEvents.Contains(BattleEvent.TakingDamage)) {
            this.IncreasePoints(playerPartySlot);
        }
        return;
    }

    public virtual void OnSuccessfulBlock(PartySlot partySlot, BattleManager battleController) {
        PlayerPartySlot? playerPartySlot = (PlayerPartySlot) partySlot;
        if (playerPartySlot == null) {
            return;
        }
        if(this.battleEvents.Contains(BattleEvent.SuccessfulBlock)) {
            this.IncreasePoints(playerPartySlot);
        }
        return;
    }

    public virtual void OnSuccessfulEvade(PartySlot partySlot, BattleManager battleController) {
        PlayerPartySlot? playerPartySlot = (PlayerPartySlot) partySlot;
        if (playerPartySlot == null) {
            return;
        }
        if(this.battleEvents.Contains(BattleEvent.SuccessfulEvade)) {
            this.IncreasePoints(playerPartySlot);
        }
        this.ManipulatePartyMorale(battleController, this.partyMoralePointsCastSkillType, this.partyMoralePointCastSkill);
        return;
    }

    public virtual void OnDealingDamage(PartySlot partySlot, BattleManager battleController) {
        PlayerPartySlot? playerPartySlot = (PlayerPartySlot) partySlot;
        if (playerPartySlot == null) {
            return;
        }
        if(this.battleEvents.Contains(BattleEvent.DealingDamage)) {
            Debug.Log("Resolve increase");
            this.IncreasePoints(playerPartySlot);
        }
        return;
    }

    public virtual void OnCastingSkill(PartySlot partySlot, BattleManager battleController) {
        PlayerPartySlot? playerPartySlot = (PlayerPartySlot) partySlot;
        if (playerPartySlot == null) {
            return;
        }
        if(this.battleEvents.Contains(BattleEvent.CastingSkill)) {
            this.IncreasePoints(playerPartySlot);
        }
        this.ManipulatePartyMorale(battleController, this.partyMoralePointsCastSkillType, this.partyMoralePointCastSkill);
        return;
    }

    private void IncreasePoints(PlayerPartySlot partySlot) {
        CharacterInfo? characterInfo = (CharacterInfo) partySlot.BattleEntity!;
        if (characterInfo == null) {
            return;
        }

        int resolvePoints = characterInfo.GetResolvePoints();
        resolvePoints += this.resolvePointGain;
        if(resolvePoints > characterInfo.GetResolveGauge()!.maxResolveGauge) {
            resolvePoints = characterInfo.GetResolveGauge()!.maxResolveGauge;
        }
        characterInfo.SetResolvePoints(resolvePoints);
        // user.battlePortrait.AddResolvePoints(this.activeSkillResolvePoints);
        // partySlot.battlePortrait.SetResolvePoints(resolvePoints);
    }

    private void ManipulatePartyMorale(BattleManager battleController, ResolveGaugePartyMoraleTypes resolveGaugePartyMoraleType, uint partyMoralePoints) {
        if (battleController == null) {
            return;
        }

        switch(resolveGaugePartyMoraleType) {
            case ResolveGaugePartyMoraleTypes.Increase:
                battleController.AddPartyPoints(partyMoralePoints);
                break;
            case ResolveGaugePartyMoraleTypes.Decrease:
                battleController.SubtractPartyPoints(partyMoralePoints);
                break;
        }

    }

    private enum ResolveGaugePartyMoraleTypes {
        None,
        Increase,
        Decrease
    }
}


public enum ResolveGaugeType {
    Point,
    Gauge
}
}