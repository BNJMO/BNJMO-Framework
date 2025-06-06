﻿using System.Collections.Generic;

namespace BNJMO
{
    public abstract class AbstractOnlineHandler : BBehaviour
    {
        #region Public Events


        #endregion

        #region Public Methods

        /* Match making */
        public abstract void CreatePrivateLobby();

        public abstract void JoinPrivateLobby(string lobbyCode);

        public abstract void QuickMatch();

        public abstract void SetLobbyLock(bool isLocked);

        public abstract void ShutdownLobbyAndMultiplayer(ELeaveOnlineSessionReason leaveReason);
        
        /* Player Listener */
        public ENetworkID OnNewClientListenerJoined(IClientListener newClientListener)
        {
            if (Authority != EAuthority.HOST
                || newClientListener == null)
                return ENetworkID.NONE;

            ENetworkID newNetworkID;
            if (newClientListener.IsHost)
            {
                if (IS_KEY_CONTAINED(ConnectedClientListeners, ENetworkID.HOST_1, true))
                    return ENetworkID.NONE;
                
                newNetworkID = ENetworkID.HOST_1;

            }
            else
            {
                newNetworkID = GetNextFreeClientNetworkID();
                if (IS_NONE(newNetworkID, true))
                    return ENetworkID.NONE;
            }

            ConnectedClientListeners.Add(newNetworkID, newClientListener);

            // Let all connected player listeners communicate their ENetworkIDs to everyone else in the party
            foreach (var clientListenerPairItr in ConnectedClientListeners)
            {
                ENetworkID networkIDItr = clientListenerPairItr.Key;
                IClientListener clientListenerItr = clientListenerPairItr.Value;
                clientListenerItr.RequestBroadcastAndSetNetworkID(networkIDItr);
            }
            return newNetworkID;
        }

        public void OnClientListenerLeft(IClientListener clientListener)
        {
            if (Authority != EAuthority.HOST
                || clientListener == null)
                return;

            ENetworkID networkID = clientListener.NetworkID;
            if (ConnectedClientListeners.ContainsKey(networkID) == false)
                return;

            ConnectedClientListeners.Remove(networkID);
        }

        public void OnLocalPlayerControllerIDAssigned(IClientListener playerListener)
        {
            StateMachine.UpdateState(EOnlineState.InOnlineSession);
            LocalClientListener = playerListener;
            BEvents.ONLINE_LaunchSessionSucceeded.Invoke(new());
        }
        
        /* Event Replication */
        public void RequestBroadcastEvent(AbstractBEventHandle eventHandle, BEventBroadcastType broadcastType,
            ENetworkID targetNetworkID)
        {
            if (StateMachine.CurrentState != EOnlineState.InOnlineSession)
            {
                LogConsoleWarning("Trying to broadcast event but the Multiplayer State is not In Party");
                return;
            }
            
            if (LocalClientListener == null)
            {
                LogConsoleWarning("Trying to broadcast event but the local player listener is null!");
                return;
            }
            
            LocalClientListener.RequestBroadcastEvent(eventHandle, broadcastType, targetNetworkID);
        }
        
        #endregion

        #region Inspector Variables
        

        #endregion

        #region Variables
        
        public abstract EAuthority Authority { get; protected set; }

        public abstract string LobbyCode { get; protected set; }
        
        public abstract ELobbyType LobbyType { get; protected set; }

        public abstract IClientListener LocalClientListener { get; protected set; }
        
        public ENetworkID LocalNetworkID
        {
            get
            {
                return LocalClientListener?.NetworkID ?? ENetworkID.LOCAL;
            }
        }

        public StateMachine<EOnlineState> StateMachine { get; } = new ();
        
        public Dictionary<ENetworkID, IClientListener> ConnectedClientListeners { get; } = new(); // TODO: Replicate to other clients. Currently only on host.
        
        #endregion

        #region Life Cycle
        

        #endregion

        #region Events Callbacks

        #endregion
        
        #region Others
        
        /* NetworkID */
        private ENetworkID GetNextFreeClientNetworkID()
        {
            ENetworkID networkID = ENetworkID.NONE;
            foreach (ENetworkID networkIDitr in BConsts.NETWORK_CLIENTS)
            {
                if (ConnectedClientListeners.ContainsKey(networkIDitr) == false)
                {
                    networkID = networkIDitr;
                    break;
                }
            }
            return networkID;
        }
        
        #endregion
    }
}
