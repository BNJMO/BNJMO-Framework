using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNJMO;

public class FeedbackCallbackOnBMenuHighlighted : AbstractFeedbackCallback
{
    #region Public Events


    #endregion

    #region Public Methods

    
    #endregion

    #region Inspector Variables

    [SerializeField]
    protected BMenu bMenu;

    [SerializeField] 
    private bool playOnEnter = true;
    
    [SerializeField] 
    private bool stopOnEnter = false;
       
    [SerializeField] 
    private bool playOnExit = false;
          
    [SerializeField] 
    private bool stopOnExit = true;
    
    
    #endregion

    #region Private Variables
    
    


    #endregion

    #region Life Cycle

    protected override void OnValidate()
    {
        if (!CanValidate()) return;
        base.OnValidate();

        SetComponentInHierarchyIfNull(ref bMenu);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (bMenu)
        {
            bMenu.HighlightEnter += BMenu_OnHighlightEnter;
            bMenu.HighlightExit += BMenu_OnHighlightExit;
        }
    }


    protected override void OnDisable()
    {
        base.OnDisable();

        if (bMenu)
        {
            bMenu.HighlightEnter -= BMenu_OnHighlightEnter;
            bMenu.HighlightExit-= BMenu_OnHighlightExit;
        }
    }
    #endregion

    #region Events Callbacks
    
    private void BMenu_OnHighlightEnter(BMenu _)
    {
        if (playOnEnter)
        {
            LogConsoleBlue(name);
            PlayFeedbacks();
        }
        // if (stopOnEnter)
        // {
        //     StopFeedbacks();
        // }
    }

    private void BMenu_OnHighlightExit(BMenu _)
    {
        // if (playOnExit)
        // {
        //     PlayFeedbacks();
        // }
        // if (stopOnExit)
        // {
        //     StopFeedbacks();
        // }
    }

    #endregion

    #region Others


    #endregion
}
