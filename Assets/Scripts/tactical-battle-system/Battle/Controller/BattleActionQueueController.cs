using UnityEngine;

namespace DPS.TacticalCombat {
public class BattleActionQueueController : MonoBehaviour
{
    #nullable enable

    private IBattleActionCommand? _battleActionCommand;

    public IBattleActionCommand? BattleActionCommand { get => this._battleActionCommand; }

    public IBattleActionCommand? GetBattleActionCommand()
    {
        return this._battleActionCommand;
    }

    public void ClearBattleActionCommand()
    {
        this._battleActionCommand = null;
    }

    public void SetBattleActionCommand(IBattleActionCommand battleActionCommand)
    {
        this.ClearBattleActionCommand();
        this._battleActionCommand = battleActionCommand;
        Debug.Log("BattleActionCommand set to: " + battleActionCommand.GetActionName());
    }



}
}