using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace DPS.TacticalCombat
{
[CreateAssetMenu(fileName = "Battle_Action_Event_[Buff or Debuff]_[Name]", menuName = "ScriptableObjects/Battle Action Event/Status Effect Event")]
public class BattleActionEventStatusEffectSO : BattleActionEventSO
{
    [SerializeField]
    private StatusEffectSO _statusChange;


    public override bool CanBeDoneOnTile(List<CombatTileController> combatTileControllers)
    {
        return true;
    }

    protected override void ExecutePartnerAction(BattleManager battleManager, IBattleActionCommand battleActionCommand, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles)
    {
        if (user.GetBattleMember() != null && user.GetBattleMember().GetCharacterSlot() != null && user.GetBattleMember().GetCharacterSlot().GetSwapCharacter() != null) {
            user.GetBattleMember().GetCharacterSlot().GetSwapCharacter()!.AddStatusEffect(_statusChange);
            BattleProcessingStatic.OnEntityBuffSkill(battleManager, user);
        }   
    }

    protected override void ExecuteAction(BattleManager battleManager, IBattleActionCommand battleActionCommand, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles)
    {
        if (primaryAbilityTargets.Count == 0)
            return;

        List<PartySlot> uniquePrimaryAbilityTargets = primaryAbilityTargets.Distinct().ToList();

        foreach (PartySlot partySlot in uniquePrimaryAbilityTargets)
        {
            partySlot.BattleEntity!.AddStatusEffect(_statusChange);
            if (_statusChange.StatusEffectType == StatusEffectSO.StatusEffectTypes.Buff)
                BattleProcessingStatic.OnEntityBuffSkill(battleManager, user);
        }
    }

}
}
