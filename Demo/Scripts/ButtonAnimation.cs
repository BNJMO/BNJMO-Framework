using UnityEngine;

using BNJMO;

[RequireComponent(typeof(AnimationLerpFloat))]
[RequireComponent(typeof(BButton))]
public class ButtonAnimation : BBehaviour
{
    private AnimationLerpFloat animationLerpFloat;
    private BButton bButton;

    private Vector3 originalScale;

    protected override void Awake()
    {
        base.Awake();

        originalScale = transform.localScale;
    }

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        bButton = GetComponent<BButton>();
        bButton.Highlighted += On_BButton_ButtonHighlighted;
        bButton.Unhighlighted += On_bButton_ButtonUnhighlighted;

        animationLerpFloat = GetComponent<AnimationLerpFloat>();
        animationLerpFloat.AnimationUpdated += On_AnimationLerpFloat_AnimationUpdated;
        animationLerpFloat.AnimationName = "AnimLerp_" + gameObject.name;
    }



    private void On_BButton_ButtonHighlighted(BButton maleficusButton)
    {
        animationLerpFloat.PlayInReverse = false;
        animationLerpFloat.StartAnimation();
    }

    private void On_bButton_ButtonUnhighlighted(BButton maleficusButton)
    {
        animationLerpFloat.PlayInReverse = true;
        animationLerpFloat.StartAnimation();
    }

    private void On_AnimationLerpFloat_AnimationUpdated(AnimationLerp<float> animationLerp, float value)
    {
        transform.localScale = originalScale * value;
    }
}

