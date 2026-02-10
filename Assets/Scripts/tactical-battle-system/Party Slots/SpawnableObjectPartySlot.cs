using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPS.Common;

namespace DPS.TacticalCombat{
[System.Serializable]
public class SpawnableObjectPartySlot : PartySlot
{


    [SerializeField]
    private SpawnedObjectController _controller;

    public SpawnedObjectController Controller { get => this._controller; }

    public SpawnableObjectPartySlot(BattleMember battleMember, SpawnableBattleObject spawnableBattleObject) : base(battleMember)
    {
        // Debug.Log("Creating new Spawnable Object Party Slot");
        this.canMove = false;
        this.battleObject = spawnableBattleObject;
        this._controller =
            new SpawnedObjectController(spawnableBattleObject.gameObject,
                                        spawnableBattleObject.CombatTileInteraction.duration,
                                        spawnableBattleObject.CombatTileInteraction.walkable,
                                        spawnableBattleObject.CombatTileInteraction.isSolid,
                                        spawnableBattleObject.CombatTileInteraction.infiniteDuration,
                                        true);
    }

    public override void BeginPhase(BattleManager battleController)
    {
        return;
    }

    public override void CombatBegin()
    {
        return;
    }

    public override void CombatEnd()
    {
        return;
    }

    public override void EndPhase(BattleManager battleController)
    {
        return;
    }

    #nullable enable
    protected override void TakeDamage(BattleManager battleController, int damage, string statusText, ElementSO? ElementSO, string? additionalText = null)
    {

        if (damage > 1)
        {
            damage = 1;
        }

        // Vector3 currentPosition = this.GetBattleEntityGO().transform.position;
        base.TakeDamage(battleController, damage, statusText, ElementSO, additionalText: additionalText);

        return;
    }

    public override void OnReaction(List<ElementSO> elementSO, System.Action callBack)
    {
        if (BattleManager.instance == null || BattleManager.instance.Grid == null || this.GetCombatTileController() == null)
        {
            callBack?.Invoke();
            return;
        }

        if (this.BattleEntity == null)
        {
            callBack?.Invoke();
            return;
        }

        SpawnedObjectInfo? spawnedObjectInfo = this.BattleEntity! as SpawnedObjectInfo;
        if (spawnedObjectInfo == null)
        {
            callBack?.Invoke();
            return;
        }

        ActiveSkillSO? reactionSkill = spawnedObjectInfo.GetOnAttacked();


        ActiveSkillSO? elementalReactionSkill = spawnedObjectInfo.GetReactionSkill(elementSO[0]);
        if (elementalReactionSkill != null)
        {
            reactionSkill = elementalReactionSkill;
        }

        if (spawnedObjectInfo!.GetRawStats().hp <= 0)
        {
            reactionSkill = spawnedObjectInfo.GetOnDeathSkill();
            callBack += () => { this.CleanupOnDeath(); };
        }

        if (reactionSkill == null)
        {
            callBack?.Invoke();
            return;
        }


        GenericDictionary<Vector3, CombatTileController> tiles = reactionSkill.GetActionTilesByAreaOfEffect(this.GetCombatTileController(), this);
        if (tiles.Count == 0)
        {
            return;
        }

        this.GetBattleMember()!.BattleCommand = new SkillCommand(reactionSkill, this, BattleProcessingStatic.PartySlotIsSpawnablePartySlot, this.GetCombatTileController(), tiles.ToValueList(), new());
        this.GetBattleMember()!.BattleCommand!.ExecuteCommand(BattleManager.instance, callBack);
    }

    private void CleanupOnDeath()
    {
        // Debug.LogWarning("Cleaning up spawned Object!");

        base.currentCombatTileController.CleanupSpawnedObject();

        this.battleObject.DestroyBattleObject();
    }

    protected override void ExecuteBattleCommandLogic(BattleManager battleController, Action callBack)
    {
        callBack?.Invoke();
        return;
    }


#nullable enable
    public GameObject? GetSpawnableObjectPartySlotOccupantAnchorPoint()
    {
        if (this.BattleEntityGO == null)
        {
            return null;
        }

        SpawnableBattleObject? spawnableBattleObject = this.BattleEntityGO.GetComponent<SpawnableBattleObject>();
        if (spawnableBattleObject == null)
        {
            return null;
        }



        return spawnableBattleObject.GetOccupantAnchorPoint();
    }


    [System.Serializable]
    public class SpawnedObjectController {
    #nullable enable
        [Tooltip("An Instance of a spawned object")]
        private GameObject? _spawnedObject;
        public GameObject? SpawnedObject { get => this._spawnedObject; }


        [Tooltip("The turn count of the spawned object.  If 0, should be removed from the Combat Tile Controller class.")]
        [SerializeField]
        private int _turnCount;

        public int TurnCount { get => this._turnCount; }

        [Tooltip("Denotes whether this spawned object lasts for an infinite amount of time")]
        [SerializeField]
        private bool _isInfinite;

        public bool IsInfinite { get => this._isInfinite; }


        [Tooltip("Denotes whether the spawned object can be walked upon or not")]
        [SerializeField]
        private bool _isWalkable;
        public bool IsWalkable { get => this._isWalkable; }


        [Tooltip("Denotes whether this spawned object can be walked through")]
        [SerializeField]
        private bool _isSolid;

        public bool IsSolid { get => this._isSolid; }

        [Tooltip("Denotes whether the object should be centered")]
        private bool _shouldCenter;

        public bool ShouldCenter { get => this._shouldCenter; }

        public SpawnedObjectController(GameObject spawnedObject, int turnCount, bool walkable, bool isSolid, bool isInfinite, bool shouldCenter)
        {
            this._spawnedObject = spawnedObject;
            this._turnCount = turnCount;
            this._isInfinite = isInfinite;
            this._isWalkable = walkable;
            this._isSolid = isSolid;
            this._shouldCenter = shouldCenter;
        }

        public SpawnedObjectController(GameObject spawnedObject, CombatTileInteractionSO combatTileInteractionSO, bool shouldCenter)
        {
            this._spawnedObject = spawnedObject;
            this._turnCount = combatTileInteractionSO.duration;
            this._isInfinite = combatTileInteractionSO.infiniteDuration;
            this._isWalkable = combatTileInteractionSO.walkable;
            this._isSolid = combatTileInteractionSO.isSolid;
            this._shouldCenter = shouldCenter;
        }


        public bool IsActive() {
            return _turnCount > 0 || this.IsInfinite;
        }

        public void DecreaseCounter() {
            if (!this.IsInfinite)
                _turnCount--;
        }

        public void DestroySpawnedObject() {
            if (this.SpawnedObject == null) {
                return;
            }
            Debug.Log("Destroying spawned object");
            GameObject oldObject = this.SpawnedObject;
            
            GameObject.Destroy(oldObject);
            this._spawnedObject = null;
        }



        public float GetFloatHeightForSpawnedObject() {
            if (!_isSolid || this.SpawnedObject == null) {
                return 0;
            }

            foreach(Transform childTransform in this.SpawnedObject.transform) {
                if (childTransform.TryGetComponent<Collider>(out Collider collider)) {
                    return collider.bounds.size.y;
                }
            }

            return 0;
        }

        public Vector3 GetVector3HeightForSpawnedObject() {
            if (this.SpawnedObject == null) {
                return new Vector3();
            }
            //Retrieve the height of the renderer in the first child'
            foreach(Transform childTransform in this.SpawnedObject.transform) {
                if (childTransform.TryGetComponent<BoxCollider>(out BoxCollider collider)) {
                    return new Vector3(0, collider.bounds.size.y, 0);
                }
            }

            return new Vector3();
        }
        
        #nullable enable
        public GameObject? GetOccupantAnchorPoint() {
            if (this.SpawnedObject == null) {
                return null;
            }

            SpawnableBattleObject? spawnableBattleObject = this.SpawnedObject.GetComponent<SpawnableBattleObject>();
            if (spawnableBattleObject == null) {
                return null;
            }

            

            return spawnableBattleObject.GetOccupantAnchorPoint();
        }
        #nullable disable

    }

}
}


