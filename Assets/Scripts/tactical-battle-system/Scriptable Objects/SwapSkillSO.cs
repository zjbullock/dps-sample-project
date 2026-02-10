using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Swap_Element_Effect_#", menuName = "ScriptableObjects/Swap Skill")]
public class SwapSkillSO : ScriptableObject, IBattleActionCommand
{
    [SerializeField]
    private ActiveSkillSO activeSkill;

    public ActiveSkillSO ActiveSkill { get => this.activeSkill; }

    [SerializeField]
    private int coolDown;

    public int CoolDown { get => this.coolDown; }

    public string GetCostInfo()
    {
        return "";
    }

    public bool ConditionMet(IBattleEntity battleEntity)
    {
        return battleEntity.GetSkillInfo().SwapSkill.CanActivateSwapSkill();
    }

    public bool ShouldDistanceCheck()
    {
        return this.activeSkill.ShouldDistanceCheck();
    }

    public bool RequireLineOfSight()
    {
        return this.activeSkill.RequireLineOfSight();
    }

    public int GetCastTime(PartySlot partySlot)
    {
        return 0;
    }

    public bool IsWithinVerticalRange(float heightDistanceToTarget)
    {
        return this.activeSkill.IsWithinVerticalRange(heightDistanceToTarget);
    }

    public BattleActionAnimationController GetBattleActionAnimationController()
    {
        return this.activeSkill.GetBattleActionAnimationController();
    }

    public bool CanBeDoneToTargetedTile(CombatTileController targetedTile, PartySlot caster, PartySlot targetedEntity, Func<PartySlot, bool> isSameEntityCheck)
    {
        return this.activeSkill.CanBeDoneToTargetedTile(targetedTile, caster, targetedEntity, isSameEntityCheck);
    }

    public ElementSO GetElement()
    {
        return this.activeSkill.GetElement();
    }

    public TargetTypes GetTargetType()
    {
        return this.activeSkill.GetTargetType();
    }

    public bool CanBeDoneOnTile(CombatTileController combatTileControllers)
    {
        return this.activeSkill.CanBeDoneOnTile(combatTileControllers);
    }

    public void ExecuteMultiTargetSkill(BattleManager battleController, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles)
    {
        this.activeSkill.ExecuteMultiTargetSkill(battleController, user, primaryAbilityTargets, combatTiles);
        return;
    }

    public void ExecutePartnerTargetSkill(BattleManager battleController, PartySlot user, List<CombatTileController> combatTiles)
    {
        this.activeSkill.ExecutePartnerTargetSkill(battleController, user, combatTiles);
    }

    public void ExecuteEndAction(BattleManager battleManager, PartySlot user, List<PartySlot> targets, List<CombatTileController> combatTiles)
    {
        this.activeSkill.ExecuteEndAction(battleManager, user, targets, combatTiles);
    }

    public void PayCost(BattleManager battleController, PartySlot user)
    {
        this.activeSkill.PayCost(battleController, user);
    }

    public GenericDictionary<Vector3, CombatTileController> GetActionTilesByAreaOfEffect(CombatTileController combatTile, PartySlot partySlot, Action processTiles = null)
    {
        return this.activeSkill.GetActionTilesByAreaOfEffect(combatTile, partySlot, processTiles);
    }

    public GenericDictionary<Vector3, CombatTileController> GetSelectActionTilesByAbilityRange(CombatTileController combatTile, float entityHeight, Action<GenericDictionary<Vector3, CombatTileController>> processTiles, bool isFlying)
    {
        return this.activeSkill.GetSelectActionTilesByAbilityRange(combatTile, entityHeight, processTiles, isFlying);
    }

    public string GetAbilityRangeText()
    {
        return this.activeSkill.GetAbilityRangeText();
    }

    public string GetAbilityAreaText()
    {
        return this.activeSkill.GetAbilityAreaText();
    }

    public ActionTypeEnums GetActionType()
    {
        return this.activeSkill.GetActionType();
    }

    public List<BattleActionEventSO> GetActionInstructions()
    {
        return this.activeSkill.GetActionInstructions();
    }

    public BattleEventProcessor GetBattleEventProcessor(BattleManager battleController, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles)
    {
        return this.activeSkill.GetBattleEventProcessor(battleController, user, primaryAbilityTargets, combatTiles);
    }

    public List<BattleActionEventSO> GetBattleActionEvents()
    {
        return this.activeSkill.GetBattleActionEvents();
    }

    public StatusAilmentSO GetStatusAilment()
    {
        return this.activeSkill.GetStatusAilment();
    }

    public BattleCommand GenerateBattleCommand(PartySlot caster, Func<PartySlot, bool> isSameTypeEntity, CombatTileController targetedTile, List<CombatTileController> tiles, List<PartySlot> targetedSlots)
    {
        return new PartySwapCommand(caster, targetedTile,tiles, targetedSlots);
    }

    public string GetActionName()
    {
        return this.activeSkill.GetActionName();
    }

    public string GetDescription()
    {
        return this.activeSkill.GetDescription();
    }

    public BattleTileTargetBehaviorSO GetBattleTileTargetBehavior()
    {
        return this.activeSkill.GetBattleTileTargetBehavior();
    }
}
}