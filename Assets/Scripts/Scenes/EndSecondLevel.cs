using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSecondLevel : MonoBehaviour
{ 
    public int count_buttons;
    public bool IsWin = false;
    private void Awake()
    {
        count_buttons = 4;
    }
    private void Start()
    {
        ButtonBoss.isDestroy += LoweCountButtons;
    }
    private void LoweCountButtons(object sender, System.EventArgs e)
    {
        count_buttons -= 1;
    }
    private void Update()
    {
        if (count_buttons == 0)
        {
            IsWin = true;
        }
    }
}
