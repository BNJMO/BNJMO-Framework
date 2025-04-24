using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNJMO;

public class FPSCounter : BBehaviour
{
    [SerializeField] private float updateInterval = 1.0f; // Update FPS display every 1 second
    private float timeSinceLastUpdate = 0.0f;
    private float deltaTime = 0.0f;
    private float fps = 0.0f;
    
    [SerializeField] private BText myText;

    protected override void OnValidate()
    {
        if (!CanValidate()) return;
        base.OnValidate();

        SetComponentIfNull(ref myText);
    }
    
    protected override void Update()
    {
        base.Update();

        if (!myText)
            return;
        
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        timeSinceLastUpdate += Time.unscaledDeltaTime;

        if (timeSinceLastUpdate >= updateInterval)
        {
            fps = 1.0f / deltaTime;
            timeSinceLastUpdate = 0.0f;
        }
        myText.SetText($"FPS: {fps:0.}");
    }
}