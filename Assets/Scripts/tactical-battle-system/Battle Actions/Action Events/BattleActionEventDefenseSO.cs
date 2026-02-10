using System.Collections.Generic;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat{
[CreateAssetMenu(fileName = "Battle_Action_Event_Defend", menuName = "ScriptableObjects/Battle Action Event/Defend Event")]
public class BattleActionEventDefenseSO : BattleActionEventSO
{
    public override bool CanBeDoneOnTile(List<CombatTileController> combatTileControllers)
    {
        return true;
    }
    protected override void ExecutePartnerAction(BattleManager battleManager, IBattleActionCommand battleActionCommand, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles)
    {
        Debug.LogWarning("Defending not allowed on partner action");
        return;
    }
    
    protected override void ExecuteAction(BattleManager battleManager, IBattleActionCommand battleActionCommand, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles)
    {
        user.BattleEntityGO!.SetAnimationState(AnimationStates.Defend);
        user.BattleEntity!.SetDefending(true);
    }
}
}
