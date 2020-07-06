using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_AdjustPlane : MonoBehaviour
{
    private Camera myCam;
    float screeny, screenx;

    public float screenRatio;
    private void Awake()
    {
        myCam = GetComponent<Camera>();
    }

    private void FixedUpdate()
    {
        screeny = Screen.height;
        screenx = Screen.width;

        screenRatio = screenx / screeny;

        float orthoFactor = screenRatio * 0.6f;

        myCam.orthographicSize = 3.2f - orthoFactor;

    }
}
