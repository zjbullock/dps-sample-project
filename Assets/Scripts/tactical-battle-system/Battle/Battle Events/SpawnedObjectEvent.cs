using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[System.Serializable]
public class SpawnedObjectEvent : IBattleEvent
{
    #nullable enable
    [SerializeField]
    protected SpawnableBattleObject? spawnableBattleObject;

    public SpawnedObjectEvent(SpawnableBattleObject spawnableBattleObject) {
        this.spawnableBattleObject = spawnableBattleObject;
        if (this.spawnableBattleObject == null) {
            return;
        }
        this.spawnableBattleObject.IsAnimating = true;
    }

    public void End()
    {
        return;
    }

    public void Execute()
    {
        return;   
    }

    public bool IsDone()
    {
        return this.spawnableBattleObject == null || (this.spawnableBattleObject != null && !this.spawnableBattleObject.IsAnimating);
    }
}
}
