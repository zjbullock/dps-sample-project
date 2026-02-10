using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "Battle_Action_Event_Spawn_Obstacle", menuName = "ScriptableObjects/Battle Action Event/Spawn Obstacle Event")]
public class BattleActionEventSpawnObjectSO : BattleActionEventSO
{
    [Tooltip("Denotes whether this spawned object alters the tile state")]
    [SerializeField]
    private CombatTileInteractionSO _combatTileInteractionSO;

    public override bool CanBeDoneOnTile(List<CombatTileController> combatTileControllers)
    {
        foreach (CombatTileController combatTile in combatTileControllers)
        {
            if (!combatTile.CanAddSpawnedObject())
            {
                Debug.Log("CANNOT SPAWN OBJECT HERE");
                return false;
            }
        }

        return true;
    }

    protected override void ExecutePartnerAction(BattleManager battleManager, IBattleActionCommand battleActionCommand, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles)
    {
        this.SetSpawnedObjects(battleManager, combatTiles);
    }

    protected override void ExecuteAction(BattleManager battleManager, IBattleActionCommand battleActionCommand, PartySlot user, List<PartySlot> primaryAbilityTargets, List<CombatTileController> combatTiles)
    {
        this.SetSpawnedObjects(battleManager, combatTiles);
    }

    private void SetSpawnedObjects(BattleManager battleManager, List<CombatTileController> combatTiles)
    {
        if (this._combatTileInteractionSO == null || this._combatTileInteractionSO.spawnableObject == null)
        {
            Debug.LogError("No Spawnable Object Set for instance of Spawnable Object Event: " + this.name);
            return;
        }

        Debug.Log("Attempting to set the spawned object for tiles");
        foreach (CombatTileController combatTile in combatTiles)
        {
            if (combatTile.CanAddSpawnedObject())
            {
                combatTile.SetSpawnedObjectFromCombatTileInteractionSO(battleManager, this._combatTileInteractionSO);
            }
        }
    }
}
}