using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace BNJMO
{
    public class PlayerPartyUI : BBehaviour
    {
        #region Public Events


        #endregion

        #region Public Methods

        public void Join()
        {
            if (Player.JoinParty())
            {
                playerIDText.SetText("PlayerID : " + Player.PlayerID);
                spectatorIDText.SetText("SpectatorID : " + Player.SpectatorID);

                EnableButtons();
            }
        }

        public void Leave()
        {
            if (Player.LeaveParty())
            {
                playerIDText.SetText("PlayerID : " + Player.PlayerID);
                spectatorIDText.SetText("SpectatorID : " + Player.SpectatorID);
                
                DisableButtons();
            }
        }
        
        public void Ready()
        {
            if (Player.SetReady())
            {
                readyText.SetText("Ready");
            }
        }

        public void NotReady()
        {
            if (Player.CancelReady())
            {
                readyText.SetText("Not Ready");
            }
        }

        public void ChangeTeam()
        {
            ETeamID newTeamID = (ETeamID) ((int) (Player.TeamID + 1) % (BManager.Inst.Config.MaxNumberOfTeams + 1));
            if (Player.SetTeamID(newTeamID))
            {
                teamIDText.SetText("TeamID : " + Player.TeamID);
            }
        }

        public void Spawn()
        { 
            Player.SpawnPawn();
        }
        
        public void DestroyPlayer()
        {
            Player.DestroyPawn();
        }
        
        #endregion

        #region Inspector Variables

        [SerializeField] private BText controllerIDText;
        [SerializeField] private BText controllerTypeText;
        [SerializeField] private BText spectatorIDText;
        [SerializeField] private BText playerIDText;
        [SerializeField] private BText teamIDText;
        [SerializeField] private BText networkText;
        [SerializeField] private BText readyText;
        
        [SerializeField] private BButton[] disabledButtonsWhenNotJoined;

        #endregion

        #region Variables

        public PlayerBase Player { get; set; }

        #endregion

        #region Life Cycle

        protected override void Start()
        {
            base.Start();
            
            BEvents.PLAYERS_PlayerDisconnected += BEvents_PlayersPlayerDisconnected;
            
            controllerIDText.SetText("ControllerID : " + Player.ControllerID);
            controllerTypeText.SetText("Controller Type : " + BInputManager.Inst.GetControllerType(Player.ControllerID).ToString());
            spectatorIDText.SetText("SpectatorID : " + Player.SpectatorID);
            playerIDText.SetText("PlayerID : " + Player.PlayerID);
            teamIDText.SetText("TeamID : " + Player.TeamID);
            networkText.SetText("NetworkID : " + Player.NetworkID.ToString());
            readyText.SetText("Not Ready");
            
            DisableButtons();
        }

        #endregion

        #region Events Callbacks

        private void BEvents_PlayersPlayerDisconnected(BEventHandle<PlayerBase> BEventHandle)
        {
            if (Player == BEventHandle.Arg1)
            {
                Destroy(gameObject);
            }
        }

        #endregion

        #region Others

        private void DisableButtons()
        {
            foreach (BButton button in disabledButtonsWhenNotJoined)
            {
                button.DisableButton();
            }
        }

        private void EnableButtons()
        {
            foreach (BButton button in disabledButtonsWhenNotJoined)
            {
                button.EnableButton();
            }
        }
        

        #endregion
    }
}
