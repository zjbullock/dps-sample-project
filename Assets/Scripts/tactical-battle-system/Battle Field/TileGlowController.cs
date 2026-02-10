using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
[ExecuteInEditMode]
public class TileGlowController : MonoBehaviour {



    [SerializeField]
    private CombatTileController combatTileController;

    public CombatTileController CombatTileController {set => this.combatTileController = value; get => this.combatTileController; }

    // Start is called before the first frame update
    void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {
        if (this.combatTileController != null && this.combatTileController.Tile != null) {
            // float newHeight = this.combatTileController.GetFloatHeightWithSpawnedObject(true);
            float newHeight = this.combatTileController.GetHeightForSpriteVector3Offset(true).y;
            if (transform.position.y != newHeight) {
                transform.position = new Vector3(transform.position.x, newHeight, transform.position.z);
            }  
        }
    }
}
}