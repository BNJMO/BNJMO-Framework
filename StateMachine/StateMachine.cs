using System.Collections;
using System;
using System.Collections.Generic;
using BNJMO;
using UnityEngine;

namespace BNJMO
{
    public class StateMachine<T> where T : Enum
    {
        #region Public Events


        #endregion

        #region Public Methods
        
        public void UpdateState(T newState)
        {
            if (EqualityComparer<T>.Default.Equals(CurrentState, newState))
                return;
        
            PreviousState = CurrentState;
            CurrentState = newState;
            Handler.Update(CurrentState, PreviousState);

            if (DebugStateChange)
            {
                Debug.Log($"State updated from {PreviousState} to {CurrentState}");
            }
        }
    
        public void RevertState()
        {
            (CurrentState, PreviousState) = (PreviousState, CurrentState);
            Handler.Update(CurrentState, PreviousState);
            
            if (DebugStateChange)
            {
                Debug.Log($"State updated from {PreviousState} to {CurrentState}");
            }
        }
        
        #endregion

        #region Inspector Variables


        #endregion

        #region Variables
        
        public StateMachineHandler<T> Handler { get; private set; } = new StateMachineHandler<T>();

        public T CurrentState { get; private set; }
        
        public T PreviousState { get; private set; }
        
        public bool DebugStateChange { get; set; }

        #endregion

        #region Life Cycle

    
        #endregion

        #region Events Callbacks


        #endregion

        #region Others



        #endregion
    }
}
