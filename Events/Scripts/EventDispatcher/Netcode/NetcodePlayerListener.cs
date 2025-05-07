using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace BNJMO
{
    public class NetcodePlayerListener : BNetCodeBehaviour
    {
        #region Public Events


        #endregion

        #region Public Methods


        #endregion

        #region Inspector Variables


        #endregion

        #region Variables

        public new bool IsHost => IsOwner && IsOwnedByServer;

        #endregion

        #region Life Cycle

        protected override void OnNetworkPostSpawn()
        {
            base.OnNetworkPostSpawn();
            
            LogConsole($"Hey I'm host : {IsOwner && IsOwnedByServer}  | ID : {NetworkObjectId}");
        }


        #endregion

        #region Events Callbacks


        #endregion

        #region Others

        

        #endregion
    }
}
