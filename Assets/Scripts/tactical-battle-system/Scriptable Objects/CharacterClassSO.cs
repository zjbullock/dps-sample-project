using System.Collections;
using System.Collections.Generic;
using DPS.TacticalCombat;
using UnityEngine;

[CreateAssetMenu(fileName = "Character_Class_", menuName = "ScriptableObjects/Character Class")]
public class CharacterClassSO : ScriptableObject
{
    [SerializeField]
    private string className;


    public string ClassName { get => this.className;  }


    [SerializeField]
    private WeaponType _weaponType;

    public WeaponType WeaponType { get => this._weaponType; }
    
    [SerializeField]
    private ArmorType _armorType;

    public ArmorType ArmorType { get => this._armorType; }


}
