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

        public StateMachine()
        {
        }

        public StateMachine(bool debugStateChange)
        {
            DebugStateChange = debugStateChange;
        }
        
        public void UpdateState(T newState)
        {
            if (EqualityComparer<T>.Default.Equals(CurrentState, newState))
                return;
        
            PreviousState = CurrentState;
            CurrentState = newState;
            
            if (DebugStateChange)
            {
                Debug.Log($"<color=white>State updated</color> from {PreviousState} to <color=white>{CurrentState}</color>");
            }
            
            Handler.Update(CurrentState, PreviousState);
        }
    
        public void RevertState()
        {
            (CurrentState, PreviousState) = (PreviousState, CurrentState);
            
            if (DebugStateChange)
            {
                Debug.Log($"<color=white>State updated</color> from {PreviousState} to <color=white>{CurrentState}</color>");
            }
            
            Handler.Update(CurrentState, PreviousState);
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
