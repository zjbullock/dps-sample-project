using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DPS.TacticalCombat {
    [System.Serializable]
    public abstract class BattleControllerBaseState
    {

        protected System.Action _onEnterState = delegate{};

        public void AddOnEnterStateListener(System.Action action)
        {
            _onEnterState += action;
        }

        protected System.Action _onProgressState = delegate{};

        public void AddOnProgressStateListener(System.Action action)
        {
            _onProgressState += action;
        }

        public virtual void EnterState(BattleManager battleController)
        {
            this._onEnterState?.Invoke();
        }

        public abstract void UpdateState(BattleManager battleController);

        public virtual void ProgressState(BattleManager battleController) {
            this._onProgressState?.Invoke();
            return;
        }

    }
}