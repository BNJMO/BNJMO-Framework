using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNJMO;

namespace XOKing
{
    public class OnlineNumberTest : BBehaviour
    {
        #region Public Events


        #endregion

        #region Public Methods


        #endregion

        #region Inspector Variables

        [SerializeField] private BContainer container;
        [SerializeField] private BText numberText;
        [SerializeField] private BButton numberIncrementButton;

        #endregion

        #region Variables

        private int counter;

        #endregion

        #region Life Cycle

        protected override void OnValidate()
        {
            base.OnValidate();
            
            SetComponentIfNull(ref container);
        }

        protected override void Awake()
        {
            base.Awake();

            ResetCounter();
            container.DisableUI();
        }

        protected override void Start()
        {
            base.Start();
            
            BEvents.ONLINE_LaunchSessionSucceeded += BEvents_ONLINE_OnLaunchSessionSucceeded;
            BEvents.ONLINE_ShutdownSession += BEvents_ONLINE_OnShutdownSession;
            BEvents.ONLINE_ClientJoined += BEvents_ONLINE_OnRemotePlayerJoined;
            BEvents.EXAMPLE_NumberIncremented += BEvents_EXAMPLE_OnNumberIncremented;
            
            numberIncrementButton.Released += NumberIncrementButton_OnReleased;
        }

        #endregion

        #region Events Callbacks

        private void BEvents_ONLINE_OnLaunchSessionSucceeded(BEventHandle<SLobbyArg> handle)
        {
            container.ShowUI(true);
        }

        private void BEvents_ONLINE_OnShutdownSession(BEventHandle<ELeaveOnlineSessionReason, ENetworkID> handle)
        {
            ResetCounter();
            container.DisableUI();
        }

        private void BEvents_ONLINE_OnRemotePlayerJoined(BEventHandle<ENetworkID> handle)
        {
            if (BOnlineManager.Inst.Authority != EAuthority.HOST)
                return;
            
            BEvents.EXAMPLE_NumberIncremented.Invoke(new(counter), BEventBroadcastType.TO_ALL);
        }

        private void BEvents_EXAMPLE_OnNumberIncremented(BEventHandle<int> handle)
        {
            SetCounter(handle.Arg1);
        }

        private void NumberIncrementButton_OnReleased(BButton button, bool cursorInside)
        {
            IncrementCounter();
        }

        #endregion

        #region Others

        private void ResetCounter()
        {
            counter = 0;
            numberText.SetText(counter.ToString());
        }

        private void IncrementCounter()
        {
            counter++;
            numberText.SetText(counter.ToString());
            
            BEvents.EXAMPLE_NumberIncremented.Invoke(new(counter), BEventBroadcastType.TO_ALL);
        }

        private void SetCounter(int newCounter)
        {
            counter = newCounter;
            numberText.SetText(counter.ToString());
        }

        #endregion
    }
}
