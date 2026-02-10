using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RawStatMultiplier {

    public RawStatMultiplier()
    {

    }

    public RawStatMultiplier(RawStatMultiplier rawStatMultiplier)
    {
        this.hp = rawStatMultiplier.hp;
        this.mp = rawStatMultiplier.mp;
        this.maxHp = rawStatMultiplier.maxHp;
        this.maxMp = rawStatMultiplier.maxMp;
        this.physicalAttack = rawStatMultiplier.physicalAttack;
        this.magicalAttack = rawStatMultiplier.magicalAttack;
        this.defense = rawStatMultiplier.defense;
        this.magicalResistance = rawStatMultiplier.magicalResistance;
        this.criticalRate = rawStatMultiplier.criticalRate;
        this.criticalDamage = rawStatMultiplier.criticalDamage;
        this.accuracy = rawStatMultiplier.accuracy;
        this.initiative = rawStatMultiplier.initiative;
        this.evasion = rawStatMultiplier.evasion;
        this.gpGainRate = rawStatMultiplier.gpGainRate;
        this.itemDropRate = rawStatMultiplier.itemDropRate;
        this.blockChance = rawStatMultiplier.blockChance;
        this.shields = rawStatMultiplier.shields;
        this.healProficiency = rawStatMultiplier.healProficiency;
    }

    public void AddMultpliers(RawStatMultiplier rawStatMultiplier)
    {
        this.hp += rawStatMultiplier.hp;
        this.mp += rawStatMultiplier.mp;
        this.maxHp += rawStatMultiplier.maxHp;
        this.maxMp += rawStatMultiplier.maxMp;
        this.physicalAttack += rawStatMultiplier.physicalAttack;
        this.magicalAttack += rawStatMultiplier.magicalAttack;
        this.defense += rawStatMultiplier.defense;
        this.magicalResistance += rawStatMultiplier.magicalResistance;
        this.criticalRate += rawStatMultiplier.criticalRate;
        this.criticalDamage += rawStatMultiplier.criticalDamage;
        this.accuracy += rawStatMultiplier.accuracy;
        this.initiative += rawStatMultiplier.initiative;
        this.evasion += rawStatMultiplier.evasion;
        this.gpGainRate += rawStatMultiplier.gpGainRate;
        this.itemDropRate += rawStatMultiplier.itemDropRate;
        this.blockChance += rawStatMultiplier.blockChance;
        this.shields += rawStatMultiplier.shields;
        this.healProficiency += rawStatMultiplier.healProficiency;   
    }

    [Range(-10f, 10f)]
    public float hp = 0f;

    [Range(-10f, 10f)]
    public float mp = 0f;

    [Range(-10f, 10f)]
    public float maxHp = 0f;   //Nullable

    [Range(-10f, 10f)]
    public float maxMp = 0f;   //Nullable

    [Range(-10f, 10f)]
    public float physicalAttack = 0f;

    [Range(-10f, 10f)]
    public float magicalAttack = 0f;

    [Range(-10f, 10f)]
    public float defense = 0f;

    [Range(-10f, 10f)]
    public float magicalResistance = 0f;

    [Range(-10f, 10f)]
    public float criticalRate = 0f;

    [Range(-10f, 10f)]
    public float criticalDamage = 0f;

    [Range(-10f, 10f)]
    public float accuracy = 0f;

    [Range(-10f, 10f)]
    public float initiative = 0f;

    [Range(-10f, 10f)]
    public float evasion = 0f;

    [Range(-10f, 10f)]
    public float gpGainRate = 0f; 

    [Range(-10f, 10f)]
    public float itemDropRate = 0f;

    [Range(-10f, 10f)]
    public float blockChance = 0f;

    [Range(-10f, 10f)]
    public float shields = 0f;

    [Range(-10f, 10f)]
    public float healProficiency = 0f;

    [Range(-10f, 10f)]
    public float castTimeReduction = 0f;
}


[Serializable]
public class RawStats {

    public RawStats() {
        this.hp = 0;
        this.mp = 0;
        this.maxHp = 0;
        this.maxMp = 0;
        this.physicalAttack = 0;
        this.magicalAttack = 0;
        this.defense = 0;
        this.magicalResistance = 0;
        this.criticalDamage = 0;
        this.criticalRate = 0;
        this.accuracy = 0;
        this.initiative = 0;
        this.evasion = 0;
        this.gpGainRate = 0;
        this.itemDropRate = 0;
        this.blockChance = 0;
        this.shields = 0;
        this.healProficiency = 0;
        this.castTimeReduction = 0;
    }

    public RawStats(RawStats rawStats) {
        if (rawStats != null) {
            this.hp = rawStats.hp;
            this.maxHp = rawStats.maxHp;
            this.mp = rawStats.mp;
            this.maxMp = rawStats.maxMp;
            this.physicalAttack = rawStats.physicalAttack;
            this.magicalAttack = rawStats.magicalAttack;
            this.defense = rawStats.defense;
            this.magicalResistance = rawStats.magicalResistance;
            this.criticalRate = rawStats.criticalRate;
            this.criticalDamage = rawStats.criticalDamage;
            this.accuracy = rawStats.accuracy;
            this.initiative = rawStats.initiative;
            this.evasion = rawStats.evasion;
            this.gpGainRate = rawStats.gpGainRate;
            this.itemDropRate = rawStats.itemDropRate;
            this.blockChance = rawStats.blockChance;
            this.shields = rawStats.shields;
            this.healProficiency = rawStats.healProficiency;
            this.castTimeReduction = rawStats.castTimeReduction;
        }

    }

    //This constructor is generally called when a character has leveled up, thus their hp returns to max
    public RawStats(CharacterStats characterStats, float statScale) {
        int hp = (int) Math.Floor((statScale * characterStats.Strength * 1 ) + (statScale * characterStats.Stamina * 2));
        int mp = 2 + (int) Math.Floor(statScale * ((0.005f) * characterStats.Intelligence) + 1.35);
        this.maxHp = hp;
        this.hp = hp;
        this.defense = (int) Math.Floor(statScale * characterStats.Stamina / 2);
        this.physicalAttack = 1 + (int) Math.Floor(statScale * characterStats.Strength / 2);
        this.magicalAttack = 1 + (int) Math.Floor(statScale * characterStats.Intelligence / 2);
        this.maxMp = mp;
        this.mp = mp;
        this.magicalResistance = (int) (statScale * characterStats.Intelligence / 2);
        this.criticalRate = 5 + (int) Math.Floor(statScale * characterStats.Agility / 20);
        this.accuracy = 100 + (int) Math.Floor(statScale * characterStats.Agility / 30);
        this.evasion = 5 + (int) Math.Floor(statScale * characterStats.Agility / 30);
        this.initiative = characterStats.Agility;
        this.criticalDamage = 50 + (int) Math.Floor(statScale * characterStats.Luck / 10);
        this.gpGainRate = (int) Math.Floor(statScale * characterStats.Luck / 20);
        this.itemDropRate = (int) Math.Floor(statScale * characterStats.Luck / 40);
        this.blockChance = 0;
        this.shields = 0;
        this.healProficiency = 0f;
        this.castTimeReduction = 0;
    }

    public int hp;
    public int maxHp;   //Nullable
    public int mp;
    public int maxMp;   //Nullable
    public int physicalAttack;
    public int magicalAttack;
    public int defense;
    public int magicalResistance;
    public int criticalRate;
    public int criticalDamage;
    public int accuracy;
    public int initiative;
    public int evasion;
    public int gpGainRate;
    public int itemDropRate;

    public int blockChance;

    public int shields;
    public int castTimeReduction;

    [Range(0, 1f)]
    public float healProficiency;

    public void AddStats(RawStats rawStats) {
        this.hp += rawStats.hp;
        this.maxHp += rawStats.maxHp;
        this.mp += rawStats.mp;
        this.maxMp += rawStats.maxMp;
        this.physicalAttack += rawStats.physicalAttack;
        this.magicalAttack += rawStats.magicalAttack;
        this.defense += rawStats.defense;
        this.magicalResistance += rawStats.magicalResistance;
        this.criticalRate += rawStats.criticalRate;
        this.criticalDamage += rawStats.criticalDamage;
        this.accuracy += rawStats.accuracy;
        this.initiative += rawStats.initiative;
        this.evasion += rawStats.evasion;
        this.gpGainRate += rawStats.gpGainRate;
        this.itemDropRate += rawStats.itemDropRate;
        this.blockChance += rawStats.blockChance;
        this.shields += rawStats.shields;
        this.healProficiency += rawStats.healProficiency;
        this.castTimeReduction += rawStats.castTimeReduction;
    }


    public void SubtractStats(RawStats rawStats) {
        this.hp -= rawStats.hp;
        this.maxHp -= rawStats.maxHp;
        this.mp -= rawStats.mp;
        this.maxMp -= rawStats.maxMp;
        this.physicalAttack -= rawStats.physicalAttack;
        this.magicalAttack -= rawStats.magicalAttack;
        this.defense -= rawStats.defense;
        this.magicalResistance -= rawStats.magicalResistance;
        this.criticalRate -= rawStats.criticalRate;
        this.criticalDamage -= rawStats.criticalDamage;
        this.accuracy -= rawStats.accuracy;
        this.initiative -= rawStats.initiative;
        this.evasion -= rawStats.evasion;
        this.gpGainRate -= rawStats.gpGainRate;
        this.itemDropRate -= rawStats.itemDropRate;
        this.blockChance -= rawStats.blockChance;
        this.shields -= rawStats.shields;
        this.healProficiency -= rawStats.healProficiency;
        this.castTimeReduction -= rawStats.castTimeReduction;
    }

    public RawStats StatMultiplier(RawStatMultiplier rawStatMultiplier) {
            RawStats rawStatCalculated = new RawStats(this);

            rawStatCalculated.maxHp = (int) ((float) rawStatCalculated.maxHp * rawStatMultiplier.maxHp);

            rawStatCalculated.maxMp = (int) ((float) rawStatCalculated.maxMp * rawStatMultiplier.maxMp);

            rawStatCalculated.physicalAttack = (int) ((float) rawStatCalculated.physicalAttack * rawStatMultiplier.physicalAttack);

            rawStatCalculated.magicalAttack = (int) ((float) rawStatCalculated.magicalAttack * rawStatMultiplier.magicalAttack);

            rawStatCalculated.defense = (int) ((float) rawStatCalculated.defense * rawStatMultiplier.defense);

            rawStatCalculated.magicalResistance =  (int) ((float) rawStatCalculated.magicalResistance * rawStatMultiplier.magicalResistance);

            rawStatCalculated.initiative = (int) ((float) rawStatCalculated.initiative * rawStatMultiplier.initiative);

            rawStatCalculated.evasion = (int) ( (float) rawStatCalculated.evasion * rawStatMultiplier.evasion);

            rawStatCalculated.criticalRate = (int)((float) rawStatCalculated.criticalRate * rawStatMultiplier.criticalRate);

            rawStatCalculated.criticalDamage = (int)((float) rawStatCalculated.criticalDamage * rawStatMultiplier.criticalDamage);

            rawStatCalculated.accuracy = (int) ( (float) rawStatCalculated.accuracy * rawStatMultiplier.accuracy);

            rawStatCalculated.gpGainRate = (int) ((float) rawStatCalculated.gpGainRate * rawStatMultiplier.gpGainRate);

            rawStatCalculated.itemDropRate = (int)((float) rawStatCalculated.itemDropRate * rawStatMultiplier.itemDropRate);

            rawStatCalculated.blockChance = (int)((float)rawStatCalculated.blockChance * rawStatMultiplier.blockChance);

            rawStatCalculated.shields = (int)((float) rawStatCalculated.shields * rawStatMultiplier.shields);

            rawStatCalculated.healProficiency *= rawStatMultiplier.healProficiency;

            rawStatCalculated.castTimeReduction = (int) ((float) rawStatCalculated.castTimeReduction * rawStatMultiplier.castTimeReduction);

        return rawStatCalculated;
    }
}
