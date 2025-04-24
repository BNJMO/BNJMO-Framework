using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class StateMachineHandler<T> where T : Enum
{
	public class StateHooks
	{
		public Action<T> Enter;
		public Action<T> Exit;
		public Action Update;
	}

	private Dictionary<T, StateHooks> _states = new Dictionary<T, StateHooks>();
	private (T current, T previous) previousFrame = (default(T), default(T));

	public StateHooks this[T stateName]
	{
		get
		{
			if (!_states.TryGetValue(stateName, out StateHooks stateHooks))
			{
				stateHooks = new StateHooks();
				_states[stateName] = stateHooks;
			}
			return stateHooks;
		}
	}

	public void Update(T currentState, T previousState)
	{
		// run state 'stay' logic
		if (_states.TryGetValue(previousFrame.current, out StateHooks current))
			current.Update?.Invoke();

		// if state hasn't changed
		if (Equals(currentState, previousFrame.current) && Equals(previousState, previousFrame.previous))
			return;

		// exit state if we've changed
		if (_states.TryGetValue(previousState, out StateHooks previous))
			previous.Exit?.Invoke(currentState);

		// enter new state
		if (_states.TryGetValue(currentState, out StateHooks next))
			next.Enter?.Invoke(previousState);

		previousFrame = (currentState, previousState);
	}
}