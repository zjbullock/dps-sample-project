using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using DPS.Common;

namespace DPS.TacticalCombat{
[CreateAssetMenu(fileName = "Active Skill", menuName = "ScriptableObjects/Active Skill/Base Skill")]
[Serializable]
public class ActiveSkillSO : ScriptableObject, IBattleActionCommand
{

    [SerializeField]
    [Tooltip("Determines how the skill will target tiles on the Battle grid")]
    protected BattleTileTargetBehaviorSO battleTargetBehavior;

    public BattleTileTargetBehaviorSO BattleTargetBehavior { get => this.battleTargetBehavior; }

    [Header("Battle Action Refs")]
    [SerializeField]
    private BattleActionAnimationController battleAnimationController;

    public BattleActionAnimationController GetBattleActionAnimationController()
    {
        return this.battleAnimationController;
    }

    [Space]
    [Header("Active Skill Info")]

    [SerializeField]
    private string skillName;

    public string GetActionName()
    {
        return this.skillName;
    }
    [TextArea(15, 20)]

    [SerializeField]
    private string description;

    [Space]
    [Header("Element Info")]
    [SerializeField]
    private ElementSO element;

    public ElementSO GetElement()
    {
        return this.element;
    }

    [Space]
    [Header("Cost Info")]
    [SerializeField]
    private CostInfoSO _costInfo;

    [Space]
    [Header("Events")]
    [SerializeField]
    private List<BattleActionEventSO> _battleActionEvents;

    [Space]
    [Header("Targeting Info")]
    [SerializeField]
    private TargetTypes targetTypes;

    [SerializeField]
    private ActionTypeEnums _attackType;

    public ActionTypeEnums AttackType { get => this._attackType; }

    public TargetTypes GetTargetType()
    {
        return this.targetTypes;
    }

    [Space]
    [Header("Aggro Info")]
    [SerializeField]
    private bool isAggro;
    public bool IsAggro { get => this.isAggro; }

    [Range(0, 5)]
    [Tooltip("The number of turns the aggro ability will be active for")]
    public int aggroDuration;

    [Range(-100, 100)]
    [Tooltip("Determines how much enmity is generated from using a particular skill.  Useful for Enemy AI when determining which character to attack.")]
    public int enmityValue;





    [Space]
    [Header("Ability Area of Effect and Range")]
    [Range(0, 20)]
    [Tooltip("The maximum distance away from the user for the skill.")]
    [SerializeField]
    private int abilityRange;

    [Range(0, 20)]
    [Tooltip("The Area of Effect Range for the skill.  Determines how far the skill extends distance wise from the emission point, and for certain attack patterns, determines where squares become active for targeting.")]
    public int abilityAreaOfEffect;

    // public AbilityPattern abilityPattern;


    [Space]
    [Header("Cast Info")]
    [Tooltip("The number of turns that must be waited before executing the ability")]
    [Range(0, 10)]
    [SerializeField]
    private uint castTime = 0;


    [Space]
    [Header("Status Ailment Info")]
    [SerializeField]
#nullable enable
    protected StatusAilmentSO? _statusAilmentSO;
#nullable disable

    [Tooltip("The status ailment activation rate")]
    [Range(1, 100)]
    public int statusAilmentActivationRate;


    [Space]
    [Header("Displacement Info")]
    [Tooltip("Skill displacement, if any")]
    [SerializeField]
    private SkillDisplacementSO skillDisplacementSO;

#nullable enable
    public void ExecuteEndAction(BattleManager battleManager, PartySlot user, List<PartySlot> targets, List<CombatTileController> combatTiles)
    {
        this.DisplaceTargets(battleManager, user, targets);
        this.AddEnmityToTarget(user, targets);
    }

    public List<BattleActionEventSO> GetActionInstructions()
    {
        return this._battleActionEvents.AsReadOnly().ToList();
    }

    public BattleEventProcessor GetBattleEventProcessor(BattleManager battleController, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles)
    {
        return new BattleEventProcessor(battleController, this, user, primaryAbilityTargets, combatTiles);
    }

    public virtual void ExecutePartnerTargetSkill(BattleManager battleController, PartySlot user, List<CombatTileController> combatTiles)
    {
        if (user.GetBattleMember()!.GetCharacterSlot() != null && user.GetBattleMember()!.GetCharacterSlot()!.GetSwapCharacter() != null)
        {
            IBattleEntity swapPartner = user.GetBattleMember()!.GetCharacterSlot()!.GetSwapCharacter()!;

            if (_statusAilmentSO != null)
            {
                _statusAilmentSO.InflictStatusAilment(swapPartner, new StatusAilmentActivation(_statusAilmentSO, statusAilmentActivationRate));
            }
        }
    }

    // public virtual void ExecuteSingleTargetSkill(BattleManager battleController, PartySlot user, PartySlot? primaryAbilityTarget,  List<CombatTileController> combatTiles) {
    //     if (primaryAbilityTarget != null) {
    //         if(primaryAbilityTarget.GetBattleMember()!.battleMemberType != user.GetBattleMember()!.battleMemberType) {
    //             primaryAbilityTarget.AddEnmityTarget(user, false, isAggro, enmityValue, aggroDuration);
    //         }
    //         if(isDefensive) {
    //             user.BattleEntityGO!.SetAnimationState(AnimationStates.Defend);
    //             user.GetBattleEntity!.SetActionStatus(ActionStatus.DefendingAction);
    //         }

    //         if(statusAilmentSO != null) {
    //             statusAilmentSO.InflictStatusAilment(primaryAbilityTarget, new StatusAilmentActivation(statusAilmentSO, statusAilmentActivationRate));
    //         }
    //         // Debug.Log("Primary Ability Target: " + primaryAbilityTarget.GetBattleEntity!.GetName());
    //         this.DisplaceTargets(battleController, user, new List<PartySlot>(){primaryAbilityTarget});
    //     }

    //     PlayerPartySlot? playerPartySlot = user as PlayerPartySlot;
    //     if (playerPartySlot != null) {
    //         this.manageResolvePoints(playerPartySlot);
    //     }
    //     BattleProcessingStatic.EntityCastSkill(battleController, user, this);
    //     return;
    // }

    public virtual void ExecuteMultiTargetSkill(BattleManager battleController, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles)
    {
        // this.ExecuteDisplacement(battleController, user, primaryAbilityTargets);
        BattleProcessingStatic.OnEntityUseAbility(battleController, user, this);
        return;
    }


    private void AddEnmityToTarget(PartySlot user, List<PartySlot> partySlots)
    {
        foreach (PartySlot partySlot in partySlots)
        {
            partySlot.AddEnmityTarget(user, false, IsAggro, enmityValue, aggroDuration);
        }
    }

    protected void DisplaceTargets(BattleManager battleController, PartySlot user, List<PartySlot> affectedPartySlots)
    {

        // if (!this.battleTargetBehavior.RequireLineOfSight())
        // {
        //     Debug.Log("Ability does not require Line of Sight, and cannot displace");
        //     return;
        // }
        Debug.Log("Executing Displacement");
        BattleProcessingStatic.DisplaceTargets(this.skillDisplacementSO, this, battleController, user, affectedPartySlots);
    }

    public virtual bool ConditionMet(IBattleEntity battleEntity)
    {
        return this._costInfo.CanPayCost(battleEntity);
    }

    #nullable disable

    public virtual string GetCostInfo()
    {
        return this._costInfo.GetCostText();
    }

    // public void OnSubmitPressed(BattleMenuController battleMenuController)
    // {
    //     if (battleTargetBehavior == null)
    //     {
    //         return;
    //     }

    //     battleTargetBehavior.OnSubmitPressed(battleMenuController, this);
    // }


    public void NavigateActionTilesAbilityForEnemy()
    {
        if (battleTargetBehavior != null)
        {
            battleTargetBehavior.NavigateActiveSkillTargetingBehaviorForEnemy();
        }
    }

    public virtual GenericDictionary<Vector3, CombatTileController> GetActionTilesByAreaOfEffect(CombatTileController combatTile, PartySlot partySlot, Action processTiles = null)
    {
        if (this.battleTargetBehavior != null)
        {
            return this.battleTargetBehavior.GetActionTilesByAreaOfEffect(combatTile, this.abilityRange + partySlot.Width, this.GetVerticalRange(), partySlot.BattleEntityGO.height, partySlot.BattleEntity.CanFly());
        }
        return new GenericDictionary<Vector3, CombatTileController>();
    }

#nullable enable
    public virtual GenericDictionary<Vector3, CombatTileController> GetSelectActionTilesByAbilityRange(CombatTileController combatTile, float entityHeight, Action<GenericDictionary<Vector3, CombatTileController>>? processTiles, bool isFlying)
    {
        if (this.battleTargetBehavior != null)
        {
            GenericDictionary<Vector3, CombatTileController> actionableTiles = this.battleTargetBehavior.GetSelectActionTilesByAbilityRange(combatTile, this.abilityAreaOfEffect, this.GetVerticalRange(), entityHeight, isFlying);
            if (processTiles != null)
            {
                processTiles(actionableTiles);
            }
            return actionableTiles;
        }
        return new GenericDictionary<Vector3, CombatTileController>();
    }

    public string GetAbilityRangeText()
    {
        return this.battleTargetBehavior.GetAbilityRangeText(this.abilityRange);
    }

    public string GetAbilityAreaText()
    {
        return this.battleTargetBehavior.GetAbilityAreaOfEffectText(this.abilityAreaOfEffect);
    }

#nullable disable

    public virtual bool ShouldDistanceCheck()
    {
        if (this.battleTargetBehavior != null)
        {
            return this.battleTargetBehavior.ShouldDistanceCheck();
        }
        return false;
    }

    public virtual bool CanBeDoneOnTile(CombatTileController combatTileControllers)
    {
        return true;
    }

    public bool RequireLineOfSight()
    {
        if (this.battleTargetBehavior == null)
        {
            Debug.LogError("No Battle Target Behavior Attached to " + this.name);
            return true;
        }
        return this.battleTargetBehavior.RequireLineOfSight();
    }

    public int GetCastTime(PartySlot partySlot)
    {
        int castTimeReduction = partySlot.BattleEntity!.GetRawStats().castTimeReduction;
        return (int)((this.castTime - castTimeReduction) + (int)Math.Abs(this.castTime - castTimeReduction)) / 2;
    }

    protected virtual float GetVerticalRange()
    {
        return 1f;
    }

    public virtual bool IsWithinVerticalRange(float heightDistanceToTarget)
    {
        if (this.AttackType == ActionTypeEnums.Physical_Melee)
        {
            return heightDistanceToTarget <= 1.5f;
        }

        return true;
    }

    public void PayCost(BattleManager battleController, PartySlot user)
    {
        this._costInfo.PayCost(user.BattleEntity);
    }

    public string GetDescription()
    {
        return this.description;
    }

    public ActionTypeEnums GetActionType()
    {
        return this._attackType;
    }



    public StatusAilmentSO GetStatusAilment()
    {
        return this._statusAilmentSO;
    }

    public List<BattleActionEventSO> GetBattleActionEvents()
    {
        return this._battleActionEvents.AsReadOnly().ToList();
    }

    public BattleCommand GenerateBattleCommand(PartySlot caster, Func<PartySlot, bool> isSameTypeEntity, CombatTileController targetedTile, List<CombatTileController> tiles, List<PartySlot> targetedSlots)
    {
        return new SkillCommand(
            this,
            caster,
            isSameTypeEntity,
            targetedTile,
            tiles,
            targetedSlots
        );
    }
    #nullable enable

    public bool CanBeDoneToTargetedTile(CombatTileController targetedTile, PartySlot caster, PartySlot? targetedEntity, Func<PartySlot, bool> isSameEntityCheck)
    {
        return this.BattleTargetBehavior.CanBeDoneToTargetedTile(targetedTile, caster, targetedEntity, isSameEntityCheck);
    }

    public BattleTileTargetBehaviorSO GetBattleTileTargetBehavior()
    {
        return this.battleTargetBehavior;
    }
}

public enum ResolvePointUsage {
    None,
    Increase,
    Decrease
}
}
