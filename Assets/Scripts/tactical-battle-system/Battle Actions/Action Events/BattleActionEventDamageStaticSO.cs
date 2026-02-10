using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Battle_Action_Event_Static_Damage_1", menuName = "ScriptableObjects/Battle Action Event/Damage Static Event")]

public class BattleActionEventDamageStaticSO : BattleActionEventSO, IBattleStatusAilmentInflict, IBattleDamageEvent
{
    [SerializeField]
    [Range(1, 9999)]
    private int _damage = 1;

    public override bool CanBeDoneOnTile(List<CombatTileController> combatTileControllers)
    {
        return true;
    }

    protected override void ExecuteAction(BattleManager battleManager, IBattleActionCommand battleActionCommand, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles)
    {
        this.PerformDamage(battleManager, battleActionCommand, user, primaryAbilityTargets, combatTiles);
    }

    protected override void ExecutePartnerAction(BattleManager battleManager, IBattleActionCommand battleActionCommand, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles)
    {
        return;
    }

    public void ProcessStatusAilment(PartySlot partySlot, IBattleActionCommand battleActionCommand)
    {
        if (battleActionCommand.GetStatusAilment() == null)
        {
            return;
        }

        battleActionCommand.GetStatusAilment().InflictStatusAilment(partySlot.BattleEntity, new StatusAilmentActivation(battleActionCommand.GetStatusAilment(), battleActionCommand.GetStatusAilment().StatusAilmentActivationRate));
    }

    public void PerformDamage(BattleManager battleManager, IBattleActionCommand battleActionCommand, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles)
    {
        List<PartySlot> uniquePrimaryAbilityTargets = primaryAbilityTargets.Distinct().ToList();
        // Debug.Log("unique ability targets: " + uniquePrimaryAbilityTargets.Count);
        foreach (PartySlot primaryAbilityTarget in uniquePrimaryAbilityTargets)
        {
            BattleProcessingStatic.StaticDamageToEntity(battleManager, _damage, primaryAbilityTarget, battleActionCommand.GetActionType(), battleActionCommand.GetElement());
            this.ProcessStatusAilment(primaryAbilityTarget, battleActionCommand);
        }
    }
}
}