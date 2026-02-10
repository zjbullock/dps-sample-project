using System.Collections.Generic;
using UnityEngine;


namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Battle_Action_Dummy", menuName = "ScriptableObjects/Battle Action Event/Dummy Event")]
public class BattleActionEventDefaultSO : BattleActionEventSO
{
    public override bool CanBeDoneOnTile(List<CombatTileController> combatTileControllers)
    {
        return true;
    }

    protected override void ExecuteAction(BattleManager battleManager, IBattleActionCommand battleActionCommand, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles)
    {
        return;
    }

    protected override void ExecutePartnerAction(BattleManager battleManager, IBattleActionCommand battleActionCommand, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles)
    {
        return;
    }
}
}
