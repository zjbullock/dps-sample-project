using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DPS.TacticalCombat {
[Serializable]
#nullable enable
public class InflictedAilment {

    public InflictedAilment() {
        this.statusAilment = null;
        this.turnCount = 0;
    }

    public InflictedAilment(StatusAilmentSO statusAilment) {
        Debug.Log("being called for some reason");
        this.statusAilment = statusAilment;
        this.turnCount = 0;
    }

    public InflictedAilment(InflictedAilment inflictedAilment) {
        this.statusAilment = inflictedAilment.statusAilment;
        this.turnCount = inflictedAilment.turnCount;
    }

    public bool LoseTurn() {
        if(this.statusAilment != null) {
            return this.statusAilment.preventsTurn;
        }
        
        return false;
    }

    public bool LoseItemOption() {
        if(this.statusAilment != null) {
            return this.statusAilment.preventsTurn;
        }
        return false;
    }

    public bool CanDealDamage() {
        if(this.statusAilment != null) {
            return this.statusAilment.canDealDamage;
        }
        return false;
    }

    public void DealDamage(BattleManager battleController, PartySlot partySlot) {
        if(this.statusAilment != null) {
            this.statusAilment.DealDamage(battleController, partySlot);
        }
        return;
    }



    #nullable enable
    public StatusAilmentSO? statusAilment;
    public int turnCount;
}
}