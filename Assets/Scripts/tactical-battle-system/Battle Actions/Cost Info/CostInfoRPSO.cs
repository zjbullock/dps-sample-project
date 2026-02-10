using UnityEngine;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Cost_Info_RP_Type_#", menuName = "ScriptableObjects/Active Skill/Cost Info/RP")]
public class CostInfoRPSO : CostInfoSO
{
    #nullable enable
    [SerializeField]
    [Range(1, 100)]
    private int _rp = 1;

    [SerializeField]
    private bool _consumesRP = true;

    public override bool CanPayCost(IBattleEntity battleEntity)
    {
        if(this._consumesRP)
        {
            if (battleEntity is not CharacterInfo characterInfo) return false;
            if (characterInfo.ResolveGaugeSO == null) return false;

            return characterInfo.GetResolvePoints() >= _rp;
        } else
        {
            return true;
        }
    }

    public override string GetCostText()
    {
        return _consumesRP ? $"{_rp} RP" : $"Gain {_rp} RP";
    }

    public override void PayCost(IBattleEntity battleEntity)
    {
        if(this._consumesRP)
        {
            this.PayRP(battleEntity);
        } else
        {
            this.GainRP(battleEntity);
        }
    }

    private void PayRP(IBattleEntity battleEntity)
    {
        if (battleEntity is not CharacterInfo characterInfo) return;
        if (characterInfo.ResolveGaugeSO == null) return;

        characterInfo.SetResolvePoints(characterInfo.GetResolvePoints() + _rp);
    }

    private void GainRP(IBattleEntity battleEntity)
    {
        if (battleEntity is not CharacterInfo characterInfo) return;
        if (characterInfo.ResolveGaugeSO == null) return;

        int newRP = characterInfo.GetResolvePoints() + _rp;
        if (newRP > characterInfo.ResolveGaugeSO.maxResolveGauge)
        {
            characterInfo.SetResolvePoints(characterInfo.ResolveGaugeSO.maxResolveGauge);
        } else
        {
            characterInfo.SetResolvePoints(newRP);
        }
    }

}
}