using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
public class ItemCommand : BattleActionCommand
{
    #nullable enable
    [SerializeField]
    private ConsumableInventoryItemSO _consumableItemSO;
    public ItemCommand(
                    ConsumableInventoryItemSO consumableItemSO,
                    PartySlot user,
                    Func<PartySlot, bool> isSameTypeEntity,
                    CombatTileController targetedTile,
                    List<CombatTileController> tiles,
                    List<PartySlot>? targetedSlots = null
                    ): base(consumableItemSO,  user, isSameTypeEntity, tiles, targetedSlots, targetedTile)
    {
        this._consumableItemSO = consumableItemSO;
    }

    public override bool CanExecuteCommand()
    {
        return true;
    }

    public override void ExecuteCommand(BattleManager battleController, Action? callBack = null, IBattleCommand? commandOverride = null)
    {
        base.ExecuteCommand(battleController, callBack, commandOverride);
        // PlayerProfileData? playerProfileData = PlayerProfileData.Instance;
        // if (playerProfileData == null)
        // {
        //     Debug.LogError("PlayerProfileData instance is null. Cannot update inventory.");
        //     return;
        // }
        // playerProfileData.inventory.RemoveInventoryItem(this._consumableItemSO, 1);
    }
}
}