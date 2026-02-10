using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat {
public abstract class PassiveSkillBehaviorSO : ScriptableObject
{


    public virtual void ExecutePostEquipmentStatAddPassiveSkill(CharacterInfo characterInfo)
    {
        return;
    }
    
    /**
        Duplicate of above functions that accept a PartySlot.  These can be used for additional behaviors that may occur outside of combat.
    */
    #nullable enable
    public virtual void ExecuteHPChangePassiveSkill(PartySlot partySlot, CombatTileController? combatTileController)
    {
        return;
    }

    public virtual void ExecuteEndPhasePassiveSkill(PartySlot partySlot, CombatTileController? combatTileController) {
        return;
    }

    public virtual void ExecutePostMovePassiveSkill(PartySlot partySlot, CombatTileController? combatTileController) {
        return;
    }

    public virtual void ExecuteEndCombatPassiveSkill(PartySlot partySlot) {
        return;
    }

    public virtual void ExecutePostCommandAbilities(PartySlot partySlot,  CombatTileController? combatTileController){
        return;
    }

    public virtual void ExecuteBeginPhasePassiveSkill(PartySlot partySlot, CombatTileController? combatTileController){
        return;
    }

    public virtual void ExecuteBeginCombatPassiveSkill(PartySlot partySlot, CombatTileController? combatTileController) {
        return;
    }

    public virtual void ExecutePostBlockPassiveSkill(PartySlot partySlot, CombatTileController? combatTileController) {
        return;
    }

    public virtual void ExecutePostEvadePassiveSkill(PartySlot partySlot, CombatTileController? combatTileController) {
        return;
    }
}
}