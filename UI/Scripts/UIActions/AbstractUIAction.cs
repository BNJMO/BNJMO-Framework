using UnityEngine;
using System;
using Sirenix.OdinInspector;

/// <summary>
/// UI element that executes a Action when interacted with.
/// Simply drag component on GameObject with a Button and it will automatically binds to event.
/// </summary>

namespace BNJMO
{
    [RequireComponent(typeof(BButton))]
    public abstract class AbstractUIAction : BBehaviour
    {
        public event Action ActionButtonExecuted;

        public BButton BButton { get { return GetComponent<BButton>(); } }

        [BoxGroup("AbstractUIAction", centerLabel: true)]
        [BoxGroup("AbstractUIAction")] [SerializeField] private bool delayedEventExecution = false;



        /// <summary>
        /// Trigger Execute() whenever the button is pressed.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            // Bind events from MaleficusButton
            BButton.Released += On_BButton_ButtonPressed;
        }

        /// <summary>
        /// Unsubscribe to ActionButtonPressed event when object is destroyed.
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            ClearEventCallbakcs(ActionButtonExecuted);
        }

        private void On_BButton_ButtonPressed(BButton maleficusButton, bool cursorInsideButton)
        {
            if (cursorInsideButton == true)
            {
                Execute();
            }
        }

        /// <summary>
        /// Action that should be triggered when Button is pressed. 
        /// Extend it in child class when needed.
        /// </summary>
        protected virtual void Execute()
        {
            if (delayedEventExecution == false)
            {
                InvokeActionExecutedEvent();
            }
            else
            {
                Invoke(nameof(DelayedExecuteCoroutine), 0.1f);
            }
        }

        private void DelayedExecuteCoroutine()
        {
            InvokeActionExecutedEvent();
        }

        protected void InvokeActionExecutedEvent()
        {
            if (ActionButtonExecuted != null)
            {
                ActionButtonExecuted.Invoke();
                LogConsole("Executed");
            }
        }
    }
}