using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Start_Work : MonoBehaviour
{
    public static SendDataHero testScript;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        testScript = gameObject.AddComponent<SendDataHero>();
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "End")
        {
            testScript = GetComponent<SendDataHero>();
            Destroy(testScript);
        }
    }
}
