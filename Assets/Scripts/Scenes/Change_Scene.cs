using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Change_Scene : MonoBehaviour
{
    public GameObject g_1;
    public float Health_new;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision != null) & (collision.gameObject.tag == "Player_1")){
            Health_new = GameObject.FindGameObjectWithTag("Player_1").GetComponent<Player>().health_hero;
            HPbarKey_hero h = collision.gameObject.GetComponent<HPbarKey_hero>();
            
            if (h != null)
            {
                if (h.count_Key >= 3)
                {
                    DontDestroyOnLoad(gameObject); //сохран€ем сам скрипт
                    NextLevel();
                }
            }
        }
    }
    public void NextLevel()
    {
        var index = SceneManager.GetActiveScene().buildIndex;
        var ind_1 = SceneManager.GetSceneByName("Win").buildIndex;
        var ind_2 = SceneManager.GetSceneByName("End").buildIndex;
        if ((index != ind_1 - 1)) { 
            SceneManager.LoadScene(index + 1);
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive); //, LoadSceneMode.Additive);

            // ѕодождите, пока последн€€ операци€ полностью загрузитс€, чтобы вернуть что-либо
            asyncLoad.completed += operation =>
            {
                Scene newScene = SceneManager.GetSceneByBuildIndex(index + 1);
                GameObject newG1 = Instantiate(g_1, new Vector3(136f, 54f, 0f), Quaternion.identity); // —оздаЄм копию из префаба
                //тут надо будет во втором параметре помен€ть координаты (посмотреть где начинаетс€ второй уровень и указать те координаты)
                SceneManager.MoveGameObjectToScene(newG1, newScene);
                g_1 = newG1;
                g_1.GetComponent<Player>().health_hero = Health_new;
                g_1.GetComponent<HPbarKey_hero>().HP = Health_new;
            };
        }
        else
        {
            SceneManager.LoadScene(index + 1);
            Destroy(this);
        }
    }
}
