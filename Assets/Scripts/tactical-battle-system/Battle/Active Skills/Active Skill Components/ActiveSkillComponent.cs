using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat {
public abstract class ActiveSkillComponent : MonoBehaviour, IBattleActionCommand
{

    [Header("Component Refs")]
    [SerializeField]
    protected CapsuleCollider skillRange;

    [SerializeField]
    protected GenericDictionary<Vector3, CombatTileController> combatTiles = new();


    [Header("Active Skill Component Configurations")]
    [SerializeField]
    protected string skillName;


    [SerializeField]
    protected string skillDescription;

    [SerializeField]
    protected ElementSO ElementSO;


    [SerializeField]
    protected StatusAilmentSO _statusAilmentSO;

    [SerializeField]
    protected SkillDisplacementSO skillDisplacementSO;

    [SerializeField]
    protected TargetTypes targetType;

    [SerializeField]
    protected bool isLineOfSight = true;

    [SerializeField]
    protected bool isOverheadAbility = false;

    [Tooltip("The number of turns that must be waited before executing the ability")]
    [Range(0, 10)]
    [SerializeField]
    protected uint castTime = 0;

    [SerializeField]
    private Vector3 previousPosition;

    public ActionTypeEnums attackType;

    [Space]
    [Header("Events")]
    [SerializeField]
    protected List<BattleActionEventSO> _battleActionEvents;

    public virtual bool CanBeDoneOnTile(CombatTileController combatTileControllers)
    {
        return true;
    }

    public virtual bool ConditionMet(IBattleEntity battleEntity)
    {
        return true;
    }

    public virtual void ExecuteMultiTargetSkill(BattleManager battleController, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles)
    {
        return;

    }

    public virtual void ExecutePartnerTargetSkill(BattleManager battleController, PartySlot user, List<CombatTileController> combatTiles)
    {

    }

    public void ExecuteEndAction(BattleManager battleManager, PartySlot user, List<PartySlot> targets, List<CombatTileController> combatTiles)
    {
        this.DisplaceTargets(battleManager, user, targets);
    }

    protected void DisplaceTargets(BattleManager battleController, PartySlot user, List<PartySlot> affectedPartySlots)
    {
        BattleProcessingStatic.DisplaceTargets(this.skillDisplacementSO, this, battleController, user, affectedPartySlots);
    }


    public virtual void ExecuteSelfTargetSkill(BattleManager battleController, PartySlot user, List<CombatTileController> combatTiles)
    {

    }

    // public virtual void ExecuteSingleTargetSkill(BattleManager battleController, PartySlot user, PartySlot primaryAbilityTarget,  List<CombatTileController> combatTiles) {

    //     if (primaryAbilityTarget != null) {
    //         if(statusAilmentSO != null) {
    //             statusAilmentSO.InflictStatusAilment(primaryAbilityTarget, new StatusAilmentActivation(statusAilmentSO, statusAilmentActivationRate));
    //         }
    //         // Debug.Log("Primary Ability Target: " + primaryAbilityTarget.GetBattleEntity!.GetName());

    //         BattleProcessingStatic.DisplaceTargets(this.skillDisplacementSO, this, battleController, user, new List<PartySlot>(){primaryAbilityTarget});
    //     }

    //     return;
    // }

    public string GetAbilityRangeText()
    {
        return this.skillRange.radius + "";
    }

    public string GetAbilityAreaText()
    {
        throw new NotImplementedException();
    }

    public GenericDictionary<Vector3, CombatTileController> GetActionTilesByAreaOfEffect(CombatTileController combatTile, PartySlot partySlot, Action processTiles = null)
    {
        return this.combatTiles;
    }

    public GenericDictionary<Vector3, CombatTileController> GetSelectActionTilesByAbilityRange(CombatTileController combatTile, float entityHeight, Action<GenericDictionary<Vector3, CombatTileController>> processTiles, bool isFlying)
    {
        //TODO: WORK ON THIS!!!

        return this.combatTiles;
    }

    public BattleActionAnimationController GetBattleActionAnimationController()
    {
        return null;
    }

    public virtual int GetCastTime(PartySlot partySlot)
    {
        return (int)this.castTime;
    }

    public ElementSO GetElement()
    {
        return this.GetElement();
    }

    public string GetCostInfo()
    {
        return "N/A";
    }

    public string GetResolveInfo()
    {
        return "N/A";
    }



    public string GetActionName()
    {
        return this.skillName;
    }

    public TargetTypes GetTargetType()
    {
        return this.targetType;
    }

    public abstract bool IsWithinVerticalRange(float heightDistanceToTarget);

    public bool RequireLineOfSight()
    {
        return this.isLineOfSight;
    }

    public bool ShouldDistanceCheck()
    {
        return this.isOverheadAbility;
    }

    protected virtual void Update()
    {
        if (this.previousPosition != this.transform.position)
        {
            this.ClearPreviousList();
            this.SetCombatTileControllersByColliderAtPosition(this.transform.position);
            this.previousPosition = this.transform.position;
        }
        return;
    }


    private void SetCombatTileControllersByColliderAtPosition(Vector3 center)
    {

        Vector3 direction = new() { [this.skillRange.direction] = 1 };
        float offset = this.skillRange.height / 2 - this.skillRange.radius;

        Vector3 pos1 = center - direction * offset;
        Vector3 pos2 = center + direction * offset;

        Collider[] colliders = Physics.OverlapCapsule(pos1, pos2, this.skillRange.radius);

        this.combatTiles = this.GetCombatTileControllers(colliders);

    }
    private void ClearPreviousList()
    {
        if (this.combatTiles.Count == 0)
        {
            return;
        }

        foreach (KeyValuePair<Vector3, CombatTileController> keyValuePair in this.combatTiles)
        {
            keyValuePair.Value.DisableActionConfirmTile();
        }

        this.combatTiles.Clear();
    }
#nullable enable

    private GenericDictionary<Vector3, CombatTileController> GetCombatTileControllers(Collider[] colliders)
    {
        GenericDictionary<Vector3, CombatTileController> combatTileControllers = new GenericDictionary<Vector3, CombatTileController>();

        foreach (Collider collider in colliders)
        {
            CombatTileController? combatTileController = this.ProcessCombatTileCollider(collider);
            if (combatTileController == null)
            {
                continue;
            }

            combatTileControllers.Add(combatTileController.Position, combatTileController);
            combatTileController.ActivateConfirmActionTile();

        }

        return combatTileControllers;
    }

    private CombatTileController? ProcessCombatTileCollider(Collider collider)
    {
        if (!collider.CompareTag("Tile"))
        {
            return null;
        }

        if (collider.gameObject == null || !collider.gameObject.TryGetComponent(out CombatTileController newCombatTileController))
        {
            return null;
        }

        return newCombatTileController;
    }

    public void PayCost(BattleManager battleController, PartySlot user)
    {
        return;
    }

    public string GetDescription()
    {
        return this.skillDescription;
    }


    public List<BattleActionEventSO> GetActionInstructions()
    {
        return this._battleActionEvents.AsReadOnly().ToList();
    }

    public ActionTypeEnums GetActionType()
    {
        return this.attackType;
    }

    public BattleEventProcessor GetBattleEventProcessor(BattleManager battleController, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles)
    {
        return new BattleEventProcessor(battleController, this, user, primaryAbilityTargets, combatTiles);
    }

#nullable enable
    public StatusAilmentSO? GetStatusAilment()
    {
        return this._statusAilmentSO;
    }
    
    public List<BattleActionEventSO> GetBattleActionEvents()
    {
        return this._battleActionEvents.AsReadOnly().ToList();
    }

    public bool CanBeDoneToTargetedTile(CombatTileController targetedTile, PartySlot caster, PartySlot? targetedEntity, Func<PartySlot, bool> isSameEntityCheck)
    {
        return true;
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

    public BattleTileTargetBehaviorSO? GetBattleTileTargetBehavior()
    {
        return null;
    }
}
}
