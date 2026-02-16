using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;
using System.Threading.Tasks;

namespace DPS
{
        namespace TacticalCombat
        {
             [SerializeField]
                public interface IBattleEntity {
                        //Generate Battle Raw Stats
                        void GenerateBattleRawStats();
                        //Retrieve Battle Raw Stats
                        RawStats GetRawStats();
                        //Set new stats
                        // void SetRawStats(RawStats rawStats);
                        //Nullify Battle Raw Stats on Combat End
                        void CombatEndStats();
                        //Execute Ability on targets.  It also requires the partyMembers for handling logic in which there could only ever be 1 entity in the party with a certain status.  i.e. aggro
                        // void Ability(PartySlot actingPartyMember, OldActiveSkillSO activeSkill, List<PartySlot> primaryAbilityTargets, List<PartySlot> healedTargets, List<PartySlot> partyMembers);
                        // void UseItem(InventoryItemSO inventoryItem, List<PartySlot> partySlots);
                        //Reduce entity HP by damage
                        void TakeDamage(int damage);
                        void HealHP(int heal);
                        GenericDictionary<ElementSO, ElementalResistance> GetElements();

                        List<StatusEffect> GetStatusEffects();
                        
                        List<StatusEffect> GetBuffs();

                        List<StatusEffect> GetDebuffs();

                        void SetBuffs(List<StatusEffect> newBuffs);

                        //Increase stats, add a status, then add to Buff Tracker
                        void AddStatusEffect(StatusEffectSO statusEffect);

                        void RemoveStatusEffect(StatusEffectSO statusEffect);
                        //Decrease stats, add status ailment, then add to debuff tracker

                        void RegenerateBattleTerrainMovementOverrides();
                        
                        Sprite GetSpritePortrait();

                        void RegenerateBattleRawStats();


                        Poise GetPoiseState();
                        // void SetActionStatus(ActionStatus actionStatus);
                        // void SetAggro(bool isAggro);

                        // void ResetAggro();

                        // bool GetAggro();

                        bool IsDead();
                        void BeginPhase();
                        bool EndPhase(bool regainMP = false);
                        
                        
                        void SetInflictedAilment(StatusAilmentSO statusAilment);

                        #nullable enable
                        InflictedAilment? GetInflictedAilment();
                        #nullable disable

                        string GetName();

                        SkillInfo GetSkillInfo();

                        List<ActiveSkillSO> GetLearnedActiveSkills();

                        Task<List<ActiveSkillSO>> GetEquippedActiveSkills();

                        Movement GetMovement();

                        StatusAilmentResistances GetStatusAilmentResistances();

                        GenericDictionary<ElementSO, int> GetBattleTerrainMovementOverride();

                        void SetBattleTerrainMovementOverride(GenericDictionary<ElementSO, int> terrainMovementOverride);

                        void SetTerrainMovementOverride(GenericDictionary<ElementSO, int> terrainMovementOverride);

                        bool CanFly();

                        void SetFly(bool canFly);

                        bool IsDowned();

                        bool IsDefending();

                        void SetDefending(bool isDefending);
                        
                        bool CanBeDisplaced();

                        void SetCanBeDisplaced(bool canBeDisplaced);

                        #nullable enable

                        BattleObject? GetBattleObject();
                        #nullable disable
                }
                
        }
}
