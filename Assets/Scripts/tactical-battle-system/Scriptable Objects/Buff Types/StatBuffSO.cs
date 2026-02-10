using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Buff_Stat_", menuName = "ScriptableObjects/Status Effect/Stat")]
public class StatBuffSO : StatusEffectSO
{
    //Stat Multiplier upon base stat to increase
    public RawStatMultiplier BuffRawStatModifier;
    public RawStats staticRawStats;


    public override void PreStatMultiplierBuffs(IBattleEntity battleEntity)
    {
        RawStats rawStats = battleEntity.GetRawStats();
        RawStats rawStatsDifference = rawStats.StatMultiplier(this.BuffRawStatModifier);
        rawStats.AddStats(rawStatsDifference);
        base.PreStatMultiplierBuffs(battleEntity);
    }

    public override void ProcessBuffList(IBattleEntity battleEntity)
    {
        RawStats rawStats = battleEntity.GetRawStats();
        rawStats.AddStats(this.staticRawStats);
        base.ProcessBuffList(battleEntity);
    }

}
}