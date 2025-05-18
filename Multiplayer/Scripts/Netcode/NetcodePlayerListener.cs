using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Unity.Netcode;

namespace BNJMO
{
    public class NetcodePlayerListener : NetcodeBBehaviour
    {
        #region Public Events


        #endregion

        #region Public Methods

        [ClientRpc]
        public void CommunicateNetworkID_ClientRpc(ENetworkID newNetworkID)
        {
            if (newNetworkID == networkID)
                return;
            
            networkID = newNetworkID;
            BEvents.MULTIPLAYER_RemotePlayerJoined.Invoke(new(networkID));
        }
        
        #endregion

        #region Inspector Variables

        [SerializeField] [ReadOnly] private ENetworkID networkID;

        #endregion

        #region Variables

        public ENetworkID NetworkID => networkID;
        
        public new bool IsHost => IsOwnedByServer;
        
        #endregion

        #region Life Cycle

        protected override void OnNetworkPostSpawn()
        {
            base.OnNetworkPostSpawn();

            AbstractMultiplayerHandler multiplayerHandler = BMultiplayerManager.Inst.MultiplayerHandler; 
            if (multiplayerHandler
                && multiplayerHandler is NetcodeMultiplayerHandler netcodeMultiplayerHandler)
            {
                netcodeMultiplayerHandler.OnNewPlayerListenerJoined(this);
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            
            AbstractMultiplayerHandler multiplayerHandler = BMultiplayerManager.Inst.MultiplayerHandler; 
            if (multiplayerHandler
                && multiplayerHandler is NetcodeMultiplayerHandler netcodeMultiplayerHandler)
            {
                netcodeMultiplayerHandler.OnPlayerListenerLeft(this);
            }

            if (networkID != ENetworkID.NONE)
            {
                BEvents.MULTIPLAYER_RemotePlayerLeft.Invoke(new (networkID));
            }
        }
        
        #endregion

        #region Events Callbacks


        #endregion

        #region Others


        #endregion
    }
}
