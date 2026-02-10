using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {

[System.Serializable]

public class BattleAnimationEvent : IBattleEvent
{

    [SerializeField]
    private List<BattleAnimationEventSpawn> animations = new List<BattleAnimationEventSpawn>();

    [SerializeField]
    private List<BattleActionController> animationObjects = new();

    [SerializeField]
    private float waitTime = 0.2f;

    [SerializeField]
    private float currentTime = 0f;

#nullable enable
    // private System.Action? postAnimationCallBack;

    [SerializeField]
    private BattleManager battleController;

    [SerializeField]
    private bool isDone = false;

    private bool IsAnimationDone() { return this.animations.Count == 0 && this.animationObjects.Count == 0; }

    public BattleAnimationEvent (List<BattleAnimationEventSpawn> animations, BattleManager battleController, bool shouldLookAtTarget = true) {
        this.animations = animations;
        this.battleController = battleController;
    }

    public void Execute()
    {
        this.CheckAnimationObjects();
        if (this.IsAnimationDone()) {
            // this.postAnimationCallBack?.Invoke();
            this.isDone = true;
            return;
        }
        this.DecreaseCurrentTime();
        if (!this.CanAct()) {
            return;
        }
        this.SpawnFromList();

    }

    private void SpawnFromList() {
        if (this.animations.Count == 0) {
            return;
        }

        this.SpawnAnimationObject(this.animations[0]);

        this.animations.RemoveAt(0);
        this.SetTimer();
        return;
    }

    private void SpawnAnimationObject(BattleAnimationEventSpawn animationEvent) {
        #nullable enable
        BattleActionController? targetAnimation = battleController.InstantiateBattleActionController(animationEvent);
        if (targetAnimation == null)
        {
            Debug.Log("Animation Event failed to create new target animation!");
            return;
        }
        this.animationObjects.Add(targetAnimation);
        
        // this.battleController.SetCameraTarget(targetAnimation.transform);
        return;
    }

    private void CheckAnimationObjects() {
        List<BattleActionController> newAnimationObjects = new();
        foreach(BattleActionController animation in this.animationObjects) {
            if (animation == null) {
                continue;
            }
            newAnimationObjects.Add(animation);
        }
        this.animationObjects = newAnimationObjects;
    }

    private void SetTimer() {
        this.currentTime = this.waitTime;
    }

    private void DecreaseCurrentTime() {
        this.currentTime -= Time.deltaTime;
    }

    private bool CanAct() {
        return this.currentTime <= 0f;
    }

    public bool IsDone()
    {
        return this.isDone;
    }

    public void End() {
        return;
    }
}


[Serializable]
public class BattleAnimationEventSpawn
{
    [Header("Prefab to Spawn")]

    [SerializeField]
    private BattleActionController _animationPrefab;

    public BattleActionController AnimationPrefab { get => this._animationPrefab; }

    [Space]
    [Header("Positioning and Rotation")]
    [SerializeField]
    private BattleObject _caster;

    public BattleObject Caster { get => this._caster; }


    [SerializeField]
    private Vector3 _position;
    public Vector3 Position { get => this._position; }

    private Quaternion _rotation;
    public Quaternion Rotation { get => this._rotation; }

    private Vector3 _lookAtTargetPosition;
    public Vector3 LookAtTargetPosition { get => this._lookAtTargetPosition; }

    
    private System.Action? _onAnimationEvent;

    public System.Action? OnAnimationEvent { get => this._onAnimationEvent; }


    private System.Action? _onAnimationEnd;

    public System.Action? OnAnimationEnd { get => this._onAnimationEnd; }

    public Transform? animationParent;

    public BattleAnimationEventSpawn(BattleActionController animation, BattleObject caster, Vector3 position, Quaternion rotation, Vector3 lookAtPosition, System.Action? duringAnimationCallBack, System.Action? onAnimationEndCallBack)
    {
        this._caster = caster;
        this._animationPrefab = animation;
        this._position = position;
        this._rotation = rotation;
        this.animationParent = null;
        this._lookAtTargetPosition = lookAtPosition;
        this._onAnimationEvent = duringAnimationCallBack;
        this._onAnimationEnd = onAnimationEndCallBack;
    }

    public BattleAnimationEventSpawn(BattleActionController animation, BattleObject caster, Transform? parent, Transform transform, Vector3 lookAtPosition, System.Action? duringAnimationCallBack, System.Action? onAnimationEndCallBack)
    {
        this._caster = caster;
        this._animationPrefab = animation;
        this._position = transform.position;
        this._rotation = transform.rotation;
        this.animationParent = parent;
        this._lookAtTargetPosition = lookAtPosition;
        this._onAnimationEvent = duringAnimationCallBack;
        this._onAnimationEnd = onAnimationEndCallBack;
    }
}
}
