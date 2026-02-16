using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Buff Passive Skill Behavior", menuName = "ScriptableObjects/Passive Skill/Behaviors/Buff Passive")]
public class StatusEffectPassiveSkillBehaviorSO : PassiveSkillBehaviorSO
{
    [Header("The activation condition for a buff will be checked by any of the Hooks in this file. ")]
    [Header("If the activation condition is met, it will add the buff to the character, assuming one isn't already there.")]
    [SerializeField]
    [Tooltip("A list of buffs and their Activation Conditions.")]
    private List<StatusEffectAndCondition> passiveBuffHpChange = new List<StatusEffectAndCondition>();

    [SerializeField]
    [Tooltip("A list of buffs and their Activation Conditions for begin phase.")]
    private List<StatusEffectAndCondition> passiveBuffBeginPhaseChange = new List<StatusEffectAndCondition>();

    [SerializeField]
    [Tooltip("A list of buffs and their Activation Conditions for end phase.")]
    private List<StatusEffectAndCondition> passiveBuffEndPhaseChange = new List<StatusEffectAndCondition>();

    [SerializeField]
    [Tooltip("A list of buffs and their Activation Conditions for end phase.")]
    private List<StatusEffectAndCondition> passiveBuffPostCommandChange = new List<StatusEffectAndCondition>();

    [SerializeField]
    [Tooltip("A list of buffs and their Activation Conditions after moving")]
    private List<StatusEffectAndCondition> passiveBuffPostMove = new List<StatusEffectAndCondition>();

    [SerializeField]
    [Tooltip("A list of buffs and their Activation Conditions for beginnning of combat.")]
    private List<StatusEffectAndCondition> passiveBuffBeginCombatChange = new List<StatusEffectAndCondition>();


    [SerializeField]
    [Tooltip("A list of buffs and their Activation Conditions after a block")]
    private List<StatusEffectAndCondition> passiveBuffPostBlock = new List<StatusEffectAndCondition>();

    [SerializeField]
    [Tooltip("A list of buffs and their Activation Conditions after an evade")]
    private List<StatusEffectAndCondition> passiveBuffPostEvade = new List<StatusEffectAndCondition>();

    [SerializeField]
    [Tooltip("Controls the behavior for this Buff Passive Skill to determine if the activated buff should be unique.  If it should be unique, all instances of a Buff applied that match an existing one in the Passive Buff list will be removed.\n\nThe list of buffs will be sorted from highest to lowest priority, first to last.")]
    private bool canOnlyApplyOneBuff = true;

    #nullable enable
    public override async Task ExecuteHPChangePassiveSkill(PartySlot partySlot, CombatTileController? combatTileController) {
        await this.ProcessStatusEffect(partySlot, combatTileController, this.passiveBuffHpChange);
    }

    public override async Task ExecuteEndPhasePassiveSkill(PartySlot partySlot, CombatTileController? combatTileController) {
        await this.ProcessStatusEffect(partySlot, combatTileController, this.passiveBuffEndPhaseChange);
    }

    public override async Task ExecutePostCommandAbilities(PartySlot partySlot, CombatTileController? combatTileController){
        await this.ProcessStatusEffect(partySlot, combatTileController, this.passiveBuffPostCommandChange);
    }

    public override async Task ExecuteBeginPhasePassiveSkill(PartySlot partySlot, CombatTileController? combatTileController){
        await this.ProcessStatusEffect(partySlot, combatTileController, this.passiveBuffBeginPhaseChange);
    }

    public override async Task ExecutePostMovePassiveSkill(PartySlot partySlot, CombatTileController? combatTileController) {
        await this.ProcessStatusEffect(partySlot, combatTileController, this.passiveBuffPostMove);
    }

    public override async Task ExecuteBeginCombatPassiveSkill(PartySlot partySlot, CombatTileController? combatTileController)
    {
        await this.ProcessStatusEffect(partySlot, combatTileController, this.passiveBuffBeginCombatChange);
    }

    public override async Task ExecutePostBlockPassiveSkill(PartySlot partySlot, CombatTileController? combatTileController)
    {
        await this.ProcessStatusEffect(partySlot, combatTileController, this.passiveBuffPostBlock);
    }

    public override async Task ExecutePostEvadePassiveSkill(PartySlot partySlot, CombatTileController? combatTileController)
    {
        await this.ProcessStatusEffect(partySlot, combatTileController, this.passiveBuffPostEvade);
    }


    private async Task ProcessStatusEffect(PartySlot partySlot, CombatTileController? combatTileController, List<StatusEffectAndCondition> passiveBuffs)
    {
        if (partySlot == null || partySlot.BattleEntity == null)
        {
            return;
        }
        
        IBattleEntity? battleEntity = partySlot.BattleEntity;
        if (battleEntity == null)
        {
            return;
        }
        
        if (this.canOnlyApplyOneBuff) {
            passiveBuffs.ForEach(buffAndCondition => {
                if (buffAndCondition != null) {
                    this.removeStatusEffect(battleEntity, buffAndCondition.buff);
                }
            });
        }
        
        foreach (StatusEffectAndCondition buffAndCondition in passiveBuffs)
        {
            bool canBeActivated = true;
            foreach (PassiveSkillBehaviorActivationConditionSO activationCondition in buffAndCondition.activationConditions)
            {
                if (activationCondition != null && !activationCondition.ActivationConditionBattleEntity(battleEntity, combatTileController))
                {
                    canBeActivated = false;
                    break;
                }
                else
                {
                    this.removeStatusEffect(battleEntity, buffAndCondition.buff);
                }
            }

            if (canBeActivated)
            {
                this.addStatusEffect(battleEntity, buffAndCondition.buff);
                if (this.canOnlyApplyOneBuff)
                {
                    break;
                }
            }
        }
    }


    private void addStatusEffect(IBattleEntity battleEntity, StatusEffectSO statusEffect)
    {
        if (statusEffect == null)
        {
            return;
        }
        
        List<StatusEffect> buffs = battleEntity.GetStatusEffects();

        if (buffs != null && buffs!.Count > 0 && buffs.Find((buff) => { return buff.statusEffect == statusEffect; }) != null) {
            Debug.LogError("Failed to add status effect!");
            return;
        }
        battleEntity.AddStatusEffect(statusEffect);
    }

    private void removeStatusEffect(IBattleEntity battleEntity, StatusEffectSO statusEffect)
    {
        if (statusEffect == null)
        {
            return;
        }
        

        List<StatusEffect> buffs = battleEntity.GetStatusEffects();
        if (battleEntity != null && buffs != null && buffs!.Count > 0) {
            statusEffect.OnRemoveStatusEffect(battleEntity);
            battleEntity.RemoveStatusEffect(statusEffect);
        }
    }
    #nullable disable    

    [Serializable]
    private class StatusEffectAndCondition {
        public StatusEffectSO buff;
        public List<PassiveSkillBehaviorActivationConditionSO> activationConditions;
    }
}
}