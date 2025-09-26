using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class PlayerPartiesManager : BBehaviour
    {
        #region Public Events


        #endregion

        #region Public Methods

        #endregion

        #region Inspector Variables

        [SerializeField] private Transform spawnOrigin;
        [SerializeField] private GameObject playerPartyUIPrefab;
        [SerializeField] private BText readyStatusText;

        #endregion

        #region Variables


        #endregion

        #region Life Cycle

        protected override void Start()
        {
            base.Start();
            
            UpdateReadyText();
            
            BEvents.PLAYERS_Connected += BEvents_OnPlayersPlayerConnected;
            BEvents.PLAYERS_Disconnected += _ => UpdateReadyText();
            BEvents.PLAYERS_BecameReady += _ => UpdateReadyText();
            BEvents.PLAYERS_CanceledReady += _ => UpdateReadyText();
            BEvents.PLAYERS_JoinedParty += _ => UpdateReadyText();
            BEvents.PLAYERS_LeftParty += _ => UpdateReadyText();
        }

        private void BEvents_OnPlayersPlayerConnected(BEventHandle<PlayerBase> BEventHandle)
        {
            PlayerPartyUI playerPartyUI = Instantiate(playerPartyUIPrefab, spawnOrigin, true).GetComponent<PlayerPartyUI>();
            playerPartyUI.transform.localScale = Vector3.one;
            playerPartyUI.Player = BEventHandle.Arg1;
            UpdateReadyText();
        }

        #endregion

        #region Events Callbacks


        #endregion

        #region Others

        private void UpdateReadyText()
        {
            bool allPlayersReady = BPlayerManager.Inst.AreAllPlayersReady();
            string text = allPlayersReady ? "All Players are ready" : "Not all Players are ready";
            readyStatusText.SetText(text);
        }

        #endregion
    }
}
