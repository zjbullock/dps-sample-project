using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Cost_Info_MP_#", menuName = "ScriptableObjects/Active Skill/Cost Info/MP")]
    public class CostInfoMPSO : CostInfoSO
    {
        [SerializeField]
        [Range(0, 30)]
        private int _mp = 1;

        public override bool CanPayCost(IBattleEntity battleEntity)
        {
            return battleEntity.GetRawStats().mp >= _mp;
        }

        public override string GetCostText()
        {
            return $"{_mp} MP";
        }

        public override void PayCost(IBattleEntity battleEntity)
        {
            battleEntity.GetRawStats().mp = DPS.Common.GeneralUtilsStatic.SubtractNoNegatives(battleEntity.GetRawStats().mp, _mp); 
            return;
        }
    }
}