using UnityEngine;

namespace DPS.TacticalCombat {
public abstract class CombatTileInteractionHookSO : ScriptableObject
{
    public abstract void OnTileEntered(CombatTileController combatTileController, BattleManager battleController, PartySlot partySlot);

    public abstract void OnTileExited(CombatTileController combatTileController, BattleManager battleController, PartySlot partySlot);

    public abstract void OnTileStateStart(CombatTileController combatTileController);

    public abstract void OnTileStateEnd(CombatTileController combatTileController);
}
}