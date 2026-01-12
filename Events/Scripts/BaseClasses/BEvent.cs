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

            if (bEventName == "")
            {
                Debug.LogError($"Trying to register a BEvent with no name!");
                return;
            }
            if (BEvents.AllReplicatedBEvents.ContainsKey(BEventName))
            {
                Debug.LogError($"Trying to register the BEvent '{BEventName}' but it is already registered!");
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
            LogEvent(eventHandle, BEventBroadcastType.LOCAL);
            BEventManager.Inst?.OnBEventInvoked(this, eventHandle, BEventBroadcastType.LOCAL, targetNetworkID);
        }

        public void Invoke(H eventHandle, BEventBroadcastType broadcastType = BEventBroadcastType.LOCAL, 
            bool logEvent = true, ENetworkID targetNetworkID = ENetworkID.NONE)                                             // TODO: Add another overload to remove bool
        {
            eventHandle.InvokingBEventName = BEventName;
            eventHandle.logEvent = logEvent;
            LogEvent(eventHandle, broadcastType);
            BEventManager.Inst?.OnBEventInvoked(this, eventHandle, broadcastType, targetNetworkID);
        }
                
        public void OnProceedInvocation(H eventHandle)
        {
            Event?.Invoke(eventHandle);
        }

        public void LogEvent(H eventHandle, BEventBroadcastType broadcastType)
        {
            if (!BManager.Inst)
                return;
            
            BConfig config = BManager.Inst.Config;
            if (config.LogBEvents
                && eventHandle.logEvent)
            {
                string logText = "<color=green>[EVENT]</color> <color=white>" + BEventName + "</color>";
                
                string logMessage = eventHandle.GetLog();
                if (logMessage != "")
                {
                    logText += " : " + logMessage;
                }
                
                if (config.LogBEventsNetworkID)
                {
                    logText += $" {BConsts.SEPARATOR} From : " + eventHandle.InvokingNetworkID;
                }  
                
                if (config.LogBEventsBroadcastType
                    && broadcastType != BEventBroadcastType.NONE)
                {
                    logText += $" {BConsts.SEPARATOR} Broadcast : " + broadcastType;
                }

                if (config.LogBEventsTimestamp)
                {
                    logText += $" {BConsts.SEPARATOR} Timestamp : " + eventHandle.InvocationTime;
                }

                if (config.LogBEventsPing)
                {
                    float ping = BUtils.GetTimeAsInt() - eventHandle.InvocationTime;
                    logText += $" {BConsts.SEPARATOR} Ping : " + ping;
                }
                Debug.Log(logText);
            }
        }

        public override void OnReplicatedEvent(string serializedBEHandle)
        {
            H deserializedBEHandle = BUtils.DeserializeObject<H>(serializedBEHandle);

            OnProceedInvocation(deserializedBEHandle);
            LogEvent(deserializedBEHandle, BEventBroadcastType.NONE);
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
