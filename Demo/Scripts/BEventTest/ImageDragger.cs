using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNJMO;
using UnityEngine.EventSystems;
using System.Reflection.Emit;

public class ImageDragger : BBehaviour, IDragHandler,  IEndDragHandler
{
    private Vector3 originalPosition;

    protected override void Awake()
    {
        base.Awake();

        originalPosition = transform.position;
    }

    protected override void InitializeEventsCallbacks()
    {
        base.InitializeEventsCallbacks();

        BEvents.TEST_ImagePosition += On_TEST_ImagePosition;
    }

    private void On_TEST_ImagePosition(BEventHandle<Vector3> bEHandle)
    {
        transform.position = bEHandle.Arg1;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 newDragPosition = eventData.pointerCurrentRaycast.worldPosition;
        newDragPosition = new Vector3(newDragPosition.x, newDragPosition.y, originalPosition.z);

        BEvents.TEST_ImagePosition.Invoke(new BEventHandle<Vector3>(newDragPosition), BEventBroadcastType.TO_ALL, true);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        LogConsole("Drag Ended");
    }
}
