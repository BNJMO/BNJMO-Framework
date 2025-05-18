using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class NetcodeBEventDispatcher : AbstractBEventDispatcher
    {
        #region Public Events


        #endregion

        #region Public Methods
                
        /* AbstractBEventDispatcher */
        public override BEventDispatcherType GetBEventDispatcherType()
        {
            return BEventDispatcherType.LOCAL;
        }

        public override ENetworkID[] GetConnectedNetworkIDs()
        {
            return new ENetworkID[] { ENetworkID.LOCAL };
        }

        public override void OnBEventInvoked<H>(BEvent<H> bEvent, H bEHandle, BEventReplicationType eventInvocationType, ENetworkID targetNetworkID)
        {
            bEvent.OnProceedInvokation(bEHandle);
        }

        public override void StartHost()
        {
        }

        public override void ConnectToHost(int hostID)
        {
        }

        public override void Disconnect()
        {
        }

        public override string[] GetAvailableHosts()
        {
            return new string[0];
        }
        
        /* Netcode Player Listener */
        // public void OnPlayerConnected(NetcodePlayerListener playerListener)
        // {
        //     ENetworkID newNetworkID;
        //     //
        //     // bool isHost = playerListener.Is
        //     // if (playerListener)
        //     // playerListenersMap.Add(playerListener, playerListener.NetworkObjectId);
        // }

        #endregion

        #region Inspector Variables


        #endregion

        #region Variables


        #endregion

        #region Life Cycle

        protected override void Awake()
        {
            base.Awake();

            LocalNetworkID = ENetworkID.LOCAL;
        }

        #endregion

        #region Events Callbacks


        #endregion

        #region Others

        // private Dictionary<NetcodePlayerListener, ENetworkID> playerListenersMap = new();


        #endregion
    }
}
