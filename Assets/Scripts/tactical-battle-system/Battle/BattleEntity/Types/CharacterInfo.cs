using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using DPS.Common;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;

namespace DPS.TacticalCombat{
[Serializable]
public class CharacterInfo : BattleEntityInfo {
    public CharacterInfo(){}
    //This constructor is useful for setting up the template for a particular character.
    public CharacterInfo(CharacterInfoSO characterInfoSO) {
        this.CharacterInfoSO = characterInfoSO;
        this.Level = new Level(characterInfoSO.Level);
        this.ResolveGaugeSO = characterInfoSO.ResolveGauge;
        GenericDictionary<ElementSO, ElementalResistance> elements = new GenericDictionary<ElementSO, ElementalResistance>();
        foreach(KeyValuePair<ElementSO, ElementalResistance> element in characterInfoSO.Elements) {
            elements.Add(element.Key, element.Value);
        }
        this.Elements = elements;


        this.skillInfo = new SkillInfo(characterInfoSO.SkillInfoSO);

        this.skillPoints = characterInfoSO.SkillPoints;
        this.allocatedSkillPoints = characterInfoSO.AllocatedSkillPoints;
        this.totalSkillPoints = this.Level.Value;

        CharacterEquipment characterEquipment = new CharacterEquipment(characterInfoSO.Equipment);
        
        this.Equipment = characterEquipment;
        this.poise = new Poise(characterInfoSO.Poise);
        this.spritePortrait = characterInfoSO.characterSpritePortrait;
        this.movement = new Movement(characterInfoSO.speed, characterInfoSO.verticalSpeed);
        this.statusAilmentResistances = new StatusAilmentResistances(characterInfoSO.statusAilmentResistances);
        this.terrainMovementOverrides = new GenericDictionary<ElementSO, int>();

        this.isFlying = false;
        this.canBeDisplaced = true;
    }   

    //This constructor is useful when grabbing save data.
    // public CharacterInfo(SerializedCharacterInfo characterInfo) {
    //     this.Level = new Level(characterInfo.Level);
    //     this.CharacterInfoSO = (CharacterInfoSO) Resources.Load(Constants.ScriptableObjects.PartyMembers + characterInfo.characterInfoSO, typeof(CharacterInfoSO));
    //     ResolveGaugeSO resolveGauge = this.CharacterInfoSO.ResolveGauge;
    //     this.resolvePoints = characterInfo.resolvePoints;
    //     this.ResolveGaugeSO = resolveGauge;

    //     GenericDictionary<ElementSO, ElementalResistance> elements = new GenericDictionary<ElementSO, ElementalResistance>();
    //     foreach(KeyValuePair<ElementSO, ElementalResistance> element in characterInfo.Elements) {
    //         elements.Add(element.Key, element.Value);
    //     }
    //     this.Elements = elements;

    //     this.BaseCharacterStats = new CharacterStats(this.CharacterInfoSO.BaseCharacterStats);

    //     this.BaseCharacterStatMultiplier = new CharacterStatModifiers(this.CharacterInfoSO.BaseCharacterStatMultiplier);

    //     this.CharacterStats = new CharacterStats(characterInfo.CharacterStats);

    //     this.BaseRawStats = new RawStats(characterInfo.BaseRawStats);

    //     this.RawStats = new RawStats(characterInfo.RawStats);

    //     this.skillInfo =  characterInfo.skills.DeSerializeSkills();

    //     this.skillPoints = characterInfo.SkillPoints;
    //     this.allocatedSkillPoints = characterInfo.AllocatedSkillPoints;

    //     EquipmentTypes equipmentTypes = new EquipmentTypes();

    //     CharacterEquipment characterEquipment = new CharacterEquipment();
    //     characterEquipment.ArmorType = characterInfo.Equipment.ArmorTypes;
    //     // characterEquipment.EquipmentSlots = characterInfo.Equipment.EquipmentSlots.ToEquipmentSlots();
        
    //     this.Equipment = characterEquipment;
    //     // this.ActionStatus = characterInfo.ActionStatus;
        
    //     this.poise = new Poise(characterInfo.poise);
    //     // this.isAggro = false;
    //     this.spritePortrait = this.CharacterInfoSO.characterSpritePortrait;
    //     this.movement = characterInfo.movement;
    //     this.statusAilmentResistances = new StatusAilmentResistances(characterInfo.statusAilmentResistances);
    //     this.terrainMovementOverrides = new GenericDictionary<ElementSO, int>();
    //     foreach(KeyValuePair<ElementSO, int> elementMovementOverride in characterInfo.elementMovementOverrides) {
    //         this.terrainMovementOverrides.Add(elementMovementOverride);
    //     }
    //     // this.animatorController = this.characterInfoSO.animator;
    //     this.canBeDisplaced = characterInfo.canBeDisplaced;   
    // }
    
    public CharacterInfoSO CharacterInfoSO;

    public Level Level;

    public int resolvePoints = 0;

    // public int maxResolvePoints;

    #nullable enable
    public ResolveGaugeSO? ResolveGaugeSO;

    public CharacterStats BaseCharacterStats;
 
    //Raw Stats after multipliers are applied
    public RawStats BaseRawStats;
    public CharacterStatModifiers BaseCharacterStatMultiplier;

    public CharacterStats CharacterStats;

    [SerializeField]
    private SkillInfo skillInfo;

    private int skillPoints = 0;

    public int SkillPoints { get => this.skillPoints; set => this.skillPoints = value; }

    private int allocatedSkillPoints = 0;

    public int AllocatedSkillPoints { get => this.allocatedSkillPoints; set => this.allocatedSkillPoints = value; }


    private int totalSkillPoints = 0;

    public int TotalSkillPoints { get => this.totalSkillPoints; set => this.totalSkillPoints = value; }

    public CharacterEquipment Equipment;


    /**
        Battle object to be used in combat.
        Gets generated at the beginning of combat.
        Nulled out when combat ends.
        Character Entities have their HP set to whatever HP is remaining from it.
    */

    //Raw Stats used during combat
    #nullable enable
    //Controls whether the avatar is dead or not


    public override SkillInfo GetSkillInfo() {
        return this.skillInfo;
    }

    public void ResetSkills()
    {
        if (this.CharacterInfoSO == null)
        {
            return;
        }

        this.allocatedSkillPoints = 0;
        this.skillPoints = this.totalSkillPoints;
        this.skillInfo = new SkillInfo(this.CharacterInfoSO.SkillInfoSO);
    }

    public CharacterRoleSO? GetCharacterRole()
    {
        if (this.CharacterInfoSO == null)
        {
            return null;
        }


        return this.CharacterInfoSO.RoleSO;
    }


    public async Task GenerateCharacterStatsAndSkills()
    {
        //Create copy of current base stats
        CharacterStats newCharacterstats = new CharacterStats(this.CharacterInfoSO.BaseCharacterStats);

        this.BaseCharacterStats = new(newCharacterstats);

        //Add stats from passives
        newCharacterstats.AddStats(ExecuteAddCharacterStatPassiveSkill());

        //Calculate additional stats based on scaled stats and stat multipliers
        CharacterStatModifiers characterStatModifiers = new(this.CharacterInfoSO.BaseCharacterStatMultiplier);

        characterStatModifiers.AddStatModifiers(ExecuteAddCharacterStatModifierPassiveSkill());

        //Add the additional stats to the new character stats.
        newCharacterstats.AddStats(newCharacterstats.StatMultiplier(characterStatModifiers));

        //Set new character stats;
        this.CharacterStats = new CharacterStats(newCharacterstats);

        //Calculate new base Raw Stats
        RawStats newRawStats = new RawStats(this.CharacterStats, this.CharacterInfoSO.Scale);

        newRawStats.AddStats(ExecuteAddCharacterRawStatPassiveSkill());

        this.BaseRawStats = newRawStats;

        RawStatMultiplier rawStatMultiplier = new(this.CharacterInfoSO.BaseRawStatMultiplier);

        rawStatMultiplier.AddMultpliers(ExecuteAddCharacterRawStatMultiplierPassiveSkill());

        //Add the base raw stat multiplier
        this.BaseRawStats.AddStats(this.BaseRawStats.StatMultiplier(rawStatMultiplier));

        //Activate Learned Passive Abilities

        //Generate the Raw Stats based on Equipment
        this.RawStats = new RawStats(this.BaseRawStats);
        this.RawStats.AddStats(await this.Equipment.AddEquipmentStats());


        if (RawStats.mp != RawStats.maxMp)
        {
            RawStats.mp = RawStats.maxMp;
        }

        if (RawStats.hp != RawStats.maxHp)
        {
            RawStats.hp = RawStats.maxHp;
        }

        StatusAilmentResistances newStatusAilmentResistances = new StatusAilmentResistances();
        newStatusAilmentResistances.AddStatusAilmentResistance(this.Equipment.AddEquipmentStatusAilmentResistances());
        this.statusAilmentResistances = newStatusAilmentResistances;
        this.ExecutePostEquipmentStatAddPassiveSkill(this);
        // PassiveSkillStatic.ActivateLearnedPassiveAbilities(this);
    }

    /**
        Should be called whenever equipment is changed, or character stats need to be regenerated.
    */
    private RawStats ExecuteAddCharacterRawStatPassiveSkill() {
        RawStats bonusStatModifiers = new();
        foreach (StatIncreaseSkillSO passiveSkillSO in this.skillInfo.LearnedStatSkills) {
            bonusStatModifiers.AddStats(passiveSkillSO.ExecuteAddCharacterRawStatPassiveSkill());
        }
        return bonusStatModifiers;
    }
    
    /**
        Should be called whenever equipment is changed, or character stats need to be regenerated.
    */
    private RawStatMultiplier ExecuteAddCharacterRawStatMultiplierPassiveSkill()
    {
        RawStatMultiplier bonusStatModifiers = new();
        foreach (StatIncreaseSkillSO passiveSkillSO in this.skillInfo.LearnedStatSkills)
        {
            bonusStatModifiers.AddMultpliers(passiveSkillSO.ExecuteAddCharacterRawStatMultiplierPassiveSkill());
        }
        return bonusStatModifiers;
    }

    /**
        Should be called whenever equipment is changed, or character stats need to be regenerated.
    */
    public CharacterStatModifiers ExecuteAddCharacterStatModifierPassiveSkill()
    {
        CharacterStatModifiers bonusStatModifiers = new();
        foreach (StatIncreaseSkillSO passiveSkillSO in this.skillInfo.LearnedStatSkills)
        {
            bonusStatModifiers.AddStatModifiers(passiveSkillSO.ExecuteAddCharacterStatModifierPassiveSkill());
        }
        return bonusStatModifiers;
    }


    /**
        Should be called whenever equipment is changed, or character stats need to be regenerated.
    */
    public CharacterStats ExecuteAddCharacterStatPassiveSkill()
    {
        CharacterStats bonusStats = new();
        foreach (StatIncreaseSkillSO passiveSkillSO in this.skillInfo.LearnedStatSkills)
        {
            bonusStats.AddStats(passiveSkillSO.ExecuteAddCharacterStatPassiveSkill());
        }
        return bonusStats;
    }


    /**
        Should be called whenever equipment is changed, or character stats need to be regenerated.
    */
    public void ExecutePostEquipmentStatAddPassiveSkill(CharacterInfo characterInfo) {
        if (this.skillInfo.PassiveSkills != null && this.skillInfo.PassiveSkills.Count > 0) {
            foreach (PassiveSkillSO passiveSkillSO in this.skillInfo.PassiveSkills) {
                passiveSkillSO.ExecutePostEquipmentStatAddPassiveSkill(characterInfo);
            }
        }
    }



    public int GetResolvePoints() {
        return this.resolvePoints;
    }

    public void SetResolvePoints(int resolvePoints) {
        this.resolvePoints = resolvePoints;
    }

    public ResolveGaugeSO? GetResolveGauge() {
        return this.ResolveGaugeSO;
    }

    public override void GenerateBattleRawStats() {
        this.BattleRawStats = new RawStats(this.RawStats);
        this.battleTerrainMovementOverrides = new GenericDictionary<ElementSO, int>();
        foreach(KeyValuePair<ElementSO, int> terrainMovementOverride in this.terrainMovementOverrides) {
            this.battleTerrainMovementOverrides.Add(terrainMovementOverride);
        }
    }


    public override void CombatEndStats() {
        // this.RawStats.hp = this.BattleRawStats.hp;
        Debug.Log("Poise for: " + this.GetName());
        this.BattleRawStats = null;
        // if (this.ActionStatus == ActionStatus.Dead) {
        //     this.RawStats.hp = 1;
        // }
        // this.RawStats.hp = this.RawStats
        // this.RawStats.mp = this.RawStats.maxMp;
        this.StatusEffects = new();
        this.poise.ResetPoiseState();
        this.InflictedAilment = null;
        this.isFlying = false;
        if(this.skillInfo.SwapSkill != null) {
            this.skillInfo.SwapSkill.RemoveSwapSkillCoolDown();
        }
        this.resolvePoints = 0;
    }

    // private CharacterEquipment ReplaceArmorSlot(EquipmentSO newEquip, CharacterEquipment equipment) {
    //     ArmorEquipmentSO? armor = newEquip as ArmorEquipmentSO;
    //     if (armor == null) {
    //         return equipment;
    //     }

    //     if(armor.equipmentType != equipment.ArmorType) {
    //         return equipment;
    //     }

    //     equipment.EquipmentSlots.ReplaceArmorEquippedItem(armor);

    //     return equipment;
    // }

    #nullable enable
    public async Task<EquipmentSO?> EquipGear(EquipmentSO equipment, EquipmentTypeEnum itemSlot)
    {
        if (equipment == null || this.Equipment == null || this.Equipment.EquipmentSlots == null)
        {
            return null;
        }
        EquipmentSO? unEquippedArmor = this.Equipment.EquipmentSlots!.ReplaceEquippedItem(equipment, itemSlot);

        await this.GenerateCharacterStatsAndSkills();
        return unEquippedArmor;
    }

    public async Task<EquipmentSO?> UnEquipGear(EquipmentTypeEnum itemSlot) {

        EquipmentSO? unEquippedItem = this.Equipment.EquipmentSlots.ReplaceEquippedItem(null, itemSlot);
        await this.GenerateCharacterStatsAndSkills();
        return unEquippedItem;
    }
    #nullable disable

    public override string GetName()
    {
        return this.CharacterInfoSO.Name.First;
    }

    public override bool EndPhase(bool regainMP = false) {
        if(base.EndPhase(regainMP)) {
            return true;
        }
        
        SwapSkill swapSkill = this.GetSkillInfo().SwapSkill;
        if (swapSkill != null && swapSkill.SpecialSwapSkill != null) {
            swapSkill.DecreaseSwapSkillCountDown();
        }
        return true;
        // this.isAggro = false;
    }

    // private GenericDictionary<string, StatusEffect> processBuffs() {
    //     GenericDictionary<string, StatusEffect> liveBuffs = new GenericDictionary<string, StatusEffect>();
    //     foreach (KeyValuePair<string, StatusEffect> buffEntry in this.StatusEffects) {
    //         StatusEffect buff = buffEntry.Value;
    //         if(buff.statusEffect.infiniteTurns) {
    //             liveBuffs.Add(buffEntry.Key, buffEntry.Value);
    //             continue;
    //         }
    //         buff.turnCount--;
    //         if(buff.turnCount > 0) {
    //             liveBuffs.Add(buffEntry.Key, buffEntry.Value);
    //         } else {
    //             buff.statusEffect.OnRemoveStatusEffect(this);
    //         }
    //     }
    //     return liveBuffs;
    // }

    // private void removeAllBuffs() {
    //     foreach (KeyValuePair<string, StatusEffect> buffEntry in this.StatusEffects) {
    //         StatusEffect buff = buffEntry.Value;
    //         buff.statusEffect.OnRemoveStatusEffect(this);
    //     }
    //     this.StatusEffects.Clear();
    // }


    #nullable enable

    public override BattleObject? GetBattleObject()
    {
        if (this.CharacterInfoSO == null) {
            return null;
        }
        return this.CharacterInfoSO.battleMemberObjectReference;
    }

    public PartyMemberPlayerFollow? GetOverworldFollowObject()
    {
        if (this.CharacterInfoSO == null)
        {
            return null;
        }

        return this.CharacterInfoSO.overworldFollowNPCObjectReference;
    }
    #nullable disable

    public override List<ActiveSkillSO> GetLearnedActiveSkills()
    {
        return this.GetSkillInfo().GetLearnedActiveSkills();
    }

    public override List<ActiveSkillSO> GetEquippedActiveSkills()
    {
        List<ActiveSkillSO> equippedSkills = new(this.GetSkillInfo().EquippedSkills ?? new());
        foreach (ActiveSkillSO activeSkillSO in this.Equipment.GetEquipmentActiveSkills())
        {
            if (equippedSkills.Contains(activeSkillSO))
            {
                continue;
            }

            equippedSkills.Add(activeSkillSO);
        }

        return equippedSkills.FindAll(skill => skill != null);
    }
}


// [Serializable]
// public class SerializedCharacterInfo {
//     public SerializedCharacterInfo(CharacterInfo characterInfo) {
//         this.characterInfoSO = characterInfo.CharacterInfoSO.name;
//         this.Level = new Level(characterInfo.Level);
//         this.resolvePoints = characterInfo.resolvePoints;

//         this.resolveGauge = characterInfo.ResolveGaugeSO != null ? characterInfo.ResolveGaugeSO.name : "";

//         // List<ElementSO> weakness = new List<ElementSO>();
//         // weakness.AddRange(characterInfo.Weaknesses);
//         // this.Weakness = weakness;

//         // List<ElementSO> resistances = new List<ElementSO>();
//         // resistances.AddRange(characterInfo.Resistances);
//         // this.Resistances = resistances;

//         // List<ElementSO> nullifies = new List<ElementSO>();
//         // nullifies.AddRange(characterInfo.Nullifies);
//         // this.Nullifies = nullifies;

//         // List<ElementSO> absorbs = new List<ElementSO>();
//         // absorbs.AddRange(characterInfo.Absorbs);
//         // this.Absorbs = absorbs;

//         GenericDictionary<ElementSO, ElementalResistance> elements = new GenericDictionary<ElementSO, ElementalResistance>();
//         foreach(KeyValuePair<ElementSO, ElementalResistance> element in characterInfo.Elements) {
//             elements.Add(element.Key, element.Value);
//         }
//         this.Elements = elements;

//         this.CharacterStats = new CharacterStats(characterInfo.CharacterStats);

//         this.BaseRawStats = new RawStats(characterInfo.BaseRawStats);

//         this.RawStats = new RawStats(characterInfo.RawStats);

//         // this.skills = new SerializableSkillInfo(characterInfo.GetSkillInfo());

//         this.SkillPoints = characterInfo.SkillPoints;
//         this.AllocatedSkillPoints = characterInfo.AllocatedSkillPoints;

//         EquipmentTypes equipmentTypes = new EquipmentTypes();

//         // equipmentTypes.MainHand = characterInfo.Equipment.EquipmentTypes.MainHand;
//         // equipmentTypes.OffHand = characterInfo.Equipment.EquipmentTypes.OffHand;

//         CharacterEquipment characterEquipment = new CharacterEquipment();
//         characterEquipment.ArmorType = characterInfo.Equipment.ArmorType;
//         characterEquipment.EquipmentSlots = characterInfo.Equipment.EquipmentSlots;
        
//         this.Equipment = new SerializedCharacterEquipment(characterEquipment);        
//         this.poise = new Poise(characterInfo.poise);
//         // this.isAggro = false;
//         this.movement = characterInfo.movement;
//         this.statusAilmentResistances = new StatusAilmentResistances(characterInfo.GetStatusAilmentResistances());
//         this.elementMovementOverrides = new GenericDictionary<ElementSO, int>();
//         foreach(KeyValuePair<ElementSO, int> elementMovementOverride in characterInfo.GetTerrainMovementOverride()) {
//             this.elementMovementOverrides[elementMovementOverride.Key] = elementMovementOverride.Value;
//         }
//         this.isFlying = characterInfo.CanFly();
//         this.canBeDisplaced = characterInfo.CanBeDisplaced();
//     }

//     public String characterInfoSO;

//     public Level Level;

//     public string resolveGauge;

//     public int resolvePoints;

//     // public List<ElementSO> Weakness;

//     // public List<ElementSO> Resistances;

//     // public List<ElementSO> Nullifies;

//     // public List<ElementSO> Absorbs;

//     public GenericDictionary<ElementSO, ElementalResistance> Elements;
    

//     public CharacterStats CharacterStats;


//     // public SerializableSkillInfo skills;

//     public int SkillPoints;

//     public int AllocatedSkillPoints;

//     public Poise poise;

//     public SerializedCharacterEquipment Equipment;
//     /**
//         Battle object to be used in combat.
//         Gets generated at the beginning of combat.
//         Nulled out when combat ends.
//         Character Entities have their HP set to whatever HP is remaining from it.
//     */

//     public RawStats BaseRawStats;
//     //Raw Stats Multipliers applied to Base Raw Stats
//     public RawStats RawStats;
//     //Raw Stats used during combat
//     //Controls whether the avatar is dead or not
//     // public ActionStatus ActionStatus;
//     public List<StatusEffect> Buffs = new List<StatusEffect>();

//     public Movement movement;

//     public StatusAilmentResistances statusAilmentResistances;

//     public GenericDictionary<ElementSO, int> elementMovementOverrides;

//     public bool isFlying;

//     public bool canBeDisplaced;

// }

// [Serializable]
// public class JobGauge {
//     public JobGauge() {}

//     public JobGauge(JobGauge jobGauge) {
//         this.name = jobGauge.name;
//         this.level = jobGauge.level;
//         this.currentPoints = jobGauge.currentPoints;
//         this.maxPoints = jobGauge.maxPoints;
//     }

//     public JobGauge(string name, int currentPoints, Level level, int exp, int maxPoints) {
//         this.name = name;
//         this.level = level;
//         this.currentPoints = currentPoints;
//         this.maxPoints = maxPoints;
//     }
//     public string name;
//     public Level level;
//     public int currentPoints;
//     public int maxPoints;
// }

[Serializable]
public class Name {
    public Name(Name name) {
        this.First = name.First;
        this.Last = name.Last;
    }
    public string First;
    public string Last;
}

[Serializable]
public class Level {
    public Level(int value, int exp) {
        this.Value = value;
        this.Exp = exp;
    }

    public Level(Level level) {
        this.Value = level.Value;
        this.Exp = level.Exp;
    }
    public int Value = 1;
    public int Exp = 0;
}


[Serializable]
public class CharacterEquipment {

    public CharacterEquipment() {
        this.EquipmentSlots = new EquipmentSlots();
    }
    public CharacterEquipment(CharacterEquipment characterEquipment) {
        this.EquipmentSlots = new(characterEquipment.EquipmentSlots);
    }

    public CharacterEquipment(EquipmentSlotsSO equipmentSlots) {
        this.EquipmentSlots = new EquipmentSlots(equipmentSlots);
    }

    #nullable enable
    public EquipmentSlots EquipmentSlots;

    
    public async Task<RawStats> AddEquipmentStats() {
        EquipmentSlots equipmentSlots = this.EquipmentSlots;
        if (equipmentSlots == null) {
            return new RawStats();
        }

        RawStats equipmentRawStats = new();
        List<AssetReference> equipmentAssets = new();
        if(equipmentSlots.WeaponRune != null && equipmentSlots.WeaponRune.RuntimeKeyIsValid() )
            {
                Debug.Log($"Asset: {equipmentSlots.WeaponRune}");
                equipmentAssets.Add(equipmentSlots.WeaponRune);
            }

            // equipmentRawStats.AddStats(equipmentSlots.WeaponRune.equipmentInfo.rawStats);

        if(equipmentSlots.Helm != null)
            equipmentRawStats.AddStats(equipmentSlots.Helm.equipmentInfo.rawStats);

        if(equipmentSlots.Body != null)
            equipmentRawStats.AddStats(equipmentSlots.Body.equipmentInfo.rawStats);

        if(equipmentSlots.Boots != null)
            equipmentRawStats.AddStats(equipmentSlots.Boots.equipmentInfo.rawStats);

        if(equipmentSlots.Accessory_1 != null) 
            equipmentRawStats.AddStats(equipmentSlots.Accessory_1.equipmentInfo.rawStats);

        if(equipmentSlots.Accessory_2 != null) 
            equipmentRawStats.AddStats(equipmentSlots.Accessory_2.equipmentInfo.rawStats);

        List<EquipmentSO>? equipmentList = await GeneralUtilsStatic.GetScriptableObjectAssetReference<EquipmentSO>(equipmentAssets) as List<EquipmentSO>;
        if (equipmentList == null)
        {
            return equipmentRawStats;

        }

        foreach(EquipmentSO equipment in equipmentList)
        {
            equipmentRawStats.AddStats(equipment.equipmentInfo.rawStats);
        }

        return equipmentRawStats;

    }

    public StatusAilmentResistances AddEquipmentStatusAilmentResistances() {
        StatusAilmentResistances statusAilmentResistances = new StatusAilmentResistances();
        EquipmentSlots equipmentSlots = this.EquipmentSlots;
        if (equipmentSlots != null) {
            if(equipmentSlots.Helm != null && equipmentSlots.Helm.equipmentInfo != null)
                statusAilmentResistances.AddStatusAilmentResistance(equipmentSlots.Helm.equipmentInfo.statusAilmentResistances);

            if(equipmentSlots.Body != null && equipmentSlots.Body.equipmentInfo != null)
                statusAilmentResistances.AddStatusAilmentResistance(equipmentSlots.Body.equipmentInfo.statusAilmentResistances);

            if(equipmentSlots.Boots != null && equipmentSlots.Boots.equipmentInfo != null)
                statusAilmentResistances.AddStatusAilmentResistance(equipmentSlots.Boots.equipmentInfo.statusAilmentResistances);

            if(equipmentSlots.Accessory_1 != null && equipmentSlots.Accessory_1.equipmentInfo != null) 
                statusAilmentResistances.AddStatusAilmentResistance(equipmentSlots.Accessory_1.equipmentInfo.statusAilmentResistances);

            if(equipmentSlots.Accessory_2 != null && equipmentSlots.Accessory_2.equipmentInfo != null) 
                statusAilmentResistances.AddStatusAilmentResistance(equipmentSlots.Accessory_2.equipmentInfo.statusAilmentResistances);
        }



        return statusAilmentResistances;
    }  

    public List<ActiveSkillSO> GetEquipmentActiveSkills() {
        List<ActiveSkillSO> itemActiveSkills = new List<ActiveSkillSO>();

        Action<ActiveSkillSO> addActiveSkill = (ActiveSkillSO activeSkillSO) => {
            if (!itemActiveSkills.Contains(activeSkillSO)) {
                itemActiveSkills.Add(activeSkillSO);
            }
        };

        EquipmentSlots equipmentSlots = this.EquipmentSlots;
        if (equipmentSlots == null) {
            return itemActiveSkills;
        }

        if(equipmentSlots.Helm != null && equipmentSlots.Helm.equipmentInfo != null && equipmentSlots.Helm.equipmentInfo.activeSkillSO != null)
            addActiveSkill(equipmentSlots.Helm.equipmentInfo.activeSkillSO);

        if(equipmentSlots.Body != null && equipmentSlots.Body.equipmentInfo != null && equipmentSlots.Body.equipmentInfo.activeSkillSO != null)
            addActiveSkill(equipmentSlots.Body.equipmentInfo.activeSkillSO);

        if(equipmentSlots.Boots != null && equipmentSlots.Boots.equipmentInfo != null && equipmentSlots.Boots.equipmentInfo.activeSkillSO != null)
            addActiveSkill(equipmentSlots.Boots.equipmentInfo.activeSkillSO);

        if(equipmentSlots.Accessory_1 != null && equipmentSlots.Accessory_1.equipmentInfo != null && equipmentSlots.Accessory_1.equipmentInfo.activeSkillSO != null)
            addActiveSkill(equipmentSlots.Accessory_1.equipmentInfo.activeSkillSO);
        if(equipmentSlots.Accessory_2 != null && equipmentSlots.Accessory_2.equipmentInfo != null && equipmentSlots.Accessory_2.equipmentInfo.activeSkillSO != null)
            addActiveSkill(equipmentSlots.Accessory_2.equipmentInfo.activeSkillSO);



        return itemActiveSkills;
    } 
}

[Serializable]
public class SerializedCharacterEquipment {
    public SerializedCharacterEquipment() {
        this._equipmentSlots = new();
    }
    public SerializedCharacterEquipment(CharacterEquipment characterEquipment) {
        this._equipmentSlots = characterEquipment != null && characterEquipment.EquipmentSlots != null ? new SerializedCharacterEquipmentSlots(characterEquipment.EquipmentSlots) : new();
    }


    #nullable enable

    private SerializedCharacterEquipmentSlots _equipmentSlots;
    public SerializedCharacterEquipmentSlots EquipmentSlots { get => this._equipmentSlots; }
}

[Serializable]
public class SerializedCharacterEquipmentSlots {
    public SerializedCharacterEquipmentSlots(EquipmentSlots equipmentSlots) {
        // this.MainHand = "";
        // this.OffHand = "";
        this.WeaponRune = "";
        this.Helm = "";
        this.Armor = "";
        this.Boots = "";
        this.Accessory_1 = "";
        this.Accessory_2 = "";

        if(equipmentSlots.WeaponRune != null) {
            this.WeaponRune = equipmentSlots.WeaponRune.AssetGUID;
        }

        // if(equipmentSlotsSO.OffHand != null) {
        //     this.OffHand = equipmentSlotsSO.OffHand.name;
        // }

        if(equipmentSlots.Helm != null) {
            this.Helm = equipmentSlots.Helm.name;
        }

        if(equipmentSlots.Body != null) {
            this.Armor = equipmentSlots.Body.name;
        }
                
        if(equipmentSlots.Boots != null) {
            this.Boots = equipmentSlots.Boots.name;
        }

        if(equipmentSlots.Accessory_1 != null) {
            this.Accessory_1 = equipmentSlots.Accessory_1.name;
        }
        if(equipmentSlots.Accessory_2 != null) {
            this.Accessory_2 = equipmentSlots.Accessory_2.name;
        }
        
    }

    public SerializedCharacterEquipmentSlots ()
    {
        this.WeaponRune = "";
        this.Helm = "";
        this.Armor = "";
        this.Boots = "";
        this.Accessory_1 = "";
        this.Accessory_2 = "";   
    }

    public string WeaponRune;
    // public string OffHand;
    public string Helm;
    public string Armor;
    public string Boots;
    public string Accessory_1;
    public string Accessory_2;

    // public EquipmentSlots ToEquipmentSlots() {
    //     EquipmentSlots equipmentSlots = new EquipmentSlots();
    //     // EquipmentSO mainHand = (EquipmentSO) Resources.Load(StringConstants.ScriptableObjects.EquipmentItems + MainHand, typeof(EquipmentSO));
    //     //     if(mainHand != null)
    //     //         equipmentSlotsSO.MainHand = (mainHand);

    //     // EquipmentSO offHand = (EquipmentSO) Resources.Load(StringConstants.ScriptableObjects.EquipmentItems + OffHand, typeof(EquipmentSO));
    //     //     if(offHand != null)
    //     //         equipmentSlotsSO.OffHand = (offHand);

                
    //     HelmEquipmentSO helm = (HelmEquipmentSO) Resources.Load(Constants.ScriptableObjects.EquipmentItems + "Armor/" + Helm, typeof(HelmEquipmentSO));
    //         if(helm != null)
    //             equipmentSlots.Helm = (helm);

                
    //     BodyEquipmentSO armor = (BodyEquipmentSO) Resources.Load(Constants.ScriptableObjects.EquipmentItems + "Armor/" + Armor, typeof(BodyEquipmentSO));
    //         if(armor != null)
    //             equipmentSlots.Body = (armor);

    //     BootsEquipmentSO boots = (BootsEquipmentSO) Resources.Load(Constants.ScriptableObjects.EquipmentItems + "Armor/" + Boots, typeof(BootsEquipmentSO));
    //         if(boots != null)
    //             equipmentSlots.Boots = (boots);

    //     AccessoryEquipmentSO accessory_1 = (AccessoryEquipmentSO) Resources.Load(Constants.ScriptableObjects.EquipmentItems + "Accessory/" + Accessory_1, typeof(AccessoryEquipmentSO));
    //         if(accessory_1 != null)
    //             equipmentSlots.Accessory_1 = accessory_1;

    //     AccessoryEquipmentSO accessory_2 = (AccessoryEquipmentSO) Resources.Load(Constants.ScriptableObjects.EquipmentItems + "Accessory/" + Accessory_2, typeof(AccessoryEquipmentSO));
    //         if(accessory_2 != null)
    //             equipmentSlots.Accessory_2 = accessory_2;

    //     return equipmentSlots;
    // }
}


[Serializable]

public class EquipmentSlots {
    // public EquipmentSlots() {

    // }
    public EquipmentSlots() {
        this._weaponRune = null;
        this._body = null;
        this._helm = null;
        this._boots = null;
        this._accessory_1 = null;
        this._accessory_2 = null;
    }

    public List<ActiveSkillSO> GetEquipmentActiveSkills()
    {
        List<ActiveSkillSO> equipmentSkills = new();
        if (this._body != null && this._body.equipmentInfo.activeSkillSO != null)
        {
            
        }
        return equipmentSkills;
    }
    
    public EquipmentSlots(EquipmentSlotsSO equipmentSlotsSO)
    {
        // this.MainHand = equipmentSlotsSO.MainHand;
        // this.OffHand = equipmentSlotsSO.OffHand;
        this._weaponRune = equipmentSlotsSO.WeaponRune;
        this._body = equipmentSlotsSO.Armor;
        this._helm = equipmentSlotsSO.Helm;
        this._boots = equipmentSlotsSO.Boots;
        this._accessory_1 = equipmentSlotsSO.Accessory_1;
        this._accessory_2 = equipmentSlotsSO.Accessory_2;
    }

    public EquipmentSlots(EquipmentSlots equipmentSlots) {
        this._weaponRune = equipmentSlots._weaponRune;
        // this.OffHand = equipmentSlotsSO.OffHand;
        this._body = equipmentSlots.Body;
        this._helm = equipmentSlots.Helm;
        this._boots = equipmentSlots.Boots;
        this._accessory_1 = equipmentSlots.Accessory_1;
        this._accessory_2 = equipmentSlots.Accessory_2;
    }

    public AssetReferenceT<WeaponRuneEquipmentSO>? _weaponRune;
    public AssetReferenceT<WeaponRuneEquipmentSO>? WeaponRune { get => this._weaponRune; set => this._weaponRune = value; }

    // public EquipmentSO? OffHand;
    #nullable enable
    private HelmEquipmentSO? _helm;

    public HelmEquipmentSO? Helm { get => this._helm; set => this._helm = value; }

    private BodyEquipmentSO? _body;

    public BodyEquipmentSO? Body { get => this._body; set => this._body = value;}

    private BootsEquipmentSO? _boots;

    public BootsEquipmentSO? Boots { get => this._boots; set => this._boots = value;}

    #nullable disable
    private AccessoryEquipmentSO _accessory_1;
    public AccessoryEquipmentSO Accessory_1 { get => this._accessory_1; set => this._accessory_1 = value; }

    private AccessoryEquipmentSO _accessory_2;
    public AccessoryEquipmentSO Accessory_2 { get => this._accessory_2; set => this._accessory_2 = value; }

    #nullable enable
    public EquipmentSO? ReplaceEquippedItem(EquipmentSO? equipmentSO, EquipmentTypeEnum equipmentType)
    {

        //Determine item to replace
        switch (equipmentType)
        {
            case EquipmentTypeEnum.Helmet:
                return this.ReplaceEquippedHelm(equipmentSO as HelmEquipmentSO);
            case EquipmentTypeEnum.Armor:
                return this.ReplaceEquippedArmor(equipmentSO as BodyEquipmentSO);
            case EquipmentTypeEnum.Boots:
                return this.ReplaceEquippedBoots(equipmentSO as BootsEquipmentSO);
            case EquipmentTypeEnum.Accessory_1:
                return this.ReplaceEquippedAccessory_1(equipmentSO as AccessoryEquipmentSO);
            case EquipmentTypeEnum.Accessory_2:
                return this.ReplaceEquippedAccessory_2(equipmentSO as AccessoryEquipmentSO);
        }

        return null;
    }
    #nullable disable

    public enum AccessorySlot
    {
        Slot_1,
        Slot_2
    }

    #nullable enable

    private HelmEquipmentSO? ReplaceEquippedHelm(HelmEquipmentSO? newHelm) {
        HelmEquipmentSO? unEquippedItem = null;
        if (this._helm != null) {
            unEquippedItem = this._helm as HelmEquipmentSO;
        }

        this._helm = newHelm;

        return unEquippedItem;
    }

    private BodyEquipmentSO? ReplaceEquippedArmor(BodyEquipmentSO? newArmor) {
        BodyEquipmentSO? unEquippedItem = null;
        if (this._body != null) {
            unEquippedItem = this._body as BodyEquipmentSO;
        }

        this._body = newArmor;

        return unEquippedItem;
    }

    private BootsEquipmentSO? ReplaceEquippedBoots(BootsEquipmentSO? newBoots) {
        BootsEquipmentSO? unEquippedItem = null;
        if (this._boots != null) {
            unEquippedItem = this._boots as BootsEquipmentSO;
        }

        this._boots = newBoots;
        return unEquippedItem;
    }

    private AccessoryEquipmentSO? ReplaceEquippedAccessory_1(AccessoryEquipmentSO? accessory) {
        AccessoryEquipmentSO? unEquippedItem = null;
        if (this._accessory_1 != null) {
            unEquippedItem = this._accessory_1 as AccessoryEquipmentSO;
        }

        this._accessory_1 = accessory;
        return unEquippedItem;
    }

    private AccessoryEquipmentSO? ReplaceEquippedAccessory_2(AccessoryEquipmentSO? accessory) {
        AccessoryEquipmentSO? unEquippedItem = null;
        if (this._accessory_2 != null) {
            unEquippedItem = this._accessory_2 as AccessoryEquipmentSO;
        }

        this._accessory_2 = accessory;
        return unEquippedItem;
    }
    
    #nullable disable
}




[Serializable]
#nullable enable
public class SwapSkill {
        [SerializeField]
        private int _currentSkillCoolDown;

        public int CurrentCoolDown { get => this._currentSkillCoolDown; }
        

        [SerializeField]
        private SwapSkillSO _defaultSwapSkill;

        public SwapSkillSO DefaultSwapSkill { get => this._defaultSwapSkill; }

        [SerializeField]
        private SwapSkillSO? _specialSwapSkill;

        public SwapSkillSO? SpecialSwapSkill { get => this._specialSwapSkill; set => this._specialSwapSkill = value; }

        public SwapSkill(SwapSkillSO defaultSwapSkill)
        {
            this._defaultSwapSkill = defaultSwapSkill;
            this._specialSwapSkill = null;
            this._currentSkillCoolDown = 0;
        }

        public SwapSkill(SwapSkillSO defaultSwapSkill, SwapSkillSO? specialSwapSkill) {
            this._defaultSwapSkill = defaultSwapSkill;
            this._specialSwapSkill = specialSwapSkill;
            this._currentSkillCoolDown = 0;
        }

        public bool CanActivateSwapSkill() {
            Debug.Log("Current Swap Skill CoolDown: " + this._currentSkillCoolDown);
            return this.SpecialSwapSkill != null && this._currentSkillCoolDown == 0;
        }

        public SwapSkillSO? GetSpecialSwapSkill() {
            return this.SpecialSwapSkill;
        }

        public void DecreaseSwapSkillCountDown() {
            if (this._currentSkillCoolDown > 0) {
                this._currentSkillCoolDown = ((this._currentSkillCoolDown - 1) + (int) Math.Abs(this._currentSkillCoolDown - 1)) / 2; 
            }
        }

        public void SetSwapSkillCoolDown() {

            if (this.SpecialSwapSkill == null)
            {
                return;
            }
            this._currentSkillCoolDown = this.SpecialSwapSkill.CoolDown;
        }

        public void RemoveSwapSkillCoolDown() {
            this._currentSkillCoolDown = 0;
        }
}
}
#nullable disable




// [Serializable]
// #nullable enable
// public class SerializedSwapSkill {
//     public string defaultSwapSkillId;
//     public string? specialSwapSkillId;

//     public SerializedSwapSkill(SwapSkill swapSkill) {
//         this.defaultSwapSkillId = swapSkill.DefaultSwapSkill.name;
//         this.specialSwapSkillId = swapSkill.SpecialSwapSkill != null ? swapSkill.SpecialSwapSkill.name : null;
//     }

//     public SwapSkill DeSerializeSwapSkill() {
//         SwapSkillSO defaultSwapSkill = (SwapSkillSO) Resources.Load(Constants.ScriptableObjects.ActiveSkills + this.defaultSwapSkillId, typeof(ActiveSkillSO));
//         SwapSkillSO? specialSwapSkill = (SwapSkillSO) Resources.Load(Constants.ScriptableObjects.ActiveSkills + this.specialSwapSkillId, typeof(ActiveSkillSO));
//         return new SwapSkill(defaultSwapSkill, specialSwapSkill);
//     }
// }
// #nullable disable

