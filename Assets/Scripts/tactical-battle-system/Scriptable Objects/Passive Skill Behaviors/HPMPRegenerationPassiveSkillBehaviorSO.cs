using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace DPS.TacticalCombat {
[CreateAssetMenu(fileName = "HP MP Regeneration Passive Skill Behavior", menuName = "ScriptableObjects/Passive Skill/Behaviors/HP MP Regeneration Passive")]
public class HPMPRegenerationPassiveSkillBehaviorSO : PassiveSkillBehaviorSO
{

    [Tooltip("If true, will heal mp")]
    [Header("MP Regeneration")]
    [SerializeField]
    private bool canRecoverMP;

    [Tooltip("The mpAmount that should be regenerated.")]
    [SerializeField]
    [Range(1, 10)]
    private int mpAmount;


    [Tooltip("If true, will heal hp")]
    [Header("HP Regeneration")]
    [SerializeField]
    private bool canRecoverHP;

    [Tooltip("The hp % that should be regenerated.")]
    [SerializeField]
    [Range(0f, 1f)]
    private float hpHealPercentage;


    #nullable enable
    public override async Task ExecuteBeginPhasePassiveSkill(PartySlot partySlot, CombatTileController? combatTileController)
    {
        if (partySlot != null && partySlot.BattleEntity != null) {
            CharacterInfo? characterInfo = partySlot!.BattleEntity! as CharacterInfo;
            if (characterInfo != null) {
                RawStats rawStats = characterInfo.BattleRawStats!;

                if (canRecoverHP) {
                    this.RegenerateHP(rawStats);
                }

                if (canRecoverMP) {
                    this.RegenerateMP(rawStats);
                }
            }
        }
    }
    #nullable disable

    private void RegenerateMP(RawStats rawStats) {
        if(rawStats.mp < rawStats.maxMp) {
            int newMp = rawStats.mp + mpAmount;
            rawStats.mp = newMp > rawStats.maxMp ? rawStats.maxMp : newMp;
        }    
    }

    private void RegenerateHP(RawStats rawStats) {
        if(rawStats.hp < rawStats.maxHp) {
            int healAmount = (int) (this.hpHealPercentage * rawStats.maxHp);
            int newHP = rawStats.hp + healAmount;
            rawStats.hp = newHP > rawStats.maxHp ? rawStats.maxHp : newHP;
        }    
    }
}
}