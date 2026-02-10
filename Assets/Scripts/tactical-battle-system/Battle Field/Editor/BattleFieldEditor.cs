using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat
{
    [ExecuteAlways]
    public class BattleFieldEditor : MonoBehaviour
    {
        [SerializeField]
        [Range(0.01f, 10f)]
        private float height = 0.01f;
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        [ContextMenu("Execute Height Changes")]
        private void ExecuteHeightChanges()
        {
            List<CombatTileController> combatTiles = new List<CombatTileController>(GetComponentsInChildren<CombatTileController>());
            foreach (CombatTileController tile in combatTiles)
            {
                Vector3 newScale = tile.Tile.gameObject.transform.localScale;
                newScale.y = height;
                tile.Tile.gameObject.transform.localScale = newScale;
            }
        }

    }
}
