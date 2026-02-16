using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace DPS.TacticalCombat {
public abstract class PassiveSkillBehaviorSO : ScriptableObject
{


    public virtual async Task ExecutePostEquipmentStatAddPassiveSkill(CharacterInfo characterInfo)
    {
        return;
    }
    
    /**
        Duplicate of above functions that accept a PartySlot.  These can be used for additional behaviors that may occur outside of combat.
    */
    #nullable enable
    public virtual async Task ExecuteHPChangePassiveSkill(PartySlot partySlot, CombatTileController? combatTileController)
    {
        return;
    }

    public virtual async Task ExecuteEndPhasePassiveSkill(PartySlot partySlot, CombatTileController? combatTileController) {
        return;
    }

    public virtual async Task ExecutePostMovePassiveSkill(PartySlot partySlot, CombatTileController? combatTileController) {
        return;
    }

    public virtual async Task ExecuteEndCombatPassiveSkill(PartySlot partySlot) {
        return;
    }

    public virtual async Task ExecutePostCommandAbilities(PartySlot partySlot,  CombatTileController? combatTileController){
        return;
    }

    public virtual async Task ExecuteBeginPhasePassiveSkill(PartySlot partySlot, CombatTileController? combatTileController){
        return;
    }

    public virtual async Task ExecuteBeginCombatPassiveSkill(PartySlot partySlot, CombatTileController? combatTileController) {
        return;
    }

    public virtual async Task ExecutePostBlockPassiveSkill(PartySlot partySlot, CombatTileController? combatTileController) {
        return;
    }

    public virtual async Task ExecutePostEvadePassiveSkill(PartySlot partySlot, CombatTileController? combatTileController) {
        return;
    }
}
}