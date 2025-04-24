using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class LocalBEventDispatcher : AbstractBEventDispatcher
    {

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
            //LogConsole(bEvent.BEventName + " : " + bEHandle.GetDebugMessage());

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


        #endregion

        #region Life Cycle
        
        protected override void Awake()
        {
            base.Awake();

            LocalNetworkID = ENetworkID.LOCAL;
        }
        
        #endregion
    }
}
