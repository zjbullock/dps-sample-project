using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
public class PlayerPartySlot : PartySlot
{

    [Tooltip("The poise UI that is associated with the character's battle status")]
    [SerializeField]
    // private PoiseUIController poiseUI;

    // public BattlePortraitController battlePortrait;

    public PlayerPartySlot()
    {
    }
    
    public PlayerPartySlot(BattleMember battleMember) : base(battleMember)
    {
    }
    #nullable enable



    protected override void ExecuteBattleCommandLogic(BattleManager battleController, System.Action? callBack)
    {
        // switch(this.currentState) {
        //     case BattleTurnResultStateEnums.None:
        //         // Debug.Log("Attempting to activate ability");
        //         this.currentState = BattleTurnResultStateEnums.Wait;
        //         break;
        //     // case BattleTurnResultStateEnums.SwapSkillActivate:
        //     //     this.currentState = BattleTurnResultStateEnums.Wait;
        //     //     break;
        //     // case BattleTurnResultStateEnums.SwapSkillResolve:
        //     //     this.processCommand(battleController, combatTileController!, () => {
        //     //         base.Execute(battleController, combatTileController, actionPhase, actionPhaseResultState);
        //     //     });
        //     //     break;
        //     case BattleTurnResultStateEnums.PoiseBreaker:
        //         break;
        // }
        this.processCommand(battleController, this.currentCombatTileController!, callBack);
        return;
    }

    #nullable disable

    // public override void SetPoiseUI(PoiseUIController poiseUI)
    // {
    //     this.poiseUI = poiseUI;
    // }

    // public override void SetDownedStatus(int poisePoints)
    // {
    //     if (this.poiseUI != null) {
    //         this.poiseUI.SetDownedStatus(poisePoints);
    //     }
    // }

    // public override void UpdateGauges() {
    //     if (this.poiseUI != null) {
    //         this.poiseUI.SetDownedStatus(battleMember.GetBattleEntity.GetPoiseBreakState().poisePoints);
    //     }
    //     return;
    // }


    public override void CombatEnd() {
        this.battleMember.CombatEndStats();
        this.EndCombatAbilities();
        return;
    }
    


    #nullable enable

    protected override void TakeDamage(BattleManager battleController, int damage, string statusText, ElementSO? ElementSO, string? additionalText = null) {
        base.TakeDamage(battleController, damage, statusText, ElementSO, additionalText);
        if (this.BattleEntity!.GetPoiseState().PoiseState == PoiseState.Broken)
        {
            battleController.OnPartyMemberPoiseBreak();

        }

        // if (this.battlePortrait != null)
        // {
        //     this.battlePortrait.SetStatusBars();
        // }


        return;
    }

    public override void ProcessDamage(BattleManager battleController, int damage, ElementSO ElementSO, string? additionalText = null)
    {

        base.ProcessDamage(battleController, damage,  ElementSO, additionalText: additionalText);
        CharacterInfo? characterInfo = this.BattleEntity as CharacterInfo;
        if(characterInfo != null && characterInfo.GetResolveGauge() != null) {
            characterInfo.GetResolveGauge()!.OnTakingDamage(this, battleController);
        }

        if(this.BattleEntity!.IsDead()) {
            this.battleObject!.SetAnimationState(AnimationStates.Ko);
            battleController.OnKOed();
        }   

        return;
    }
    
    public override void PostMovePhase(BattleManager battleController) {
        this.PostMoveAbilities();
        CharacterInfo? characterInfo = this.battleMember!.GetBattleEntity! as CharacterInfo;
        if(characterInfo != null && characterInfo!.GetResolveGauge() != null) {
            characterInfo!.GetResolveGauge()!.OnBeginPhase(this, battleController);
        }
        base.PostMovePhase(battleController);
    }


    public override void EndPhase(BattleManager battleController) {
        this.EndPhaseAbilities();
        CharacterInfo? characterInfo = this.battleMember!.GetBattleEntity! as CharacterInfo;
        if(characterInfo != null && characterInfo!.GetResolveGauge() != null) {
            characterInfo!.GetResolveGauge()!.OnEndPhase(this, battleController);
        }
        base.EndPhase(battleController);
        // if(this.battlePortrait != null) {
        //     this.battlePortrait.SetStatusBars();
        // }
    }

    public override void BeginPhase(BattleManager battleController) {
        base.BeginPhase(battleController);


        this.BeginPhaseAbilities();
        CharacterInfo? characterInfo = this.battleMember!.GetBattleEntity! as CharacterInfo;
        if(characterInfo != null && characterInfo!.GetResolveGauge() != null) {
            characterInfo!.GetResolveGauge()!.OnBeginPhase(this, battleController);
        }
        // if (this.battlePortrait != null) {
        //     this.battlePortrait.SetStatusBars();
        // }
    }

    public override void OnEvade(BattleManager battleManager)
    {
        this.activatePostEvadeCombatAbilities();
        base.OnEvade(battleManager);
        this.OnEvadeCharacterInfoHandler(battleManager);
    }

    private void OnEvadeCharacterInfoHandler(BattleManager battleManager)
    {
        CharacterInfo? characterInfo = this.battleMember!.GetBattleEntity! as CharacterInfo;
        if (characterInfo == null)
        {
            return;
        }

        if (characterInfo!.GetResolveGauge() != null)
        {
            characterInfo!.GetResolveGauge()!.OnSuccessfulEvade(this, battleManager);
        }

        CharacterRoleSO? characterRoleSO = characterInfo.GetCharacterRole();
        if (characterRoleSO != null)
        {
            characterRoleSO.OnEvade(battleManager, this);  

        }
    }

    public override void OnBlock(BattleManager battleManager)
    {
        this.activatePostBlockCombatAbilities();
        base.OnBlock(battleManager);
        this.OnBlockCharacterInfoHandler(battleManager);
    }

    private void OnBlockCharacterInfoHandler(BattleManager battleManager)
    {
        CharacterInfo? characterInfo = this.battleMember!.GetBattleEntity! as CharacterInfo;
        if (characterInfo == null)
        {
            return;
        }

        if (characterInfo!.GetResolveGauge() != null)
        {
            characterInfo!.GetResolveGauge()!.OnSuccessfulBlock(this, battleManager);
        }

        CharacterRoleSO? characterRoleSO = characterInfo.GetCharacterRole();
        if (characterRoleSO != null)
        {
            characterRoleSO.OnBlock(battleManager, this);
        }
        
    }
    #nullable disable


    public override void EndCombatAbilities()
    {
        this.activateEndCombatAbilities();
        base.EndCombatAbilities();
    }

    #nullable enable
    public override void PostCommandAbilities()
    {
        this.activatePostCommandAbilities();
        base.PostCommandAbilities();
    }

    public override void HPChangeAbilities() {
        this.activateHPChangeAbilities();
        base.HPChangeAbilities();
    }

    public override void BeginPhaseAbilities()
    {
        this.activateBeginPhaseAbilities();
        base.BeginPhaseAbilities();
    }

    public override void PostMoveAbilities() {
        this.activatePostMoveAbilities();
        base.PostMoveAbilities();
    }

    public override void EndPhaseAbilities()
    {
        this.activateEndPhaseAbilities();
        base.EndPhaseAbilities();
    }

    public override void BeginCombatAbilities() {
        // Debug.Log("Doing combat begin stuff");
        this.activateBeginCombatAbilities();
        base.BeginCombatAbilities();
    }


    #nullable disable

    #nullable enable
    private void activateEndCombatAbilities() {
        IBattleEntity? characterInfo = this.BattleEntity as CharacterInfo;
        if (characterInfo != null) {
            if (characterInfo.GetSkillInfo().PassiveSkills != null && characterInfo.GetSkillInfo().PassiveSkills!.Count > 0) {
                foreach(PassiveSkillSO passiveSkillSO in characterInfo.GetSkillInfo().PassiveSkills!) {
                    passiveSkillSO.ExecuteEndCombatPassiveSkill(this);
                }
            }  
        }

    }

    public void activateBeginCombatAbilities() {
        IBattleEntity? characterInfo = this.BattleEntity;
        if ( characterInfo != null && characterInfo.GetSkillInfo() != null) {
            // Debug.Log("Activating begin combat abilities");

            if (characterInfo.GetSkillInfo().PassiveSkills != null && characterInfo.GetSkillInfo() != null && characterInfo.GetSkillInfo().PassiveSkills!.Count > 0) {
                foreach(PassiveSkillSO passiveSkillSO in characterInfo.GetSkillInfo().PassiveSkills!) {
                    passiveSkillSO.ExecuteBeginCombatPassiveSkill(this, this.currentCombatTileController);
                }
            }  
        }
        
    }

    public void activatePostCommandAbilities() {
        IBattleEntity? characterInfo = this.BattleEntity as CharacterInfo;
        if (characterInfo != null) {

            if (characterInfo.GetSkillInfo().PassiveSkills != null && characterInfo.GetSkillInfo().PassiveSkills!.Count > 0) {
                foreach(PassiveSkillSO passiveSkillSO in characterInfo.GetSkillInfo().PassiveSkills!) {
                    passiveSkillSO.ExecutePostCommandAbilities(this, this.currentCombatTileController);
                }
            }
        }

    }

    public void activatePostEvadeCombatAbilities() {
        IBattleEntity? characterInfo = this.BattleEntity as CharacterInfo;
        if (characterInfo != null) {


            if (characterInfo.GetSkillInfo().PassiveSkills != null && characterInfo.GetSkillInfo().PassiveSkills!.Count > 0) {
                foreach(PassiveSkillSO passiveSkillSO in characterInfo.GetSkillInfo().PassiveSkills!) {
                    passiveSkillSO.ExecutePostEvadePassiveSkill(this, this.currentCombatTileController);
                }
            }
        } 
    }

    public void activatePostBlockCombatAbilities() {
        IBattleEntity? characterInfo = this.BattleEntity as CharacterInfo;
        if (characterInfo != null) {

            if (characterInfo.GetSkillInfo().PassiveSkills != null && characterInfo.GetSkillInfo().PassiveSkills!.Count > 0) {
                foreach(PassiveSkillSO passiveSkillSO in characterInfo.GetSkillInfo().PassiveSkills!) {
                    passiveSkillSO.ExecutePostBlockPassiveSkill(this, this.currentCombatTileController);
                }
            }
        } 
    }


    private void activateHPChangeAbilities() {
        IBattleEntity? characterInfo = this.BattleEntity as CharacterInfo;
        if (characterInfo != null) {


            if (characterInfo.GetSkillInfo().PassiveSkills != null && characterInfo.GetSkillInfo().PassiveSkills!.Count > 0) {
                foreach(PassiveSkillSO passiveSkillSO in characterInfo.GetSkillInfo().PassiveSkills!) {
                    passiveSkillSO.ExecuteHPChangePassiveSkill(this, this.currentCombatTileController);
                }
            }
        }
    }

    private void activateEndPhaseAbilities() {
        IBattleEntity? characterInfo = this.BattleEntity as CharacterInfo;
        if (characterInfo != null) {


            if (characterInfo.GetSkillInfo().PassiveSkills != null && characterInfo.GetSkillInfo().PassiveSkills!.Count > 0) {
                foreach(PassiveSkillSO passiveSkillSO in characterInfo.GetSkillInfo().PassiveSkills!) {
                    passiveSkillSO.ExecuteEndPhasePassiveSkill(this, this.currentCombatTileController);
                }
            }
        }
    }

    private void activateBeginPhaseAbilities() {
        IBattleEntity? characterInfo = this.battleMember!.GetBattleEntity! as CharacterInfo;
        if (characterInfo != null) {

            if (characterInfo.GetSkillInfo().PassiveSkills != null && characterInfo.GetSkillInfo().PassiveSkills!.Count > 0) {
                foreach(PassiveSkillSO passiveSkillSO in characterInfo.GetSkillInfo().PassiveSkills!) {
                    passiveSkillSO.ExecuteBeginPhasePassiveSkill(this, this.currentCombatTileController);
                }
            }
        }
        
    }

    private void activatePostMoveAbilities() {
        IBattleEntity? characterInfo = this.BattleEntity as CharacterInfo;
        if (characterInfo != null) {


            if (characterInfo.GetSkillInfo().PassiveSkills != null && characterInfo.GetSkillInfo().PassiveSkills!.Count > 0) {
                foreach(PassiveSkillSO passiveSkillSO in characterInfo.GetSkillInfo().PassiveSkills!) {
                    passiveSkillSO.ExecutePostMovePassiveSkill(this, this.currentCombatTileController);
                }
            }
        }
    }
    
    // private Command? useSwapInSkill(BattleController battleController, CombatTileController combatTileController) {
    //     return this.battleMember.GetSwapSkill(battleController, combatTileController, this.GetBattleEntityGO(), SwapSkillType.SwapIn);
    // }

    // private Command? useSwapOutSkill(BattleController battleController, CombatTileController combatTileController) {
    //     return this.battleMember.GetSwapSkill(battleController, combatTileController, this.GetBattleEntityGO(), SwapSkillType.SwapOut);
    // }

    // public SkillCommand? useSwapInSkill(BattleManager battleController) {
    //     SwapSkill? swapSkill = this.battleMember.GetSwapSkill(battleController, this.GetCombatTileController(), this.BattleEntityGO);
    //     if (swapSkill == null) {
    //         return null;
    //     }
    //     SwapSkillSO? swapSkillSO = swapSkill.SwapSkillSO;
    //     if (swapSkillSO == null) {
    //         return null;
    //     }
    //     List<CombatTileController> tileList = new List<CombatTileController>();
    //     this.GetBattleMember()!.Tiles = swapSkillSO.GetActionTilesByAreaOfEffect(this.GetCombatTileController(), this);
        
    //     if (this.GetBattleMember()!.Tiles.Count > 0)
    //     {
    //         tileList = this.GetBattleMember()!.Tiles.ToValueList();
    //         if ( tileList.Count > 0)
    //         {
    //             swapSkill.SetSwapSkillCoolDown();
    //         }

    //     }

    //     return new SkillCommand(swapSkillSO, this, BattleProcessingStatic.PartySlotIsPlayerPartySlot, this.GetCombatTileController(), tileList, new());
    // }

    // public SkillCommand? useSwapOutSkill(BattleManager battleController, CombatTileController combatTileController) {
    //     SwapSkill? swapSkill = this.battleMember.GetSwapSkill(battleController, combatTileController, this.BattleEntityGO);
    //     if (swapSkill == null) {
    //         return null;
    //     }
    //     ActiveSkillSO? activeSkill = swapSkill.activeSkill;
    //     if (activeSkill == null) {
    //         return null;
    //     }
    //     List<CombatTileController> tileList = new List<CombatTileController>();
    //     this.GetBattleMember()!.Tiles = activeSkill.GetActionTilesByAreaOfEffect(combatTileController, this);
    //     if (this.GetBattleMember()!.Tiles != null && this.GetBattleMember()!.Tiles.Count > 0) {
    //         tileList = this.GetBattleMember()!.Tiles.ToValueList();
    //         if(tileList.Count > 0) {
    //             swapSkill.SetSwapSkillCoolDown();
    //         }

    //     }
    //     return new SkillCommand(activeSkill, this, BattleProcessingStatic.PartySlotIsPlayerPartySlot, this.GetCombatTileController(), tileList, new());
    // }
#nullable disable

    #nullable enable
    public void SwapPartyMember(BattleManager battleController)
    {
        this.battleMember.SwapPartyMembers();
        this.SetBattleMemberGO(this.GetCombatTileController());
        // CharacterInfo? characterInfo = this.BattleEntity as CharacterInfo;
        // this.UpdateGauges();
        // this.CurrentState = BattleTurnResultStateEnums.Wait;
        this.GetCombatTileController().SetPartyOccupant(battleController, this);
        battleController.OnSwap();
        base.onBattleStatsChange?.Invoke(this);
    }
    #nullable disable

    public bool CanSwap()
    {
        BattleEntitySlot characterSlot = this.battleMember.GetCharacterSlot();
        return characterSlot != null && characterSlot.GetSwapCharacter() != null;
    }

    public void SetCharacter(BattleObject battleObject) {
        base.battleObject = battleObject;
    }

    private void SetBattleMemberGO(CombatTileController combatTileController) {
        if (this.battleMember.GetBattleEntity == null || this.battleMember.GetBattleEntity.GetBattleObject() == null) {
            return;
        }
        Debug.Log("Attempting to set animator");
        BattleObject battleObject = this.BattleEntityGO;
        this.battleObject = null;

        BattleObject partyMember = GameObject.Instantiate(this.battleMember.GetBattleEntity.GetBattleObject(), combatTileController.GetHeightForSpriteVector3Offset(this.battleMember.GetBattleEntity.CanFly()), battleObject.transform.rotation); 
        
        GameObject.Destroy(battleObject.gameObject);

        if (partyMember.TryGetComponent<PlayerBattleObject>(out PlayerBattleObject playerBattleObject)) {
            this.battleObject = playerBattleObject;
        }
        if (this.battleObject == null) {
            Debug.LogError("No Battle Object set.");
            return;
        }
        this.BattleEntityGO.SetBattleMember(this.battleMember);
    }
    
}
}