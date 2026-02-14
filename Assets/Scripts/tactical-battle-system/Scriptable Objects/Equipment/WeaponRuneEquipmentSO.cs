using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
    [CreateAssetMenu(fileName = "Weapon_", menuName = "ScriptableObjects/Inventory/Equipment/Weapon")]
    [System.Serializable]
    public class WeaponRuneEquipmentSO : EquipmentSO {
        [SerializeField]
        private WeaponType _weaponType;

        public WeaponType WeaponType { get => this._weaponType; }
    }
}