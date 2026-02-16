using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DPS.Common;
using System.Threading.Tasks;

namespace DPS
{
    namespace TacticalCombat
    {
        [Serializable]
        public abstract class BattleEntityInfo: IBattleEntity
        {
        public BattleEntityInfo() {

        }

        public RawStats RawStats;

        public RawStats BattleRawStats;
        // public ActionStatus ActionStatus;

        public Sprite spritePortrait;

        public Movement movement;

        public List<StatusEffect> StatusEffects = new();

        [SerializeField]
        protected GenericDictionary<ElementSO, int> terrainMovementOverrides = new GenericDictionary<ElementSO, int>();

        [SerializeField]
        protected GenericDictionary<ElementSO, int> battleTerrainMovementOverrides = new GenericDictionary<ElementSO, int>();

        #nullable enable
        public InflictedAilment? InflictedAilment;
        #nullable disable

        [SerializeField]
        protected bool isFlying;

        [SerializeField]
        protected bool _isDefending;

        [SerializeField]
        protected bool canBeDisplaced;

        public GenericDictionary<ElementSO, ElementalResistance> Elements;
        protected Poise poise;


        [SerializeField]
        protected StatusAilmentResistances statusAilmentResistances;
        

        // [SerializeField]
        // protected RuntimeAnimatorController animatorController;

        // public RuntimeAnimatorController animator { set => this.animatorController = value; get => this.animatorController; }

    public void AddStatusEffect(StatusEffectSO statusEffect) {
            this.StatusEffects = statusEffect.PruneOverlappingStatusEffects(this.StatusEffects);
            if(this.StatusEffects.Find((buff) => { return buff.statusEffect == statusEffect; }) == null) {
                Debug.Log(this.GetName());
                Debug.Log("Adding Status Effect: " + statusEffect.statusName);
                StatusEffect newStatus = new StatusEffect(statusEffect);
                this.StatusEffects.Add(newStatus);
            }

            this.RegenerateBattleRawStats();
        }
        
        public void RemoveStatusEffect(StatusEffectSO statusEffectSO)
        {
            this.StatusEffects.RemoveAll((statusEffect) => { return statusEffect.statusEffect == statusEffectSO; });
            this.RegenerateBattleRawStats();
        }

        public void SetInflictedAilment(StatusAilmentSO statusAilment) {
            if (statusAilment != null) {
                this.InflictedAilment = new InflictedAilment(statusAilment);
            } else {
                this.InflictedAilment = null;
            }
        }
        #nullable enable

        public InflictedAilment? GetInflictedAilment() {
            return this.InflictedAilment;
        }

        public void BeginPhase() {
            this.SetDefending(false);
            if(this.BattleRawStats.mp < this.BattleRawStats.maxMp) {
                this.BattleRawStats.mp++;
            }
        }

        public virtual void RegenerateBattleRawStats() {
            //Regenerates Battle Raw Stats.
            int currentHP = this.BattleRawStats.hp;
            int currentMP = this.BattleRawStats.mp;
            this.BattleRawStats = new RawStats(this.RawStats);
            this.RegenerateBattleTerrainMovementOverrides();

            foreach(StatusEffect buff in this.StatusEffects) {
                buff.statusEffect.PreStatMultiplierBuffs(this);
            }

            foreach(StatusEffect buff in this.StatusEffects) {
                buff.statusEffect.ProcessBuffList(this);
            }

            this.BattleRawStats.hp = currentHP > this.BattleRawStats.maxHp ? this.BattleRawStats.maxHp : currentHP;
            this.BattleRawStats.mp = currentMP > this.BattleRawStats.maxMp ? this.BattleRawStats.maxMp : currentMP;
        }

        public void RegenerateBattleTerrainMovementOverrides() {
            this.battleTerrainMovementOverrides = new GenericDictionary<ElementSO, int>();
            foreach(KeyValuePair<ElementSO, int> terrainMovementOverride in this.terrainMovementOverrides) {
                this.battleTerrainMovementOverrides.Add(terrainMovementOverride);
            }
        }


        public void SetInflictedAilment(InflictedAilment ailment) {
            this.InflictedAilment = ailment;
        }

        public List<StatusEffect> GetStatusEffects() {
            return this.StatusEffects;
        }

        public List<StatusEffect> GetBuffs()
        {
            return this.StatusEffects.FindAll((statusEffect) => { return statusEffect.statusEffect.StatusEffectType == StatusEffectSO.StatusEffectTypes.Buff; });
        }

        public List<StatusEffect> GetDebuffs()
        {
            return this.StatusEffects.FindAll((statusEffect) => { return statusEffect.statusEffect.StatusEffectType == StatusEffectSO.StatusEffectTypes.Debuff; });
        }

        public void SetBuffs(List<StatusEffect> newBuffs)
        {
            // Debug.Log(this.GetName());
            // Debug.Log("Setting new buffs");
            this.StatusEffects = newBuffs;
        }


        private List<StatusEffect> processStatusEffects() {
            List<StatusEffect> liveStatusEffects = new();
            
            foreach (StatusEffect statusEffect in this.StatusEffects)
            {
                if (statusEffect.statusEffect.infiniteTurns)
                {
                    liveStatusEffects.Add(statusEffect);
                    continue;
                }
                statusEffect.turnCount--;
                if (statusEffect.turnCount > 0)
                {
                    liveStatusEffects.Add(statusEffect);
                }
                else
                {
                    statusEffect.statusEffect.OnRemoveStatusEffect(this);
                }
            }
            return liveStatusEffects;
        }

        private void removeAllStatusEffects() {
            foreach (StatusEffect buffEntry in this.StatusEffects) {
                buffEntry.statusEffect.OnRemoveStatusEffect(this);
            }
            this.StatusEffects.Clear();
        }

        public virtual bool EndPhase(bool regainMP = false) {
            if(this.IsDead()) {
                return true;
            }
            Debug.Log("end phase for: " + this.GetName());
            this.StatusEffects = this.processStatusEffects();
            this.RegenerateBattleRawStats();

            if (this.InflictedAilment != null && this.InflictedAilment.statusAilment != null) {
                Debug.Log("Here be inflicted ailment");
                Debug.Log(this.InflictedAilment);
                this.InflictedAilment.turnCount++;
                if(!this.InflictedAilment.statusAilment.isInfiniteTurnCount && this.InflictedAilment.turnCount >= this.InflictedAilment.statusAilment.minTurnCount) {
                    System.Random random = new System.Random();
                    if (random.Next(1, 100) <= 10 ||
                        (this.InflictedAilment.turnCount >= this.InflictedAilment.statusAilment.maxTurnCount)) {
                        this.InflictedAilment = null;
                    }
                }
            }
            if(regainMP && this.BattleRawStats!.mp < this.BattleRawStats.maxMp) {
                this.BattleRawStats.mp++;
            }
            return false;
        }

        public virtual void TakeDamage(int damage) {
            // this.BattleRawStats!.hp = ((this.BattleRawStats.hp - damage) + (int) Math.Abs(this.BattleRawStats.hp - damage)) / 2;
            this.BattleRawStats!.hp = DPS.Common.GeneralUtilsStatic.SubtractNoNegatives(this.BattleRawStats!.hp, damage);
            if (this.IsDead()) {
                this.removeAllStatusEffects();
                this.RegenerateBattleRawStats();
                return;
            }
        }

        public void HealHP(int heal) {
            this.BattleRawStats!.hp += heal;
            if (this.BattleRawStats!.hp > this.BattleRawStats!.maxHp) {
                this.BattleRawStats!.hp = this.BattleRawStats!.maxHp;
            }
        }

        public bool CanFly() {
            return this.isFlying;
        }

        public void SetFly(bool canFly) {
            this.isFlying = canFly;
        }

        public bool CanBeDisplaced() {
            return this.canBeDisplaced;
        }   

        public void SetCanBeDisplaced(bool canBePulled) {
            this.canBeDisplaced = canBePulled;
        }

        public GenericDictionary<ElementSO, int> GetBattleTerrainMovementOverride() {
            return this.battleTerrainMovementOverrides;
        }

        public void SetBattleTerrainMovementOverride(GenericDictionary<ElementSO, int> terrainMovementOverride) {
            this.battleTerrainMovementOverrides = terrainMovementOverride;
        }

        public void SetTerrainMovementOverride(GenericDictionary<ElementSO, int> terrainMovementOverrides) {
            foreach (var terrainMovementOverride in terrainMovementOverrides) {
                if (this.terrainMovementOverrides.ContainsKey(terrainMovementOverride.Key) && terrainMovementOverride.Value > this.terrainMovementOverrides[terrainMovementOverride.Key])
                {
                    this.terrainMovementOverrides[terrainMovementOverride.Key] = terrainMovementOverride.Value;
                }
                else
                {
                    this.terrainMovementOverrides[terrainMovementOverride.Key] = terrainMovementOverride.Value;
                }

            }
        }

        public GenericDictionary<ElementSO, int> GetTerrainMovementOverride() {
            return this.terrainMovementOverrides;
        }

        // public RuntimeAnimatorController GetAnimator() {
        //     return this.animatorController;
        // }

        public abstract BattleObject? GetBattleObject();

        public virtual void GenerateBattleRawStats() {
            this.BattleRawStats = new RawStats(this.RawStats);
            this.battleTerrainMovementOverrides = new GenericDictionary<ElementSO, int>();
            if (this.terrainMovementOverrides != null) {
                foreach(KeyValuePair<ElementSO, int> terrainMovementOverride in this.terrainMovementOverrides) {
                    this.battleTerrainMovementOverrides.Add(terrainMovementOverride);
                }
            }

        }

        public StatusAilmentResistances GetStatusAilmentResistances() {
            return this.statusAilmentResistances;
        }

        public virtual RawStats GetRawStats() {
            return this.BattleRawStats;
        }

        public GenericDictionary<ElementSO, ElementalResistance> GetElements() {
            return this.Elements;
        }

        public Poise GetPoiseState() {
            return this.poise;
        }

        public void ProgressPoiseBreakState() {
            this.poise.ProgressPoiseBrokenState();
        }


        // public ActionStatus GetActionStatus() {
        //     return this.ActionStatus;
        // }

        public Movement GetMovement() {
            return this.movement;
        }

        // public void SetActionStatus(ActionStatus actionStatus) {
        //     this.ActionStatus = actionStatus;
        // }

        public virtual bool IsDead() {
            return this.BattleRawStats == null || this.BattleRawStats.hp <= 0;
        }

        public bool IsDowned() {
            return this.poise.IsPoiseBroken();
        }

        public bool IsDefending()
        {
            return this._isDefending;
        }

        public void SetDefending(bool isDefending)
        {
            this._isDefending = isDefending;
        }
        
        public Sprite GetSpritePortrait()
        {
            return this.spritePortrait;
        }

        #nullable enable
        public virtual SkillInfo? GetSkillInfo() {
            return null;
        }
        #nullable disable


        public virtual void CombatEndStats() { return; }

        // public abstract ResolveGaugeSO GetResolveGauge();
        // public abstract int GetResolvePoints();
        // public abstract void SetResolvePoints(int resolvePoints);
        public abstract string GetName();

        public abstract List<ActiveSkillSO> GetLearnedActiveSkills();

        public abstract Task<List<ActiveSkillSO>> GetEquippedActiveSkills();
    }

    }
}
