using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public abstract class AbstractBEventsCollection : AbstractSingletonManager<BEvents>
    {
        /// <summary>
        /// Example of dictionary that needs to be defined in the children classes.
        /// Define entries directly in the attribute declaration so that they are already contained for the Awake method.
        /// Have a look at the BEventsCollection for a reference.
        /// </summary>
        private Dictionary<string, AbstractBEvent> allReplicatedBEvents = new Dictionary<string, AbstractBEvent>();

        /// <summary>
        /// Return reference to the private allREplicatedBevents Dictionary
        /// </summary>
        /// <returns></returns>
        public abstract Dictionary<string, AbstractBEvent> GetAllReplicatedBEvents();
    }
}
