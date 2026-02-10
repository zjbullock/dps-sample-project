using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/**
    This class was created to hold a list of all reacting Party Slots.
    It's primary purpose is to serve as an easy way of gathering all entities that are reacting to a battle event, and processing them sequentially in order.
*/

namespace DPS.TacticalCombat {
[Serializable]
public class BattleReactionEvent : IBattleEvent
{
    [Header("REACTION EVENT")]

    [SerializeField]
    private BattleManager _battleManager;

    [SerializeField]
    private ReactingPartySlotHandler.ReactingPartySlot reactingPartySlot;

    [SerializeField]
    private bool _isDone;

    [SerializeField]
    private System.Action _callBack;

    public BattleReactionEvent(BattleManager battleManager, ReactingPartySlotHandler.ReactingPartySlot partySlot, System.Action callBack)
    {
        this._battleManager = battleManager;
        this.reactingPartySlot = partySlot;
        this._isDone = false;
        this._callBack = callBack;
        // battleController.StartCoroutine(ExecuteReactingPartySlotEvent());
        this.ExecutePartySlotEvent();
    }


    public void Execute()
    {
        return;
    }

    public bool IsDone()
    {
        return _isDone && !this._battleManager.BattleEventController.IsDisplacing();
    }

    public void End()
    {
        this._callBack?.Invoke();
        return;
    }


    // IEnumerator ExecuteReactingPartySlotEvent()
    // {
    //     yield return new WaitForSeconds(2f);
    //     foreach (ReactingPartySlotHandler.ReactingPartySlot reactingPartySlot in reactingPartySlots)
    //     {
    //         reactingPartySlot.PartySlot.OnReaction(new List<ElementSO>(reactingPartySlot.Elements), () => { this.DecreaseCounter(); });
    //         yield return new WaitForSeconds(1f);
    //     }
    // }

    private void ExecutePartySlotEvent()
    {
        reactingPartySlot.PartySlot.OnReaction(new List<ElementSO>(reactingPartySlot.Elements), () => { this._isDone = true; });
    }
}

}