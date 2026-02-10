using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Stat_Increase_Skill_", menuName = "ScriptableObjects/Stat Skill/Stat Increase Skill")]

public class StatIncreaseSkillSO : ScriptableObject, IBattleActionDescription
{
    [Header("Skill Descriptors")]
    [SerializeField]
    private string skillName;

    [SerializeField]
    private string skillDescription;

    [Header("Stats")]

    [SerializeField]
    private CharacterStats characterStats;

    [SerializeField]
    private CharacterStatModifiers characterStatModifiers;

    [SerializeField]
    private RawStats rawStats;

    [SerializeField]
    private RawStatMultiplier rawStatMultiplier;

    public RawStats ExecuteAddCharacterRawStatPassiveSkill()
    {
        return this.rawStats;
    }

    public RawStatMultiplier ExecuteAddCharacterRawStatMultiplierPassiveSkill()
    {
        return this.rawStatMultiplier;
    }


    public CharacterStatModifiers ExecuteAddCharacterStatModifierPassiveSkill()
    {
        return this.characterStatModifiers;
    }


    public CharacterStats ExecuteAddCharacterStatPassiveSkill()
    {
        return this.characterStats;
    }

    public string GetActionName()
    {
        return this.skillName;
    }

    public string GetDescription()
    {
        return this.skillDescription;
    }
}
}