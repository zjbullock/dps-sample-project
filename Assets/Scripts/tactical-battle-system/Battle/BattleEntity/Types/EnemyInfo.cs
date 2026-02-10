using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DPS.Common;

namespace DPS.TacticalCombat {
public class EnemyInfo: BattleEntityInfo   {
    public EnemyInfo(EnemyInfoSO enemyInfoSO) {
        this.Name = enemyInfoSO.EnemyName;
        this.Description = enemyInfoSO.Description;
        this.RawStats = new RawStats(enemyInfoSO.BaseRawStats);
        this.BattleRawStats = new RawStats(this.RawStats);
        this.StatusEffects = new();
        
        this.enemyAIs = new List<IEnemyAI>();
        foreach(IEnemyAI enemyAI in enemyInfoSO.EnemyAIList) {
            this.enemyAIs.Add(enemyAI);
        }

        GenericDictionary<ElementSO, ElementalResistance> elements = new GenericDictionary<ElementSO, ElementalResistance>();
        foreach(KeyValuePair<ElementSO, ElementalResistance> element in enemyInfoSO.Elements) {
            elements.Add(element.Key, element.Value);
        }
        this.Elements = elements;

        // this.animatorController = null;
        this.poise = new Poise(enemyInfoSO.poise);
        this.attemptedActiveSkills = new List<IBattleActionCommand>();
        // this.enemyType = enemyInfoSO.enemyType;
        this.letterDesignation = "";
        // this.isAggro = false;
        this.spritePortrait = enemyInfoSO.enemySpritePortrait;
        this.movement = new Movement(enemyInfoSO.speed, enemyInfoSO.verticalSpeed);

        this.noActionAI = enemyInfoSO.defendAI;
        this.basicAttackAI = enemyInfoSO.basicAttackAI;
        this.statusAilmentResistances = new StatusAilmentResistances(enemyInfoSO.statusAilmentResistances);
        this.terrainMovementOverrides = new GenericDictionary<ElementSO, int>();
        foreach(KeyValuePair<ElementSO, int> terrainMovementOverride in enemyInfoSO.terrainMovementOverride) {
            this.terrainMovementOverrides[terrainMovementOverride.Key] = terrainMovementOverride.Value;
        }
        this.isFlying = false;

        List<PassiveSkillSO> passiveSkills = new List<PassiveSkillSO>();
        foreach(PassiveSkillSO passiveSkill in enemyInfoSO.PassiveSkills) {
            passiveSkills.Add(passiveSkill);
        }

        this.passiveSkills = passiveSkills;
        this.dropChances = enemyInfoSO.dropChances;
        this.gp = enemyInfoSO.gp;
        this.canBeDisplaced = enemyInfoSO.canBeDisplaced;
        this.enemyGameObjectReference = enemyInfoSO.enemyBattleObject;
    }

    public EnemyInfo(PrefabEnemyBattleObject prefabEnemyBattleObject) {

        this.isFlying = false;
        this.letterDesignation = "";

        EnemyInfoSO enemyInfoSO = prefabEnemyBattleObject.EnemyInfoSO;


        this.StatusEffects = new();


        this.Name = enemyInfoSO.EnemyName;
        this.Description = enemyInfoSO.Description;

        this.enemyAIs = new List<IEnemyAI>();
        foreach(IEnemyAI enemyAI in enemyInfoSO.EnemyAIList) {
            this.enemyAIs.Add(enemyAI);
        }

        this.RawStats = new RawStats(enemyInfoSO.BaseRawStats);
        this.BattleRawStats = new RawStats(this.RawStats);

        GenericDictionary<ElementSO, ElementalResistance> elements = new GenericDictionary<ElementSO, ElementalResistance>();
        foreach(KeyValuePair<ElementSO, ElementalResistance> element in enemyInfoSO.Elements) {
            elements.Add(element.Key, element.Value);
        }
        this.Elements = elements;

        // this.animatorController = null;
        this.poise = new Poise(enemyInfoSO.poise);
        this.attemptedActiveSkills = new List<IBattleActionCommand>();
        // this.enemyType = enemyInfoSO.enemyType;
        // this.isAggro = false;
        this.spritePortrait = enemyInfoSO.enemySpritePortrait;
        this.movement = new Movement(enemyInfoSO.speed, enemyInfoSO.verticalSpeed);

        this.noActionAI = enemyInfoSO.defendAI;
        this.basicAttackAI = enemyInfoSO.basicAttackAI;
        this.statusAilmentResistances = new StatusAilmentResistances(enemyInfoSO.statusAilmentResistances);
        this.terrainMovementOverrides = new GenericDictionary<ElementSO, int>();
        foreach(KeyValuePair<ElementSO, int> terrainMovementOverride in enemyInfoSO.terrainMovementOverride) {
            this.terrainMovementOverrides[terrainMovementOverride.Key] = terrainMovementOverride.Value;
        }

        List<PassiveSkillSO> passiveSkills = new List<PassiveSkillSO>();
        foreach(PassiveSkillSO passiveSkill in enemyInfoSO.PassiveSkills) {
            passiveSkills.Add(passiveSkill);
        }

        this.passiveSkills = passiveSkills;
        this.dropChances = enemyInfoSO.dropChances;
        this.gp = enemyInfoSO.gp;
        this.canBeDisplaced = enemyInfoSO.canBeDisplaced;
        this.enemyGameObjectReference = enemyInfoSO.enemyBattleObject;


    }

    

    public string Name;

    public string letterDesignation;

    public string Description;

    public List<IEnemyAI> enemyAIs;

    public List<IBattleActionCommand> attemptedActiveSkills;

    // public EnemyTypeEnums enemyType;

    public DefendAISO noActionAI;

    public EnemyAISO basicAttackAI;

    private List<DropChance> dropChances;

    private uint gp;    
    
    [SerializeField]
    public List<PassiveSkillSO> passiveSkills;

    public EnemyBattleObject enemyGameObjectReference;

    // public bool isAggro;

    public string GetLetterDesignation() {
        return this.letterDesignation;
    }

    // public void SetAggro(bool isAggro) {
    //     this.isAggro = isAggro;
    // }

    // public bool GetAggro() {
    //     return this.isAggro;
    // }

    public void AddLastUseActiveSkill(IBattleActionCommand activeSkill) {
        this.attemptedActiveSkills.Add(activeSkill);
    }


    public override string GetName() {
        return String.Format("{0} {1}", this.Name, this.GetLetterDesignation());
    }
    /**
        Should only be called after equipment is changed.
    */
    public void ActivateEquipmentChangeAbilities(RawStats equipmentStats) {

    }

    public override bool IsDead() {
        return this.BattleRawStats.hp <= 0;
    }

    #nullable enable
    public DroppedItems GetDroppedItems(System.Random random, int additionalDropChance = 0, float multiplyDropChance = 1f) {
        List<InventoryItemSO> obtainedItems = new List<InventoryItemSO>();
        foreach(DropChance droppedItemChance in this.dropChances) {
            InventoryItemSO? droppedItem = droppedItemChance.RollInventoryItemChance(random, additionalDropChance, multiplyDropChance);
            if (droppedItem != null) {
                obtainedItems.Add(droppedItem);
            }
        }
        return new DroppedItems(this.gp, obtainedItems);
    }

    public override BattleObject GetBattleObject()
    {
        return this.enemyGameObjectReference;
    }

    public override List<ActiveSkillSO> GetLearnedActiveSkills()
    {
        return new();
    }

    public override List<ActiveSkillSO> GetEquippedActiveSkills()
    {
        return new();
    }
}

public struct DroppedItems {
    public uint gp;
    public List<InventoryItemSO> inventoryItems;

    public DroppedItems(uint gp, List<InventoryItemSO> inventoryItemSOs) {
        this.gp = gp;
        this.inventoryItems = inventoryItemSOs;
    }
}
}