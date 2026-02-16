using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;
using System.Threading.Tasks;

namespace DPS.TacticalCombat
{
 [System.Serializable]
public class SpawnedObjectInfo : IBattleEntity
{   

    public SpawnedObjectInfo(SpawnableObjectInfoSO spawnableObjectInfo) {
        this.rawStats = new RawStats(spawnableObjectInfo.rawStats);
        this.elementalResistances = spawnableObjectInfo.elementalResistances;
        this.spawnedObjectName = spawnableObjectInfo.spawnedObjectName;
        this.reactionSkills = new GenericDictionary<ElementSO, ActiveSkillSO>();
        foreach(KeyValuePair<ElementSO, ActiveSkillSO> keyValuePair in spawnableObjectInfo.reactionSkills) {
            this.reactionSkills[keyValuePair.Key] = keyValuePair.Value;
        }
        this.onDeathSkill = spawnableObjectInfo.onDeathSkill;
        this.onAttackedSkill = spawnableObjectInfo.onAttackedSkill;
        this.canBeDisplaced = spawnableObjectInfo.canBeDisplaced;
    }

    [SerializeField]
    private RawStats rawStats;

    [SerializeField]
    private GenericDictionary<ElementSO, ElementalResistance> elementalResistances = new GenericDictionary<ElementSO, ElementalResistance>{};

    [SerializeField]
    private string spawnedObjectName;

    [SerializeField]
    private GenericDictionary<ElementSO, ActiveSkillSO> reactionSkills;

    [SerializeField]
    private ActiveSkillSO onDeathSkill;

    [SerializeField]
    private ActiveSkillSO onAttackedSkill;

    [SerializeField]
    private bool canBeDisplaced;

    public void ActivateEquipmentChangeAbilities(RawStats equipmentStats)
    {
       return;
    }

    public void AddStatusEffect(StatusEffectSO buff)
    {
        return;
    }

    public void BeginPhase()
    {
        return;
    }

    public SkillInfo GetSkillInfo() {
        return null;
    }

    

    #nullable enable
    public ActiveSkillSO? GetReactionSkill(ElementSO incomingSkill) {
        if (this.reactionSkills.TryGetValue(incomingSkill, out ActiveSkillSO value)) {
            return value;
        }
        return null;
    }

    public ActiveSkillSO? GetOnDeathSkill() {
        return this.onDeathSkill;
    }

    public ActiveSkillSO? GetOnAttacked() {
        return this.onAttackedSkill;
    }
    #nullable disable
    
    public bool CanFly()
    {
       return false;
    }

    public RuntimeAnimatorController GetAnimator() {
        return null;
    }

    public void CombatEndStats()
    {
        return;
    }

    public bool EndPhase(bool regainMP = false)
    {
        return true;
    }

    public void GenerateBattleRawStats()
    {
        return;
    }

    public List<StatusEffect> GetStatusEffects()
    {
        return new();
    }

    public List<StatusEffect> GetBuffs()
    {
        return new();
    }

    public  List<StatusEffect> GetDebuffs()
    {
        return new();
    }

    public Poise GetPoiseState()
    {
        return new();
    }

    public GenericDictionary<ElementSO, ElementalResistance> GetElements()
    {
        return this.elementalResistances;
    }

    public InflictedAilment GetInflictedAilment()
    {
        return null;
    }

    public string GetLetterDesignation()
    {
        return null;
    }

    #nullable enable
    public BattleObject? GetBattleObject() {
        return null;
    }
    #nullable disable
    
    public Movement GetMovement()
    {
        return null;
    }

    public string GetName()
    {
        return this.spawnedObjectName;
    }

    public RawStats GetRawStats()
    {
        return this.rawStats;
    }

    public ResolveGaugeSO GetResolveGauge()
    {
        return null;
    }

    public int GetResolvePoints()
    {
        return 0;
    }

    public Sprite GetSpritePortrait()
    {
        return null;
    }

    public StatusAilmentResistances GetStatusAilmentResistances()
    {
        return null;
    }

    public Vector3 GetTargetingPositionOffset()
    {
        return new Vector3();
    }

    public GenericDictionary<ElementSO, int> GetBattleTerrainMovementOverride()
    {
        return null;
    }

    public void SetBattleTerrainMovementOverride(GenericDictionary<ElementSO, int> terrainOverrides) {
        return;
    }

    public void SetTerrainMovementOverride(GenericDictionary<ElementSO, int> terrainOverrides) {
        return;
    }

    public void HealHP(int heal)
    {
        return;
    }

    public bool IsDead()
    {
        return this.rawStats.hp <= 0;
    }

    public bool IsDowned()
    {
        return false;
    }

    public void RegenerateBattleRawStats()
    {
        return;
    }

    public void RemoveStatusEffect(StatusEffectSO statusEffect)
    {
        return;
    }


    // public void SetActionStatus(ActionStatus actionStatus)
    // {
    //    return;
    // }

    public void SetBuffs(List<StatusEffect> newBuffs)
    {
        return;
    }


    public void SetFly(bool canFly)
    {
        return;
    }

    public void SetInflictedAilment(StatusAilmentSO statusAilment)
    {
        return;
    }

    public void SetResolvePoints(int resolvePoints)
    {
        return;
    }

    public void TakeDamage(int damage)
    {

        this.rawStats!.hp = ((this.rawStats.hp - damage) + (int) System.Math.Abs(this.rawStats.hp - damage)) / 2;
    }

    public void RegenerateBattleTerrainMovementOverrides()
    {
        return;
    }

    public bool CanBeDisplaced()
    {
        return this.canBeDisplaced;
    }

    public void SetCanBeDisplaced(bool canBePulled)
    {
        return;
    }

    public List<ActiveSkillSO> GetLearnedActiveSkills()
    {
        return this.GetSkillInfo().GetLearnedActiveSkills();
    }

    public async Task<List<ActiveSkillSO>> GetEquippedActiveSkills()
    {
        return this.GetSkillInfo().EquippedSkills;
    }

    public bool IsDefending()
    {
        return false;
    }

    public void SetDefending(bool isDefending)
    {
        return;
    }
}
   
}
