
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class GameManager : AbstractSingletonManager<GameManager>
    {
        public AbstractGameMode CurrentGameMode { get; private set; }
        public EGameMode CurrentGameModeType { get; private set; }
        public int GameRemainingTime { get; private set; }

        #region Behaviour Lifecycle
        protected override void Start()
        {
            base.Start();

            if (BManager.Inst.Config.IsUseDebugGameMode == true)
            {
                SetGameMode(BManager.Inst.Config.DebugGameMode);
            }
        }

        protected override void InitializeEventsCallbacks()
        {
            base.InitializeEventsCallbacks();

            BEvents.Inst.GAME_GameTimeUpdated += On_GAME_GameTimeUpdated;
            BEvents.APP_AppSceneUpdated.Event += On_APP_SceneChanged;
        }

        private void On_APP_SceneChanged(StateBEHandle<EAppScene> eventHandle)
        {
            EAppScene scene = eventHandle.NewState;

            FindAndBindButtonActions();

            //switch (scene)
            //{
            //    case EAppScene.MENU:
            //        if (CurrentGameMode != null)
            //        {
            //            Destroy(CurrentGameMode);
            //        }
            //        break;

            //    //case EAppScene.GAME:
            //    //    AddChosenGameMode();
            //    //    break;
            //}
        }
        #endregion

        #region Game Actions

        public void SetGameMode(EGameMode gameMode)
        {
            CurrentGameModeType = gameMode;
        }

        public void StartGame()
        {
            if (CurrentGameMode != null)
            {
                Destroy(CurrentGameMode);
            }

            if (CurrentGameModeType == EGameMode.NONE)
            {
                LogConsoleError("Trying to start a game, but not game mode selected!");
                return;
            }

            if (CreateGameMode())
            {
                StartCoroutine(StartGameCoroutine());
            }
        }

        private IEnumerator StartGameCoroutine()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            BEvents.Inst.Invoke_GAME_GameStarted(CurrentGameMode);
        }

        /// <summary>
        /// Returns true if succeeded spawning a valid GameMode, otherwise false
        /// </summary>
        private bool CreateGameMode()
        {
            switch (CurrentGameModeType)
            {
                // Example:
                case EGameMode.TEST:
                    CurrentGameMode = gameObject.AddComponent<TestGameMode>();
                break;
            }

            if ((IS_NOT_NULL(CurrentGameMode))
               && (ARE_ENUMS_EQUAL(CurrentGameModeType, CurrentGameMode.GameModeType)))
            {
                LogConsole("Spawned : " + CurrentGameModeType);
                return true;
            }
            LogConsole("Couldn't spawn : " + CurrentGameModeType);

            return false;
        }


        public void PauseOrUnpauseGame()
        {
            if (AppStateManager.Inst.CurrentState == EAppState.IN_GAME_IN_RUNNING)
            {
                BEvents.Inst.Invoke_GAME_GamePaused(CurrentGameMode);
            }
            else if (AppStateManager.Inst.CurrentState == EAppState.IN_GAME_IN_PAUSED)
            {
                BEvents.Inst.Invoke_GAME_GameUnPaused(CurrentGameMode);
            }
        }

        public void EndGame(bool wasAborted = false)
        {
            // Invoke event
            if (CurrentGameMode)
            {
                Destroy(CurrentGameMode);
            }

            BEvents.Inst.Invoke_GAME_GameEnded(CurrentGameMode, wasAborted);
        }

        #endregion

        #region Event Callbacks 

        private void On_GAME_GameTimeUpdated(int newTime)
        {
            GameRemainingTime = newTime;
            if (newTime == 0)
            {
                EndGame();
            }
        }


        #endregion

        private void FindAndBindButtonActions()
        {
            PauseGameUIAction[] pauseGameActions = FindObjectsOfType<PauseGameUIAction>();
            foreach (PauseGameUIAction action in pauseGameActions)
            {
                action.ActionButtonExecuted += () =>
                {
                    PauseOrUnpauseGame();
                };
            }

            AbortGameUIAction[] abortGameActions = FindObjectsOfType<AbortGameUIAction>();
            foreach (AbortGameUIAction action in abortGameActions)
            {
                action.ActionButtonExecuted += () =>
                {
                    EndGame(true);
                };
            }
        }
    }
  }
