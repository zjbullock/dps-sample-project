using System;
using UnityEngine;

namespace DPS.TacticalCombat {
[Serializable]
public class BattleFirstActionEvent : IBattleEvent
{
    [Header("FIRST EVENT")]
    [SerializeField]
    private BattleManager _battleManager;

    [SerializeField]
    private bool isDone = false;

    public BattleFirstActionEvent(BattleManager battleManager)
    {
        this._battleManager = battleManager;
    }

    public void Execute()
    {
        return;
    }

    public bool IsDone()
    {
        return this.isDone && this._battleManager.BattleEventController.CanProgressDespite(this);
    }

    public void End()
    {
        this._battleManager.BattleEventController.ReactingPartySlotHandler.ExecuteNextEvent(this._battleManager);
        return;
    }

    public void CompleteFirstEvent()
    {
        this.isDone = true;
    }
}
}