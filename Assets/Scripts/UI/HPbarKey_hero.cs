using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class HPbarKey_hero : MonoBehaviour
{
    public static HPbarKey_hero Instance { get; private set; }

    public bool isFindKey = false;
    public byte count_Key = 0;
    public float HP = 100f;
    public DataHero dat;
    public bool IsFindKey_()
    {
        return isFindKey;
    }
    private void Start()
    {
        dat = GetComponent<DataHero>();
        dat.HP_hero = HP / 100f;
        dat.Count_key = count_Key;
        Player.Instance.TakeHP += LowerHP;
        Player.Instance.TakeCount += CountKeyHero;
        Player.Instance.TakeMed += IncreaseHP;
    }
    private void LowerHP(object sender, System.EventArgs e)
    {
        HP -= 10f;
        dat.HP_hero = HP / 100f;
    }
    private void IncreaseHP(object sender, System.EventArgs e)
    {
        HP += 10f;
        dat.HP_hero = HP / 100f;
    }
    private void CountKeyHero(object sender, System.EventArgs e)
    {
        count_Key += 1;
        dat.Count_key = count_Key;
    }
}
