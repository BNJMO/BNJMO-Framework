using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class ShutdownOnlineSessionButton : AbstractBButtonCallback
    {
        #region Public Events


        #endregion

        #region Public Methods


        #endregion

        #region Inspector Variables

        [SerializeField] private BMenuNavigationButton bMenuNavigationButton;

        #endregion

        #region Variables


        #endregion

        #region Life Cycle

        protected override void OnValidate()
        {
            base.OnValidate();

            SetComponentIfNull(ref bMenuNavigationButton);
        }

        protected override void Start()
        {
            base.Start();

            BEvents.ONLINE_LaunchSessionFailed += BEvents_ONLINE_OnLaunchSessionFailed;
            BEvents.ONLINE_ShutdownSession += BEvents_ONLINE_OnShutdownSession;
        }

        #endregion

        #region Events Callbacks

        public override void OnBButtonPressed()
        {
            BOnlineManager.Inst.ShutdownLobbyAndMultiplayer(ELeaveOnlineSessionReason.LeaveFromUI);
        }
        
        private void BEvents_ONLINE_OnLaunchSessionFailed(BEventHandle<EJoinOnlineSessionFailureType> bEventHandle)
        {
            if (BButton.IsShown == false
                || bMenuNavigationButton == null)
                return;
            
            bMenuNavigationButton.NavigateToMenu();
        }

        private void BEvents_ONLINE_OnShutdownSession(BEventHandle<ELeaveOnlineSessionReason, ENetworkID> bEventHandle)
        {
            ELeaveOnlineSessionReason sessionReason = bEventHandle.Arg1;
            if (sessionReason == ELeaveOnlineSessionReason.LeaveFromUI
                || BButton.IsShown == false
                || bMenuNavigationButton == null)
                return;
            
            bMenuNavigationButton.NavigateToMenu();
        }
        
        #endregion

        #region Others


        #endregion
    }
}
