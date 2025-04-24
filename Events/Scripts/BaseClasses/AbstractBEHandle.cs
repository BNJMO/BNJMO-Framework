using System;

namespace BNJMO
{
    [Serializable]
    public class AbstractBEHandle
    {
        public string InvokingBEventName = "";

        public ENetworkID InvokingNetworkID = ENetworkID.NONE;

        public int InvocationTime;

        public bool DebugEvent = true;

        public AbstractBEHandle()
        {
            InvocationTime = BUtils.GetTimeAsInt();
        }

        ///// <summary>
        ///// Gets the corresponding Debug Message to this event handle.
        ///// If returned object is null, this means there is no Debug Message associated to this Event Handle
        ///// </summary>
        public virtual string GetDebugMessage()
        {
            return "";
        }
    }
}