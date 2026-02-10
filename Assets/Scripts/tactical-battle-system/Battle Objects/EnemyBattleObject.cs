using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat{
public class EnemyBattleObject : SpriteBattleObject
{


    protected override void UpdateBattleObject() {


        if (!base.isReady && base.battleMember != null) {
            // base.statusBar = (GameObject) Instantiate(Resources.Load(Constants.UI.EnemyHP), base.GetTargetAnchorPointPosition(), transform.rotation );
            // base.statusBar.transform.SetParent(transform, true);
            // base.healthBar = statusBar.GetComponentInChildren<GaugeController>();
            // base.healthBar.SetGaugeMaxValue(battleMember.GetBattleEntity.GetRawStats().maxHp);
            // base.poiseUI = statusBar.GetComponentInChildren<PoiseUIController>();
            // base.poiseUI.SetDownedStatus(battleMember.GetBattleEntity.GetPoiseBreakState().poisePoints);
            // base.statusBar.SetActive(false);
            base.isReady = true;
        }

        // if (!shouldBeDestroyed || base.popupEvents.Count != 0) {
        //     return;
        // }
    }

    public override void DestroyBattleObject()
    {
        // if(base.battleMember == null || !base.GetBattleEntity!.IsDead()) {
        //     return;
        // }

        Debug.Log("Destroying ENEMY: " + this.gameObject.name);
        // if (base.statusBar != null)
        // {
        //     Destroy(base.statusBar);
        // }
        Destroy(this.gameObject);
    }


}
}
