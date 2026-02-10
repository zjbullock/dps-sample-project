using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat{
[System.Serializable]
public class BattleActionAnimationController
{
    [SerializeField]
    [Tooltip("The animation GameObject that will play on the targeted tile")]
    private BattleActionController targetLocationAnimation;

    public BattleActionController TargetLocationAnimation { get => this.targetLocationAnimation; }

    [SerializeField]
    [Tooltip("The animation gameObject that will play on targeted battle objects")]
    private BattleActionController targetAnimation;

    public BattleActionController TargetAnimation { get => this.targetAnimation; }

    [SerializeField]
    [Tooltip("The animated GameObject that will play on caster tiles")]
    private BattleActionController userAnimation;

    public BattleActionController UserAnimation { get => this.userAnimation;}
    

    public AnimationTrigger animationTrigger;

}
}