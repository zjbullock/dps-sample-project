using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
public class EnemyPartySlot : PartySlot
{

    public EnemyPartySlot(BattleMember battleMember) : base(battleMember)
    {
    }

    #nullable enable
    protected override void ExecuteBattleCommandLogic(BattleManager battleController, System.Action? callBack)
    {
        foreach(KeyValuePair<Vector3, CombatTileController> keyValuePair in battleController.Grid) {
            keyValuePair.Value.DisableActionTile();
            keyValuePair.Value.DisableActionConfirmTile();
        }
        if (battleController.CurrentlyActingMember.GetBattleMember()!.battleMemberType == BattleMemberType.Enemy) {
            battleController.BattleFieldController.ClearActionGrid();
            battleController.BattleFieldController.ClearConfirmActionGrid();
        }

        // switch(this.currentState) {
        //     case BattleTurnResultStateEnums.None:
        //         this.currentState = BattleTurnResultStateEnums.Wait;

        //     break;
        // }

        callBack += () =>
        {
            this.battleMember.BattleCommand = null;
        };

        this.processCommand(battleController, this.GetCombatTileController(), callBack);
        return;
    }

    protected override void TakeDamage(BattleManager battleController, int damage, string statusText, ElementSO? ElementSO, string? additionalText = null) {
        base.TakeDamage(battleController, damage, statusText, ElementSO, additionalText);
        if (this.BattleEntity!.GetPoiseState().PoiseState == PoiseState.Broken)
        {
            battleController.OnInflictPoiseBreak();

        }
        this.HandleDeath(battleController);
        return;
    }

    public override void SetDownedStatus(int poisePoints)
    {
        // if (this.BattleEntityGO != null && this.BattleEntityGO.poiseUI != null) {
        //     this.BattleEntityGO.poiseUI.SetDownedStatus(poisePoints);
        // }
    }

    public override void ProcessDamage(BattleManager battleController, int damage, ElementSO ElementSO, string? additionalText = null) {

        if (this.BattleEntity == null)
        {
            return;
        }

        base.ProcessDamage(battleController, damage,  ElementSO, additionalText: additionalText);
        
        // Debug.Log("Attempting removal of battle entity");
        return;
    }

    public override void UpdateGauges() {
        if (this.BattleEntityGO != null) {
            this.BattleEntityGO.UpdateGauges();
        }
        return;
    }

    public override void CombatEnd()
    {
        return;
    }

    public override void BeginPhase(BattleManager battleController) {
        this.BeginPhaseAbilities();
        base.BeginPhase(battleController);
    }


    public override void BeginPhaseAbilities()
    {
        this.activateBeginPhaseAbilities();
        base.BeginPhaseAbilities();
    }




    private void activateBeginPhaseAbilities() {
        EnemyInfo? enemyInfo = this.BattleEntity as EnemyInfo;
        if (enemyInfo != null) {
            if (enemyInfo.passiveSkills != null && enemyInfo.passiveSkills.Count > 0) {
                foreach(PassiveSkillSO passiveSkillSO in enemyInfo.passiveSkills) {
                    passiveSkillSO.ExecuteBeginPhasePassiveSkill(this, this.GetCombatTileController());
                }
            }
        }
        
    }

    public override void BeginCombatAbilities() {
        Debug.Log("Trying to activate enemy combat abilities");
        this.activateBeginCombatAbilities();
        base.BeginCombatAbilities();
    }

    private void activateBeginCombatAbilities() {
        EnemyInfo? enemyInfo = this.BattleEntity as EnemyInfo;
        if (enemyInfo != null) {
            Debug.Log("Found passive skills for: " + enemyInfo.GetName());
            if (enemyInfo.passiveSkills != null && enemyInfo.passiveSkills.Count > 0) {
                foreach(PassiveSkillSO passiveSkillSO in enemyInfo.passiveSkills) {
                    passiveSkillSO.ExecuteBeginCombatPassiveSkill(this, this.GetCombatTileController());
                }
            }
        }
        
    }

    public void HandleDeath(BattleManager battleController)
    {
        
        if (!this.BattleEntity!.IsDead())
        {
            return;
        }


        EnemyInfo? enemyInfo = this.BattleEntity! as EnemyInfo;
        if (enemyInfo == null) {
            // Debug.Log("Enemy Party Slot, but no EnemyInfo");
            return;
        }
        
        // Debug.Log("Enemy Info somehow not dead");
        
        System.Random random = new();

        DroppedItems droppedItems = enemyInfo.GetDroppedItems(random);

        battleController.ObtainedGP += droppedItems.gp;
        
        foreach (InventoryItemSO inventoryItemSO in droppedItems.inventoryItems)
        {
            if (battleController.ObtainedDrops.TryGetValue(inventoryItemSO, out int value))
            {
                battleController.ObtainedDrops[inventoryItemSO]++;
            }
            else
            {
                battleController.ObtainedDrops[inventoryItemSO] = 1;
            }
        }

        if (this.currentCombatTileController != null)
        {
            Debug.Log("Attempting removal of battle entity");
            this.currentCombatTileController.RemoveOccupantAndProcessEvent(battleController);
        }

        this.battleObject.DestroyBattleObject();
        battleController.EnemyPartyController.PartySlots.Remove(this);
        battleController.OnEnemyKO();
    }

    public EnemyCommandEvent GetEnemyCommandEvent(BattleManager battleController) {
        return new StandardEnemyCommandEvent(battleController, this);
    }
}
}