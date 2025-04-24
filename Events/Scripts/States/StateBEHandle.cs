using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class StateBEHandle<E> : AbstractBEHandle
    {
        public E NewState { get; }
        public E LastState { get; }

        public StateBEHandle(E newState, E lastState) : base()
        {
            NewState = newState;
            LastState = lastState;
        }

        public override string GetDebugMessage()
        {
            return "State updated from " + LastState + " to " + NewState;
        }
    }
}
