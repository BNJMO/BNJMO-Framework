using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public abstract class AbstractBEvent 
    {
        public string BEventName { get; protected set; }

        public abstract void OnReplicatedEvent(string serializedBEHandle);
    }
}
