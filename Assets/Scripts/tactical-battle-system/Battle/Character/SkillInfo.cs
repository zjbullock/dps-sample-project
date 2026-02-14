using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DPS.TacticalCombat{
[Serializable]
#nullable enable
public class SkillInfo
{
    public SkillInfo(
        ActiveSkillSO attackSkill,
        ActiveSkillSO defendSkill,
        SwapSkill swapSkill,
        List<SkillTier> skillTiers,
        List<ActiveSkillSO> resolveSkills,
        List<ActiveSkillSO?> equippedSkills
    )
    {
        // this.SwapSkill = new();
        // this.skillTiers = new List<SkillTier>();
        // this.ResolveSkills = new();

        this.AttackSkill = attackSkill;
        this.DefendSkill = defendSkill;
        this.SwapSkill = swapSkill;
        this.skillTiers = skillTiers;
        this.ResolveSkills = resolveSkills;
        this.EquippedSkills = equippedSkills;
    }

    public SkillInfo(StartingSkills skillInfo)
    {

        this.AttackSkill = skillInfo.AttackSkill;

        this.DefendSkill = skillInfo.DefendSkill;

        this.SwapSkill = new SwapSkill(skillInfo.DefaultSwapSkill, skillInfo.SpecialSwapSkill);

        this.skillTiers = new();

        foreach (SkillTier skillTier in skillInfo.skillTiers)
        {
            this.skillTiers.Add(new(skillTier));
        }

        this.ResolveSkills = new(skillInfo.ResolveSkills);

        this.EquippedSkills = new(new ActiveSkillSO[6]);


        if (skillInfo.StartingEquippedSkills != null && skillInfo.StartingEquippedSkills.Count > 0)
        {
            for (int i = 0; i < skillInfo.StartingEquippedSkills.Count; i++)
            {
                this.EquippedSkills[i] = skillInfo.StartingEquippedSkills[i];
            }
        }
    }

    public ActiveSkillSO AttackSkill;

    public ActiveSkillSO DefendSkill;
    public SwapSkill SwapSkill;

    [SerializeField]
    private List<SkillTier> skillTiers;

    public ReadOnlyCollection<SkillTier> SkillTiers { get => this.skillTiers.AsReadOnly(); }

    public List<ActiveSkillSO?> EquippedSkills = new(new ActiveSkillSO[6]);

    public List<ActiveSkillSO> ResolveSkills;

    public void EquipActiveSkill(ActiveSkillSO activeSkill, int skillSlot)
    {
        if (EquippedSkills == null || skillSlot >= EquippedSkills.Count)
        {
            return;
        }

        EquippedSkills[skillSlot] = activeSkill;
    }

    public void UnEquipActiveSkill(int skillSlot)
    {
        if (EquippedSkills == null || skillSlot >= EquippedSkills.Count)
        {
            return;
        }

        EquippedSkills[skillSlot] = null;
    }

    public List<ActiveSkillSO?> GetEquippedSkills()
    {
        List<ActiveSkillSO?> activeSkills = new List<ActiveSkillSO?>();

        if (this.EquippedSkills != null)
            activeSkills.AddRange(this.EquippedSkills);


        return activeSkills;
    }


    private bool AlreadyPossessesActiveSkill(ActiveSkillSO activeSkillSO)
    {

        bool HasLearnedSkill(ActiveSkillSO activeSkillSO)
        {
            foreach (SkillTier skillTier in this.skillTiers)
            {
                if (skillTier.IsSkillLearned(activeSkillSO))
                {
                    return true;
                }
            }

            return false;
        }

        if (HasLearnedSkill(activeSkillSO))
        {
            return true;
        }
        else if (this.ResolveSkills != null && this.ResolveSkills.Contains(activeSkillSO))
        {
            return true;
        }
        return false;
    }

    public List<ActiveSkillSO> GetLearnedActiveSkills()
    {
        List<ActiveSkillSO> learnedSkills = new();
        foreach (SkillTier skillTier in this.skillTiers)
        {
            learnedSkills.AddRange(skillTier.GetLearnedActiveSkills());
        }
        return learnedSkills;
    }

    public List<PassiveSkillSO> PassiveSkills { get => this.GetLearnedPassiveSkills();  }

    public List<PassiveSkillSO> GetLearnedPassiveSkills()
    {
        List<PassiveSkillSO> learnedSkills = new();
        foreach (SkillTier skillTier in this.skillTiers)
        {
            learnedSkills.AddRange(skillTier.GetLearnedPassiveSkills());
        }
        return learnedSkills;
    }

    public List<StatIncreaseSkillSO> LearnedStatSkills { get => this.GetLearnedStatSkills();  }

    private List<StatIncreaseSkillSO> GetLearnedStatSkills()
    {
        List<StatIncreaseSkillSO> learnedSkills = new();
        foreach (SkillTier skillTier in this.skillTiers)
        {
            learnedSkills.AddRange(skillTier.GetLearnedStatSkills());
        }
        return learnedSkills;
    }

    public void LearnPassiveSkill(SkillTier skillTier)
    {
    }

}
#nullable disable

[Serializable]
public class SkillTierBase {

    [Serializable]
    public abstract class SkillNode
    {
        [SerializeField]
        protected bool isLearned = false;

        public bool IsLearned { get => this.isLearned; set => this.isLearned = value; }

        public abstract string GetSkillName();

        public async Task LearnSkill(CharacterInfo characterInfo)
        {
            if (this.isLearned)
            {
                return;
            }
            this.isLearned = true;
            characterInfo.AllocatedSkillPoints++;
            characterInfo.SkillPoints--;
            await characterInfo.GenerateCharacterStatsAndSkills();
        }
    }

    [Serializable]
    public class ActiveSkillNode: SkillNode
    {
        [SerializeField]
        private ActiveSkillSO activeSkill;

        public ActiveSkillSO ActiveSkill { get => this.activeSkill; }

        public ActiveSkillNode(ActiveSkillNode activeSkillNode)
        {
            this.activeSkill = activeSkillNode.ActiveSkill;
            this.isLearned = activeSkillNode.IsLearned;
        }

        public override string GetSkillName()
        {
            return activeSkill.GetActionName();
        }
    }

    [Serializable]
    public class PassiveSkillNode : SkillNode
    {
        [SerializeField]
        private PassiveSkillSO passiveSkill;

        public PassiveSkillSO PassiveSkill { get => this.passiveSkill; }

        public PassiveSkillNode(PassiveSkillNode passiveSkillNode)
        {
            this.passiveSkill = passiveSkillNode.PassiveSkill;
            this.isLearned = passiveSkillNode.IsLearned;
        }

        public override string GetSkillName()
        {
            return passiveSkill.GetActionName();
        }
    }

    [Serializable]
    public class StatSkillNode : SkillNode
    {
        [SerializeField]
        private StatIncreaseSkillSO statSkill;

        public StatIncreaseSkillSO StatSkill { get => this.statSkill; }
        public StatSkillNode(StatSkillNode statSkillNode)
        {
            this.statSkill = statSkillNode.StatSkill;
            this.isLearned = statSkillNode.IsLearned;
        }
        public override string GetSkillName()
        {
            return statSkill.GetActionName();
        }
    }

    [SerializeField]

    protected List<ActiveSkillNode> activeSkillNodes;

    public ReadOnlyCollection<ActiveSkillNode> GetActiveSkillNodes(){
        return activeSkillNodes.AsReadOnly();  
    }

    [SerializeField]
    protected List<PassiveSkillNode> passiveSkillNodes;

    public ReadOnlyCollection<PassiveSkillNode> GetPassiveSkillNodes(){
        return passiveSkillNodes.AsReadOnly();
        
    }

    [SerializeField]
    protected List<StatSkillNode> statSkillNodes;
    public ReadOnlyCollection<StatSkillNode> GetStatSkillNodes(){
        return statSkillNodes.AsReadOnly();
        
    }

    [SerializeField]
    [Range(0, 20)]
    protected int previousTierLearnedSkillCount = 15;
}


[Serializable]
public class SkillTier : SkillTierBase {

    public SkillTier(SkillTier skillTier)
    {
        this.activeSkillNodes = new();
        foreach (ActiveSkillNode activeSkillNode in skillTier.activeSkillNodes)
        {
            this.activeSkillNodes.Add(new(activeSkillNode));
        }


        this.passiveSkillNodes = new();
        foreach (PassiveSkillNode passiveSkillNode in skillTier.passiveSkillNodes)
        {
            this.passiveSkillNodes.Add(new(passiveSkillNode));
        }


        this.statSkillNodes = new();
        foreach (StatSkillNode statSkillNode in skillTier.statSkillNodes)
        {
            this.statSkillNodes.Add(new(statSkillNode));
        }
    }

    public bool CanUseSkillTier(SkillTier previousSkillTier)
    {
        return previousSkillTier.GetLearnedSkillCount() >= this.previousTierLearnedSkillCount;
    }

    public List<ActiveSkillSO> GetLearnedActiveSkills() {
        List<ActiveSkillSO> learnedSkills = new();
        foreach (ActiveSkillNode skillNode in this.GetActiveSkillNodes()) {
            if (!skillNode.IsLearned) {
                continue;
            }
            learnedSkills.Add(skillNode.ActiveSkill);
        }
        return learnedSkills;
    }

    public List<PassiveSkillSO> GetLearnedPassiveSkills() {
        List<PassiveSkillSO> learnedSkills = new();
        foreach (PassiveSkillNode skillNode in this.GetPassiveSkillNodes()) {
            if (!skillNode.IsLearned) {
                continue;
            }
            learnedSkills.Add(skillNode.PassiveSkill);
        }
        return learnedSkills;
    }

    public List<StatIncreaseSkillSO> GetLearnedStatSkills()
    {
        List<StatIncreaseSkillSO> learnedSkills = new();
        foreach (StatSkillNode skillNode in this.GetStatSkillNodes()) {
            if (!skillNode.IsLearned) {
                continue;
            }
            learnedSkills.Add(skillNode.StatSkill);
        }
        return learnedSkills;
    }

    public bool IsSkillLearned(ActiveSkillSO activeSkillSO)
    {
        return this.GetActiveSkillNodes().Where((node) => node.ActiveSkill == activeSkillSO && node.IsLearned).Count() > 0;
    }

    public bool IsSkillLearned(PassiveSkillSO passiveSkillSO) {
        return this.GetPassiveSkillNodes().Where((node) => node.PassiveSkill == passiveSkillSO && node.IsLearned).Count() > 0;

    }

    public int GetLearnedSkillCount() {
        int learnedSkills = 0;

        learnedSkills += this.GetActiveSkillNodes().ToList().FindAll((skill) => skill.IsLearned).Count;
        learnedSkills += this.GetPassiveSkillNodes().ToList().FindAll((skill) => skill.IsLearned).Count;

        return learnedSkills;
    }
}


// [Serializable]
// #nullable enable
// public class SerializableSkillInfo {
//     public SerializableSkillInfo(SkillInfo skillInfo) {

//         this.attackSkillId = skillInfo.AttackSkill.name;

//         this.defendSkillId = skillInfo.DefendSkill.name;

//         this.swapSkill = new SerializedSwapSkill(skillInfo.SwapSkill);

//         // List<string> startingActiveSkills = new List<string>();
//         // if(skillInfo.StartingActiveSkills != null) {
//         //     foreach(ActiveSkillSO activeSkillSO in skillInfo.StartingActiveSkills) {
//         //         startingActiveSkills.Add(activeSkillSO.name);
//         //     }
//         // }

//         // List<string> activeSkills = new List<string>();
//         // if(skillInfo.LearnedSkills != null) {
//         //     foreach(ActiveSkillSO activeSkillSO in skillInfo.LearnedSkills) {
//         //         activeSkills.Add(activeSkillSO.name);
//         //     }
//         // }

//         List<string> resolveSkills = new List<string>();
//         if(skillInfo.ResolveSkills != null) {
//             foreach(ActiveSkillSO activeSkillSO in skillInfo.ResolveSkills) {
//                 resolveSkills.Add(activeSkillSO.name);
//             }
//         }

//         // List<string> passiveSkills = new List<string>();
//         // if(skillInfo.PassiveSkills != null) {
//         //     foreach(PassiveSkillSO passiveSkillSO in skillInfo.PassiveSkills) {
//         //         passiveSkills.Add(passiveSkillSO.name);
//         //     }
//         // }



//         // this.startingActiveSkillIds = startingActiveSkills.ToArray();
//         // this.activeSkillIds = activeSkills.ToArray();
//         // this.passiveSkillIds = passiveSkills.ToArray();
//         this.resolveSkillIds = resolveSkills.ToArray();

//         this.skillTiers = skillInfo.SkillTiers.ToArray();


//         List<string?> equippedSkills = new();
//         foreach(ActiveSkillSO? activeSkillSO in skillInfo.EquippedSkills) {
//             if(activeSkillSO == null)
//             {
//                 equippedSkills.Add(null);
//                 continue;
//             }
//             equippedSkills.Add(activeSkillSO.name);
//         }

//         this.equippedSkills = equippedSkills.ToArray();
//     }

//     public string? attackSkillId;

//     public string? defendSkillId;

//     public SerializedSwapSkill swapSkill;

//     public string[] resolveSkillIds;


//     public SkillTier[] skillTiers;
    
//     public string?[] equippedSkills;

//     public SkillInfo DeSerializeSkills() {

//         ActiveSkillSO attackSkill = (ActiveSkillSO) Resources.Load(Constants.ScriptableObjects.ActiveSkills + attackSkillId, typeof(ActiveSkillSO));
//         ActiveSkillSO defendSkill = (ActiveSkillSO) Resources.Load(Constants.ScriptableObjects.ActiveSkills + defendSkillId, typeof(ActiveSkillSO));
        
//         // if (startingActiveSkillIds != null) {
//         //     skillInfo.StartingActiveSkills = new List<ActiveSkillSO>();
//         //     for(int i = 0; i < startingActiveSkillIds.Length; i++) {
//         //         ActiveSkillSO activeSkill = (ActiveSkillSO) Resources.Load(Constants.ScriptableObjects.ActiveSkills + startingActiveSkillIds[i], typeof(ActiveSkillSO));
//         //         if(activeSkill != null) {
//         //            skillInfo.StartingActiveSkills.Add(activeSkill);
//         //         }
//         //     }
//         // } 

//         // if (activeSkillIds != null) {
//         //     skillInfo.LearnedSkills = new List<ActiveSkillSO>();
//         //     for(int i = 0; i < activeSkillIds.Length; i++) {
//         //         ActiveSkillSO activeSkill = (ActiveSkillSO) Resources.Load(Constants.ScriptableObjects.ActiveSkills + activeSkillIds[i], typeof(ActiveSkillSO));
//         //         if(activeSkill != null) {
//         //            skillInfo.LearnedSkills.Add(activeSkill);
//         //         }
//         //     }
//         // } 

//         List<ActiveSkillSO> resolveSkills = new();
//         for(int i = 0; i < resolveSkillIds.Length; i++) {
//             ActiveSkillSO activeSkill = (ActiveSkillSO) Resources.Load(Constants.ScriptableObjects.ResolveSkills + resolveSkillIds[i], typeof(ActiveSkillSO));
//             if(activeSkill != null) {
//                 Debug.Log("Found resolve skill");
//                 resolveSkills.Add(activeSkill);
//             }
//         }

//         // if (passiveSkillIds != null) {
//         //     skillInfo.PassiveSkills = new List<PassiveSkillSO>();
//         //     for(int i = 0; i < passiveSkillIds.Length; i++) {
//         //         PassiveSkillSO passiveSkill = (PassiveSkillSO) Resources.Load(Constants.ScriptableObjects.ActiveSkills + passiveSkillIds[i], typeof(PassiveSkillSO));
//         //         if(passiveSkill != null) {
//         //            skillInfo.PassiveSkills.Add(passiveSkill);
//         //         }
//         //     }
//         // }   

//         SwapSkill swapSkill = this.swapSkill.DeSerializeSwapSkill();

//         return new(
//             attackSkill,
//             defendSkill,
//             swapSkill,
//             new(),
//             resolveSkills,
//             new()
//         );
//     }

// }
}