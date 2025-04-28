using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

namespace BNJMO
{
    public class BEventManager : AbstractSingletonManager<BEventManager>
    {
        #region Public Events

        #endregion

        #region Public Methods
        
        public AbstractBEventDispatcher BEventDispatcher { get; private set; }

        public ENetworkState NetworkState { get; private set; } = ENetworkState.NOT_CONNECTED;

        public ENetworkID LocalNetworkID
        {
            get
            {
                if (IS_NOT_NULL(BEventDispatcher))
                {
                    return BEventDispatcher.LocalNetworkID;
                }
                return ENetworkID.NONE;
            }
        }

        public string LocalIPAddress { get; private set; } = "localhost";

        public ENetworkID[] GetConnectedNetworkIDs()
        {
            if (IS_NOT_NULL(BEventDispatcher))
            {
                return BEventDispatcher.GetConnectedNetworkIDs();
            }
            return new ENetworkID[0];
        }

        public void StartHost()
        {
            if (ARE_EQUAL(LocalNetworkID, ENetworkID.LOCAL)
                && IS_NOT_NULL(BEventDispatcher))
            {
                BEventDispatcher.StartHost();
            }
        }

        public string[] GetAvailableHosts()
        {
            if (IS_NOT_NULL(BEventDispatcher))
            {
                return BEventDispatcher.GetAvailableHosts();
            }
            return new string[0];
        }

        public void Disconnect()
        {
            if (ARE_NOT_EQUAL(LocalNetworkID, ENetworkID.LOCAL)
                && IS_NOT_NULL(BEventDispatcher))
            {
                BEventDispatcher.Disconnect();
            }
        }

        public void ConnectToHost(int hostID)
        {
            if (ARE_EQUAL(LocalNetworkID, ENetworkID.LOCAL)
                && IS_NOT_NULL(BEventDispatcher))
            {
                BEventDispatcher.ConnectToHost(hostID);
            }
        }

        public void SetBEventDispatcher(BEventDispatcherType bEventDispatcherMode)
        {
            // Disconnect
            if (NetworkState != ENetworkState.NOT_CONNECTED)
            {
                Disconnect();
            }

            // Remove existing Dispatcher
            AbstractBEventDispatcher bEventDispatcher = FindObjectOfType<AbstractBEventDispatcher>();
            if (bEventDispatcher)
            {
                RemoveBEventDispatcher(bEventDispatcher);
            }

            // Add new Dispatcher
            bool success = AddBeventDispatcher(bEventDispatcherMode);
            if (success)
            {
                BEvents.NETWORK_NewBEventDispatcherSet.Invoke(new BEHandle<AbstractBEventDispatcher>(BEventDispatcher));
            }
        }

        public void CalculatePing(ENetworkID fromNetworkID)
        {
            BEvents.NETWORK_Ping.Invoke(new BEHandle<ENetworkID, int>(LocalNetworkID, BUtils.GetTimeAsInt()), BEventReplicationType.TO_TARGET, true, fromNetworkID);
        }
 
        /* Callbacks from Dispatcher */
        public void OnBEventInvoked<H>(BEvent<H> bEvent, H bEHandle, BEventReplicationType bEInvocationType, ENetworkID targetNetworkID) where H : AbstractBEHandle
        {
            if (!BEventDispatcher)
            {
                BEventDispatcher = FindFirstObjectByType<AbstractBEventDispatcher>();
            }
            if (IS_NOT_NULL(BEventDispatcher))
            {
                bEHandle.InvokingNetworkID = LocalNetworkID;
                BEventDispatcher.OnBEventInvoked(bEvent, bEHandle, bEInvocationType, targetNetworkID);
            }
        }

        public void OnBEventReplicated(string serializedBEHandle)
        {
            //AbstractBEHandle deserializedBEHandle = JsonConvert.DeserializeObject<AbstractBEHandle>(serializedBEHandle);
            AbstractBEHandle deserializedBEHandle = BUtils.DeserializeObject<AbstractBEHandle>(serializedBEHandle);

            string callingBEventName = deserializedBEHandle.InvokingBEventName;

            if ((IS_NOT_NULL(allReplicatedBEvents))
                && (IS_KEY_CONTAINED(allReplicatedBEvents, callingBEventName))
                && (IS_NOT_NULL(allReplicatedBEvents[callingBEventName])))
            {
                allReplicatedBEvents[callingBEventName].OnReplicatedEvent(serializedBEHandle);
            }
        }

        public void OnUpdatedNetworkState(ENetworkState newNetworkState, AbstractBEventDispatcher bEventDispatcher)
        {
            if (ARE_EQUAL(BEventDispatcher, bEventDispatcher))
            {
                NetworkState = newNetworkState;
                BEvents.NETWORK_NetworkStateUpdated.Invoke(new BEHandle<ENetworkState>(NetworkState));
            }
        }

        #endregion

        #region Inspector Variables


        #endregion

        #region Private Variables
        
        private Dictionary<string, AbstractBEvent> allReplicatedBEvents = new Dictionary<string, AbstractBEvent>();

        #endregion

        #region Life Cycle
        
        protected override void OnEnable()
        {
            base.OnEnable();

            BEvents.NETWORK_Ping += On_NETWORK_Ping;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            BEvents.NETWORK_Ping -= On_NETWORK_Ping;
        }

        protected override void Awake()
        {
            base.Awake();

            // Initialize all replicated BEvents from every BEventsCollection
            foreach (AbstractBEventsCollection bEventsCollection in FindObjectsOfType<AbstractBEventsCollection>())
            {
                foreach (var tupple in bEventsCollection.GetAllReplicatedBEvents())
                {
                    if (IS_KEY_NOT_CONTAINED(allReplicatedBEvents, tupple.Key))
                    {
                        allReplicatedBEvents.Add(tupple.Key, tupple.Value);
                    }
                }
            }

            // Get local IP Address of this device
            LocalIPAddress = BUtils.GetLocalIPAddress();

            // Initialize dispatcher
            SetBEventDispatcher(BManager.Inst.Config.EventDispatcherType);
        }

        protected override void Update()
        {
            base.Update();

            LogCanvas(BConsts.DEBUGTEXT_NetworkState, NetworkState.ToString());
        }

        #endregion

        #region Events Callbacks
        
        private void On_NETWORK_Ping(BEHandle<ENetworkID, int> handle)
        {
            ENetworkID requestingNetworkID = handle.Arg1;
            int startTime = handle.Arg2;
            ENetworkID invokingNetworkID = handle.InvokingNetworkID;

            // Half-way
            if (LocalNetworkID != requestingNetworkID)
            {
                BEvents.NETWORK_Ping.Invoke(new BEHandle<ENetworkID, int>(requestingNetworkID, startTime), BEventReplicationType.TO_TARGET, true, requestingNetworkID);
            }
            // Round Trip
            else
            {
                float ping = BUtils.GetTimeAsInt() - startTime;
                LogConsole("Ping [" + requestingNetworkID + "] : " + ping + "ms");
            }
        }

        #endregion

        #region Others
        
        private bool AddBeventDispatcher(BEventDispatcherType bEventDispatcherMode)
        {
            switch (bEventDispatcherMode)
            {
                case BEventDispatcherType.LOCAL:
                    BEventDispatcher = gameObject.AddComponent<LocalBEventDispatcher>();
                    return true;


                case BEventDispatcherType.NONE:
                    LogConsoleWarning("No Event Dispatcher Mode was selected! Default mode will be used : " + BEventDispatcherType.LOCAL);
                    BEventDispatcher = gameObject.AddComponent<LocalBEventDispatcher>();
                    return true;
            }
            return false;
        }

        private void RemoveBEventDispatcher(AbstractBEventDispatcher bEventDispatcher)
        {
            // Destroy Dispatcher Component
            Destroy(bEventDispatcher);
        }

        #endregion
    }
}