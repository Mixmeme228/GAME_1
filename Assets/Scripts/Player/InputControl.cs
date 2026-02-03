using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControl : MonoBehaviour
{
    public static InputControl Instance { get; private set; }
    private bool isGetSpace;
    public bool isAlreadyGetSpace;
    public bool IsGetSpace_()
    {
        return isGetSpace;
    }
    public bool IsAlreadyGetSpace_()
    {
        return isAlreadyGetSpace;
    }
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        isGetSpace = false;
        isAlreadyGetSpace = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isGetSpace = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isGetSpace = false;
            isAlreadyGetSpace = false;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            isAlreadyGetSpace = true;
        }
    }
}
