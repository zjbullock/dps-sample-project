using UnityEngine;

namespace DPS
{
    namespace TacticalCombat
    {
        [System.Serializable]
public class BattleEntitySlot {


    [SerializeReference]
    private IBattleEntity frontLine;
    #nullable enable
    [SerializeReference]
    private IBattleEntity? swapCharacter = null;

    public BattleEntitySlot(IBattleEntity frontLineCharacterInfo, IBattleEntity? backLineCharacterInfo = null) {
        this.frontLine = frontLineCharacterInfo;
        this.swapCharacter = null;
        if (backLineCharacterInfo != null) {
            this.swapCharacter = backLineCharacterInfo;
        }
    }

    // public BattleEntitySlot(SerializedCharacterSlot serializedCharacterSlot) {
    //     this.frontLine = new CharacterInfo(serializedCharacterSlot.frontLine);
    //     this.swapCharacter = null;
    //     if (serializedCharacterSlot.swapPartner != null) {
    //         this.swapCharacter = new CharacterInfo(serializedCharacterSlot.swapPartner);
    //     }
    // }

    public BattleEntitySlot(BattleEntitySlot characterSlot) {
        this.frontLine = characterSlot.GetFrontLineCharacter();
        this.swapCharacter = null;
        if (characterSlot.GetSwapCharacter() != null) {
            this.swapCharacter = characterSlot.GetSwapCharacter();
        }
    }

    public void GenerateBattleRawStats()
    {
        this.frontLine.GenerateBattleRawStats();
        if (this.swapCharacter == null)
        {
            return;
        }
        this.swapCharacter.GenerateBattleRawStats();
    }

    public void CombatEndStats() {
        Debug.Log("PErforming Combat End Stats in Character Slot for FrontLine");
        Debug.Log("Front Line name: " + this.frontLine.GetName());
        this.frontLine.CombatEndStats();
        if (this.swapCharacter != null) {
            Debug.Log("Performing Combat End Stats in Character Slot for Swap Character");
            Debug.Log(this.swapCharacter);
            Debug.Log("Swap Character name: " + this.swapCharacter.GetName());
            this.swapCharacter.CombatEndStats();
        }
    }
    /**
        Retrieve the old member, set the new backline member, then return the old swap partner.
    */
    public IBattleEntity? SetSwapPartner(IBattleEntity battleEntity) {
        IBattleEntity? oldSwapPartner = null;
        if (this.swapCharacter != null) {
            oldSwapPartner = this.swapCharacter;
        }
        this.swapCharacter = battleEntity;
        return oldSwapPartner;
    }

    public IBattleEntity? SwapPartners() {
        if(this.swapCharacter == null) {
            return null;
        }

        IBattleEntity oldFrontLine = this.frontLine;
        this.frontLine = this.swapCharacter;
        this.swapCharacter = oldFrontLine;
        return this.frontLine;
    }

    

    /**
        Clear out the swap partner, and retrieve the member.
    */
    public IBattleEntity? ClearSwapPartner() {
        CharacterInfo? characterInfo = this.swapCharacter! as CharacterInfo;
        this.swapCharacter = null;
        return characterInfo;
    }
    

    public IBattleEntity GetFrontLineCharacter() {
        return this.frontLine;
    }

    public IBattleEntity? GetSwapCharacter() {
        return this.swapCharacter;
    }
    
}
    }
}
