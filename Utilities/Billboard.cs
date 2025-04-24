using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNJMO;

public class Billboard : BBehaviour
{
    protected override void Update()
    {
        base.Update();

        if (Camera.main)
        {
            transform.LookAt(Camera.main.transform);
        }
    }
}
