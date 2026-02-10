using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DPS.TacticalCombat{
[Serializable]
public class CombatTileControllerAbnormalState : CombatTileControllerBaseState
{
    [SerializeField]
    public bool infiniteDuration;
    public int stateDuration;
    

    #nullable enable
    public GameObject? tileEffect;

    public CombatTileControllerAbnormalState(BattleManager battleManager, CombatTileController combatTileController, CombatTileInteractionSO combatTileInteraction) {
        this.combatTileInteractionSO = combatTileInteraction;
        this.infiniteDuration = combatTileInteraction.infiniteDuration;
        if (!combatTileInteraction.infiniteDuration)
        {
            this.stateDuration = combatTileInteraction.duration;
        }
        // if (spawnableBattleObject == null || !spawnableBattleObject.GetSpawnableObjectInfo()) {
        //     return;
        // }

        // if (partySlot == null) {
        //     BattleEntitySlot battleEntitySlot = new BattleEntitySlot(new SpawnedObjectInfo(spawnableBattleObject.GetSpawnableObjectInfo()));
        //     BattleMember battleMember = new BattleMember(battleEntitySlot);
        //     this.SpawnableObjectPartySlot = new SpawnableObjectPartySlot(battleMember) {
        //         CombatTiles = new List<CombatTileController>(){combatTileController}
        //     };

        // } else {
        //     this.SpawnableObjectPartySlot = partySlot;
        // }

        // if (this.SpawnableObjectPartySlot != null) {
        //     this.SpawnableObjectPartySlot.SetBattleObject(spawnableBattleObject);
        //     this.SpawnableObjectPartySlot.SetCombatTileController(battleManager, combatTileController);
        // }
    }
    #nullable disable

    public override void EndState(CombatTileController combatTileController)
    {
        this.EndAbnormalState(combatTileController);
        return;
    }

    public override void EnterState(CombatTileController combatTileController)
    {
        combatTileInteractionSO.OnEnteringState(combatTileController, this);
        if(this.tileEffect != null) {
            tileEffect.transform.SetParent(combatTileController.Tile.gameObject.transform);
            tileEffect.transform.position = combatTileController.GetTileBasePosition(false);
        }

        return;
    }

    public override void UpdateTileTurnState(BattleManager battleController, CombatTileController combatTileController) {
        base.UpdateTile(combatTileController);
        if (tileEffect != null) {
            tileEffect.transform.position = combatTileController.GetTileBasePosition(false);
        }
        if (!this.infiniteDuration) {
            this.stateDuration--;
            if (this.stateDuration == 0) {
                combatTileController.ResetToDefaultState();
            }
        }
    }

    public override void UpdateTileState(CombatTileController combatTileController)
    {
        if (tileEffect != null) {
            tileEffect.transform.position = combatTileController.GetTileBasePosition(false);
        }

        return;
    }

    public override void UpdateTile(CombatTileController combatTileController)
    {

    }

    // public override List<PartySlot> GetPartySlots(CombatTileController combatTileController) {
    //     List<PartySlot> partySlots = new List<PartySlot>();
    //     partySlots.AddRange(base.GetPartySlots(combatTileController));
    //     if (combatTileController.SpawnableObjectPartySlot != null) {
    //         partySlots.Add(combatTileController.SpawnableObjectPartySlot);
    //     }
    //     return partySlots; 
    // }

    private void EndAbnormalState(CombatTileController combatTileController) {
        // if(this.SpawnableObjectPartySlot != null) {
        //     this.SpawnableObjectPartySlot = null;
        // }

        if (tileEffect != null) {
            GameObject.Destroy(tileEffect);
        }
        combatTileInteractionSO.OnExitingState(combatTileController, this);
    }
}
}
