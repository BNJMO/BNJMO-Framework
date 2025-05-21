﻿using System;
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

        public void OnBEventInvoked<H>(BEvent<H> bEvent, H eventHandle, BEventBroadcastType broadcastType, 
            ENetworkID targetNetworkID) where H : AbstractBEventHandle
        {
            if (broadcastType == BEventBroadcastType.LOCAL)
            {
                bEvent.OnProceedInvocation(eventHandle);
            }
            else
            {
                if (BMultiplayerManager.Inst.HandlerStateMachine.CurrentState != EMultiplayerState.InParty)
                {
                    LogConsoleWarning($"Trying to broadcast event '{eventHandle.InvokingBEventName}' but Multiplayer Manager is not In Party");
                    bEvent.OnProceedInvocation(eventHandle);
                    return;
                }
                
                switch (broadcastType)
                {
                    case BEventBroadcastType.TO_ALL:
                        bEvent.OnProceedInvocation(eventHandle);
                        BMultiplayerManager.Inst.RequestBroadcastEvent(eventHandle, broadcastType, targetNetworkID);
                        break;
                    case BEventBroadcastType.TO_TARGET when BMultiplayerManager.Inst.LocalNetworkID == targetNetworkID:
                        bEvent.OnProceedInvocation(eventHandle);
                        break;
                    case BEventBroadcastType.TO_TARGET:
                    case BEventBroadcastType.TO_ALL_OTHERS:
                        BMultiplayerManager.Inst.RequestBroadcastEvent(eventHandle, broadcastType, targetNetworkID);
                        break;
                }
            }
        }

        public void OnBEventBroadcast(string serializedHandle)
        {
            AbstractBEventHandle deserializedBEventHandle = BUtils.DeserializeObject<AbstractBEventHandle>(serializedHandle);

            string callingBEventName = deserializedBEventHandle.InvokingBEventName;

            if (IS_KEY_CONTAINED(BEvents.AllReplicatedBEvents, callingBEventName)
                && IS_NOT_NULL(BEvents.AllReplicatedBEvents[callingBEventName]))
            {
                BEvents.AllReplicatedBEvents[callingBEventName].OnReplicatedEvent(serializedHandle);
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