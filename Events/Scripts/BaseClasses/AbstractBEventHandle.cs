using System;
using UnityEngine.Serialization;

namespace BNJMO
{
    [Serializable]
    public class AbstractBEventHandle
    {
        public string InvokingBEventName = "";

        public ENetworkID InvokingNetworkID = ENetworkID.LOCAL;

        public int InvocationTime = BUtils.GetTimeAsInt();

        public bool logEvent = true;

        ///// <summary>
        ///// Gets the corresponding Debug Message to this event handle.
        ///// If returned object is null, this means there is no Debug Message associated to this Event Handle
        ///// </summary>
        public virtual string GetLog()
        {
            return "";
        }
    }
}