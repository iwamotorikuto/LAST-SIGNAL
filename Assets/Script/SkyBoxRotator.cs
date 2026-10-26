using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxRotator : MonoBehaviour
{
    public float rotationSpeed = 1.0f; //回転速度

    void Update()
    {
        float angle = Mathf.Repeat(Time.time * rotationSpeed, 360.0f);
        RenderSettings.skybox.SetFloat("_Rotation", angle);
    }
}
