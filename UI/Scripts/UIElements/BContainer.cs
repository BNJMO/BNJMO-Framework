using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace BNJMO
{
    public class BContainer : BUIElement
    {
        protected override void OnValidate()
        {
            if (!CanValidate()) return;
            
            objectNamePrefix = "C_";

            base.OnValidate();
            
        }

    }
}
