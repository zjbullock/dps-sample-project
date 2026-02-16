using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;
using System.Threading.Tasks;

namespace DPS.TacticalCombat
{
  [System.Serializable]
public class SummonedEntityInfo : BattleEntityInfo, IBattleEntity
{   
    public SummonedEntityInfo(SummonedEntityInfoSO summonedEntityInfoSO) {
        this.Name = summonedEntityInfoSO.Name;
        this.Level = summonedEntityInfoSO.Level;
        this.Elements = new GenericDictionary<ElementSO, ElementalResistance>();
        foreach(KeyValuePair<ElementSO, ElementalResistance> keyValuePair in summonedEntityInfoSO.Elements) {
            this.Elements.Add(keyValuePair);
        }

        this.skills = new SkillInfo(summonedEntityInfoSO.SkillInfoSO);

        this.poise = new Poise(summonedEntityInfoSO.Poise);

        this.RawStats = new RawStats(summonedEntityInfoSO.BaseRawStats);

        this.BattleRawStats = new RawStats(this.RawStats);

        this.InflictedAilment = null;

        this.spritePortrait = summonedEntityInfoSO.characterSpritePortrait;

        this.movement = new Movement(summonedEntityInfoSO.speed, summonedEntityInfoSO.verticalSpeed);

        this.statusAilmentResistances = new StatusAilmentResistances(summonedEntityInfoSO.statusAilmentResistances);

        this.terrainMovements = new GenericDictionary<ElementSO, int>();
        foreach(KeyValuePair<ElementSO, int> elementMovement in summonedEntityInfoSO.terrainMovementOverride) {
            this.terrainMovements.Add(elementMovement);
        }

        this.battleTerrainMovements = new GenericDictionary<ElementSO, int>();

        this.isFlying = false;

        this.turnDuration = summonedEntityInfoSO.turnDuration;
    }

    [SerializeField]
    private string Name;

    [SerializeField]
    public  int Level;

    #nullable disable


    private int resolvePoints;

    // public int maxResolvePoints;

    #nullable enable
    private ResolveGaugeSO? resolveGauge;
    //Takes 2x damage
    //Takes 2x damage AND Poise Gauge Decreased
    // public List<ElementSO> Weaknesses;
    // //Take only 1/2 damage
    // public List<ElementSO> Resistances;
    // //Completely blocks the damage
    // public List<ElementSO> Nullifies;
    // //Drain the incoming damage as HP
    // public List<ElementSO> Absorbs;


    [Tooltip("The length of time in turns that this summoned entity is capable of lasting")]
    [SerializeField]
    private int turnDuration;

    [SerializeField]
    private SkillInfo skills;


    [SerializeField]
    private GenericDictionary<ElementSO, int> terrainMovements;

    [SerializeField]
    private GenericDictionary<ElementSO, int> battleTerrainMovements;

    public override SkillInfo GetSkillInfo() {
        return this.skills;
    }

    public override bool EndPhase(bool regainMP = false) {
        if( base.EndPhase(regainMP)) {
            return true;
        }
        this.turnDuration--;
        if(this.turnDuration <= 0) {
            this.BattleRawStats.hp = 0;
        }
        return true;
    }

    public override string GetName()
    {
        return this.Name;
    }

    public  void RegenerateBattleTerrainMovements() {
        this.battleTerrainMovements = new GenericDictionary<ElementSO, int>();
        foreach(KeyValuePair<ElementSO, int> terrainMovement in this.terrainMovements) {
            this.battleTerrainMovements.Add(terrainMovement);
        }
    }

    #nullable enable
    public override BattleObject? GetBattleObject()
    {
        return null;
    }

    public override List<ActiveSkillSO> GetLearnedActiveSkills()
    {
        return this.GetSkillInfo().GetLearnedActiveSkills();
    }

    public override async Task<List<ActiveSkillSO?>> GetEquippedActiveSkills()
    {
        return this.GetSkillInfo().EquippedSkills ?? new();
    }
#nullable disable
}
  
}
