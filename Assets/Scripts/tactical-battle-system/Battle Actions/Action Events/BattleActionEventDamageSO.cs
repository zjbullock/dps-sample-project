using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Battle_Action_Event_Damage_[Type]_1.00", menuName = "ScriptableObjects/Battle Action Event/Damage Event")]
public class BattleActionEventDamageSO : BattleActionEventSO, IBattleStatusAilmentInflict, IBattleDamageEvent {

    // [SerializeField]
    // private RawStatMultiplier _statMultiplier;

    [SerializeField]
    [Range(-10f, 10f)]
    private float _damageMultiplier = 1f;

    // [SerializeField]
    // private RawStats _additionalStats;

    protected override void ExecutePartnerAction(BattleManager battleManager, IBattleActionCommand battleActionCommand, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles)
    {
        Debug.LogWarning("Damage partners not allowed");
        return;
    }
    
    protected override void ExecuteAction(BattleManager battleManager, IBattleActionCommand battleActionCommand, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles)
    {
        this.PerformDamage(battleManager, battleActionCommand, user, primaryAbilityTargets, combatTiles);
        return;
    }

    #nullable enable
    public void PerformDamage(BattleManager battleManager, IBattleActionCommand battleActionCommand, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles)
    {
        RawStats tempStats = new(user.BattleEntity!.GetRawStats());
        // RawStats statDifference = tempStats.StatMultiplier(_statMultiplier);
        // tempStats.AddStats(statDifference);

        // RawStats rawStaticStats = new(_additionalStats);
        // tempStats.AddStats(rawStaticStats);

        System.Random random = new();
        List<PartySlot> uniquePrimaryAbilityTargets = primaryAbilityTargets.Distinct().ToList();

        // Debug.Log("unique ability targets: " + uniquePrimaryAbilityTargets.Count);
        foreach (PartySlot primaryAbilityTarget in uniquePrimaryAbilityTargets)
        {
            DamageInfoDTO? damageInfo = BattleProcessingStatic.GetEntityToEntityDamageInfo(battleManager, user, tempStats, primaryAbilityTarget, random, battleActionCommand.GetActionType(), battleActionCommand.GetElement());
            if (damageInfo == null)
            {
                continue;
            }
            damageInfo.Damage = Mathf.RoundToInt(damageInfo.Damage * _damageMultiplier);
            BattleProcessingStatic.PerformDamageToEntity(battleManager, damageInfo);
            this.ProcessStatusAilment(primaryAbilityTarget, battleActionCommand);
        }
    }
    #nullable disable

    public void ProcessStatusAilment(PartySlot partySlot, IBattleActionCommand battleActionCommand)
    {
        if (battleActionCommand.GetStatusAilment() == null)
        {
            return;
        }

        battleActionCommand.GetStatusAilment().InflictStatusAilment(partySlot.BattleEntity, new StatusAilmentActivation(battleActionCommand.GetStatusAilment(), battleActionCommand.GetStatusAilment().StatusAilmentActivationRate));
    }

    public override bool CanBeDoneOnTile(List<CombatTileController> combatTiles)
    {
        return true;
    }

}
}

