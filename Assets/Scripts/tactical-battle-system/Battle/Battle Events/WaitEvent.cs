using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class WaitEvent : IBattleEvent
{

    private float timeInSeconds;

    #nullable enable
    private System.Action? callBack;

    public WaitEvent(float timeInSeconds, System.Action? callBack = null) {
        this.timeInSeconds = timeInSeconds;
        this.callBack = callBack;
    }

    public void End()
    {
        this.callBack?.Invoke();
        this.callBack = null;
    }

    public void Execute()
    {
        this.timeInSeconds -= Time.deltaTime;
    }

    public bool IsDone()
    {
        return this.timeInSeconds <= 0f;
    }
    #nullable disable
}
