using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Sprite that always face the camera
/// </summary>

public class Billboard : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(Camera.main.transform.position, -Vector3.up);
    }
}

