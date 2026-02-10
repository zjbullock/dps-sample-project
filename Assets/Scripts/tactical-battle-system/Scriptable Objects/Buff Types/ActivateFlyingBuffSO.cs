using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat {

[CreateAssetMenu(fileName = "Buff_Fly_0", menuName = "ScriptableObjects/Status Effect/Fly")]
public class ActivateFlyingBuffSO : StatusEffectSO
{

    public override void PreStatMultiplierBuffs(IBattleEntity battleEntity)
    {
        base.PreStatMultiplierBuffs(battleEntity);
    }


    public override void ProcessBuffList(IBattleEntity battleEntity)
    {
        base.ProcessBuffList(battleEntity);
        if(!battleEntity.CanFly()) {
            battleEntity.SetFly(true);
        }
    }


    public override bool OnTakeDamage(ElementSO ElementSO, PartySlot partySlot, CombatTileController tileController)
    {   
        IBattleEntity battleEntity = partySlot.BattleEntity;
        //If entity can currently fly
        if (battleEntity.CanFly()) {
            GenericDictionary<ElementSO, ElementalResistance> elementalResistances =  battleEntity.GetElements();
            if (elementalResistances != null && elementalResistances.ContainsKey(ElementSO) && elementalResistances[ElementSO] == ElementalResistance.Weak)  {
                battleEntity.SetFly(false);
                // partySlot.SetBattleObjectVector3(tileController);
                return false;
            }

            Poise poise = battleEntity.GetPoiseState();
            if (poise != null && poise.PoiseState == PoiseState.Broken) {
                battleEntity.SetFly(false);
                // partySlot.SetBattleObjectVector3(tileController);
                return false;
            }

            InflictedAilment inflictedAilment = battleEntity.GetInflictedAilment();
            if (inflictedAilment != null && inflictedAilment.statusAilment != null && inflictedAilment.statusAilment.preventsTurn) {
                battleEntity.SetFly(false);
                // partySlot.SetBattleObjectVector3(tileController);
                return false;
            }
            
            return true;
        } 

        return false;
    }

    

    public override void OnRemoveStatusEffect(IBattleEntity battleEntity)
    {
        if (battleEntity.CanFly()) {
            battleEntity.SetFly(false);
        }
    }
}
}