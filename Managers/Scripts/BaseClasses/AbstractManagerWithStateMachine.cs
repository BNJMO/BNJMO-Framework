using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BNJMO
{
    /// <summary>
    /// A Manager class that uses a state machine. See parent AbstractManager class for more information about what a Manager is.
    /// IMPORTANT: do following INSTRUCTIONS steps when inheriting from this class
    /// 1) Initialize start states
    /// 2) Define debugStateID
    /// 3) Bind "StateUpdateEvent" in start method of child class
    /// </summary>
    /// <typeparam name="E"></typeparam>
    public abstract class AbstractSingletonManagerWithStateMachine<T, E> : AbstractSingletonManager<T> where T : AbstractSingletonManager<T> where E : Enum
    {
        [Header("State Machine")]
        [SerializeField] protected bool preventReupdateSameState = true;

        protected event Action<StateBEHandle<E>> StateUpdateEvent;

        /// <summary>
        /// Start default state according to scene
        /// Assign in Awake from MaleficusTypes
        /// </summary>
        protected Dictionary<EAppScene, E> startStatesMap = new Dictionary<EAppScene, E>();

        public E CurrentState   { get; protected set; }
        public E LastState      { get; protected set; }

        /// <summary>
        /// Debug current state with DebugManager using this ID
        /// </summary>
        protected string debugStateID;

        protected override void Awake()
        {
            base.Awake();

            InitializeStateMachine();
        }

        protected override void Start()
        {
            base.Start();

            BindStateMachineEvent();
        }

        protected override void OnNewSceneReinitialize(EAppScene newScene, EAppScene lastScene)
        {
            base.OnNewSceneReinitialize(newScene, lastScene);

            StartCoroutine(DelayedUpdateStateCoroutine(newScene));
        }

        private IEnumerator DelayedUpdateStateCoroutine(EAppScene newScene)
        {
            yield return new WaitForEndOfFrame();

            if (IS_KEY_CONTAINED(startStatesMap, newScene))
            {
                UpdateState(startStatesMap[newScene]);
            }
        }

        protected override void UpdateDebugText()
        {
            base.UpdateDebugText();

            // Debug current state
            DebugManager.Inst.DebugLogCanvas(debugStateID, CurrentState.GetType() + " : " + CurrentState);
        }

        /// <summary>
        /// /* INSTRUCTIONS */
        /// 1) Initialize start states
        /// 2) Define debug State ID 
        /// </summary>
        protected abstract void InitializeStateMachine();

        /// <summary>
        /// /* INSTRUCTIONS */
        /// 
        /// 3) Bind event
        ///     Example:
        ///     StateUpdateEvent += EventManager.Instance.Invoke_UI_MenuStateUpdated;
        /// </summary>
        protected abstract void BindStateMachineEvent();

        /// <summary>
        /// Update the state and trigger corresponding event
        /// </summary>
        public virtual void UpdateState(E newState)
        {
            // Prevent
            if ((preventReupdateSameState == true)
                && (newState.Equals(CurrentState)))
            {
                LogConsoleWarning("Preventing reupdating same state : <color=gray>" + newState.ToString() + "</color>");
                return;
            }

            LastState = CurrentState;
            CurrentState = newState;

            if (StateUpdateEvent != null)
            {
                StateUpdateEvent.Invoke(new StateBEHandle<E>(newState, LastState));
            }
        }


    }
}