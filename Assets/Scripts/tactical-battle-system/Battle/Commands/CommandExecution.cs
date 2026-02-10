using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
public abstract class CommandExecution
{
    [SerializeField]
    protected PartySlot user;

    [SerializeField]
    protected List<PartySlot> affectedTargets;

    [SerializeField]
    protected List<CombatTileController> affectedTiles;

    [SerializeField]
    protected BattleManager battleController;

    [SerializeField]
    protected IBattleActionCommand battleAction;

    public CommandExecution(IBattleActionCommand battleAction, PartySlot user, List<PartySlot> affectedTargets, List<CombatTileController> affectedTiles, BattleManager battleController)
    {
        this.battleAction = battleAction;
        this.user = user;
        this.affectedTargets = affectedTargets ?? new();
        this.affectedTiles = affectedTiles ?? new();
        this.battleController = battleController;
    }

    public abstract void ExecuteCommand();
}
}



// public class CommandExecutionSingleTarget : CommandExecution
// {
//     public CommandExecutionSingleTarget(IBattleActionCommand battleAction, PartySlot user, List<PartySlot> affectedTargets, List<CombatTileController> affectedTiles, BattleManager battleController) : base(battleAction, user, affectedTargets, affectedTiles, battleController)
//     {
//     }

//     public override void ExecuteCommand()
//     {
//         battleAction.ExecuteSingleTargetSkill(battleController, user, affectedTargets[0] ?? null,  affectedTiles);
//     }
// }