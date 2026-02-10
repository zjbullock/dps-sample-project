using System.Collections.Generic;
using UnityEngine;
using System;

namespace DPS.TacticalCombat {

[Serializable]
public abstract class PartySlot {
    public PartySlot() {
        this.battleMember = null;
        this.canMove = true;
        this.enmityList = new List<EnmityTarget>();
        this.summonedEntities = new List<SummonedEntityPartySlot>();
    }

    public PartySlot(BattleMember battleMember, BattleObject battleObject = null) {
        this.battleMember = battleMember;
        this.canMove = true;
        this.enmityList = new List<EnmityTarget>();
        this.summonedEntities = new List<SummonedEntityPartySlot>();
        this.battleObject = battleObject;
    }

    [SerializeField, SerializeReference]
    protected BattleMember battleMember;

    [SerializeField, SerializeReference]
    protected BattleObject battleObject;


    #nullable enable
    [SerializeField]
    protected CombatTileController currentCombatTileController;

    [SerializeField]
    protected CombatTileController previousCombatTileController;

    public int Width { get => this.GetWidth(); }

    protected virtual int GetWidth() {
        return 0;
    }

    public bool canMove;

    [SerializeField]
    public List<EnmityTarget> enmityList;

    [Header("Subscribable Events")]
    [SerializeField]
    #nullable enable
    protected Action<PartySlot>? onBattleStatsChange = delegate {};

    #nullable disable

    [SerializeField]
    [Tooltip("The list of summoned entities that are tied to this battle object")]
    private List<SummonedEntityPartySlot> summonedEntities;

    public CombatTileController GetCombatTileController() {
        return this.currentCombatTileController;
    }

    public virtual void SetCombatTileController(BattleManager battleManager, CombatTileController combatTileController) {
        if (this.currentCombatTileController == null) {
            this.previousCombatTileController = combatTileController;
        } else {
            this.previousCombatTileController = this.currentCombatTileController;
        }
        this.currentCombatTileController = combatTileController;
        this.BattleEntityGO.OnSetCombatTileController(battleManager, this);
    }

#nullable enable
    public IBattleEntity? BattleEntity  {
        get => this.battleMember.GetBattleEntity;
    }

    public BattleMember GetBattleMember() {
        return this.battleMember;
    }

    public void ResetTileCoordinatesToPrevious() {
        Debug.LogWarning("Previous Tile: " + this.previousCombatTileController.Position);
        this.currentCombatTileController = this.previousCombatTileController;
    }
    


    protected void processCommand(BattleManager battleController, CombatTileController combatTileController, System.Action? callBack, BattleCommand? commandOverride = null)
    {
        if (commandOverride != null)
        {
            Debug.Log("Performing command override!");
            Debug.Log("Command override: " + commandOverride.CommandName);
            commandOverride.ExecuteCommand(battleController, callBack);
            return;
        }
        
        if (this.battleMember.BattleCommand != null) {
            Debug.Log("Performing base command");
            Debug.Log("Base Command: " + this.battleMember.BattleCommand.CommandName);
            this.battleMember.BattleCommand.ExecuteCommand(battleController, callBack);
            return;
        }
        
        callBack?.Invoke();

    }

    public void AddEnmityTarget(PartySlot newEnmityTarget, bool isInfinite, bool isAggro, int enmityValue, int aggroDuration) {
        if (newEnmityTarget == null) {
            return;
        }

        if (typeof(SpawnableObjectPartySlot).IsAssignableFrom(newEnmityTarget.GetType())) {
            return;
        }
        
        if(this.battleMember.battleMemberType == BattleMemberType.Enemy) {
            if(isAggro) {
                foreach(EnmityTarget enmityTarget in this.enmityList) {
                    enmityTarget.isAggro = false;
                }
            }
            foreach(EnmityTarget aggroTarget in enmityList) {
                if (aggroTarget.partySlot == newEnmityTarget) {
                    ModifyEnmityForTarget(aggroTarget, enmityValue, isAggro);
                    return;
                }
            }

            this.enmityList.Add(new EnmityTarget(newEnmityTarget, isInfinite, enmityValue, isAggro, aggroDuration));
            ReOrganizeEnmityList();
        } else {
            if (isAggro) {
                this.enmityList.Clear();
                this.enmityList.Add(new EnmityTarget(newEnmityTarget, isInfinite, enmityValue, isAggro, aggroDuration));
                ReOrganizeEnmityList();
            }
        }
    }

    public BattleObject BattleEntityGO { get => this.battleObject; }

    public void SetBattleObject(BattleObject battleObject) {
        battleObject.SetBattleMember(this.battleMember);
        this.battleObject = battleObject;
    }


    private void ModifyEnmityForTarget(EnmityTarget aggroTarget, int enmityValue, bool isAggro) {
        aggroTarget.enmityValue += enmityValue;
        aggroTarget.isAggro = isAggro;
        ReOrganizeEnmityList();
    }

    public void ReOrganizeEnmityList() {
        this.enmityList.Sort((enmity1, enmity2) => {
            if(enmity2.enmityValue > enmity1.enmityValue) {
                return 1;
            } else if (enmity2.enmityValue < enmity1.enmityValue) {
                return -1;
            } else {
                return 0;
            }
        });

        this.enmityList.Sort((enmity1, enmity2) => {
            if(enmity2.isAggro && !enmity1.isAggro) {
                return 1;
            } else if (!enmity2.isAggro && enmity1.isAggro) {
                return -1;
            } else {
                return 0;
            }
        });    
    }

    public virtual void SetDownedStatus(int poisePoints) {
        return;
    }

    public virtual void UpdateGauges() {
        return;
    }

    protected void EndPhaseEnmityReduction() {
        List<EnmityTarget> newEnmityList = new List<EnmityTarget>();
        foreach(EnmityTarget aggroTarget in this.enmityList) {
            EnmityTarget newEnmity = new EnmityTarget(aggroTarget);
            if (newEnmity.isAggro) {
                newEnmity.aggroDuration--;
                if (newEnmity.aggroDuration > 0) {
                    newEnmityList.Add(newEnmity);
                    continue;
                }
                newEnmity.isAggro = false;
            }
            newEnmity.enmityValue -= 10;
            if ( newEnmity.isInfinite || (newEnmity.enmityValue >= 0) ) {
                newEnmityList.Add(newEnmity);
            }

        }
        this.enmityList = newEnmityList;
        this.ReOrganizeEnmityList();
    }

    public void RemoveDeadEnmityTargets() {
        List<EnmityTarget> newEnmityList = new List<EnmityTarget>();
        foreach(EnmityTarget aggroTarget in this.enmityList) {
            if(aggroTarget.partySlot.BattleEntity != null && !aggroTarget.partySlot.BattleEntity!.IsDead()) {
                newEnmityList.Add(aggroTarget);
            }
        }
        this.enmityList = newEnmityList;
    }


    public virtual void EndPhase(BattleManager battleController) {
        this.canMove = true;

        this.RemoveDeadEnmityTargets();
        this.EndPhaseEnmityReduction();

        // if (combatTileController != null) {
        //     this.SetBattleObjectVector3(combatTileController);
        // }

        if (this.BattleEntity!.IsDowned() ) {
            Debug.Log("party member downed");
            return;
        }

        this.GetBattleMember()!.EndPhase();

    }

    public virtual void PostMovePhase(BattleManager battleController) {
        return;
    }

    public virtual void CombatEnd() {
        if (this.battleMember != null && this.BattleEntity != null) {
            this.BattleEntity!.CombatEndStats();
        }
        this.EndCombatAbilities();
        return;
    }

    public virtual void BeginPhase(BattleManager battleController) {
        this.BattleEntity!.BeginPhase();
        this.SetPreviousCombatTileControllerAsCurrent();
        this.onBattleStatsChange?.Invoke(this);
        if(this.BattleEntity!.IsDowned()) {
            return;
        }
        this.battleObject?.SetAnimationState(AnimationStates.Default);
    }

    private void SetPreviousCombatTileControllerAsCurrent() {
        this.previousCombatTileController = this.currentCombatTileController;
    }

    public virtual void CombatBegin() {
        this.BeginCombatAbilities();
        // if (combatTileController != null) {
        //     this.SetBattleObjectVector3(combatTileController);
        // }
        return;
    }

    public virtual void EndCombatAbilities() {
        return;
    }
    public virtual void BeginCombatAbilities() {
        return;
    }

    public virtual void PostCommandAbilities() {
        return;
    }

    public virtual void HPChangeAbilities() {
        return;
    }

    public virtual void EndPhaseAbilities() {
        return;
    }

    public virtual void BeginPhaseAbilities() {
        return;
    }

    public virtual void PostMoveAbilities() {
        return;
    }
    
    public virtual void OnEvade(BattleManager battleController) {
        this.StatusMessage(DamageStatusTexts.MISS + "");
        return;
    }

    public virtual void OnBlock (BattleManager battleController) {
        this.StatusMessage(DamageStatusTexts.BLOCK + "");
        return;
    }

    public virtual void OnReaction(List<ElementSO> elements, System.Action callBack)
    {
        callBack?.Invoke();
        return;
    }

    public virtual void ProcessDamage(BattleManager battleController, int damage, ElementSO element, string? additionalText = null) {

        string statusText = "";
        switch(this.BattleEntity!.GetElements()[element]) {
            case ElementalResistance.Nullified:
                this.StatusMessage(DamageStatusTexts.UNAFFECTED + "");
                break;
            case ElementalResistance.Absorbed:
                this.HealHP(damage, DamageStatusTexts.DRAIN + "");
                break;
            default:
                this.TakeDamage(battleController, damage, statusText, element, additionalText: additionalText);
                break;
        }
        this.HPChangeAbilities();
        return;
    }
    

    protected virtual void TakeDamage(BattleManager battleController, int damage, string statusText, ElementSO? element, string? additionalText = null) {
        if (this.BattleEntity!.IsDead()) {
            return;
        }

        PoiseState initialPoiseState =  this.BattleEntity!.GetPoiseState() != null ? this.BattleEntity!.GetPoiseState().PoiseState : PoiseState.None;

        if (element != null)
        {
            switch (this.BattleEntity!.GetElements()[element])
            {
                case ElementalResistance.Weak:
                    if (!this.BattleEntity!.IsDefending() && this.BattleEntity!.GetPoiseState() != null && this.BattleEntity!.GetPoiseState().PoiseState == PoiseState.None)
                    {
                        this.ProgressPoiseBrokenState();
                    }

                    damage *= 2;
                    statusText = DamageStatusTexts.WEAK + "";
                    break;

                case ElementalResistance.Resist:
                    damage /= 2;
                    statusText = DamageStatusTexts.RESIST + "";
                    break;
            }  
        }


        damage = damage <= 0 ? 1 : damage;
        this.BattleEntity!.TakeDamage(damage);
        if (this.battleObject != null) {
            this.battleObject.TakeDamage("-" + (statusText.Length > 0 ? " " + statusText : "") + (additionalText != null ? " " + additionalText : "") + " " + damage);
        }
        this.onBattleStatsChange?.Invoke(this);

        if (this.BattleEntity.IsDead())
        {
            return;
        }
        
        this.TakeDamageBuffEffects(element);
        this.processPoiseBreak(initialPoiseState);
        return;
    }
    

    public void ProgressPoiseBrokenState()
    {
        this.BattleEntity!.GetPoiseState().ProgressPoiseBrokenState();
        this.SetDownedStatus(this.BattleEntity!.GetPoiseState().poisePoints);
        if (this.BattleEntity!.GetPoiseState().PoiseState == PoiseState.None)
        {
            this.battleObject!.SetAnimationState(AnimationStates.Default);
        }
    }

    private void processPoiseBreak(PoiseState initialPoiseState) {
        if (this.BattleEntity!.GetPoiseState() == null || !this.BattleEntity!.IsDowned()) {
            return ;
        }

        this.processCommandLoss();
        if (this.BattleEntity!.GetPoiseState().PoiseState == PoiseState.Broken && initialPoiseState == PoiseState.None) {
            this.battleObject!.SetAnimationState(AnimationStates.PoiseBreak);

        }


        return;
    }

    private void processCommandLoss() {
        Debug.Log("Should clear battle member battle command");
        if (this.battleMember.BattleCommand != null) {
            Debug.Log("Clearing battle member battle command");
            this.battleMember.BattleCommand = null;
            this.BattleEntityGO?.ToggleEffect(AnimationEffect.Casting, false);

        }
    }
    


    protected void TakeDamageBuffEffects(ElementSO? ElementSO) {
        if (ElementSO != null) {
            List<StatusEffect> buffs = this.BattleEntity!.GetBuffs(); 
            if (buffs != null) {
                List<StatusEffect> newBuffs = new();
                foreach(StatusEffect buff in buffs) {
                    if (buff.statusEffect != null) {
                        bool keepBuff = buff.statusEffect.OnTakeDamage(ElementSO, this, this.currentCombatTileController);
                        if (keepBuff) {
                            newBuffs.Add(buff);
                        }
                    }
                }
                this.BattleEntity!.SetBuffs(newBuffs);

            }

        }
        return;
    }

    public void HealHP(int hpAmount, string? statusText = null) {
        
        if (this.battleMember != null) {
            this.BattleEntity!.HealHP(hpAmount);
        }

        if (!this.BattleEntity!.IsDead())
        {
            this.BattleEntityGO.SetAnimationState(AnimationStates.Default);
        }

        if (this.battleObject != null)
        {
            this.battleObject.HealHP("+ " + (statusText != null ? statusText + " " : "") + hpAmount);
        }

        this.HPChangeAbilities();
        this.onBattleStatsChange?.Invoke(this);
    }

    // public virtual void SetPoiseUI(PoiseUIController poiseUI) {
    //     return;
    // }

    public void StatusMessage(string statusText) {
        if (this.battleObject != null) {
            this.battleObject.StatusMessage(statusText);
        }
    }

    public bool IsNullBattleMember() {
        if (this.battleMember == null) {
            return true;
        }

        return false;
    }


    public List<SummonedEntityPartySlot> GetSummonedEntities() {
        return this.summonedEntities;
    }

    public void SetSummonedEntities(List<SummonedEntityPartySlot> summonableEntities) {
        foreach(SummonedEntityPartySlot summonedEntityPartySlot in this.summonedEntities) {
            summonedEntityPartySlot.SelfDestruct();
        }

        this.summonedEntities.Clear();
        foreach(SummonedEntityPartySlot summonableEntity in summonableEntities) {
            this.summonedEntities.Add(summonableEntity);
        }
    }

    protected abstract void ExecuteBattleCommandLogic(BattleManager battleController, System.Action callBack);

    public void ExecuteBattleCommand(BattleManager battleController, System.Action callBack)
    {
        System.Action finalCallBack = () =>
        {
            callBack?.Invoke();
            // if (battleController.DetermineIfBattleOver())
            // {
            //     return;
            // }
            if (this.currentCombatTileController == null)
            {
                return;
            }

            // actionPhaseResultState.ProcessNormalActionEndResult(actionPhase, battleController, combatTileController);

            this.battleMember.BattleCommand = null;
        };

        this.ExecuteBattleCommandLogic(battleController, finalCallBack);
        // battleController.currentlyActingMember.SetBattleObjectVector3(combatTileController);
        return;
    }
    


    // private class AnimationEvent : IBattleEvent {

    //     private BattleActionController battleActionController;

    //     public AnimationEvent(BattleActionController battleActionController) {
    //         this.battleActionController = battleActionController;
    //     }

    //     public void Execute() {
    //         return;
    //     }

    //     public bool IsDone() {
    //         return this.battleActionController.IsAnimationEnded();
    //     }
    // }

    

    #nullable enable

    public void ReCalculateCommandTiles()
    {
        if (this.battleMember == null) {
            return;
        }
        BattleCommand? battleCommand = this.battleMember.BattleCommand;
        if (battleCommand == null) {
            return;
        }

        battleCommand.ReCalculateCommandTiles(this);
        return;
    }

    public bool CanMoveToTile(CombatTileController combatTileController) {
        return this.BattleEntityGO.CanMoveToTile(combatTileController, this);
    }

    public void LookAt(Vector3 position) {
        if (this.BattleEntityGO == null) {
            return;
        }
        this.BattleEntityGO.transform.LookAt(new Vector3(position.x, this.BattleEntityGO.transform.position.y, position.z));
    }

    public void ToggleTargetIndicator(bool toggleTargetIndicator) {
        if(this.battleObject == null) {
            return;
        }
        this.battleObject.TargetBattleObject(toggleTargetIndicator);
        return;
    }

    // public void ToggleCastingCircle(bool toggleCastingCircle) {
    //     if(this.battleObject == null) {
    //         return;
    //     }
    //     this.battleObject.ToggleCastingCircle(toggleCastingCircle);
    //     return;
    // }

    // public void ToggleThinking ( bool toggleThinking) {
    //     if (this.battleObject == null) {
    //         return;
    //     }

    //     this.battleObject.ToggleThinking(toggleThinking);
    // }

    #region Battle Subcriptions
    public void SubscribeToOnBattleStatsChange(Action<PartySlot> listener) {
        this.onBattleStatsChange += listener;
    }

    public void UnsubscribeFromOnBattleStatsChange(Action<PartySlot> listener) {
        this.onBattleStatsChange -= listener;
    }

    public void RemoveAllOnBattleStatsChangeListeners() {
        this.onBattleStatsChange = delegate {};
    }

    public void OnBattleStatsChange() {
        this.onBattleStatsChange?.Invoke(this);
    }
    #endregion Battle Subcriptions
}

[Serializable]
public class EnmityTarget {

    public EnmityTarget(PartySlot partySlot, bool isInfinite, int enmityValue, bool isAggro, int aggroDuration) {
        this.partySlot = partySlot;
        this.isInfinite = isInfinite;
        this.enmityValue = enmityValue;
        this.name = partySlot.BattleEntity!.GetName();
        this.isAggro = isAggro;
        this.aggroDuration = aggroDuration;
    }

    public EnmityTarget(EnmityTarget aggroTarget) {
        this.partySlot = aggroTarget.partySlot;
        this.isInfinite = aggroTarget.isInfinite;
        this.enmityValue = aggroTarget.enmityValue;
        this.name = aggroTarget.partySlot.BattleEntity!.GetName();
        this.aggroDuration = aggroTarget.aggroDuration;
    }

    [NonSerialized]
    public PartySlot partySlot;

    public string name;

    public bool isInfinite;

    public int enmityValue;

    public bool isAggro;

    public int aggroDuration;
}


public delegate void CombatReaction();
}