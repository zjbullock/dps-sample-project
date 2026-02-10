using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DPS.TacticalCombat {
    public abstract class EnemyPartySlotController : PartySlotController
    {
        public override void PreparePartyMembers(BattleManager battleController)
        {
            // PlayerProfileData.Instance.EncounterLoader.ClearEncounter();
        }

        // public override void OnDetermineEncounterOver(BattleManager battleManager)
        // {
        //     // List<PartySlot> newPartySlots = new();

        //     // foreach (EnemyPartySlot partySlot in base._partySlots.Cast<EnemyPartySlot>())
        //     // {
        //     //     if (partySlot.BattleEntity.IsDead())
        //     //     {
        //     //         partySlot.HandleDeath(battleManager);
        //     //         continue;
        //     //     }

        //     //     newPartySlots.Add(partySlot);
        //     // }


        //     // base._partySlots = newPartySlots;
        // }
    }
}