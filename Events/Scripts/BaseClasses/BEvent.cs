using UnityEngine;
using System;
using Newtonsoft.Json;

namespace BNJMO
{
    public class BEvent<H> : AbstractBEvent where H : AbstractBEventHandle
    {
        
        #region Public Events

        public event Action<H> Event;

        #endregion

        #region Public Methods

        /* Constructor */
        public BEvent(string bEventName)
        {
            BEventName = bEventName;
            BEvents.AllReplicatedBEvents.Add(BEventName, this);
        }
        
        /* Invocation */
        public void Invoke(H eventHandle)
        {
            Invoke(eventHandle, BEventBroadcastType.LOCAL, true);
        }

        public void Invoke(H eventHandle, bool logEvent = true, ENetworkID targetNetworkID = ENetworkID.NONE)
        {
            eventHandle.InvokingBEventName = BEventName;
            eventHandle.logEvent = logEvent;
            BEventManager.Inst.OnBEventInvoked(this, eventHandle, BEventBroadcastType.LOCAL, targetNetworkID);
        }

        public void Invoke(H eventHandle, BEventBroadcastType eventInvocationType = BEventBroadcastType.LOCAL, 
            bool logEvent = true, ENetworkID targetNetworkID = ENetworkID.NONE)
        {
            eventHandle.InvokingBEventName = BEventName;
            eventHandle.logEvent = logEvent;
            BEventManager.Inst.OnBEventInvoked(this, eventHandle, eventInvocationType, targetNetworkID);
        }
                
        public void OnProceedInvocation(H eventHandle)
        {
            // Log event
            string logMessage = eventHandle.GetLog();
            if (BManager.Inst.Config.IsLogEvents
                && logMessage != ""
                && eventHandle.logEvent)
            {
                string networkID = "";
                if (BManager.Inst.Config.IsLogEventsNetworkID)
                {
                    networkID = " - Sent by : " + eventHandle.InvokingNetworkID;
                }
                Debug.Log("<color=green>[EVENT]</color> "
                    //+ "<color=red>[" + BUtils.GetTimeAsString() + "] </color>"
                    + BEventName + " : " + logMessage + networkID);
            }

            // Invoke event to all local listeners
            if (Event != null)
            {
                Event.Invoke(eventHandle);
            }
        }

        public override void OnReplicatedEvent(string serializedBEHandle)
        {
            //H deserializedBEHandle = JsonConvert.DeserializeObject<H>(serializedBEHandle);
            H deserializedBEHandle = BUtils.DeserializeObject<H>(serializedBEHandle);

            OnProceedInvocation(deserializedBEHandle);
        }

        /* Subscription */
        public static BEvent<H> operator +(BEvent<H> a, Action<H> b)
        {
            a.Event += b;
            return a;
        }

        public static BEvent<H> operator -(BEvent<H> a, Action<H> b)
        {
            a.Event -= b;
            return a;
        }
        
        public void AddListener(Action<H> callbackAction)
        {
            Event += callbackAction;
        }

        public void RemoveListener(Action<H> callbackAction)
        {
            Event -= callbackAction;
        }

        public void ClearAllListeners()
        {
            Delegate[] delegates = Event.GetInvocationList();
            foreach (Delegate myDelegate in delegates)
            {
                Event -= (myDelegate as Action<H>);
            }
        }

        #endregion

        #region Inspector Variables

        #endregion

        #region Variables

        #endregion

        #region Life Cycle

        #endregion

        #region Events Callbacks

        #endregion

        #region Others

        #endregion

    }
}
