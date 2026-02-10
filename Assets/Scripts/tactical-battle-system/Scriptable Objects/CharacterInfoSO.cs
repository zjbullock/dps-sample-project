using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Party_Member_CharacterName_#", menuName = "ScriptableObjects/Character Info")]
public class CharacterInfoSO: ScriptableObject {


    public Name Name;

    public Level Level;

    public CharacterClassSO Class;

    public CharacterRoleSO RoleSO;
    
    public int Poise;
    public ResolveGaugeSO ResolveGauge;
    // public List<ElementSO> Weakness;

    // public List<ElementSO> Resistances;

    // public List<ElementSO> Nullifies;

    // public List<ElementSO> Absorbs;

    public GenericDictionary<ElementSO, ElementalResistance> Elements = new GenericDictionary<ElementSO, ElementalResistance>{

    };


    public CharacterStats BaseCharacterStats;
    public CharacterStatModifiers BaseCharacterStatMultiplier;
    public float Scale = 1;
    public CharacterStats CharacterStats;

    public CharacterStats LevelUpStats;
    
    public RawStats BaseRawStats;
    public RawStatMultiplier BaseRawStatMultiplier;

    public StartingSkills SkillInfoSO;
    
    public int SkillPoints;

    public int AllocatedSkillPoints;

    public CharacterEquipmentAsset Equipment;

    public Sprite characterSpritePortrait;

    public Sprite characterSpriteFullBody;
    
    [Header("Movement speeds")]
    [Tooltip("Controls how far the character can move on their turn.")]
    [Range(0, 15)]
    public int speed;

    [Tooltip("Controls how high this character can traverse terrain.  Default value is 0.5f")]
    [Range(0f, 99f)]
    public float verticalSpeed = 0.5f;
    
    [Tooltip("Status Ailment Resistances")]
    public StatusAilmentResistances statusAilmentResistances;

    [Tooltip("The animator for the character")]
    public RuntimeAnimatorController animator;

    [Tooltip("Battle Member Object")]
    public BattleObject battleMemberObjectReference;

    [Tooltip("Overworld Member NPC Follower Reference")]
    public PartyMemberPlayerFollow overworldFollowNPCObjectReference;

    [Tooltip("Character Advancement CharacterInfoSO")]
    public CharacterInfoSO characterAdvancementInfoSO;
}

[Serializable]
public class StartingSkills
{
    [SerializeField]
    private ActiveSkillSO attackSkill;

    public ActiveSkillSO AttackSkill { get => this.attackSkill; }

    [SerializeField]
    public ActiveSkillSO defendSkill;
    public ActiveSkillSO DefendSkill { get => this.defendSkill; }

    public List<SkillTier> skillTiers;
    public List<ActiveSkillSO> StartingEquippedSkills;

    public List<ActiveSkillSO> ResolveSkills;

    [SerializeField]
    private SwapSkillSO defaultSwapSkill;
    public SwapSkillSO DefaultSwapSkill { get => this.defaultSwapSkill; }
    
    [SerializeField]
    private SwapSkillSO specialSwapSkill;

    public SwapSkillSO SpecialSwapSkill { get => this.specialSwapSkill; }
}

[Serializable]
public class CharacterEquipmentAsset {

    public ArmorTypes ArmorType;
    public EquipmentSlotsSO EquipmentSlots;
}

[Serializable]
public class EquipmentSlotsSO {

    public WeaponRuneEquipmentSO WeaponRune;
    // public EquipmentSO OffHand;
    public HelmEquipmentSO Helm;
    public BodyEquipmentSO Armor;
    public BootsEquipmentSO Boots;
    public AccessoryEquipmentSO Accessory_1;
    public AccessoryEquipmentSO Accessory_2;

}
}