using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public abstract class AbstractBEventDispatcher : BBehaviour
    {
        public abstract BEventDispatcherType GetBEventDispatcherType();

        public ENetworkID LocalNetworkID { get; protected set; } = ENetworkID.LOCAL;

        public abstract ENetworkID[] GetConnectedNetworkIDs();

        public abstract void OnBEventInvoked<H>(BEvent<H> bEvent, H bEHandle, BEventReplicationType eventInvocationType, ENetworkID targetNetworkID) where H : AbstractBEHandle;

        public abstract void StartHost();
        
        public abstract string[] GetAvailableHosts();

        public abstract void ConnectToHost(int hostID);

        public abstract void Disconnect();

    }
}