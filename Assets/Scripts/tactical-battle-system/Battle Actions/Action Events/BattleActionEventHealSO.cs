using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Battle_Action_Event_Heal_Static_0_and_Percent_0%", menuName = "ScriptableObjects/Battle Action Event/Heal Event")]
public class BattleActionEventHealSO : BattleActionEventSO
{
    [Range(0f, 1f)]
    [SerializeField]
    private float _healAmountPercentage = 0f;

    [Range(0, 9999)]
    [SerializeField]
    private int _healAmountStatic = 0;
    

    protected override void ExecutePartnerAction(BattleManager battleManager, IBattleActionCommand battleActionCommand, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles)
    {
        if (user.GetBattleMember() != null && user.GetBattleMember().GetCharacterSlot() != null && user.GetBattleMember().GetCharacterSlot().GetSwapCharacter() != null)
        {
            int healAmount = this.performPercentageHeal(user.BattleEntity, user.GetBattleMember().GetCharacterSlot().GetSwapCharacter());

            healAmount += this.performStaticHeal();


            BattleProcessingStatic.EntityHeal(battleManager, user, user);
            user.GetBattleMember().GetCharacterSlot().GetSwapCharacter()!.HealHP(healAmount);

        }
        return;
    }

    protected override void ExecuteAction(BattleManager battleManager, IBattleActionCommand battleActionCommand, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles)
    {
        if (primaryAbilityTargets.Count == 0)
            return;

        List<PartySlot> uniquePrimaryAbilityTargets = primaryAbilityTargets.Distinct().ToList();

        foreach (PartySlot partySlot in uniquePrimaryAbilityTargets)
        {
            if ((partySlot.GetBattleMember()! == null || partySlot.BattleEntity!.IsDead()) && partySlot.BattleEntityGO != null)
            {
                Debug.Log("Target still dead");
                partySlot.BattleEntityGO.StatusMessage("UNAFFECTED");
                continue;
            }
            int healAmount = this.performPercentageHeal(user.BattleEntity, partySlot.BattleEntity);

            healAmount += this.performStaticHeal();

            BattleProcessingStatic.EntityHeal(battleManager, user, partySlot);
            partySlot.HealHP(healAmount, null);
        }
    }

    private int performPercentageHeal(IBattleEntity user, IBattleEntity healedSlot)
    {
        if (_healAmountPercentage == 0f)
        {
            return 0;
        }

        RawStats rawStats = healedSlot.GetRawStats();

        RawStats userRawStats = user.GetRawStats();

        int percentageHeal = (int)((float) rawStats.maxHp * (_healAmountPercentage + ((float)userRawStats.healProficiency * _healAmountPercentage)));


        return percentageHeal;
    }


    private int performStaticHeal()
    {
        if (_healAmountStatic == 0)
        {
            return 0;
        }

        return _healAmountStatic;
    }

    public override bool CanBeDoneOnTile(List<CombatTileController> combatTiles)
    {
        return true;
    }
}
}