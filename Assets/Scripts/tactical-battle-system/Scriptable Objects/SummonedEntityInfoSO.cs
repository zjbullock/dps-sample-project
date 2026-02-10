using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;
using DPS.TacticalCombat;

[CreateAssetMenu(fileName = "Summonable_Entity_#", menuName = "ScriptableObjects/Summoned Entity")]
public class SummonedEntityInfoSO : ScriptableObject
{
    public string Name;

    [Range(1, 200)]
    public int Level;
    
    public int Poise = 2;

    public RawStats BaseRawStats;


    public GenericDictionary<ElementSO, ElementalResistance> Elements = new GenericDictionary<ElementSO, ElementalResistance>{};
    // Start is called before the first frame update

    public StartingSkills SkillInfoSO;
    public Vector3 targetingPositionOffset;
    public Sprite characterSpritePortrait;

    [Header("Movement speeds")]
    [Tooltip("Controls how far the character can move on their turn.")]
    [Range(0, 15)]
    public int speed;

    [Tooltip("Controls how high this character can traverse terrain.  Default value is 0.5f")]
    [Range(0f, 99f)]
    public float verticalSpeed = 0.5f;
    
    [Tooltip("Status Ailment Resistances")]
    public StatusAilmentResistances statusAilmentResistances;

    [Tooltip("Overrides the movement cost of a tile if its element is in this dictionary")]
    public GenericDictionary<ElementSO, int> terrainMovementOverride = new GenericDictionary<ElementSO, int>();

    [Tooltip("The turn duration for this object")]
    [Range(2, 100)]
    public int turnDuration = 5;

    [Tooltip("The Game Object for the Entity that will be instanced on the grid")]
    [SerializeField]
    public GameObject summonableEntityObject;


    [Tooltip("Determines whether this summoned entity can be moved in combat forcibly")]
    public bool canBePulled;
}
