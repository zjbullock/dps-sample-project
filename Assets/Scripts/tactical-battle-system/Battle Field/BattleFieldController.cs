using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using DPS.Common;

namespace DPS.TacticalCombat{
public class BattleFieldController : MonoBehaviour
{

    [Serializable]
    public struct RangeConstraints {

        [Range(0, 10)]
        public int min;

        [Range(0, 10)]
        public int max;

        public RangeConstraints(int min, int max) {
            this.min = min;
            this.max = max;
        }
    }

    [SerializeField]
    private BattleManager _battleManager;

    [SerializeField]
    private List<TileEventAnimatorController> tileEventAnimatorControllers = new List<TileEventAnimatorController>();

    [SerializeField]
    private GenericDictionary<SpawnableBattleObject, RangeConstraints> spawnableObjectMap = new GenericDictionary<SpawnableBattleObject, RangeConstraints>();

    [Header("Grid Maps")]
    [SerializeField]
    [Tooltip("The main grid of combat tiles in the battlefield")]
    private GenericDictionary<Vector3, CombatTileController> _grid;

    public GenericDictionary<Vector3, CombatTileController> Grid { get => this._grid; set => this._grid = value; }

    [SerializeField]
    [Tooltip("Grid of combat tiles available for action selection")]
    private GenericDictionary<Vector3, CombatTileController> _actionGrid = new GenericDictionary<Vector3, CombatTileController>();   

    public GenericDictionary<Vector3, CombatTileController> ActionGrid { get => this._actionGrid; set => this.SetActionTiles(value); }


    [SerializeField]
    [Tooltip("Grid of combat tiles available for confirming actions")]
    private GenericDictionary<Vector3, CombatTileController> _confirmActionGrid = new GenericDictionary<Vector3, CombatTileController>();   

    public GenericDictionary<Vector3, CombatTileController> ConfirmActionGrid { get => this._confirmActionGrid; set => this._confirmActionGrid = value; }

    [SerializeField]
    [Tooltip("Grid of combat tiles available for movement")]
    private GenericDictionary<Vector3, CombatTileController> _movementGrid = new GenericDictionary<Vector3, CombatTileController>();

    public GenericDictionary<Vector3, CombatTileController> MovementGrid { get => this._movementGrid; set => this._movementGrid = value; }

    


    [Header("Spawnable Objects and Structures")]
    [SerializeField]
    private List<SpawnableBattleObject> _spawnableBattleObjects;

    public List<SpawnableBattleObject> SpawnableBattleObjects { get => this._spawnableBattleObjects; }



    [SerializeField]
    private List<SpawnableBattleObject> _existingSpawnableBattleObjects;

    [SerializeField]
    private List<BattleStageStructure> _battleStageStructures;

    #nullable enable
    
    private List<SpawnableBattleObject> GetBattleObjects() {
        List<SpawnableBattleObject> spawnables = new();

        spawnables.AddRange(_spawnableBattleObjects);
        spawnables.AddRange(_existingSpawnableBattleObjects);

        return spawnables;
    }

    void Awake() {
        this._grid = new();
        this._spawnableBattleObjects = new();
        this._existingSpawnableBattleObjects = new();
        this._battleStageStructures = new();
        this.GetGrid();
        this.SetBattleController();
        this.SpawnObjects();

    }

    void Start() {
        this.SetTileEventAnimatorControllers();
    }

    private void SetBattleController()
    {
        List<CombatTileController> combatTileControllers = this._grid.ToValueList();
        
        foreach (CombatTileController tile in combatTileControllers)
        {
            tile.SetBattleController(this._battleManager);
        }
    }

    public void GetGrid()
    {
        CombatTileController[] battleField = GetComponentsInChildren<CombatTileController>();

        if (battleField.Length == 0)
        {
            return;
        }

        this._grid.Clear();

        foreach (CombatTileController tile in battleField)
        {
            Vector3 fPosition = new(tile.transform.position.x, 0, tile.transform.position.z);
            Vector3Int position = Vector3Int.RoundToInt(fPosition);
            tile.Position = position;
            this._grid[position] = tile;
        }
    }

    public CombatTileController? GetCombatTileAtPosition(Vector3Int position )
    {
        if (this._grid.ContainsKey(position))
        {
            return this._grid[position];
        }

        return null;
    }

    private void SetTileEventAnimatorControllers() {
        this.tileEventAnimatorControllers = new List<TileEventAnimatorController>(this.GetComponentsInChildren<TileEventAnimatorController>());
    }

    private void SetExistingStageObjects() {

        SpawnableBattleObject[] existingBattleObjects = GetComponentsInChildren<SpawnableBattleObject>();

        if (existingBattleObjects.Length == 0) {
            return;
        }

        this._existingSpawnableBattleObjects?.Clear();

        foreach(SpawnableBattleObject spawnableBattleObject in existingBattleObjects) {
            spawnableBattleObject.Grid = _grid;
            spawnableBattleObject.SetSpawnedObject(_battleManager);
        } 
        this._existingSpawnableBattleObjects = new List<SpawnableBattleObject>(existingBattleObjects);
    }

    private void SetExistingBattleStructure() {
        
        BattleStageStructure[] existingBattleObjects = GetComponentsInChildren<BattleStageStructure>();

        if (existingBattleObjects.Length == 0) {
            return;
        }

        this._battleStageStructures?.Clear();

        foreach(BattleStageStructure battleStageStructure in existingBattleObjects) {
            battleStageStructure.SetStageStructureProperties();
        } 

        this._battleStageStructures = new List<BattleStageStructure>(existingBattleObjects);
    }

    public void SetRandomStageObjects() {
      if (this.spawnableObjectMap.Count == 0 || this._grid.Count == 0) {
            return;
        }

        List<CombatTileController> combatTileList = this._grid.ToValueList();


        System.Random rand = new();

        combatTileList = combatTileList.OrderBy(_ => rand.Next()).ToList();


        List<SpawnableBattleObject> objectsToSpawnList = new();
        foreach(var spawnableObject in this.spawnableObjectMap) {
            RangeConstraints rangeConstraints = spawnableObject.Value;
            int spawnCount = rand.Next(rangeConstraints.min, rangeConstraints.max + 1);
            for(int i = 0; i < spawnCount; i++) {
                objectsToSpawnList.Add(spawnableObject.Key);
            }
        }

        int combatTileListCursor = 0;
        Debug.Log("Objects to spawn: " + objectsToSpawnList.Count);

        while (objectsToSpawnList.Count > 0 && combatTileListCursor < combatTileList.Count) {            
            CombatTileController combatTileController = combatTileList[combatTileListCursor];

            if (combatTileController.SpawnableObjectPartySlot == null) {

                combatTileController.SetAndInstantiateSpawnedObject(this._battleManager, objectsToSpawnList[0]);
                objectsToSpawnList.RemoveAt(0);
            }

            combatTileListCursor++;
        }  
    }

    public void SpawnObjects() {
        this.SetExistingStageObjects();
        this.SetExistingBattleStructure();
        // this.SetRandomStageObjects();
    }
    
    public void OnTurnEnd(BattleManager battleController)
    {
        foreach (TileEventAnimatorController tileEventAnimatorController in this.tileEventAnimatorControllers)
        {
            tileEventAnimatorController.OnTurnEnd(battleController);
        }

        foreach (SpawnableBattleObject spawnableBattleObject in this.GetBattleObjects())
        {
            spawnableBattleObject.OnTurnEnd(battleController);
        }

        foreach (KeyValuePair<Vector3, CombatTileController> keyValuePair in this._grid)
        {
            keyValuePair.Value.OnTurnEnd(battleController);
        }
    }

    public void GetPossibleMoves(CombatTileController combatTileController, PartySlot partySlot)
    {
        this._movementGrid.Clear();
        this._movementGrid = BattleProcessingStatic.GetPossibleMoves(
            combatTileController,
            partySlot,
            this._grid
        );
    }

    public void AddConfirmActionTile(CombatTileController combatTileController)
    {
        if (this._confirmActionGrid.ContainsKey(combatTileController.Position))
        {
            Debug.Log("Confirm Action Tile already contains this key");
            return;
        }
        Debug.Log("Confirm Action Tile being Added to Confirm Action Grid");
        this._confirmActionGrid.Add(new(combatTileController.Position, combatTileController));
        combatTileController.ActivateConfirmActionTile();

    }

    public void AddActionTile(CombatTileController combatTileController)
    {
        if (this._actionGrid.ContainsKey(combatTileController.Position))
        {
            return;
        }
        this._actionGrid.Add(new(combatTileController.Position, combatTileController));
        combatTileController.ActivateActionTile();

    }

    public void SetActionTiles(GenericDictionary<Vector3, CombatTileController> actionTiles)
    {
        if(this._actionGrid.Count > 0)
        {
            this.ClearActionGrid();
        }
        this._actionGrid = actionTiles;
    }

    public void ClearActionGrid()
    {
        foreach (KeyValuePair<Vector3, CombatTileController> keyValuePair in this._actionGrid)
        {
            keyValuePair.Value.DisableActionTile();
        }
        this._actionGrid.Clear();   
    }

    public void ClearConfirmActionGrid()
    {
        foreach (KeyValuePair<Vector3, CombatTileController> keyValuePair in this._confirmActionGrid)
        {
            keyValuePair.Value.DisableActionConfirmTile();
        }
        this._confirmActionGrid.Clear();   
    }

    public void ClearMovementGrid()
    {
        foreach (KeyValuePair<Vector3, CombatTileController> keyValuePair in this._movementGrid)
        {
            keyValuePair.Value.DisableMovementTile(true);
        }
        this._movementGrid.Clear();
    }

}
}