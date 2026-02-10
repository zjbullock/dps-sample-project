using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Role_Tank_#", menuName = "ScriptableObjects/Character Role/Tank")]
public class CharacterTankRoleSO : CharacterRoleSO
{
    [SerializeField]
    private uint aggroPoints = 5;

    [SerializeField]
    private uint takingDamageFromAggroedEnemiesPoints = 5;

    [SerializeField]
    private uint successfulEvadePoints = 10;

    [SerializeField]
    private uint successBlockPoints = 10;

    public override void OnSkillCast(BattleManager battleController, PartySlot partySlot, ActiveSkillSO activeSkillSO)
    {
        if (activeSkillSO.IsAggro) {
            battleController.AddPartyPoints(this.aggroPoints);
        }
        base.OnSkillCast(battleController, partySlot, activeSkillSO);
    }

    public override void OnTakingDamage(BattleManager battleController, PartySlot attackerPartySlot, PartySlot defenderPartySlot)
    {
        if(attackerPartySlot.enmityList != null && attackerPartySlot.enmityList.Count > 0) {
            foreach (EnmityTarget enmityTarget in attackerPartySlot.enmityList) {
                if (enmityTarget.partySlot == defenderPartySlot && enmityTarget.isAggro) {
                    battleController.AddPartyPoints(this.takingDamageFromAggroedEnemiesPoints);
                }
            }
        }
        base.OnTakingDamage(battleController, attackerPartySlot, defenderPartySlot);
    }

    public override void OnEvade(BattleManager battleController, PartySlot partySlot){
        battleController.AddPartyPoints(this.successfulEvadePoints);
        base.OnEvade(battleController, partySlot);
        return;
    }

    public override void OnBlock(BattleManager battleController, PartySlot partySlot)
    {
        battleController.AddPartyPoints(this.successBlockPoints);
        base.OnBlock(battleController, partySlot);
    }
}
}