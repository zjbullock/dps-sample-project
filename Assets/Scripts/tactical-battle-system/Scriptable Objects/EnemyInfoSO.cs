using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DPS.Common;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Enemy Info", menuName = "ScriptableObjects/Enemy Info")]
public class EnemyInfoSO : ScriptableObject {

    [SerializeField]
    private string enemyName;
    public string EnemyName {get => this.enemyName; }

    public string Description;
    public RawStats BaseRawStats;
    public RawStats RawStats;
    // public List<ElementSO> Weakness  = new List<ElementSO>{ElementSO.Chaos, ElementSO.Order};
    // public List<ElementSO> Resistances;

    // public List<ElementSO> Nullifies;

    // public List<ElementSO> Absorbs;

    public GenericDictionary<ElementSO, ElementalResistance> Elements = new GenericDictionary<ElementSO, ElementalResistance>{};

    public Poise poise;

    //A list of enemy AI behavior.  It is sorted with highest priority abilities at the top.
    [SerializeField]
    private List<EnemyAISO> enemyAIList;
    public List<EnemyAISO> EnemyAIList { get => this.enemyAIList;}

    public Sprite enemySpritePortrait;

    [Tooltip("Controls how far the enemy can move on their turn.")]
    [Range(0, 15)]
    public int speed;

    [Tooltip("Controls how high this character can traverse terrain")]
    [Range(0f, 99f)]
    public float verticalSpeed;

    public DefendAISO defendAI;

    [Tooltip("The basic attack functionality of the target")]
    public EnemyAISO basicAttackAI;

    [Tooltip("Status Ailment Resistances")]
    public StatusAilmentResistances statusAilmentResistances;

    [Tooltip("Overrides the movement cost of a tile if its element is in this dictionary")]
    public GenericDictionary<ElementSO, int> terrainMovementOverride = new GenericDictionary<ElementSO, int>();

    [Tooltip("Passive skills for the enemy")]
    public List<PassiveSkillSO> PassiveSkills;

    [Tooltip("The Possible drops for different items")]

    public List<DropChance> dropChances;


    [Range(0, 10000000)]
    [Tooltip("The amount of gp gained from being slain")]
    public uint gp;

    [Tooltip("Determines if this target can be pulled by skills and abilities")]
    public bool canBeDisplaced = true;

    [Tooltip("Reference to the Battle Object")]
    public EnemyBattleObject enemyBattleObject;
}
}