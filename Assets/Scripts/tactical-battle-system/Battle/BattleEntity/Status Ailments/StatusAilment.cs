using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DPS.TacticalCombat {
[Serializable]
public class StatusAilment {
    public StatusAilment() {}

    public StatusAilment(StatusAilmentSO statusAilmentSO) {
        Debug.Log(statusAilmentSO);
        this.name = statusAilmentSO.statusAilmentName;
        this.description = statusAilmentSO.description;
        this.affectsMaxHP = statusAilmentSO.affectsMaxHP;
        this.percentageDamage = statusAilmentSO.percentageDamage;
        this.flavorText = statusAilmentSO.flavorText;
        this.isInfiniteTurnCount = statusAilmentSO.isInfiniteTurnCount;
        this.minTurnCount = statusAilmentSO.minTurnCount;
        this.maxTurnCount = statusAilmentSO.maxTurnCount;
    }

    public StatusAilment(StatusAilment statusAilment) {
        this.name = statusAilment.name;
        this.description = statusAilment.description;
        this.affectsMaxHP = statusAilment.affectsMaxHP;
        this.percentageDamage = statusAilment.percentageDamage;
        this.flavorText = statusAilment.flavorText;
        this.isInfiniteTurnCount = statusAilment.isInfiniteTurnCount;
        this.minTurnCount = statusAilment.minTurnCount;
        this.maxTurnCount = statusAilment.maxTurnCount;
    }

        // Start is called before the first frame update
    public string name;
    public string description;

    //DoT flags
    //If true, percentage damage comes from max HP.  If false, uses current HP  
    public bool  affectsMaxHP;
    [RangeAttribute(0f, 1f)]
    public float percentageDamage;

    //Loss of Turn flags
    public string flavorText;

    public bool isInfiniteTurnCount;
    public int minTurnCount;
    public int maxTurnCount;
}
}