using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DPS.TacticalCombat {
public class SummonedEntityPartySlot : PlayerPartySlot
{
    public SummonedEntityPartySlot()
    {
    }

    public SummonedEntityPartySlot(BattleMember battleMember) : base(battleMember)
    {
    }

    #nullable enable


    // public override void ProcessDamage(BattleManager battleController, int damage, ElementSO ElementSO, string? additionalText = null) {
    //     base.ProcessDamage(battleController, damage,  ElementSO, additionalText: additionalText);
    //     Debug.Log("Attempting removal of battle entity");
    //     this.CleanupSummonedEntity(battleController);
        
    // }


    // public override void EndPhase(BattleManager battleController)
    // {
    //     base.EndPhase(battleController);
    // }

    private void CleanupSummonedEntity(BattleManager battleController) {
        Debug.Log("Attempting removal of battle entity");
        if (this.GetCombatTileController() != null) {
            Debug.Log("Attempting removal of battle entity");
            this.GetCombatTileController().RemoveOccupantAndProcessEvent(battleController);
        }
        
        this.battleObject.DestroyBattleObject();
    }

    
    public void SelfDestruct() {
        BattleManager? battleManager = BattleManager.instance;
        if (battleManager == null)
        {
            Debug.LogError("Battle Manager is not active");
            return;
        }
        battleManager.PlayerPartyController.PartySlots.Remove(this);
        this.CleanupSummonedEntity(battleManager);

    }
}
}