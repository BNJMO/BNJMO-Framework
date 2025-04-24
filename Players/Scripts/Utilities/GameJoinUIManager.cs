using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class GameJoinUIManager : BBehaviour
    {
        #region Public Events


        #endregion

        #region Public Methods
        public void JoinGame()
        {
            //EPlayerID playerID = PlayerManager.Instance.OnPlayerJoinRequest(BEventManager.Instance.LocalNetworkID);
            //LogConsole("UI- Game Joined as : " + playerID);
        }

        public void LeaveGame()
        {

        }

        #endregion

        #region Inspector Variables
        [SerializeField]
        private BText joinedPlayersText;
                
        //[SerializeField]
        //private BText ownPlayerIDText;

        #endregion

        #region Private Variables


        #endregion

        #region Life Cycle
        protected override void Update()
        {
            base.Update();

            if (joinedPlayersText)
            {
                string joinedPlayers = "";
                foreach (EPlayerID playerID in PlayerManager.Instance.GetJoinedPlayers())
                {
                    EControllerID controllerID = PlayerManager.Instance.GetAssignedControllerID(playerID);
                    joinedPlayers += playerID + " : " + controllerID + "\n";
                }
                joinedPlayersText.SetText(joinedPlayers);
            }

            //if (ownPlayerIDText)
            //{
            //    ownPlayerIDText.SetText(PlayerManager.Instance.player)
            //}
        }

        #endregion

        #region Events Callbacks


        #endregion

        #region Others


        #endregion
    }
}
