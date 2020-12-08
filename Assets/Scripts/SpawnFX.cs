using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Just destroy an FX after X seconds from being spawned
/// </summary>

public class SpawnFX : MonoBehaviour
{

    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
