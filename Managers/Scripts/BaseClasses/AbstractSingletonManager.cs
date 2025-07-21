using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BNJMO
{
    /// <summary>
    /// Singleton behaviour class, used for components that should only have one instance.
    /// Child class can decide weither to initialize on the Awake or through the public function Initialize.
    /// </summary>
    /// <typeparam name="T"> Child class </typeparam>
    public abstract class AbstractSingletonManager<T> : AbstractManager 
        where T : AbstractSingletonManager<T>
    {
        [FoldoutGroup("Singleton Manager")]
        [FoldoutGroup("Singleton Manager")] [SerializeField] private bool overrideSingletonReference = true;
        [FoldoutGroup("Singleton Manager")] [SerializeField] private bool getSingletonReferenceOnValidate = true;
        [FoldoutGroup("Singleton Manager")] [SerializeField] [ReadOnly] private AbstractManager singletonInstanceReference;

        public static T Inst { get; private set; }

        /// <summary>
        /// Returns whether the instance has been initialized or not.
        /// </summary>
        public static bool IsInstanceSet 
        { 
            get
            { 
                if (Inst == null)
                {
                    //Debug.Log("<color=yellow>WARNING! </color> Trying to access reference for Instance of " + "<color=gray>[" + typeof(T) + "]</color> but it was not set!");
                }
                return Inst != null; 
            }
        }

        protected override void OnValidate()
        {
            if (!CanValidate()) return;
            base.OnValidate();

            // if (getSingletonReferenceOnValidate == true)
            // {
            //     InitSingletonReference();
            // }
        }

        /// <summary>
        /// Base awake method that sets the singleton's unique instance.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            // if (getSingletonReferenceOnValidate == false
            //     || IsInstanceSet == false)
            if (!IsInstanceSet)
            {
                InitSingletonReference();
            }
        }

        /// <summary>
        /// Base awake method that resets the singleton's unique instance.
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (Inst == this)
            {
                Inst = null;
            }
        }

        private void InitSingletonReference()
        {
            // if (overrideSingletonReference == true)
            // {
                if (Inst != null)
                {
                    // LogConsoleWarning("Trying to instantiate a second instance of singleton class <color=cyan> " + typeof(T) + " </color> !");
                }
                else
                {
                    Inst = (T)this;
                    singletonInstanceReference = Inst;
                }
            // }
        }
    }
}