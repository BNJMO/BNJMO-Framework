using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    /// <summary>
    /// Defines the rules for a specific game mode.
    /// Don't forget to define the correct game mode inside the awake of the extending children classes!
    /// </summary>
    /// <typeparam name="T"> Type of information that defines the player's stats (e.g. Reamining lives)</typeparam>
    public abstract class ConcreteGameMode<T> : AbstractGameMode where T : AbstractPlayerStats, new()
    {
        public Dictionary<EPlayerID, T> PlayerStats { get; protected set; } = new Dictionary<EPlayerID, T>();

        protected abstract void InitializePlayerStats();

        protected override void On_GAME_GameStarted(AbstractGameMode gameMode)
        {
            base.On_GAME_GameStarted(gameMode);

            // TODO : Very dirty fix due to AI spawning late
            StartCoroutine(LateOnInitializePlayerStats(gameMode));
        }

        private IEnumerator LateOnInitializePlayerStats(AbstractGameMode gameMode)
        {
            yield return new WaitForEndOfFrame();
            if (GameModeType == gameMode.GameModeType)
            {
                Debug.Log("Initializing player stats for : " + gameMode.GameModeType);
                InitializePlayerStats();
            }

        }
    }
}
