using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleEvent
{
    public void Execute();

    public bool IsDone();

    public void End();
}


public class SimpleActionEvent: IBattleEvent
{
    [SerializeField]
    private System.Action action;
    public SimpleActionEvent(System.Action action)
    {
        this.action = action;
    }

    public bool IsDone()
    {
        return true;
    }

    public void Execute()
    {
        this.action?.Invoke();
    }

    public void End()
    {
        return;
    }

}