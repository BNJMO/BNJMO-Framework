using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNJMO;

public class ServerToggleButton : BBehaviour    // TODO: Delete
{
    private BButton bButton;

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        bButton = GetComponent<BButton>();
        if (IS_NOT_NULL(bButton))
        {
            bButton.Released += On_BButton_ButtonReleased;
        }
    }

    protected override void Update()
    {
        base.Update();

    }

    private void On_BButton_ButtonReleased(BButton bButton, bool cursorInside)
    {

    }
}
