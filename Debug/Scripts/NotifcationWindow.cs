using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BNJMO
{
    public class NotifcationWindow : BUIElement
    {
        #region Public Events


        #endregion

        #region Public Methods

        // TODO : add priority?

        public void Notify(string text)
        {
            OnNotificationAdded(text);
        }

        #endregion

        #region Serialized Fields
        [BoxGroup("Notifcation Window", centerLabel: true)] 
        [SerializeField] 
        private BContainer panelContainer;

        [BoxGroup("Notifcation Window")]
        [SerializeField] 
        private BText bText;

        [BoxGroup("Notifcation Window")]
        [SerializeField] 
        private AnimationLerpTransform animLerp_Up;

        [BoxGroup("Notifcation Window")]
        [SerializeField] 
        private AnimationLerpTransform animLerp_Down;

        [BoxGroup("Notifcation Window")]
        [SerializeField]
        private float animationDuration = 0.5f;

        [BoxGroup("Notifcation Window")]
        [SerializeField]
        private float notificationStayDuration = 1.0f;

        [BoxGroup("Notifcation Window")]
        [SerializeField]
        private float panelExtensionSize = 1.0f;

        [BoxGroup("Notifcation Window")]
        [SerializeField]
        private int lettersPerLine = 50;

        [BoxGroup("Notifcation Window")]
        [SerializeField]
        private int maxNumberOfLines = 3;

        [BoxGroup("Notifcation Window")]
        [SerializeField]
        private AudioClip notificationSound;

        [BoxGroup("Notifcation Window")]
        [SerializeField]
        [TextArea]
        private string testText = "Test";

        [Button("Notify Test Text")] private void Button_TestText() { TestText(); }


        #endregion

        #region Private Variables
        private Vector3 panelOriginalSize;
        private Queue<string> notificationsQueue = new Queue<string>();
        private IEnumerator WaitThenPlayDownAnimationEnumerator;
        private bool isNotificationRunning = false;

        #endregion

        #region Life Cycle
        protected override void OnValidate()
        {
            if (!CanValidate()) return;
            
            objectNamePrefix = "C_";
            UIElementName = "Notification Window";

            base.OnValidate();

            if (bText == null)
            {
                bText = GetComponentInChildren<BText>();
            }

            if (panelContainer == null)
            {
                panelContainer = GetComponentInChildren<BContainer>();
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (IS_NOT_NULL(panelContainer))
            {
                panelOriginalSize = panelContainer.transform.localScale;
            }

            if (IS_NOT_NULL(animLerp_Up)
                && IS_NOT_NULL(animLerp_Down))
            {

                animLerp_Up.PlayDuration = animationDuration;
                animLerp_Up.AnimationUpdated += On_AnimLerp_Up_AnimationUpdated;
                animLerp_Up.AnimationEnded += On_AnimLerp_Up_AnimationEnded;

                animLerp_Down.PlayDuration = animationDuration;
                animLerp_Down.AnimationUpdated += On_AnimLerp_Down_AnimationUpdated;
                animLerp_Down.AnimationEnded += On_AnimLerp_Down_AnimationEnded;
            }
        }




        /* Up */
        private void On_AnimLerp_Up_AnimationUpdated(AnimationLerp<Transform> arg1, Transform value)
        {
            //transform.localPosition = value.position;
        }

        private void On_AnimLerp_Up_AnimationEnded(AnimationLerp<Transform> obj)
        {
            StartNewCoroutine(ref WaitThenPlayDownAnimationEnumerator, WaitThenPlayDownAnimationCoroutine());
        }

        /* Down */
        private void On_AnimLerp_Down_AnimationUpdated(AnimationLerp<Transform> arg1, Transform value)
        {
            //transform.localPosition = value.position;
        }

        private void On_AnimLerp_Down_AnimationEnded(AnimationLerp<Transform> obj)
        {
            OnNotificationEnded();
        }

        #endregion

        #region Events Callbacks




        #endregion

        #region Others
        private void OnNotificationAdded(string text)
        {
            if (IS_NOT_NULL(animLerp_Up)
                && IS_NOT_NULL(animLerp_Down))
            {
                if (notificationsQueue.Count == 0
                    && isNotificationRunning == false)
                {
                    SetNotification(text);
                }
                else
                {
                    notificationsQueue.Enqueue(text);
                }
            }
        }

        private void SetNotification(string text)
        {
            if (IS_NOT_NULL(bText))
            {
                // Resize the panel image according to the given text size
                string textToShow = "";
                int linesCount = 1;
                int lettersCount = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    if (lettersCount == lettersPerLine)
                    {
                        if (linesCount + 1 > maxNumberOfLines)
                        {
                            textToShow = textToShow.Remove(textToShow.Length - 3);
                            textToShow += "...";
                            break;
                        }
                        else
                        {
                            linesCount++;
                            textToShow += "\n";
                        }
                        lettersCount = 0;
                    }
                    textToShow += text[i];
                    lettersCount++;
                }
                ExtendPanel(linesCount);

                // Set text with spaces
                bText.SetText(textToShow);

                // Start animation
                isNotificationRunning = true;
                animLerp_Up.StartAnimation();

                // Play notification sound
                if (notificationSound)
                {
                    BAudioManager.SpawnSoundObject(notificationSound);
                }
            }
        }

        private void OnNotificationEnded()
        {
            isNotificationRunning = false;

            if (notificationsQueue.Count > 0)
            {
                string newText = notificationsQueue.Dequeue();
                SetNotification(newText);
            }
        }

        private IEnumerator WaitThenPlayDownAnimationCoroutine()
        {
            yield return new WaitForSeconds(notificationStayDuration);

            if (IS_NOT_NULL(animLerp_Down))
            {
                animLerp_Down.StartAnimation();
            }
        }
        
        private void ExtendPanel(int linesCount)
        {
            if (IS_NOT_NULL(panelContainer)
                && linesCount > 0)
            {
                linesCount--;
                panelContainer.transform.localScale = panelOriginalSize + new Vector3(0.0f, panelExtensionSize * linesCount, 0.0f);
            }
        }


        private void TestText()
        {
            LogNotification(testText);
            //Notify(testText);
        }

        #endregion
    }
}
