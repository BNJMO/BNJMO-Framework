using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using Unity.Netcode;

namespace BNJMO
{
    public class NetcodePlayerListener : NetcodeBBehaviour, IMultiplayerPlayerListener
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
                LogConsoleRed("Broadcasting event from HOST");
                BroadcastEvent_ClientRpc(serializedHandle, broadcastType, targetNetworkID, NetworkID);
            }
            else
            {
                LogConsoleRed("Broadcasting event to server");
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

        bool IMultiplayerPlayerListener.IsLocalPlayer
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
                multiplayerHandler.OnNewPlayerListenerJoined(this);
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            AbstractMultiplayerHandler multiplayerHandler = BMultiplayerManager.Inst.MultiplayerHandler;
            if (multiplayerHandler)
            {
                multiplayerHandler.OnPlayerListenerLeft(this);
            }

            if (networkID != ENetworkID.NONE)
            {
                BEvents.MULTIPLAYER_RemotePlayerLeft.Invoke(new(networkID));
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
            if (newNetworkID == networkID)
                return;

            // The NetworkID should be only changed from NONE to the new one. If the value here is not NONE, it means 
            // the NetworkID of this player listener changed during runtime which is not a normal behaviour.
            IS_NONE(networkID);

            networkID = newNetworkID;

            if (IsLocalPlayer)
            {
                AbstractMultiplayerHandler multiplayerHandler = BMultiplayerManager.Inst.MultiplayerHandler;
                if (multiplayerHandler)
                {
                    multiplayerHandler.OnLocalPlayerControllerIDAssigned(this);
                }
            }

            BEvents.MULTIPLAYER_RemotePlayerJoined.Invoke(new(networkID));
        }

        [ServerRpc]
        private void BroadcastEvent_ServerRpc(string serializedHandle, BEventBroadcastType broadcastType,
            ENetworkID targetNetworkID, ENetworkID fromNetworkID)
        {
            LogConsoleRed($"On Broadcast server | from {fromNetworkID}");
            BroadcastEvent_ClientRpc(serializedHandle, broadcastType, targetNetworkID, NetworkID);
        }
        
        [ClientRpc]
        private void BroadcastEvent_ClientRpc(string serializedHandle, BEventBroadcastType broadcastType, 
            ENetworkID targetNetworkID, ENetworkID fromNetworkID)
        {
            LogConsoleRed($"On Broadcast client | from {fromNetworkID}");

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
