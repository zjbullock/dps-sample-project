using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
public class PartyMemberPlayerFollow : PlayerFollow
{
    protected override void HandleFollowTargetOutOfRange()
    {
        
        if(this.navMeshAgent == null) {
            return;
        }

        navMeshAgent.Warp(followTarget.transform.position);
    }

}
}