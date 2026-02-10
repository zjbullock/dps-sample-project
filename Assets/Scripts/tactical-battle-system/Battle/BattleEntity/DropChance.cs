using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[System.Serializable]
public class DropChance 
{
    [SerializeField]
    private InventoryItemSO inventoryItemSO;
    
    [SerializeField]
    [Tooltip("Drop chance for determining if the item should be acquired or not. \n Defaults to 75 for 75%.")]
    [Range(0, 100)]
    private int dropChance = 75;

    public DropChance(int dropChance, InventoryItemSO inventoryItemSO) {
        this.dropChance = dropChance;
        this.inventoryItemSO = inventoryItemSO;
    }

    #nullable enable
    public InventoryItemSO? RollInventoryItemChance(System.Random random, int additionalDropChance = 0, float multiplyDropChance = 1f) {
        int rolledChance = random.Next(0, 99);
        if ((rolledChance - additionalDropChance) < (this.dropChance * multiplyDropChance)) {
            return this.inventoryItemSO;
        }

        return null;
    }
    #nullable disable
}
}