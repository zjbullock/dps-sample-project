using System.Collections.Generic;
using UnityEngine;

namespace DPS.TacticalCombat
{
    public class EncounterConditionController : MonoBehaviour
    {   
        [SerializeField]
        List<EncounterCondition> _winConditions;
        
        [SerializeField]
        List<EncounterCondition> _loseConditions;

        public bool IsEncounterWon(BattleManager battleManager)
        {
            if (this._winConditions.Count == 0)
            {
                throw new System.Exception("No Win Conditions assigned!");
            }

            return DetermineEncounterState(this._winConditions, battleManager);
        }

        public bool IsEncounterLost(BattleManager battleManager)
        {
            if (this._loseConditions.Count == 0)
            {
                throw new System.Exception("No Lose Conditions assigned!");
            }

            
            return DetermineEncounterState(this._loseConditions, battleManager);
        }

        private bool DetermineEncounterState(List<EncounterCondition> encounterConditions, BattleManager battleManager)
        {
            foreach (EncounterCondition encounterCondition in encounterConditions)
            {
                if(!IsEncounterConditionMet(encounterCondition, battleManager)) {
                    continue;  
                }
                return false;
            }

            return true;     
        }

        private bool IsEncounterConditionMet(EncounterCondition encounterCondition, BattleManager battleManager)
        {
            if( encounterCondition == null || encounterCondition.IsEncounterConditionMet(battleManager)) {
                return false;  
            } 

            return true;
        }
    }
}
