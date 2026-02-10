using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat{
public class PlayerBattleObject : SpriteBattleObject
{
    [Header("Prefabs")]
    [SerializeField]
    private GameObject battleObjectCursor;
    protected override void UpdateBattleObject() {
        if (!base.isReady && base.battleMember != null) {
            base.playerCursor = (GameObject) Instantiate(this.battleObjectCursor);
            base.playerCursor.transform.SetParent(transform, false);
            base.playerCursor.transform.position = base.GetTargetAnchorPointPosition();
            base.playerCursor.SetActive(false);

            base.isReady = true;
        }
    }

}
}