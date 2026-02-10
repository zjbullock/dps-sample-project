using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DPS.TacticalCombat {

[CreateAssetMenu(fileName = "Passive_Skill_", menuName = "ScriptableObjects/Passive Skill/Passive Skill")]
public class PassiveSkillSO : ScriptableObject, IBattleActionDescription {
    [SerializeField]
    private string skillName;

    [TextArea(15,20)]
    [SerializeField]
    public string description;

    


    [Tooltip("The list of passive skill behaviors that will activate")]
    [SerializeField]
    List<PassiveSkillBehaviorSO> passiveSkillBehaviors = new List<PassiveSkillBehaviorSO>();


#nullable enable

    public virtual void ExecutePostEquipmentStatAddPassiveSkill(CharacterInfo characterInfo)
    {
        if (this.passiveSkillBehaviors.Count == 0)
        {
            return;
        }

        foreach (PassiveSkillBehaviorSO passiveSkill in this.passiveSkillBehaviors)
        {
            passiveSkill.ExecutePostEquipmentStatAddPassiveSkill(characterInfo);
        }
    }

    /**
        Duplicate of above functions that accept a PartySlot.  These can be used for additional behaviors that may occur outside of combat.
    */
    public virtual void ExecuteHPChangePassiveSkill(PartySlot partySlot,  CombatTileController? combatTileController) {
        if (this.passiveSkillBehaviors.Count > 0) {
            foreach(PassiveSkillBehaviorSO passiveSkill in this.passiveSkillBehaviors) {
                passiveSkill.ExecuteHPChangePassiveSkill(partySlot, combatTileController);
            }
        }
    }

    public virtual void ExecuteEndPhasePassiveSkill(PartySlot partySlot, CombatTileController? combatTileController) {
        if (this.passiveSkillBehaviors.Count > 0) {
            foreach(PassiveSkillBehaviorSO passiveSkill in this.passiveSkillBehaviors) {
                passiveSkill.ExecuteEndPhasePassiveSkill(partySlot, combatTileController);
            }
        }
    }

    public virtual void ExecuteEndCombatPassiveSkill(PartySlot partySlot) {
        if (this.passiveSkillBehaviors.Count > 0) {
            foreach(PassiveSkillBehaviorSO passiveSkill in this.passiveSkillBehaviors) {
                passiveSkill.ExecuteEndCombatPassiveSkill(partySlot);
            }
        }
    }

    public virtual void ExecuteBeginCombatPassiveSkill(PartySlot partySlot, CombatTileController? combatTileController) {
        if (this.passiveSkillBehaviors.Count > 0) {
            foreach(PassiveSkillBehaviorSO passiveSkill in this.passiveSkillBehaviors) {
                passiveSkill.ExecuteBeginCombatPassiveSkill(partySlot, combatTileController);
            }
        }
    }

    public virtual void ExecutePostCommandAbilities(PartySlot partySlot,  CombatTileController? combatTileController){
        if (this.passiveSkillBehaviors.Count > 0) {
            foreach(PassiveSkillBehaviorSO passiveSkill in this.passiveSkillBehaviors) {
                passiveSkill.ExecutePostCommandAbilities(partySlot, combatTileController);
            }
        }
    }

    public virtual void ExecuteBeginPhasePassiveSkill(PartySlot partySlot, CombatTileController? combatTileController){
        if (this.passiveSkillBehaviors.Count > 0) {
            foreach(PassiveSkillBehaviorSO passiveSkill in this.passiveSkillBehaviors) {
                passiveSkill.ExecuteBeginPhasePassiveSkill(partySlot, combatTileController);
            }
        }
    }

    public virtual void ExecutePostMovePassiveSkill(PartySlot partySlot, CombatTileController? combatTileController){
        if (this.passiveSkillBehaviors.Count > 0) {
            foreach(PassiveSkillBehaviorSO passiveSkill in this.passiveSkillBehaviors) {
                passiveSkill.ExecutePostMovePassiveSkill(partySlot, combatTileController);
            }
        }
    }

    public virtual void ExecutePostEvadePassiveSkill(PartySlot partySlot, CombatTileController? combatTileController) {
        if (this.passiveSkillBehaviors.Count == 0) {
            return;
        }

        foreach(PassiveSkillBehaviorSO passiveSkill in this.passiveSkillBehaviors) {
            passiveSkill.ExecutePostEvadePassiveSkill(partySlot, combatTileController);
        }
    }

    public virtual void ExecutePostBlockPassiveSkill(PartySlot partySlot, CombatTileController? combatTileController) {
        if (this.passiveSkillBehaviors.Count == 0) {
            return;
        }

        foreach(PassiveSkillBehaviorSO passiveSkill in this.passiveSkillBehaviors) {
            passiveSkill.ExecutePostBlockPassiveSkill(partySlot, combatTileController);
        }
    }

    public string GetActionName()
    {
        return this.skillName;
    }

    public string GetDescription()
    {
        return this.description;
    }
}
}