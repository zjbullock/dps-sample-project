using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace DPS.TacticalCombat{
public class BattleEventController : MonoBehaviour
{
    [SerializeField, SerializeReference]
    private ReactingPartySlotHandler _reactingPartySlotHandler;

    public ReactingPartySlotHandler ReactingPartySlotHandler { get => this._reactingPartySlotHandler; }

    [SerializeField, SerializeReference]
    private List<IBattleEvent> _battleEventProcessor = new List<IBattleEvent>();

    [SerializeField, SerializeReference]
    private List<DisplacementEvent> _battleDisplacementEventProcessor = new List<DisplacementEvent>();

    void Awake()
    {
        this._reactingPartySlotHandler = new();
    }

    public bool CanProgress()
    {
        return this._battleEventProcessor.Count == 0 && _battleDisplacementEventProcessor.Count == 0;
    }

    public bool CanProgressDespite(IBattleEvent battleEvent)
    {
        List<IBattleEvent> currentBattleEvents = new(this._battleDisplacementEventProcessor);
        currentBattleEvents.RemoveAll((action) => action == battleEvent);

        List<DisplacementEvent> displacementEvents = new(this._battleDisplacementEventProcessor);
        displacementEvents.RemoveAll((action) => action == battleEvent);

        return currentBattleEvents.Count == 0 && displacementEvents.Count == 0;
    }

    public void ProcessNextEvent(BattleManager battleManager)
    {
        this.ReactingPartySlotHandler.ExecuteNextEvent(battleManager);
    }

    public bool IsDisplacing()
    {
        return this._battleDisplacementEventProcessor.Count > 0;
    }

    public void AddBattleEvent(IBattleEvent battleEvent)
    {
        if (battleEvent == null)
        {
            return;
        }

        StartCoroutine(ProcessUpdateStateEvent(battleEvent));
    }

    #nullable enable
    public void AddBattleEvent(DisplacementEvent displacementEvent)
    {
        if (displacementEvent == null)
        {
            return;
        }

        DisplacementEvent? displacement =
        _battleDisplacementEventProcessor.Find(
            (displacement) => displacement.PartySlot == displacementEvent.PartySlot
        );

        if (displacement != null)
        {
            return;
        }

        StartCoroutine(ProcessUpdateStateEvent(displacementEvent));
    }

    private IEnumerator ProcessUpdateStateEvent(IBattleEvent battleEvent)
    {
        if (battleEvent == null)
        {
            yield break;
        }

        if (!_battleEventProcessor.Contains(battleEvent))
        {
            _battleEventProcessor.Add(battleEvent);
        }


        while (!battleEvent.IsDone())
        {
            battleEvent.Execute();
            yield return null;
        }

        battleEvent.End();

        if (_battleEventProcessor.Contains(battleEvent))
        {
            _battleEventProcessor.Remove(battleEvent);
        }
    }
    

    private IEnumerator ProcessUpdateStateEvent(DisplacementEvent battleEvent)
    {
        if (battleEvent == null)
        {
            yield break;
        }

        if (!_battleDisplacementEventProcessor.Contains(battleEvent))
        {
            _battleDisplacementEventProcessor.Add(battleEvent);
        }


        while (!battleEvent.IsDone())
        {
            battleEvent.Execute();
            yield return null;
        }

        battleEvent.End();

        if (_battleDisplacementEventProcessor.Contains(battleEvent))
        {
            _battleDisplacementEventProcessor.Remove(battleEvent);
        }
    }
    
}

public class ReactingPartySlotHandler
{
    
    [Serializable]
    public struct ReactingPartySlot
    {
        [SerializeField, SerializeReference]
        private PartySlot partySlot;

        public PartySlot PartySlot { get => this.partySlot; }

        [SerializeField]
        List<ElementSO> elements;

        public ReadOnlyCollection<ElementSO> Elements { get => this.elements.AsReadOnly(); }

        public ReactingPartySlot(PartySlot partySlot, ElementSO element)
        {
            this.partySlot = partySlot;
            this.elements = new() { element };
        }
        public ReactingPartySlot(PartySlot partySlot, List<ElementSO> elements)
        {
            this.partySlot = partySlot;
            this.elements = new();
            this.elements.AddRange(elements);
        }

        public void AddElement(ElementSO elementSO)
        {
            if (this.elements.Contains(elementSO))
            {
                return;
            }

            this.elements.Add(elementSO);
        }
    }


    [SerializeField]
    private List<ReactingPartySlot> reactingPartySlots;

    public ReactingPartySlotHandler()
    {
        this.reactingPartySlots = new();
    }

    private bool DoesReactingPartySlotsContainPartySlot(PartySlot partyslot) {
        foreach (ReactingPartySlot reactingPartySlot in this.reactingPartySlots)
        {
            if (reactingPartySlot.PartySlot == partyslot)
            {
                return true;
            }
        }
        return false;
    }

    private ReactingPartySlot GetReactingPartyslot(PartySlot partySlot) {
        return this.reactingPartySlots.Find((slot) => { return slot.PartySlot == partySlot; });
    }

    //Adds reacting party slots and keeps track of what element was used.  No duplicate elements.
    public void AddReactingPartySlots(List<PartySlot> partySlots, ElementSO element)
    {
        foreach (PartySlot partySlot in partySlots)
        {
            if (partySlot == null || partySlot.BattleEntity == null)
            {
                continue;
            }

            if (partySlot.BattleEntity.IsDead())
            {
                continue;
            }

            if (!this.DoesReactingPartySlotsContainPartySlot(partySlot))
            {
                this.reactingPartySlots.Add(new(partySlot, element));
                continue;
            }

            ReactingPartySlot reactingPartySlot = this.GetReactingPartyslot(partySlot);

            if (reactingPartySlot.Elements.Contains(element))
            {
                continue;
            }

            reactingPartySlot.AddElement(element);
        }
    }

    public void ExecuteNextEvent(BattleManager battleManager)
    {
        if (this.reactingPartySlots.Count == 0)
        {
            return;
        }
        ReactingPartySlot nextReaction = this.reactingPartySlots[0];
        this.reactingPartySlots.RemoveAt(0);
        battleManager.BattleEventController.AddBattleEvent(new BattleReactionEvent(battleManager, nextReaction, () => { this.ExecuteNextEvent(battleManager);  }));
    }
}

}