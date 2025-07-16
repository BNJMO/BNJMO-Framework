using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

namespace BNJMO
{
    public class BSelectionCaret : BUIElement
    {
        #region Public Events


        #endregion

        #region Public Methods


        #endregion

        #region Inspector Variables

        [BoxGroup("Selection Caret", centerLabel: true)] 
        
        [SerializeField] [BoxGroup("Selection Caret")] 
        private TMP_SelectionCaret selectionCaret;

        #endregion

        #region Variables


        #endregion

        #region Life Cycle

        protected override void OnValidate()
        {
            if (!CanValidate()) return;
            base.OnValidate();

            objectNamePrefix = "SC_";

            base.OnValidate();
            
            SetComponentIfNull(ref selectionCaret);
        }

        #endregion

        #region Events Callbacks


        #endregion

        #region Others

        protected override void OnUIShown()
        {
            base.OnUIShown();

            if (selectionCaret)
            {
                selectionCaret.enabled = true;
            }
        }

        protected override void OnUIHidden()
        {
            base.OnUIHidden();

            if (selectionCaret)
            {
                selectionCaret.enabled = false;
            }
        }

        #endregion
    }
}
