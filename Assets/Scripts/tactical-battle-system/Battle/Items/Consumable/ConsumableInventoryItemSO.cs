using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat {
public class ConsumableInventoryItemSO : InventoryItemSO, IBattleActionCommand
{

    [SerializeField]
    [Tooltip("Determines how the skill will target tiles on the Battle grid")]
    protected BattleTileTargetBehaviorSO battleTargetBehavior;

    public BattleTileTargetBehaviorSO BattleTargetBehavior { get => this.battleTargetBehavior; }

    [Range(0f, 20f)]
    [Tooltip("The maximum distance away from the user for the item.  Determines how far the ability can be moved away from the user.")]
    public int itemArea;

    [Tooltip("The Area of Effect Range for the skill.  Determines how far the skill extends distance wise from the emission point, and for certain attack patterns, determines where squares become active for targeting.")]
    [Range(0f, 20f)]
    public int itemRange;

    [Range(0, 100)]
    [Tooltip("The Vertical Targeting Range for the skill.  Determines how far the skill extends vertically, and for certain attack patterns, determines where squares become active for targeting.  This value is added to the height of the character for calculations.")]
    public int heightRange = 2;

    [SerializeField]
    private ActionTypeEnums _actionType = ActionTypeEnums.Magical;

    #nullable enable
    [SerializeField]
    private StatusAilmentSO? _statusAilmentSO;
    #nullable disable

    [SerializeField]
    private BattleActionAnimationController battleAnimationController;

    public BattleActionAnimationController GetBattleActionAnimationController()
    {
        return this.battleAnimationController;
    }

    [Space]
    [Header("Events")]
    [SerializeField]
    private List<BattleActionEventSO> _battleActionEvents;

    public List<BattleActionEventSO> GetActionInstructions()
    {
        return this._battleActionEvents.AsReadOnly().ToList();
    }

    public string GetCostInfo()
    {
        return "";
    }

    public string GetResolveInfo()
    {
        return "";
    }

    public bool ConditionMet(IBattleEntity battleEntity)
    {
        return true;
    }

    public bool ShouldDistanceCheck()
    {
        if (this.battleTargetBehavior != null)
        {
            return this.battleTargetBehavior.ShouldDistanceCheck();
        }
        return false;
    }

    public int GetCastTime(PartySlot partySlot)
    {
        return 0;
    }

    public bool IsWithinVerticalRange(float heightDistanceToTarget)
    {
        return true;
    }

    public ElementSO GetElement()
    {
        return this.element;
    }

    public TargetTypes GetTargetType()
    {
        return this.targetTypes;
    }

    public bool CanBeDoneOnTile(CombatTileController combatTileControllers)
    {
        return true;
    }

    public virtual void ExecuteMultiTargetSkill(BattleManager battleController, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles)
    {
        return;
    }

    public void ExecutePartnerTargetSkill(BattleManager battleController, PartySlot user, List<CombatTileController> combatTiles)
    {
        return;
    }

    // public virtual void ExecuteSingleTargetSkill(BattleManager battleController, PartySlot user, PartySlot primaryAbilityTarget, List<CombatTileController> combatTiles)
    // {
    //     return;
    // }

    public void PayCost(BattleManager battleController, PartySlot user)
    {
        return;
    }

    public GenericDictionary<Vector3, CombatTileController> GetActionTilesByAreaOfEffect(CombatTileController combatTile, PartySlot partySlot, Action processTiles = null)
    {
        if (this.battleTargetBehavior != null)
        {
            return this.battleTargetBehavior.GetActionTilesByAreaOfEffect(combatTile, this.itemRange + partySlot.Width, 99, partySlot.BattleEntityGO.height, partySlot.BattleEntity.CanFly());
        }
        return new GenericDictionary<Vector3, CombatTileController>();
    }

    public GenericDictionary<Vector3, CombatTileController> GetSelectActionTilesByAbilityRange(CombatTileController combatTile, float entityHeight,Action<GenericDictionary<Vector3, CombatTileController>> processTiles, bool isFlying)
    {
        if (this.battleTargetBehavior != null)
        {
            GenericDictionary<Vector3, CombatTileController> actionableTiles = this.battleTargetBehavior.GetSelectActionTilesByAbilityRange(combatTile, this.itemArea, 99, entityHeight, isFlying);
            if (processTiles != null)
            {
                processTiles(actionableTiles);
            }
            return actionableTiles;
        }
        return new GenericDictionary<Vector3, CombatTileController>();
    }

    public string GetActionName()
    {
        return this.itemName;
    }

    public string GetDescription()
    {
        return this.description;
    }


    public bool RequireLineOfSight()
    {
        return this.battleTargetBehavior.RequireLineOfSight();
    }


    // protected void HandleOnSubmitPressed(BattleMenuController battleMenuController)
    // {
    //     battleTargetBehavior.OnSubmitPressed(battleMenuController, this);
    // }



    // public void OnSubmitPressed(BattleMenuController battleMenuController)
    // {
    //     if (battleTargetBehavior == null)
    //     {
    //         return;
    //     }

    //     battleTargetBehavior.OnSubmitPressed(battleMenuController, this);

    // }

    public string GetAbilityRangeText()
    {
        return this.battleTargetBehavior.GetAbilityRangeText(this.itemRange);
    }

#nullable enable
    public string GetAbilityAreaText()
    {
        return this.battleTargetBehavior.GetAbilityAreaOfEffectText(this.itemArea);
    }

    public ActionTypeEnums GetActionType()
    {
        return this._actionType;
    }

    public void ExecuteEndAction(BattleManager battleManager, PartySlot user, List<PartySlot> targets, List<CombatTileController> combatTiles)
    {
        return;
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

    public BattleCommand GenerateBattleCommand(PartySlot caster, Func<PartySlot, bool> isSameTypeEntity, CombatTileController targetedTile, List<CombatTileController> tiles, List<PartySlot> targetedSlots)
    {
        return new ItemCommand(
            this,
            caster,
            isSameTypeEntity,
            targetedTile,
            tiles,
            targetedSlots
        );
    }

    public bool CanBeDoneToTargetedTile(CombatTileController targetedTile, PartySlot caster, PartySlot? targetedEntity, Func<PartySlot, bool> isSameEntityCheck)
    {
        return this.BattleTargetBehavior.CanBeDoneToTargetedTile(targetedTile, caster, targetedEntity, isSameEntityCheck);
    }

    public BattleTileTargetBehaviorSO GetBattleTileTargetBehavior()
    {
        return this.battleTargetBehavior;
    }
}
}