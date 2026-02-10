using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;


namespace DPS.TacticalCombat {
[Serializable]
public class BattleFocusCameraEvent : IBattleEvent
{
    private bool IsReady = false;

    public BattleFocusCameraEvent(CinemachineCameraEventController cinemachineCameraEventController) {
        if (cinemachineCameraEventController == null) {
            return;
        }

        cinemachineCameraEventController.AddCinemachineEvent(() => {
            Debug.Log("Focus Camera is finished and ready!");
            // this.IsReady = true;
        });

    }

    public void End()
    {
        return;
    }

    public void Execute()
    {
        Debug.Log("Executing Battle Focus Camera Event");
        return;
    }

    public bool IsDone()
    {
        return this.IsReady;
    }
}
}