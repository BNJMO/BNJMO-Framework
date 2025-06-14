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

            if (BEvents.AllReplicatedBEvents.ContainsKey(BEventName))
            {
                Debug.LogError($"The BEvent '{BEventName}' already exists in the list of events!");
                return;
            }
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
            BEventManager.Inst?.OnBEventInvoked(this, eventHandle, BEventBroadcastType.LOCAL, targetNetworkID);
        }

        public void Invoke(H eventHandle, BEventBroadcastType eventInvocationType = BEventBroadcastType.LOCAL, 
            bool logEvent = true, ENetworkID targetNetworkID = ENetworkID.NONE)                                             // TODO: Add another overload to remove bool
        {
            eventHandle.InvokingBEventName = BEventName;
            eventHandle.logEvent = logEvent;
            BEventManager.Inst?.OnBEventInvoked(this, eventHandle, eventInvocationType, targetNetworkID);
        }
                
        public void OnProceedInvocation(H eventHandle)
        {
            // Log event
            BConfig config = BManager.Inst.Config;
            if (config.LogEvents
                && eventHandle.logEvent)
            {
                string logText = "<color=green>[EVENT]</color> <color=white>" + BEventName + "</color>";
                
                string logMessage = eventHandle.GetLog();
                if (logMessage != "")
                {
                    logText += " : " + logMessage;
                }
                
                if (config.LogEventsNetworkID)
                {
                    logText += " | From : " + eventHandle.InvokingNetworkID;
                }

                if (config.LogEventsTimestamp)
                {
                    logText += " | Timestamp : " + eventHandle.InvocationTime;
                }

                if (config.LogEventsPing)
                {
                    float ping = BUtils.GetTimeAsInt() - eventHandle.InvocationTime;
                    logText += " | Ping : " + ping;
                }
                Debug.Log(logText);
            }

            // Invoke event to all local listeners
            if (Event != null)
            {
                Event.Invoke(eventHandle);
            }
        }

        public override void OnReplicatedEvent(string serializedBEHandle)
        {
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
