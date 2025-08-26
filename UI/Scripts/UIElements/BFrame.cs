using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BNJMO
{
    public class BFrame : BUIElement
    {
        #region Public Events

        public event Action<BFrame, BMenu> HighlightedMenuChanged;

        #endregion

        #region Public Methods
        
        // TODO
        public bool HasFocus { get; private set; }

        // TODO
        public void OnGainedFocus()
        {

            //BEventsCollection.UI_FocusedFrameUpdated.Invoke(new BEHandle<BFrame>(this));
        }

        // TODO
        public void OnLostFocus()
        {

        }

        public bool UpdateHighlightedBMenu(BMenu newBBMenu, bool recordHistory = true)
        {
            if (parentBMenu
                && parentBMenu.IsHighlighted == false)
            {
                return false;
            }

            if (IS_NOT_NULL(newBBMenu)
                && IS_VALUE_CONTAINED(childrenBMenus, newBBMenu))
            {
                BMenu oldHighlightedBMenu = highlightedBMenuReference;
                
                // Push old to stack if it's different
                if (recordHistory && oldHighlightedBMenu != null 
                && (menuHistory.Count == 0 || menuHistory.Peek() != oldHighlightedBMenu))
                {
                    menuHistory.Push(oldHighlightedBMenu);
                }
                
                highlightedBMenuReference = newBBMenu;

                if (Application.isPlaying
                && BManager.Inst)
                {
                    bool logBMenuHighlightBEvents = BConfig.Inst && BConfig.Inst.LogBMenuHighlightBEvents;
                    BEvents.UI_HighlightedBMenuUpdated?.Invoke(new BEventHandle<BMenu, BMenu>(highlightedBMenuReference, oldHighlightedBMenu), 
                        BConfig.Inst.LogUIBEvents && BConfig.Inst.LogBMenuHighlightBEvents);
                }

                highlightedBMenuReference.OnHighlighted();


                // Deactivate all other BMenu
                foreach (BMenu bMenu in childrenBMenus)
                {
                    // Prevent deactivating BMenus from nested BFrames
                    if (bMenu.ParentBFrame == this)
                    {
                        if (bMenu != highlightedBMenuReference)
                        {
                            bMenu.OnHighlightExit();
                        }
                    }
                }
                
                HighlightedMenuChanged?.Invoke(this, highlightedBMenuReference);
                
                return true;
            }

            return false;
        }

        public void SetStartHighlightedBMenu(BMenu bMenu)
        {
            if (IS_NOT_NULL(bMenu))
            {
                startHighlightedBMenu = bMenu;
            }
        }
        
        public void GoToPreviousMenu()
        {
            if (menuHistory.Count == 0)
            {
                LogConsoleWarning($"[BFrame: {UIElementName}] No previous menu to return to.");
                return;
            }

            BMenu previousMenu = menuHistory.Pop();
            if (previousMenu != null)
            {
                UpdateHighlightedBMenu(previousMenu, recordHistory: false);
            }
            else
            {
                LogConsoleWarning($"[BFrame: {UIElementName}] Previous menu was null.");
            }
        }


        public void ClearMenuHistory()
        {
            menuHistory.Clear();
        }
        #endregion

        #region Inspector Variables

        [BoxGroup("BFrame", centerLabel: true)]
        [BoxGroup("BFrame"), SerializeField] 
        bool startWithFocus = true;
        
        [BoxGroup("BFrame"), SerializeField, ChildGameObjectsOnly,
        InfoBox("$infoNoStartBMenuHighlight", InfoMessageType.Error, "showNoStartBMenuHighlight")]
        private BMenu startHighlightedBMenu;
        
        [HorizontalGroup("BFrame/Group"), SerializeField] 
        private string newBMenuName = "BMenu";
        
        [HorizontalGroup("BFrame/Group") ,Button("Add BMenu")] 
        private void Button_AddBMenu() => AddBMenu();

        [Title("References")]
        [BoxGroup("BFrame"), SerializeField, ReadOnly] 
        private BMenu highlightedBMenuReference;
        
        [Space(7)]
        [BoxGroup("BFrame"), SerializeField, TableList(DrawScrollView = true)] 
        private List<BFrameChildBMenu> childrenBMenusList = new ();

        [BoxGroup("BFrame"), Button("Revalidate")]
        private void OnRevalidateButton() => Revalidate();

        #endregion

        #region Variables

        private BMenu[] childrenBMenus = new BMenu[0];
        private bool showNoStartBMenuHighlight = false;
        private const string infoNoStartBMenuHighlight = "You need to select one of the children BMenu as Start Hihghlight!";

        // Menu history stack
        private readonly Stack<BMenu> menuHistory = new();

        #endregion

        #region Life Cycle



        #endregion

        #region Events Callbacks


        #endregion

        #region Others
        
        protected override void OnValidate()
        {
            if (!CanValidate()) return;
            
            objectNamePrefix = "F_";

            base.OnValidate();

            // Get all children BMenu
            childrenBMenus = GetComponentsInChildren<BMenu>();
            childrenBMenusList = new List<BFrameChildBMenu>();
            foreach (BMenu bMenu in childrenBMenus)
            {
                if (bMenu.ParentBFrame == this)
                {
                    childrenBMenusList.Add(new BFrameChildBMenu(bMenu, this));
                }
            }

            // Check Start Highlight BMenu
            highlightedBMenuReference = null;
            if (startHighlightedBMenu)
            {
                showNoStartBMenuHighlight = false;
                highlightedBMenuReference = startHighlightedBMenu;
            }
            else // Try to get the first BMenu child
            {
                if ((childrenBMenus.Length > 0)
                    && (childrenBMenus[0]))
                {
                    startHighlightedBMenu = childrenBMenus[0];
                    showNoStartBMenuHighlight = false;
                    highlightedBMenuReference = startHighlightedBMenu;
                }
                else
                {
                    LogConsoleError(infoNoStartBMenuHighlight);
                    showNoStartBMenuHighlight = true;
                }
            }
        }

        protected override void InitializeComponents()
        {
            base.InitializeComponents();

            // Get all children BMenu
            childrenBMenus = GetComponentsInChildren<BMenu>();

            // Check Start Highlight BMenu
            highlightedBMenuReference = startHighlightedBMenu;
            if (highlightedBMenuReference == null)
            {
                LogConsoleError("No BMenu selected as highlight!");
            }
        }

        protected override void LateStart()
        {
            base.LateStart();

            if (startWithFocus)
            {
                bool logUIBEvents = BConfig.Inst && BConfig.Inst.LogUIBEvents;
                BEvents.UI_FocusedFrameUpdated?.Invoke(new BEventHandle<BFrame>(this), logUIBEvents);
            }

            if (highlightedBMenuReference)
            {
                UpdateHighlightedBMenu(highlightedBMenuReference);
            }
            else
            {
                LogConsoleWarning("BMenu highlight not correctly initialized!");
            }
        }
  
        protected override void OnUIShown()
        {
            base.OnUIShown();

            if (highlightedBMenuReference)
            {
                UpdateHighlightedBMenu(highlightedBMenuReference);
            }
        }

        private void AddBMenu()
        {
#if UNITY_EDITOR

            BMenu objectPrefab = Resources.Load<BMenu>(BConsts.PATH_BMenu);
            if (objectPrefab)
            {
                BMenu spawnedObject = Instantiate(objectPrefab);

                if (spawnedObject)
                {
                    spawnedObject.UIElementName = newBMenuName;
                    spawnedObject.Revalidate();

                    // Set this object as parent
                    spawnedObject.transform.parent = transform;
                    spawnedObject.transform.localPosition = Vector3.zero;
                    spawnedObject.transform.localScale = Vector3.one;

                    // Set spawned object as selected
                    Selection.SetActiveObjectWithContext(spawnedObject, Selection.activeContext);

             
                    Revalidate();
                }
                else
                {
                    Debug.LogError("Couldn't spawn object!");
                }
            }
            else
            {
                Debug.LogError("The 'BMenu' prefab was not found in the Resources folder!");
            }
#endif
        }
        
        #endregion
    }
    
    [Serializable]
    public class BFrameChildBMenu
    {
        public BFrameChildBMenu(BMenu bMenu, BFrame bFrame)
        {
            childBMenu = bMenu;
            bFrameReference = bFrame;
        }

        [SerializeField] [ReadOnly] BMenu childBMenu;
        [TableColumnWidth(60, resizable:false)] [Button("Prev"), VerticalGroup("Highlight")] public void Button_PreviewHighlight() 
        {
            if (bFrameReference)
            {
                bFrameReference.UpdateHighlightedBMenu(childBMenu);
            }
        }
        [TableColumnWidth(60, resizable:false)] [Button("Set"), VerticalGroup("Start")] public void Button_SetHighlight() 
        {
            if (bFrameReference)
            {
                bFrameReference.SetStartHighlightedBMenu(childBMenu);
            }
        }

        private BFrame bFrameReference;
    }
}