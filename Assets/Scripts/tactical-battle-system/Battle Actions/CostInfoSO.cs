using UnityEngine;

namespace DPS.TacticalCombat{
public abstract class CostInfoSO : ScriptableObject
{
    public abstract bool CanPayCost(IBattleEntity battleEntity);

    public abstract string GetCostText();

    public abstract void PayCost(IBattleEntity battleEntity);
}
}