using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Battle_Tile_Interaction_Hook_Buff_", menuName = "ScriptableObjects/Tile Interactions/Hooks/Buff")]

public class BuffCombatTileInteractionHookSO : CombatTileInteractionHookSO
{
    [SerializeField]
    private StatusEffectSO buff;


    public override void OnTileEntered(CombatTileController combatTileController, BattleManager battleController, PartySlot partySlot)
    {
        this.addBuff(partySlot);
    }

    public override void OnTileExited(CombatTileController combatTileController, BattleManager battleController, PartySlot partySlot)
    {
        this.removeBuff(partySlot);
    }

    public override void OnTileStateEnd(CombatTileController combatTileController)
    {
        this.removeBuff(combatTileController.GetPartyOccupant());
    }

    public override void OnTileStateStart(CombatTileController combatTileController)
    {
        this.addBuff(combatTileController.GetPartyOccupant());
    }

    #nullable enable
    private void addBuff(PartySlot? partySlot) {
        if (this.buff == null) {
            Debug.LogError("No buff found for buff combat tile interaction hook");
            return;
        }

        if (partySlot == null) {
            return;
        }

        IBattleEntity? battleEntity = partySlot.BattleEntity;
        if (battleEntity == null) {
            return;
        }

        List<StatusEffect> buffs = battleEntity.GetBuffs();

        if (buffs != null & buffs!.Count > 0 && buffs.Find((buff) => { return buff.statusEffect == this.buff; }) != null) {
            return;
        }
        battleEntity.AddStatusEffect(buff);
    }

    private void removeBuff(PartySlot? partySlot) {
        if (this.buff == null) {
            Debug.LogError("No buff found for buff combat tile interaction hook");
            return;
        }

        if (partySlot == null) {
            return;
        }

        IBattleEntity? battleEntity = partySlot.BattleEntity;
        if (battleEntity == null) {
            return;
        }

        List<StatusEffect> buffs = battleEntity.GetBuffs();
        if (battleEntity != null && buffs != null && buffs!.Count > 0) {
            this.buff.OnRemoveStatusEffect(battleEntity);
            battleEntity.RemoveStatusEffect(buff);
        }
    }
}
}