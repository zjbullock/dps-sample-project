using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DPS.TacticalCombat {
public class BattleStageStructure : MonoBehaviour
{
    [SerializeField]
    private CombatTileInteractionSO combatTileInteraction;

    [SerializeField]
    private BoxCollider structureCollider;

    [SerializeField]
    private List<CombatTileController> combatTileControllers;

    void Awake() {
        this.combatTileControllers ??= new List<CombatTileController>();
        
        if (this.structureCollider == null) {
            this.structureCollider = GetComponent<BoxCollider>();
        }
    }

    public void SetStageStructureProperties() {
        this.SetCombatTileDefaultInteractionOverride();


    }

    private void SetCombatTileDefaultInteractionOverride () {
        if (this.structureCollider == null || !this.structureCollider.isTrigger) {
            return;
        }

        Vector3 worldCenter = this.structureCollider.transform.TransformPoint(this.structureCollider.center);
        Vector3 worldHalfExtents = Vector3.Scale(this.structureCollider.size, this.structureCollider.transform.lossyScale) * 0.5f;

        Collider[] colliders = Physics.OverlapBox(worldCenter, worldHalfExtents, this.structureCollider.transform.rotation);


        foreach(Collider collider in colliders) {
            this.ProcessCombatTileCollider(collider);
        }  
    }

    private void ProcessCombatTileCollider(Collider collider) {
        if (!collider.CompareTag("Tile")) {
            return;
        }

        if (collider.gameObject == null || !collider.gameObject.TryGetComponent(out CombatTileController newCombatTileController)) {
            return;
        }

        if(this.combatTileControllers.Contains(newCombatTileController)) {
            return;
        }
        newCombatTileController.OverrideDefaultStateTileInteraction(this.combatTileInteraction);
        this.combatTileControllers.Add(newCombatTileController);
    }
}
}