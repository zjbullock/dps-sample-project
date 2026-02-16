using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using UnityEngine;

namespace DPS.TacticalCombat {
public abstract class PartySlotController : MonoBehaviour
{
    [Header("Component Refs")]
    [SerializeField]
    protected Transform _lookAtTransform;
    
    [Header("Runtime Vals")]
    public int AliveCount { get => this.GetAliveCounts(); }

    [SerializeField, SerializeReference]
    protected List<PartySlot> _partySlots;

    public List<PartySlot> PartySlots { get => this._partySlots; }

    public abstract Task PreparePartyMembers(BattleManager battleController);

    private int GetAliveCounts()
    {
        int aliveCounts = 0;
        foreach (PartySlot partySlot in this.PartySlots)
        {
            if (partySlot.GetBattleMember() == null)
            {
                continue;
            }

            if (!partySlot.BattleEntity!.IsDead())
            {
                aliveCounts++;
            }
        }

        return aliveCounts;
    }


    public void TogglePartyMemberObjects(bool active)
    {
        foreach (PartySlot partySlot in this.PartySlots)
        {
            partySlot.BattleEntityGO.gameObject.SetActive(active);
        }
    }
}
}