using UnityEngine;
using Sirenix.OdinInspector;
using System;
using UnityEditor;
using UnityEngine.Serialization;

namespace BNJMO
{
    public abstract class BUIElement : BBehaviour
    {
        #region Public Events
        
        public event Action<BUIElement> BUIElementShown;
        public event Action<BUIElement> BUIElementHidden;

        #endregion

        #region Public Methods
        
        public bool ShowUI(bool forceSetEnabled = false, bool forceWhenParenBMenuAndBFrameAreNotActive = false)
        {
            if (forceSetEnabled)
            {
                isUIDisabled = false;
            }
            
            if (IsUIDisabledFromHierarchy())
                return false;

            OnUIShown();
            
            InvokeEventIfBound(BUIElementShown, this);
            isShown = true;

            // Propagate hierarchically
            for (int i = 0; i < transform.childCount; i++)
            {
                if ((transform)
                    && (transform.GetChild(i))
                    && (transform.GetChild(i).GetComponent<BUIElement>()))
                {
                    transform.GetChild(i).GetComponent<BUIElement>().ShowUI(forceSetEnabled, forceWhenParenBMenuAndBFrameAreNotActive);
                }
            }
            return true;
        }

        public void HideUI(bool setDisabled = false)
        {
            OnUIHidden();

            if (setDisabled)
            {
                isUIDisabled = true;
            }
            
            InvokeEventIfBound(BUIElementHidden, this);
            isShown = false;

            // Propagate hierarchically
            for (int i = 0; i < transform.childCount; i++)
            {
                if ((transform)
                    && (transform.GetChild(i))
                    && (transform.GetChild(i).GetComponent<BUIElement>()))
                {
                    transform.GetChild(i).GetComponent<BUIElement>().HideUI(setDisabled);
                }
            }
        }

        public void DisableUI()
        {
            HideUI(true);            
        }
        
        public bool IsUIDisabledFromHierarchy()
        {
            bool isUIDisabledFromHierarchy = IsUIDisabled;
            
            if (isUIDisabledFromHierarchy)
                return true;
            
            if (parentBUIElement)
            {
                isUIDisabledFromHierarchy = parentBUIElement.IsUIDisabledFromHierarchy();
            }
            
            return isUIDisabledFromHierarchy;
        }
        
        public void SetMatchParentSize(bool value)
        {
            matchParentSize = value;
            if (matchParentSize)
            {
                MatchParentSize();
            }
        }
        
        public void MatchParentSize()
        {
            RectTransform rectTransform = transform as RectTransform;
            RectTransform parentRectTransform = transform.parent as RectTransform;
            if (parentRectTransform)
            {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentRectTransform.rect.width);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   parentRectTransform.rect.height);
            }
        }

        public void MatchAllChildrenSizes()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform
                    && transform.GetChild(i)
                    && transform.GetChild(i).GetComponent<BUIElement>())
                {
                    transform.GetChild(i).GetComponent<BUIElement>().MatchParentSize();
                    transform.GetChild(i).GetComponent<BUIElement>().MatchAllChildrenSizes();
                }
            }
        }
        
        public void RevalidateWithChildren()
        {
            Revalidate();
            
            // Propagate hierarchically
            for (int i = 0; i < transform.childCount; i++)
            {
                if ((transform)
                    && (transform.GetChild(i))
                    && (transform.GetChild(i).GetComponent<BUIElement>()))
                {
                    transform.GetChild(i).GetComponent<BUIElement>().Revalidate();
                }
            }
        }
        
        #endregion

        #region Inspector Variables
        
        [BoxGroup("BUIElement", centerLabel: true)] 
        [SerializeField] protected string uIElementName = "";

        [Title("Parent BIUElements (Automatically populated)")]
        [FoldoutGroup("BUIElement/More")] [SerializeField] 
        protected BFrame parentBFrame;
        
        [FoldoutGroup("BUIElement/More")] [SerializeField] 
        protected BMenu parentBMenu;
        
        [FoldoutGroup("BUIElement/More")] [SerializeField] 
        protected BUIElement parentBUIElement;

        [Title("Configuration")]
        [FoldoutGroup("BUIElement/More")] [FormerlySerializedAs("overrideGameOjbectName")] [SerializeField] 
        protected bool overrideGameObjectName = true;
        
        [FoldoutGroup("BUIElement/More")] [SerializeField] 
        protected bool propagateUINameToChildren = false;
        
        [FoldoutGroup("BUIElement/More")] [SerializeField] 
        protected bool revalidateAllDirectChildren = true;
        
        [FoldoutGroup("BUIElement/More")] [ReadOnly] [SerializeField]
        protected string objectNamePrefix = "C_";
        
        [Title("Visibility")]
        [FoldoutGroup("BUIElement/More")] [FormerlySerializedAs("isDisabled")] [SerializeField] 
        private bool isUIDisabled = false;
                
        [FoldoutGroup("BUIElement/More")] [SerializeField] 
        protected bool matchParentSize = false;
        
        [FoldoutGroup("BUIElement/More")] [Button("Show UI")] [HideIf("isUIDisabled")] 
        private void EnableUIElement_Button() => ShowUI();
        
        [FoldoutGroup("BUIElement/More")] [Button("Hide UI")] 
        private void DisableUIElement_Button() => HideUI();
               
        [FoldoutGroup("BUIElement/More")]  [Button("Match Parent Size")]
        private void MatchParentSize_Button() => MatchParentSize();
                      
        [FoldoutGroup("BUIElement/More")]  [Button("Match All Childreen Sizes")]
        private void MatchAllChildrenSizes_Button() => MatchAllChildrenSizes();
        
        [Title("Refresh")]
        [FoldoutGroup("BUIElement/More")] [Button("Revalidate")] 
        private void Revalidate_Button() => Revalidate(); 
        
        [FoldoutGroup("BUIElement/More")] [Button("Revalidate With Children")] 
        private void RevalidateWithChildren_Button() => RevalidateWithChildren(); 
 

        #endregion

        #region Variables
        public string UIElementName { get { return uIElementName; } set { uIElementName = value; } }
        public BFrame ParentBFrame { get { return parentBFrame; } }
        public BMenu ParentBMenu { get { return parentBMenu; } }
        public string UINameExtension { get; set; }
        public bool IsShown { get { return isShown; } }
        public bool IsUIDisabled => isUIDisabled;

        [SerializeField]
        [HideInInspector]
        private bool isShown = true;


        #endregion

        #region Life Cycle


        protected override void OnValidate()
        {
            if (!CanValidate()) return;
            base.OnValidate();

            // Find BFrame and BMenu in parents
            parentBFrame = GetComponentInParents<BFrame>();
            parentBMenu = GetComponentInParents<BMenu>();
            parentBUIElement = GetComponentInDirectParent<BUIElement>();

            // Revalidate children UI Elements (only direct children. Revalidation will be propagated if they have revalidateAllDirectChildren set to true)
            if (revalidateAllDirectChildren)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if ((transform)
                        && (transform.GetChild(i))
                        && (transform.GetChild(i).GetComponent<BUIElement>()))
                    {
                        if (propagateUINameToChildren)
                        {
                            transform.GetChild(i).GetComponent<BUIElement>().UIElementName = UIElementName;
                        }

                        transform.GetChild(i).GetComponent<BUIElement>().Revalidate();
                    }
                }
            }

            // Update GameObject name
            if (overrideGameObjectName)
            {
                // Prevent renaming object if is the parent of a prefab
                if (IsGameObjectInMainStage() == false
                    && transform.parent == null)
                    return;

                if (uIElementName == ""
                    && gameObject.name != "")
                {
                    uIElementName = gameObject.name;
                }
                gameObject.name = objectNamePrefix + uIElementName + UINameExtension;
            }
            
            // Match parent size
            if (matchParentSize)
            {
                MatchParentSize();
            }
        }


        #endregion

        #region Events Callbacks
                
        protected virtual void OnUIShown() { }

        protected virtual void OnUIHidden() { }

        #endregion

        #region Private Methods


        #endregion





    }
}