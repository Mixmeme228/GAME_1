using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHero: MonoBehaviour {
    public static DataHero Instance { get; private set; }
    public float HP_hero;
    public byte Count_key;
    public Vector3 pos;

    private void Update()
    {
        pos = transform.position;
    }
}
