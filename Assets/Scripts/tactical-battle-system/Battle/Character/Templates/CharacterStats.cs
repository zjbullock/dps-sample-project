using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterStatModifiers
{

    public float Strength = 0f;
    public float Stamina = 0f;
    public float Intelligence = 0f;
    public float Agility = 0f;
    public float Luck = 0f;

    public CharacterStatModifiers()
    {

    }
    public CharacterStatModifiers(CharacterStatModifiers characterStatModifiers)
    {
        this.Strength = characterStatModifiers.Strength;
        this.Stamina = characterStatModifiers.Stamina;
        this.Intelligence = characterStatModifiers.Intelligence;
        this.Agility = characterStatModifiers.Agility;
        this.Luck = characterStatModifiers.Luck;
    }


    public void AddStatModifiers(CharacterStatModifiers characterStatModifiers)
    {
        this.Strength += characterStatModifiers.Strength;
        this.Stamina += characterStatModifiers.Stamina;
        this.Intelligence += characterStatModifiers.Intelligence;
        this.Agility += characterStatModifiers.Agility;
        this.Luck += characterStatModifiers.Luck; 
    }
}

[Serializable]
public class CharacterStats {
    public int Strength = 0;
    public int Stamina = 0;
    public int Intelligence = 0;
    public int Agility = 0;
    public int Luck = 0;

    public CharacterStats()
    {
        
    }

    public CharacterStats(int strength, int stamina, int intelligence, int agility, int luck, int scale)
    {
        Strength = strength;
        Stamina = stamina;
        Intelligence = intelligence;
        Agility = agility;
        Luck = luck;
    }

    public CharacterStats(CharacterStats characterStats) {
        Strength = characterStats.Strength;
        Stamina = characterStats.Stamina;
        Intelligence = characterStats.Intelligence;
        Agility = characterStats.Agility;
        Luck = characterStats.Luck;
    }

    public void AddStats(CharacterStats characterStats) {
        this.Strength += characterStats.Strength;
        this.Stamina += characterStats.Stamina;
        this.Intelligence += characterStats.Intelligence;
        this.Agility += characterStats.Agility;
        this.Luck += characterStats.Luck;
    }
    public void SubtractStats(CharacterStats characterStats) {
        this.Strength -= characterStats.Strength;
        this.Stamina -= characterStats.Stamina;
        this.Intelligence -= characterStats.Intelligence;
        this.Agility -= characterStats.Agility;
        this.Luck -= characterStats.Luck;
    }


    public CharacterStats StatMultiplier(CharacterStatModifiers characterStatModifiers) {
        CharacterStats calculatedStats = new CharacterStats(this);
        calculatedStats.Strength =  (int) (this.Strength * characterStatModifiers.Strength);
        calculatedStats.Stamina =  (int) (this.Stamina * characterStatModifiers.Stamina);
        calculatedStats.Intelligence =  (int) (this.Intelligence * characterStatModifiers.Intelligence);
        calculatedStats.Agility =  (int) (this.Agility * characterStatModifiers.Agility);
        calculatedStats.Luck =  (int) (this.Luck * characterStatModifiers.Luck);
        return calculatedStats;
    }
}
