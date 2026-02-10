using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DPS.Common;

namespace DPS.TacticalCombat
{
public abstract class PlayerFollow : MonoBehaviour {

    [Header("Component Refs")]
    [SerializeField]
    protected NavMeshAgent navMeshAgent;

    [SerializeField]
    [Range(0f, 1500f)]
    [Tooltip("Represents the maximum distance a follow can be from the player.")]
    private float maximumDistanceFromPlayer = 10f;

    [SerializeField]
    protected GameObject followTarget;

    [SerializeField]
    private SpriteAnimationController animationController;


    private float defaultSpeed = 0f;


    [SerializeField]
    [Tooltip("The rigid body of this game object")]
    private Rigidbody rb = null;
    
    protected virtual void Awake() {
        this.navMeshAgent = GetComponent<NavMeshAgent>();
        this.defaultSpeed = navMeshAgent.speed;
        // this.defaultScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        // this.flippedScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        if(this.rb == null) {
            this.rb = GetComponent<Rigidbody>();
        }
        if (this.animationController == null) {
            this.animationController = GetComponentInChildren<SpriteAnimationController>();
        }
    }

    // Start is called before the first frame update
    void Start() {

        GameObject playerMovementObject = GameObject.FindGameObjectWithTag("Player");
        if (playerMovementObject != null && this.followTarget == null) {
            this.followTarget = playerMovementObject.gameObject;
        }
        
        #nullable enable
        // this.sceneTransition = GameObject.FindGameObjectWithTag("SceneTransition").GetComponent<SceneTransitionService>();
        // GameObject? sceneContextObject = GameObject.FindGameObjectWithTag("SceneContext");
        // if (sceneContextObject != null && sceneContextObject.TryGetComponent<PauseService>(out PauseService pauseService)) {
        //     this.pauseService =  pauseService;
        // }

        #nullable disable
    }

    public void SetNewFollowTarget(GameObject newTarget) {
        if (newTarget == null) {
            return;
        }
        Debug.Log(newTarget.name);
        Debug.Log("updating follow target");
        this.followTarget = newTarget;
    }


    //TODO:  Figure out how to get sprite renderer to flip to face player character
    void Update() {

        // if (SceneTransitionService.instance != null && SceneTransitionService.instance.IsTransitioning()) {
        //     return;
        // }

        // if (this.pauseService != null && this.pauseService.IsPaused()) {
        //     return;
        // }

        if (followTarget == null) {
            this.followTarget = GameObject.FindGameObjectWithTag("Player");
        }

        this.DetermineGameObjectDirection();
        this.PerformMovement();

        // if(this.childSpriteObject != null) {
        //     this.childSpriteObject.transform.localScale = IsLeftOfPlayer() ? this.flippedScale : this.defaultScale;
        // } else {
        //     transform.localScale = IsLeftOfPlayer() ? this.flippedScale : this.defaultScale;
        // }

        // if (spriteRenderer != null) {
        //     spriteRenderer.flipX = (player.transform.forward.x + player.transform.right.x) < (transform.forward.x + transform.right.x);
        // }
    }

    private void PerformMovement() {
        if (this.navMeshAgent == null) {
            return;
        }

        if (this.navMeshAgent.enabled && this.navMeshAgent.isOnNavMesh && this.navMeshAgent.isStopped) {
            this.StopMovement();
            return;
        }

        if (Vector3.Distance(followTarget.transform.position, this.transform.position) <= navMeshAgent.stoppingDistance) {
            this.StopMovement();
            return;
        }
        
        
        if (Vector3.Distance(followTarget.transform.position, this.transform.position) > navMeshAgent.stoppingDistance &&
            Vector3.Distance(followTarget.transform.position, this.transform.position) <= maximumDistanceFromPlayer) {
            this.HandleFollowTargetInRange();
            return;
        }
        
        this.HandleFollowTargetOutOfRange();
    }

    protected virtual void HandleFollowTargetInRange() {

        if (navMeshAgent.isOnNavMesh) {
            navMeshAgent.SetDestination(followTarget.transform.position);
        }
        
        if (!this.navMeshAgent.enabled) {
            this.ToggleNavMeshAgent(true);
        }

        if (Vector3.Distance(followTarget.transform.position, this.transform.position) < navMeshAgent.stoppingDistance + 2) {
            navMeshAgent.speed = defaultSpeed / 2;
            this.SetMovementAnimation(isWalking: true, isRunning: false);
            return;
        }

        navMeshAgent.speed = defaultSpeed;
        this.SetMovementAnimation(isRunning: true, isWalking: false);

        return;
    }

    protected abstract  void HandleFollowTargetOutOfRange(); 


    /// <summary>
    /// Rotates the controller's game object to face the direction of movement
    /// </summary>
    /// <param name="direction"></param>
    private void DetermineGameObjectDirection() {
        if (this.rb == null) {
            return;
        }

        Vector3 direction = rb.linearVelocity;

        if (direction != Vector3.zero) {
            this.transform.rotation = Quaternion.LookRotation(direction.normalized);
        }
    }

    public void SetMovementAnimation(bool isRunning, bool isWalking) {
        if (this.animationController == null || !this.animationController.IsReady()) {
            return;
        }
        this.animationController.SetMovement(isRunning, isWalking);
    }

    public void StopMovement() {
        if(this.navMeshAgent == null) {
            return;
        }
        
        this.ToggleNavMeshAgent(false);

        this.SetMovementAnimation(false, false);
    }

    private void ToggleNavMeshAgent(bool enable) {
        if (this.navMeshAgent == null) {
            return;
        }
        this.navMeshAgent.enabled = enable;
    }

    public void SetDestination(Vector3 destination) {
        navMeshAgent.SetDestination(destination);
        return;
    }

    // private bool IsLeftOfPlayer() {
    //     // Vector3 playerLocal = player.transform.InverseTransformPoint(transform.position); 
    //     // if (this.playerFollowType == PlayerFollowTypes.NPC) {
    //     //     Debug.Log("Player local" + playerLocal);
    //     // }

    //     // return playerLocal.x < 0;
    //     Vector3 Dir = player.transform.position - transform.position;
    //     Dir = Quaternion.Inverse(transform.rotation) * Dir;

    //     return Dir.x > 0;
    // }

    private enum PlayerFollowTypes {
        NPC,
        Enemy
    }
}
}
