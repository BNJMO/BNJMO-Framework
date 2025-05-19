using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class StateBEventHandle<E> : AbstractBEventHandle
    {
        public E NewState { get; }
        public E LastState { get; }

        public StateBEventHandle(E newState, E lastState) : base()
        {
            NewState = newState;
            LastState = lastState;
        }

        public override string GetLog()
        {
            return "State updated from " + LastState + " to " + NewState;
        }
    }
}
