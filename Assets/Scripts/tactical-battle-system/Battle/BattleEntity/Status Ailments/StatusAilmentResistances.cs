using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat{
[System.Serializable]
public class StatusAilmentResistances {
    
    [Range(-100,100)]
    public int poisonResistance;
    
    [Range(-100,100)]
    public int burnResistance;

    [Range(-100,100)]
    public int freezeResistance;

    [Range(-100,100)]
    public int paralysisResistance;

    [Range(-100,100)]
    public int sleepResistance;


    public StatusAilmentResistances() {
        this.poisonResistance = 0;
        this.burnResistance = 0;
        this.freezeResistance = 0;
        this.paralysisResistance = 0;
        this.sleepResistance = 0;
    }

    public StatusAilmentResistances(StatusAilmentResistances statusAilmentResistances) {
        this.poisonResistance = statusAilmentResistances.poisonResistance;
        this.burnResistance = statusAilmentResistances.burnResistance;
        this.freezeResistance = statusAilmentResistances.freezeResistance;
        this.paralysisResistance = statusAilmentResistances.paralysisResistance;
        this.sleepResistance = statusAilmentResistances.sleepResistance;
    }

    public void AddStatusAilmentResistance(StatusAilmentResistances statusAilmentResistances) {
        this.poisonResistance += statusAilmentResistances.poisonResistance;
        this.burnResistance += statusAilmentResistances.burnResistance;
        this.freezeResistance += statusAilmentResistances.freezeResistance;
        this.paralysisResistance += statusAilmentResistances.paralysisResistance;
        this.sleepResistance += statusAilmentResistances.sleepResistance;
    }

    public void SubtractStatusAilmentResistance(StatusAilmentResistances statusAilmentResistances) {
        this.poisonResistance -= statusAilmentResistances.poisonResistance;
        this.burnResistance -= statusAilmentResistances.burnResistance;
        this.freezeResistance -= statusAilmentResistances.freezeResistance;
        this.paralysisResistance -= statusAilmentResistances.paralysisResistance;
        this.sleepResistance -= statusAilmentResistances.sleepResistance;
    }

}
}