using UnityEngine;

namespace DPS.TacticalCombat
{
public class BattleManagerStates
    {
        #nullable enable
        //Keeps track of the current battle state
        [SerializeReference]
        private BattleControllerBaseState? _currentState;

        public BattleControllerBaseState? CurrentState { get => this._currentState; set => this._currentState = value; }

        [SerializeReference]
        private BattleControllerStartState _startState;

        public BattleControllerStartState StartState { get => this._startState; }

        [SerializeReference]
        private BattleControllerTurnStartState _turnStartState;

        public BattleControllerTurnStartState TurnStartState { get => this._turnStartState; }

        [SerializeReference]
        private BattleControllerDetermineCommandState _determineCommandPhase;

        public BattleControllerDetermineCommandState DetermineCommandState { get => this._determineCommandPhase; }

        [SerializeReference]
        private BattleControllerEnemyCommandState _enemyCommandState;

        public BattleControllerEnemyCommandState EnemyCommandState { get => this._enemyCommandState; }

        [SerializeReference]
        private BattleControllerPlayerCommandstate _playerCommandstate;

        public BattleControllerPlayerCommandstate PlayerCommandstate { get => this._playerCommandstate; }

        [SerializeReference]
        private BattleControllerDeclareActionPhaseState _declareActionPhaseState;

        public BattleControllerDeclareActionPhaseState DeclareActionPhaseState { get => this._declareActionPhaseState; }

        [SerializeReference]
        private BattleControllerResultActionPhaseState _resultActionPhaseState;

        public BattleControllerResultActionPhaseState ResultActionPhaseState { get => this._resultActionPhaseState; }

        [SerializeReference]
        private BattleControllerEnemyMovementstate _enemyMovementState;

        public BattleControllerEnemyMovementstate EnemyMovementState { get => this._enemyMovementState; }

        [SerializeReference]
        private BattleControllerEndPhaseState _endPhaseState;

        public BattleControllerEndPhaseState EndPhaseState { get => this._endPhaseState; }

        [SerializeReference]
        private BattleControllerWinState _winState;

        public BattleControllerWinState WinState { get => this._winState; }

        [SerializeReference]
        private BattleControllerLoseState _loseState;

        public BattleControllerLoseState LoseState { get => this._loseState; }

        public BattleManagerStates()
        {
            this._startState = new();
            this._turnStartState = new();
            this._determineCommandPhase = new();
            this._enemyCommandState = new();
            this._playerCommandstate = new();
            this._declareActionPhaseState = new();
            this._resultActionPhaseState = new();
            this._enemyMovementState = new();
            this._endPhaseState = new ();
            this._winState = new();
            this._loseState = new();


            this._currentState = null;
        }

        public void SwitchState(BattleControllerBaseState state, BattleManager battleManager)
        {
            this._currentState = state;
            this._currentState.EnterState(battleManager);
        }

        public void SwitchToStartState(BattleManager battleManager)
        {
            this.SwitchState(this._startState, battleManager);
        }

        public void UpdateState(BattleManager battleManager)
        {
            if (this._currentState == null)
            {
                return;
            }
            this._currentState.UpdateState(battleManager);

        }
    }
}
