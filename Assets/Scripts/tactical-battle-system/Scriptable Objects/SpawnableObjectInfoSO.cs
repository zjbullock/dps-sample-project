using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;
using DPS.TacticalCombat;

[CreateAssetMenu(fileName = "Battle_Spawned_Object_Info_", menuName = "ScriptableObjects/Battle Spawned Object Info")]

public class SpawnableObjectInfoSO : ScriptableObject
{

    public string spawnedObjectName;

    public RawStats rawStats;

    public GenericDictionary<ElementSO, ElementalResistance> elementalResistances = new GenericDictionary<ElementSO, ElementalResistance>{};

    public GenericDictionary<ElementSO, ActiveSkillSO> reactionSkills;

    public ActiveSkillSO onDeathSkill;

    public ActiveSkillSO onAttackedSkill;

    public bool canBeDisplaced = false;
}
