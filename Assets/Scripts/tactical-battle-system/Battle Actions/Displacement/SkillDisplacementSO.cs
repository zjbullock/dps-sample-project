using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat {
public abstract class SkillDisplacementSO : ScriptableObject
{

    [SerializeField]
    [Tooltip("The distance that the target will be displaced")]
    [Range(0, 10)]
    protected int displaceDistance = 0;

    [SerializeField]
    protected ElementSO _element;

    public ElementSO Element { get => this._element; }

    [SerializeField]
    protected AnimationCurve _animationCurve;

    public AnimationCurve AnimationCurve { get => this._animationCurve; }

    // Update is called once per frame
    public abstract void Execute(BattleManager battleController, IBattleActionCommand activeSkill, GenericDictionary<Vector3, CombatTileController> grid, PartySlot user, List<PartySlot> affectedPartySlots);
    
}
}