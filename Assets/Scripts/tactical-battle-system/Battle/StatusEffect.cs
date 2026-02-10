using System.Collections;
using System.Collections.Generic;
using System;

namespace DPS.TacticalCombat {
[Serializable]
public class StatusEffect {
    public StatusEffect(StatusEffectSO statusEffect) {
        this.turnCount = statusEffect.turnCount;
        this.statusEffect = statusEffect;
    }
    public StatusEffect(StatusEffect statusEffect) {
        this.turnCount = statusEffect.turnCount;
        this.statusEffect = statusEffect.statusEffect;
    }
    //Turn Duration
    public int turnCount;
    //Stat Multiplier upon base stat to increase
    public StatusEffectSO statusEffect;

    //Amount By which stats are increased, generated from BuffRawStatModifier
    // public RawStats BuffRawStats;
}
}
