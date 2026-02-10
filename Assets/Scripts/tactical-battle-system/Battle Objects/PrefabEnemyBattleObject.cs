using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat{
public class PrefabEnemyBattleObject : EnemyBattleObject
{
    [Header("Enemy Battle Object Configurations")]
    [SerializeField]
    private EnemyInfoSO enemyInfoSO;

    public EnemyInfoSO EnemyInfoSO { get => this.enemyInfoSO; }

    [SerializeField]
    private CapsuleCollider enemyCollider;

    [SerializeField]
    [Tooltip("How much grace the hitbox of enemy attacks are provided for non-stage abilities.")]
    [Range(0, 10)]
    private int enemyHitBoxGraceValue = 5;

    public int EnemyHitBoxGraceValue => this.enemyHitBoxGraceValue;
    
    public int Width => (int) Math.Round(this.enemyCollider.radius);

    [SerializeField]
    private List<CombatTileController> inhabitedTiles = new List<CombatTileController>();

    protected override void BattleObjectAwake()
    {
        base.BattleObjectAwake();
        if (this.enemyCollider == null) {
            this.enemyCollider = GetComponent<CapsuleCollider>();
        }
    }

    #nullable enable
    private List<CombatTileController> GetCombatTileControllersByColliderAtPosition(Vector3 center) {

        Vector3 direction = new() { [this.enemyCollider.direction] = 1};
        float offset = this.enemyCollider.height / 2 - this.enemyCollider.radius;

        Vector3 pos1 = center - direction * offset;
        Vector3 pos2 = center + direction * offset;

        Collider[] colliders = Physics.OverlapCapsule(pos1, pos2, this.enemyCollider.radius);

        List<CombatTileController> combatTileControllers = this.GetCombatTileControllers(colliders);

        return combatTileControllers;
    }

    private List<CombatTileController> GetCombatTileControllers(Collider[] colliders) {
        List<CombatTileController> combatTileControllers = new List<CombatTileController>();
        foreach(Collider collider in colliders) {
            CombatTileController? combatTileController = this.ProcessCombatTileCollider(collider);
            if (combatTileController == null) {
                continue;
            }

            combatTileControllers.Add(combatTileController);
        }  

        return combatTileControllers;
    }

    private CombatTileController? ProcessCombatTileCollider(Collider collider) {
        if (!collider.CompareTag("Tile")) {
            return null;
        }

        if (collider.gameObject == null || !collider.gameObject.TryGetComponent(out CombatTileController newCombatTileController)) {
            return null;
        }

        return newCombatTileController;
    }
    #nullable disable

    public override bool CanMoveToTile(CombatTileController possibleMoveTile, PartySlot partySlot)
    {
        if (this.enemyCollider == null) {
            return true;
        }

        List<CombatTileController> combatTileControllers = this.GetCombatTileControllersByColliderAtPosition(possibleMoveTile.Position);
        foreach(CombatTileController combatTileController in combatTileControllers) {
            if (combatTileController.Height != partySlot.GetCombatTileController().Height ||
                !combatTileController.CanAddOccupant(partySlot)) {
                return false;
            }
        }
        return true;
    }

    private void ClearCombatTiles(PartySlot partySlot) {
        foreach(CombatTileController combatTileController in this.inhabitedTiles) {
            if (partySlot.GetCombatTileController() == combatTileController) {
                continue;
            }
            combatTileController.RemoveOccupant();
        }
        this.inhabitedTiles.Clear();
    }

    public override void OnSetCombatTileController(BattleManager battleManager, PartySlot partySlot)
    {
        this.ClearCombatTiles(partySlot);
        this.inhabitedTiles = this.GetCombatTileControllersByColliderAtPosition(partySlot.GetCombatTileController().Position);

        foreach(CombatTileController combatTileController in this.inhabitedTiles) {
            if (partySlot.GetCombatTileController() == combatTileController) {
                continue;
            }
            combatTileController.SetAdditionalPartyOccupant(battleManager, partySlot);
            
        }

        base.OnSetCombatTileController(battleManager, partySlot);
    }
}
}
