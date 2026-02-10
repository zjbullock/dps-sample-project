using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
public interface IBattleCommand 
{
    #nullable enable
    void ExecuteCommand(BattleManager battleController, System.Action? callBack = null, IBattleCommand? commandOverride = null);
    // BattleActionAnimationController? GetBattleActionAnimationController();
    bool CanExecuteCommand();
    #nullable disable
}
}