using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Battle_Values_[Name]", menuName = "ScriptableObjects/Battle Values")]
public class BattleValueSO: ScriptableObject {

    [Tooltip("Changes the movement speed on the battlefield")]
    [Range(0.1f, 20f)]
    [SerializeField]
    private float baseMovementSpeed = 9f;

    public float BaseMovementSpeed { get => this.baseMovementSpeed;}


    [Tooltip("The fraction of damage that a collision does based on the original attack.")]
    [Range(0f, 1f)]
    [SerializeField]
    private float displacementCollisionDamage = 0.1f;

    public  float DisplacementCollisionDamage { get => this.displacementCollisionDamage; }

    [SerializeField]
    [Range(0f, 100f)]
    protected float _displacementTime = 5f;

    public float DisplacementTime { get => this._displacementTime; }

    [Tooltip("The multiplier for flying entities' evasion when attacked by melee attacks.")]
    [Range(0, 1f)]
    [SerializeField]
    private float flyingEntityEvasionMultiplier = 0.25f;

    public  float FlyingEntityEvasionMultiplier { get => this.flyingEntityEvasionMultiplier; }


    [Tooltip("Wait Timer for entities that do not possess an animation")]
    [Range(0, 2)]
    [SerializeField]
    private float noAnimationWaitTimer = 1f;
    public  float NoAnimationWaitTimer { get => this.noAnimationWaitTimer; }

    [Range(0, 100)]
    [SerializeField]
    private uint swapPoints = 10;
    public  uint SwapPoints { get => this.swapPoints; }

    [Range(0, 100)]
    [SerializeField]
    private uint enemyKOPoints = 5;
    public  uint EnemyKOPoints { get => this.enemyKOPoints; } 

    [Range(0, 100)]
    [SerializeField]
    private uint partyMemberKOPoints = 5;
    public uint PartyMemberKOPoints { get => this.partyMemberKOPoints;}

    [Range(0, 100)]
    [SerializeField]
    private uint inflictPoiseBreakPoints = 5;
    public uint InflictPoiseBreakPoints { get => this.inflictPoiseBreakPoints; }

    [Range(0, 100)]
    [SerializeField]
    private uint partyMemberPoiseBrokenPoints = 5;
    public  uint PartyMemberPoiseBrokenPoints { get => this.partyMemberPoiseBrokenPoints; }

    [Range(0, 100)]
    [SerializeField]
    private uint partyMemberDebuffedPoints = 1;
    public  uint PartyMemberDebuffedPoints { get => this.partyMemberDebuffedPoints; }

    [Range(100, 500)]
    [SerializeField]
    private uint maxPartyMoraleGauge = 500;
    public  uint MaxPartyMoraleGauge { get => this.maxPartyMoraleGauge; }

}
