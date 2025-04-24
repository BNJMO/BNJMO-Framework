using System.Collections;
using System.Collections.Generic;
using BNJMO;
using UnityEngine;

public class BButtonAnimation : BBehaviour
{
    [SerializeField]
    private BButton bButton;
    
    [SerializeField] 
    private AnimationLerpFloat animLerp_HoverEnter;

    [SerializeField] 
    private AnimationLerpFloat animLerp_HoverExit;

    private float startScale;

    protected override void OnValidate()
    {
        if (!CanValidate()) return;
        base.OnValidate();

        if (bButton == null)
        {
            bButton = GetComponent<BButton>();
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (IS_NOT_NULL(bButton))
        {
            bButton.HoveredEnter += On_BButton_ButtonHoveredEnter;
            bButton.HoveredExit += On_BButton_ButtonHoveredExit;
            bButton.Enabled += On_BButton_ButtonEnabled;
            bButton.Disabled += On_BButton_ButtonDisabled;
        }

        if (IS_NOT_NULL(animLerp_HoverEnter))
        {
            animLerp_HoverEnter.AnimationUpdated += On_AnimLerp_HoverEnter_AnimationProgressed;
        }

        if (IS_NOT_NULL(animLerp_HoverExit))
        {
            animLerp_HoverExit.AnimationUpdated += On_AnimLerp_HoverExit_AnimationProgressed;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (bButton)
        {
            bButton.HoveredEnter -= On_BButton_ButtonHoveredEnter;
            bButton.HoveredExit -= On_BButton_ButtonHoveredExit;
            bButton.Enabled -= On_BButton_ButtonEnabled;
            bButton.Disabled -= On_BButton_ButtonDisabled;
        }

        if (animLerp_HoverEnter)
        {
            animLerp_HoverEnter.AnimationUpdated -= On_AnimLerp_HoverEnter_AnimationProgressed;
        }

        if (animLerp_HoverExit)
        {
            animLerp_HoverExit.AnimationUpdated -= On_AnimLerp_HoverExit_AnimationProgressed;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        startScale = transform.localScale.x;
    }

    private void On_AnimLerp_HoverExit_AnimationProgressed(AnimationLerp<float> arg1, float value)
    {
        transform.localScale = Vector3.one * value;
    }

    private void On_AnimLerp_HoverEnter_AnimationProgressed(AnimationLerp<float> arg1, float value)
    {
        transform.localScale = Vector3.one * value;
    }

    private void On_BButton_ButtonHoveredEnter(BButton obj)
    {
        animLerp_HoverExit.StopAnimation();

        animLerp_HoverEnter.StartValue = transform.localScale.x;
        animLerp_HoverEnter.StartAnimation();
    }

    private void On_BButton_ButtonHoveredExit(BButton obj)
    {
        animLerp_HoverEnter.StopAnimation();

        animLerp_HoverExit.StartValue = transform.localScale.x;
        animLerp_HoverExit.EndValue = startScale;
        animLerp_HoverExit.StartAnimation();
    }

    private void On_BButton_ButtonDisabled(BButton obj)
    {
        transform.localScale = Vector3.one * animLerp_HoverEnter.StartValue;
    }

    private void On_BButton_ButtonEnabled(BButton obj)
    {
        transform.localScale = Vector3.one * animLerp_HoverEnter.StartValue;
    }

}
