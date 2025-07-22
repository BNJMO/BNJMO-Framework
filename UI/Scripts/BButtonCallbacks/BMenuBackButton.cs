using UnityEngine;
using Sirenix.OdinInspector;

namespace BNJMO
{
    [RequireComponent(typeof(BButton))]
    public class BMenuBackButton : BBehaviour
    {
        #region Public Events


        #endregion
        
        #region Public Methods

        public void NavigateBack()
        {
            parentBFrame?.GoToPreviousMenu();
        }

        #endregion
        
        #region Inspector Variables

        [BoxGroup("BackButton", centerLabel: true)]
        [SerializeField, SceneObjectsOnly, ReadOnly]
        private BButton bButtonReference;

        [SerializeField, ReadOnly, BoxGroup("BackButton")]
        private BFrame parentBFrame;
        
        [SerializeField, BoxGroup("BackButton")]
        private bool navigateOnButtonPress = true;

        #endregion
        
        #region Variables

        public bool NavigateOnButtonPress => navigateOnButtonPress;
        
        #endregion

        #region Life Cycle

        protected override void OnValidate()
        {
            base.OnValidate();
            
            SetComponentIfNull(ref bButtonReference);
            parentBFrame = bButtonReference?.ParentBFrame;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (bButtonReference)
            {
                bButtonReference.Released += Button_OnReleased;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (bButtonReference)
            {
                bButtonReference.Released -= Button_OnReleased;
            }
        }

        #endregion

        #region Events Callbacks

        private void Button_OnReleased(BButton bButton, bool cursorInside)
        {
            if (!cursorInside || !navigateOnButtonPress) return;

            NavigateBack();
        }

        #endregion

        #region Others
        
        
        #endregion
        
    }
}
