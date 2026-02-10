using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
public class StatPassiveSkillBehaviorSO : PassiveSkillBehaviorSO
{
    [SerializeField]
    private RawStats rawStats;

    [SerializeField]
    private RawStatMultiplier rawStatMultiplier;
}
}