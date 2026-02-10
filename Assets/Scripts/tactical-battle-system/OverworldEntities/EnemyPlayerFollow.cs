using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
public class EnemyPlayerFollow : PlayerFollow
{

    private Vector3 startingPosition;

    [SerializeField]
    private int maxChaseLevel;

    [SerializeField]
    [Range(0f, 1500f)]
    [Tooltip("Represents the maximum distance a follow can be from their start position.  Used for Enemy Player Follow Type.")]
    private float maximumDistanceFromStartPosition = 20f;

    protected override void Awake()
    {
        this.startingPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        base.Awake();
    }

    protected override void HandleFollowTargetInRange()
    {
        if(this.navMeshAgent == null) {
            return;
        }

        if (Vector3.Distance(transform.position, this.startingPosition) >= this.maximumDistanceFromStartPosition) {
            navMeshAgent.SetDestination(startingPosition);
            return;
        }

        if (Vector3.Distance(transform.position, this.startingPosition) <= 2f) {
            navMeshAgent.SetDestination(followTarget.transform.position);
        }

        base.HandleFollowTargetInRange();
    }

    protected override void HandleFollowTargetOutOfRange()
    {
        if (navMeshAgent.isOnNavMesh) {
            navMeshAgent.SetDestination(startingPosition);
        }
    }
}
}