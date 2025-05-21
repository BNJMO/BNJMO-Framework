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

            AbstractMultiplayerHandler multiplayerHandler = BMultiplayerManager.Inst.MultiplayerHandler;
            if (multiplayerHandler)
            {
                multiplayerHandler.OnNewClientListenerJoined(this);
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            AbstractMultiplayerHandler multiplayerHandler = BMultiplayerManager.Inst.MultiplayerHandler;
            if (multiplayerHandler)
            {
                multiplayerHandler.OnClientListenerLeft(this);
            }

            if (networkID != ENetworkID.NONE)
            {
                BEvents.MULTIPLAYER_ClientLeft.Invoke(new(networkID));
            }
        }

        #endregion

        #region Events Callbacks


        #endregion

        #region Others

        /* RPCs */
        [ClientRpc]
        private void BroadcastNetworkID_ClientRpc(ENetworkID newNetworkID)
        {
            if (BMultiplayerManager.Inst.HandlerStateMachine.CurrentState != EMultiplayerState.InParty
                || newNetworkID == networkID)
                return;

            // The NetworkID should be only changed from NONE to the new one. If the value here is not NONE, it means 
            // the NetworkID of this player listener changed during runtime which is not a normal behaviour.
            IS_NONE(networkID);

            networkID = newNetworkID;

            if (IsLocalPlayer
                && IsHost == false)
            {
                AbstractMultiplayerHandler multiplayerHandler = BMultiplayerManager.Inst.MultiplayerHandler;
                if (multiplayerHandler)
                {
                    multiplayerHandler.OnLocalPlayerControllerIDAssigned(this);
                }
            }

            BEvents.MULTIPLAYER_ClientJoined.Invoke(new(networkID));
        }

        [ServerRpc]
        private void BroadcastEvent_ServerRpc(string serializedHandle, BEventBroadcastType broadcastType,
            ENetworkID targetNetworkID, ENetworkID fromNetworkID)
        {
            if (BMultiplayerManager.Inst.HandlerStateMachine.CurrentState != EMultiplayerState.InParty)
                return;
            
            BroadcastEvent_ClientRpc(serializedHandle, broadcastType, targetNetworkID, NetworkID);
        }
        
        [ClientRpc]
        private void BroadcastEvent_ClientRpc(string serializedHandle, BEventBroadcastType broadcastType, 
            ENetworkID targetNetworkID, ENetworkID fromNetworkID)
        {
            if (BMultiplayerManager.Inst.HandlerStateMachine.CurrentState != EMultiplayerState.InParty)
                return;

            switch (broadcastType)
            {
                case BEventBroadcastType.TO_ALL:
                case BEventBroadcastType.TO_ALL_OTHERS:
                    if (fromNetworkID != BMultiplayerManager.Inst.LocalNetworkID)
                    {
                        BEventManager.Inst.OnBEventBroadcast(serializedHandle);
                    }
                    break;
                
                case BEventBroadcastType.TO_TARGET:
                    if (fromNetworkID != targetNetworkID
                        && targetNetworkID == BMultiplayerManager.Inst.LocalNetworkID)
                    {
                        BEventManager.Inst.OnBEventBroadcast(serializedHandle);
                    }
                    break;
            }
        }
        
    #endregion
    }
}
