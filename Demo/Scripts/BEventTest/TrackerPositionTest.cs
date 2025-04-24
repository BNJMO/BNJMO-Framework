using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNJMO;

public class TrackerPositionTest : BBehaviour
{
    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        BEvents.TEST_TrackerMoved += On_TEST_TrackerMoved;
    }

    private void On_TEST_TrackerMoved(BEHandle<Vector3> handle)
    {
        transform.position = handle.Arg1;
    }
}
