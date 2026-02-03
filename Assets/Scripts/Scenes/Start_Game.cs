using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Start_Game : MonoBehaviour
{
    void Start()
    { 
    }
    void Update()
    {
    }
    public void NextLevel()
    {
        SceneManager.LoadScene(2);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
    }
}
