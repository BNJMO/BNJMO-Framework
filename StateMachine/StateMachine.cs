using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


public class StateMachine<T> where T : Enum
{
    public StateMachineHandler<T> Handler { get; private set; } = new StateMachineHandler<T>();
	
    public T CurrentState { get; private set; }
    public T PreviousState { get; private set; }
	
    public void UpdateState(T newState)
    {
        if (EqualityComparer<T>.Default.Equals(CurrentState, newState))
            return;
        
        PreviousState = CurrentState;
        CurrentState = newState;
        Handler.Update(CurrentState, PreviousState);
    }
    
    public void RevertState()
    {
        T temp = CurrentState;
        CurrentState = PreviousState;
        PreviousState = temp;
        Handler.Update(CurrentState, PreviousState);
    }
}