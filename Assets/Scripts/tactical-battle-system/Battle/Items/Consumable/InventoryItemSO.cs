using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DPS.TacticalCombat {
// [CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Inventory/Item")]
[Serializable]
public abstract class InventoryItemSO : ScriptableObject {
    public Sprite itemSprite;
    //Name displayed for the item.
    public string itemName;
    //Price paid for the item.
    public int price;
    //Slot of inventory the item uses
    public InventorySlots inventorySlots;
    //How much the item will sell for in the market.
    public int reSellPrice;
    //Flavor text and details for the item.
    public string description;
    //Denotes item targets
    public TargetTypes targetTypes;

    public ElementSO element;
}
}