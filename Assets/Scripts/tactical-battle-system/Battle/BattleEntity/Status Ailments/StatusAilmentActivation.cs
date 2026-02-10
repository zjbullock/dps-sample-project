using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[Serializable]
#nullable enable
public class StatusAilmentActivation {

    public StatusAilmentActivation(StatusAilmentActivation statusAilmentActivation) {
        this.statusAilment = statusAilmentActivation.statusAilment;
        this.activationRate = statusAilmentActivation.activationRate;
    }

    public StatusAilmentActivation(StatusAilmentSO statusAilment, int activationRate) {
        Debug.Log(statusAilment);
        this.statusAilment = statusAilment;
        this.activationRate = activationRate;
    }

    public StatusAilmentSO  statusAilment;
    [RangeAttribute(1, 100)]
    public int activationRate;
}

}