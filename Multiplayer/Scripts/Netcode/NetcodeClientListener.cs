using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using Unity.Netcode;

namespace BNJMO
{
    public class NetcodeClientListener : NetcodeBBehaviour, IClientListener
    {
        #region Public Events


        #endregion

        #region Public Methods

        public void RequestBroadcastAndSetNetworkID(ENetworkID newNetworkID)
        {
            BroadcastNetworkID_ClientRpc(newNetworkID);
        }

        public void RequestBroadcastEvent(AbstractBEventHandle eventHandle, BEventBroadcastType broadcastType,
            ENetworkID targetNetworkID)
        {
            string serializedHandle = JsonConvert.SerializeObject(eventHandle, new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            if (IsHost)
            {
                BroadcastEvent_ClientRpc(serializedHandle, broadcastType, targetNetworkID, NetworkID);
            }
            else
            {
                BroadcastEvent_ServerRpc(serializedHandle, broadcastType, targetNetworkID, NetworkID);
            }
        }

        #endregion

        #region Inspector Variables

        [SerializeField] [ReadOnly] private ENetworkID networkID;

        #endregion

        #region Variables

        public ENetworkID NetworkID
        {
            get => networkID;
            set => networkID = value;
        }

        public new bool IsHost
        {
            get => IsOwnedByServer;
            set { }
        }

        bool IClientListener.IsLocalClient
        {
            get => IsLocalPlayer;
            set { }
        }

        #endregion

        #region Life Cycle

        protected override void OnNetworkPostSpawn()
        {
            base.OnNetworkPostSpawn();

            AbstractOnlineHandler onlineHandler = BOnlineManager.Inst.OnlineHandler;
            if (onlineHandler)
            {
                onlineHandler.OnNewClientListenerJoined(this);
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            AbstractOnlineHandler onlineHandler = BOnlineManager.Inst.OnlineHandler;
            if (onlineHandler)
            {
                onlineHandler.OnClientListenerLeft(this);
            }

            if (NetworkID != ENetworkID.NONE)
            {
                BEvents.ONLINE_ClientLeft.Invoke(new(NetworkID));
            }
        }

        #endregion

        #region Events Callbacks


        #endregion

        #region Others

        /* Network ID RPCs */
        [ClientRpc]
        private void BroadcastNetworkID_ClientRpc(ENetworkID newNetworkID)
        {
            if (newNetworkID == NetworkID
                || newNetworkID == BOnlineManager.Inst.LocalNetworkID)
                return;
            

            // The NetworkID should be only changed from NONE to the new one. If the value here is not NONE, it means 
            // the NetworkID of this player listener changed during runtime which is not a normal behaviour.
            IS_NONE(NetworkID);

            NetworkID = newNetworkID;

            if (IsLocalPlayer)
            {
                ResponseNetworkID_ServerRpc(newNetworkID);
            }
        }

        [ServerRpc]
        private void ResponseNetworkID_ServerRpc(ENetworkID newNetworkID)
        {
            ConfirmNetworkID_ClientRpc(newNetworkID);
        }

        [ClientRpc]
        private void ConfirmNetworkID_ClientRpc(ENetworkID newNetworkID)
        {
            if (IsLocalPlayer)
            {
                AbstractOnlineHandler onlineHandler = BOnlineManager.Inst.OnlineHandler;
                if (IS_NOT_NULL(onlineHandler))
                {
                    onlineHandler.OnLocalPlayerControllerIDAssigned(this);
                }
            }
            BEvents.ONLINE_ClientJoined.Invoke(new(NetworkID));
        }

        /* BEvents RPCs */
        [ServerRpc]
        private void BroadcastEvent_ServerRpc(string serializedHandle, BEventBroadcastType broadcastType,
            ENetworkID targetNetworkID, ENetworkID fromNetworkID)
        {
            if (ARE_ENUMS_NOT_EQUAL(BOnlineManager.Inst.HandlerStateMachine.CurrentState, EOnlineState.InOnlineSession, true))
                return;
            
            BroadcastEvent_ClientRpc(serializedHandle, broadcastType, targetNetworkID, NetworkID);
        }
        
        [ClientRpc]
        private void BroadcastEvent_ClientRpc(string serializedHandle, BEventBroadcastType broadcastType, 
            ENetworkID targetNetworkID, ENetworkID fromNetworkID)
        {
            if (ARE_ENUMS_NOT_EQUAL(BOnlineManager.Inst.HandlerStateMachine.CurrentState, EOnlineState.InOnlineSession, true))
                return;

            switch (broadcastType)
            {
                case BEventBroadcastType.TO_ALL:
                case BEventBroadcastType.TO_ALL_OTHERS:
                    if (fromNetworkID != BOnlineManager.Inst.LocalNetworkID)
                    {
                        BEventManager.Inst.OnBEventBroadcast(serializedHandle);
                    }
                    break;
                
                case BEventBroadcastType.TO_TARGET:
                    if (fromNetworkID != targetNetworkID
                        && targetNetworkID == BOnlineManager.Inst.LocalNetworkID)
                    {
                        BEventManager.Inst.OnBEventBroadcast(serializedHandle);
                    }
                    break;
            }
        }
        
    #endregion
    }
}
