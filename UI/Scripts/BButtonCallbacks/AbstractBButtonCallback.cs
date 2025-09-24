using UnityEngine;
using System;
using Sirenix.OdinInspector;

/// <summary>
/// UI element that executes a Action when interacted with.
/// Simply drag component on GameObject with a Button and it will automatically binds to event.
/// </summary>

namespace BNJMO
{
    public abstract class AbstractBButtonCallback : BBehaviour
    {
        #region Public Events
        
        public event Action ButtonPressed;  

        #endregion

        #region Public Methods

        public abstract void OnBButtonPressed();

        #endregion

        #region Inspector Variables

        [SerializeField]
        private BButton button;

        [SerializeField] 
        private float executionDelay = 0.0f;

        #endregion

        #region Private Variables

        public BButton BButton => button;

        #endregion

        #region Life Cycle

        protected override void OnValidate()
        {
            base.OnValidate();
            
            if (!CanValidate()) 
                return;

            SetComponentIfNull(ref button);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (IS_NOT_VALID(button, true))
                return;

            button.Released += On_ButtonReleased;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (button)
            {
                button.Released -= On_ButtonReleased;
            }
        }

        #endregion

        #region Events Callbacks

        private void On_ButtonReleased(BButton bButton, bool isInside)
        {
            if (!isInside)
                return;

            if (executionDelay > 0.0f)
            {
                Wait(executionDelay, () => 
                {
                    OnBButtonPressed();
                    ButtonPressed?.Invoke(); 
                });
            }
            else
            {
                OnBButtonPressed();
                ButtonPressed?.Invoke(); 
            }
        }

        #endregion

        #region Others


        #endregion
    }
}