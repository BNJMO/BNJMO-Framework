using UnityEngine;
using System;
using Newtonsoft.Json;

namespace BNJMO
{
    public class BEvent<H> : AbstractBEvent where H : AbstractBEHandle
    {
        public event Action<H> Event;

        public BEvent(string bEventName)
        {
            BEventName = bEventName;
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

        public void Invoke(H eventHandle)
        {
            Invoke(eventHandle, BEventReplicationType.LOCAL, true);
        }

        public void Invoke(H eventHandle, bool debugEvent = true, ENetworkID targetNetworkID = ENetworkID.NONE)
        {
            eventHandle.InvokingBEventName = BEventName;
            eventHandle.DebugEvent = debugEvent;
            BEventManager.Inst.OnBEventInvoked(this, eventHandle, BEventReplicationType.LOCAL, targetNetworkID);
        }

        public void Invoke(H eventHandle, BEventReplicationType eventInvocationType = BEventReplicationType.LOCAL, bool debugEvent = true, ENetworkID targetNetworkID = ENetworkID.NONE)
        {
            eventHandle.InvokingBEventName = BEventName;
            eventHandle.DebugEvent = debugEvent;
            BEventManager.Inst.OnBEventInvoked(this, eventHandle, eventInvocationType, targetNetworkID);
        }
                
        public void OnProceedInvokation(H eventHandle)
        {
            // Debug event
            string debugMessage = eventHandle.GetDebugMessage();
            if ((BManager.Inst.Config.IsDebugLogEvents == true)
                && (debugMessage != "")
                && (eventHandle.DebugEvent == true))
            {
                string networkID = "";
                if (BManager.Inst.Config.IsDebugEventsNetworkID == true)
                {
                    networkID = " - Sent by : " + eventHandle.InvokingNetworkID;
                }
                Debug.Log("<color=green>[EVENT]</color> "
                    //+ "<color=red>[" + BUtils.GetTimeAsString() + "] </color>"
                    + BEventName + " : " + debugMessage + networkID);
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

            OnProceedInvokation(deserializedBEHandle);
        }

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
    }
}
