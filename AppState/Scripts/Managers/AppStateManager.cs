using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace BNJMO
{
    [Serializable]
    public struct SStartAppState
    {
        public EAppScene AppScene;
        public EAppState AppState;
    }

    public class AppStateManager : AbstractSingletonManagerWithStateMachine<AppStateManager, EAppState>
    {
        [Header("AppState")]
        [SerializeField] private SStartAppState[] startStates = new SStartAppState[0];

        protected override void InitializeStateMachine()
        {
            // 1) Initialize start states
            foreach (SStartAppState startState in startStates)
            {
                if (IS_KEY_NOT_CONTAINED(startStatesMap, startState.AppScene))
                {
                    startStatesMap.Add(startState.AppScene, startState.AppState);
                }
            }

            // 2) Define debugStateID
            debugStateID = "AppState";
        }

        protected override void BindStateMachineEvent()
        {
            // 3) Bind event
            StateUpdateEvent += BEvents.APP_AppStateUpdated.Invoke;
        }

        protected override void InitializeEventsCallbacks()
        {
            base.InitializeEventsCallbacks();


            BEvents.Instance.GAME_GameStarted += On_GAME_GameStarted;
            BEvents.Instance.GAME_GamePaused += On_GAME_GamePaused;
            BEvents.Instance.GAME_GameUnPaused += On_GAME_GameUnPaused;
            BEvents.Instance.GAME_GameEnded += On_GAME_GameEnded;
        }

        #region Event Callbacks
        private void On_GAME_GameStarted(AbstractGameMode gameMode)
        {
            UpdateState(EAppState.IN_GAME_IN_RUNNING);
        }

        private void On_GAME_GamePaused(AbstractGameMode gameMode)
        {
            UpdateState(EAppState.IN_GAME_IN_PAUSED);
        }

        private void On_GAME_GameUnPaused(AbstractGameMode gameMode)
        {
            UpdateState(EAppState.IN_GAME_IN_RUNNING);
        }

        private void On_GAME_GameEnded(AbstractGameMode gameMode, bool wasAborted)
        {
            UpdateState(EAppState.IN_GAME_IN_OVER);
        }
        #endregion
    }
}