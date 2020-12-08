using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static MonoBehaviour instance;
    public float re = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad (transform.gameObject);
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
