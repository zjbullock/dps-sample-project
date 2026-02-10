using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[System.Serializable]
public class BattleControllerWinState : BattleControllerBaseState
{
    private enum WinState {
        None,
        Result,
        End,
    }

    private WinState winState;

    [SerializeField]
    private AudioClip victoryFanfair;

    public BattleControllerWinState() {
        this.winState = WinState.None;
    }

    public override void EnterState(BattleManager battleController)
    {
        battleController.BGMController.StopAudio(callback: () => {
            battleController.BGMController.SetAudioClip(this.victoryFanfair);
            // battleController.ActivateResultScreen();
            // this.SetResultScreenContent(battleController);

            this.winState = WinState.Result;
        });
        base.EnterState(battleController);

        return;
    }

    public override void UpdateState(BattleManager battleController)
    {
        switch(winState) {
            case WinState.Result:
                // if (battleController.playerInputActions.BattleUI.Submit.WasPressedThisFrame()) {
                //     battleController.audioController.PlayAudio(SoundEffectEnums.Confirm);
                //     this.winState = WinState.End;
                //     return;
                // }
                break;
            case WinState.End:
                // battleController.SceneTransitionService?.FadeToPlayerPositionScene();
                // battleController.playerPartyController.inventory.PurgeQueuedItemList();
                // this.winState = WinState.None;
                break;
        }
    }

    // #nullable enable
    // private void SetResultScreenContent(BattleManager battleController) {
    //     List<SubListItem> listItems = new List<SubListItem>();
    //     foreach(var item in battleController.ObtainedDrops) {
    //         listItems.Add(new SubListItem(item.Key.itemName, null, item.Value + "", item.Value, false));
    //     }

    //     listItems.Sort((a, b) => {
    //         InventoryItemSO? inventoryItemA = a.subListObject as InventoryItemSO;
    //         InventoryItemSO? inventoryItemB = b.subListObject as InventoryItemSO;
    //         if (inventoryItemA != null && inventoryItemB != null) {
    //             int result = inventoryItemB.inventorySlots.CompareTo(inventoryItemA.inventorySlots);
    //             return result;
    //         }
    //         return 0;

    //    });

    //     battleController.BattleMenuController.ResultScreenController.SetContents(battleController.ObtainedGP, listItems);
    // }
    // #nullable disable
}
}