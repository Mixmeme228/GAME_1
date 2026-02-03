using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Utils
{
    public static class Common1
    {
        public static Vector2 GetRandomDir()
        {
            return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }
    }
    //функция для получения рандомного направления движения по осям
}
