﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Camera following a target
/// </summary>

public class CameraFollow : MonoBehaviour
{
    public float move_speed = 2f;
    public GameObject follow_target;
    public Vector3 follow_offset;
    
    private Vector3 current_vel;

    private static CameraFollow _instance;

    void Awake()
    {
        _instance = this;
    }

    void LateUpdate()
    {
        if (follow_target != null)
        {
            Vector3 target_pos = follow_target.transform.position + follow_offset;
            transform.position = Vector3.SmoothDamp(transform.position, target_pos, ref current_vel, 1f / move_speed);
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("Screenmanager Resolution Width", 800);
        PlayerPrefs.SetInt("Screenmanager Resolution Height", 600);
        PlayerPrefs.SetInt("Screenmanager Is Fullscreen mode", 0);
    }

    public static CameraFollow Get()
    {
        return _instance;
    }
}