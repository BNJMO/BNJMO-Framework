using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNJMO;
using MoreMountains.Feedbacks;

public abstract class AbstractFeedbackCallback : BBehaviour
{
    #region Public Events


    #endregion

    #region Public Methods

    public void PlayFeedbacks()
    {
        if (IS_VALID(feedbacks))
        {
            feedbacks.PlayFeedbacks();
        }
    }

    public void StopFeedbacks()
    {
        if (IS_VALID(feedbacks))
        {
            feedbacks.StopFeedbacks();
        }
    }

    public void ResetFeedbacks()
    {
        if (IS_VALID(feedbacks))
        {
            feedbacks.ResetFeedbacks();
        }
    }
    
    #endregion

    #region Inspector Variables

    [SerializeField]
    protected MMFeedbacks feedbacks;

    #endregion

    #region Private Variables


    #endregion

    #region Life Cycle

    protected override void OnValidate()
    {
        if (!CanValidate()) return;
        base.OnValidate();

        SetComponentIfNull(ref feedbacks);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        feedbacks.StoppedPlaying += On_Feedbacks_StoppedPlaying;
        feedbacks.FinishedPlaying += On_Feedback_FinishedPlaying;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (feedbacks)
        {
            feedbacks.StoppedPlaying -= On_Feedbacks_StoppedPlaying;
            feedbacks.FinishedPlaying -= On_Feedback_FinishedPlaying;
        }
    }
    #endregion

    #region Events Callbacks

    protected virtual void On_Feedbacks_StoppedPlaying()
    {
    }

    protected virtual void On_Feedback_FinishedPlaying()
    {
    }

    #endregion

    #region Others


    #endregion
}
