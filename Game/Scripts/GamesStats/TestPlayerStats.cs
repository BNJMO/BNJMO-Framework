using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class TestPlayerStats : AbstractPlayerStats
    {
        public int RemainingLives { get; private set; }
        public int NumberOfHitPlayers { get; private set; }
        public int NumberOfKilledPlayers { get; private set; }
        public int TimeOfDeath { get; private set; }
        public bool IsGameOver { get { return RemainingLives == 0; } }
        public int Rank { get; private set; } = 0;
        public EPlayerID LastHitBy { get; private set; }

        public TestPlayerStats()
        {

        }
        public TestPlayerStats(EPlayerID playerStatID, int maximumNumberOfLives)
        {
            PlayerID = playerStatID;
            RemainingLives = maximumNumberOfLives;
            NumberOfKilledPlayers = 0;
            LastHitBy = EPlayerID.NONE;
        }

        /// <summary>
        /// Decrement by 1 a player's lives and tell if he died.
        /// </summary>
        /// <returns> are reamining lives = 0 </returns>
        public bool DecrementPlayerLives()
        {
            RemainingLives--;
            return RemainingLives == 0;
        }

        /// <summary>
        /// Icrements the number of killed players by 1
        /// </summary>
        public void IncrementNumberOfHitPlayers()
        {
            NumberOfHitPlayers++;
        }
        /// <summary>
        ///  Set the time when the player is killed
        /// </summary>
        public void SetTimeOfDeath(int time)
        {
            TimeOfDeath = time;
        }
        /// <summary>
        /// Set the player rank
        /// </summary>
        /// <param name="playerRank"></param>
        public void SetRank(int playerRank)
        {
            Rank = playerRank;
        }

        /// <summary>
        /// Icrements the number of killed players by 1
        /// </summary>
        public void IncrementNumberOfKilledPlayers()
        {
            NumberOfKilledPlayers++;
        }

        /// <summary>
        /// Sets the Player ID of the last player that hit this player.
        /// Used to determine who finally killed this player.
        /// </summary>
        public void SetLastHitBy(EPlayerID hitByPlayerID)
        {
            LastHitBy = hitByPlayerID;
        }
    }
}