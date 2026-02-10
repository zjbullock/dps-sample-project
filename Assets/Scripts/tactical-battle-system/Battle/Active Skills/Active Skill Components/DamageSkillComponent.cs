using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DPS.TacticalCombat{
public class DamageSkillComponent : ActiveSkillComponent
{
    // [SerializeField]
    // public RawStatMultiplier rawStatMultiplier;

    [SerializeField]
    [Range(-10f, 10f)]
    private float _damageMultiplier = 1f;

    // [SerializeField]
    // private RawStats rawStatAdditional;

    // public override void ExecuteSingleTargetSkill(BattleManager battleController, PartySlot user, PartySlot primaryAbilityTarget,  List<CombatTileController> combatTiles) {
    //     if (primaryAbilityTarget == null)
    //         return;

    //     RawStats tempStats = new RawStats(user.GetBattleEntity!.GetRawStats());
    //     RawStats statDifference = tempStats.StatMultiplier(rawStatMultiplier);
    //     tempStats.AddStats(statDifference);
    //     RawStats rawStaticStats = new RawStats(rawStatAdditional);
    //     tempStats.AddStats(rawStaticStats);
    //     System.Random random = new System.Random();

    //     BattleProcessingStatic.PerformEntityToEntityDamage(battleController, user, tempStats, primaryAbilityTarget, random, this.attackType, this.GetElement());
    //     base.ExecuteSingleTargetSkill(battleController, user, primaryAbilityTarget,  combatTiles);    
    //     return;
    // }

    #nullable enable
    public override void ExecuteMultiTargetSkill(BattleManager battleController, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles)
    {
        RawStats tempStats = new(user.BattleEntity!.GetRawStats());
        // RawStats statDifference = tempStats.StatMultiplier(rawStatMultiplier);
        // tempStats.AddStats(statDifference);
        // RawStats rawStaticStats = new(rawStatAdditional);
        // tempStats.AddStats(rawStaticStats);
        System.Random random = new();

        List<PartySlot> hitEnemies = new();
        List<PartySlot> uniquePrimaryAbilityTargets = primaryAbilityTargets.Distinct().ToList();

        foreach (PartySlot primaryAbilityTarget in uniquePrimaryAbilityTargets)
        {
            DamageInfoDTO? damageInfo = BattleProcessingStatic.GetEntityToEntityDamageInfo(battleController, user, tempStats, primaryAbilityTarget, random, this.attackType, this.GetElement());
            if (damageInfo != null)
            {
                damageInfo.Damage = Mathf.RoundToInt(damageInfo.Damage * _damageMultiplier);
                BattleProcessingStatic.PerformDamageToEntity(battleController, damageInfo);
            }
            hitEnemies.Add(primaryAbilityTarget);
        }
        base.ExecuteMultiTargetSkill(battleController, user, hitEnemies, combatTiles);
        return;

    }
    #nullable disable

    public override bool IsWithinVerticalRange(float heightDistanceToTarget)
    {
        if (this.attackType == ActionTypeEnums.Physical_Melee)
        {
            return heightDistanceToTarget <= 1.5f;
        }

        return true;
    }
}
}
