using UnityEngine;

namespace DPS.TacticalCombat
{
    public abstract class EncounterCondition : MonoBehaviour
    {
         public abstract bool IsEncounterConditionMet(BattleManager battleManager);
    }
}
