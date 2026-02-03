using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.ExceptionServices;

namespace Game.Utils_1
{
    public static class Common2
    {
        /*
        public static float maxSpeed = 73f;
        public static float minSpeed = 55f;
        */
        public static string GetRandomirStringOf12(int length)
        {
            string str = "";
            for (int i = 0; i < length; i++)
            {
                str += UnityEngine.Random.Range(0, 1).ToString();
            }
            return str;
        }
        public static Vector3 GetRandomirPoint_1(float min_x, float max_x, float min_y, float max_y)
        {
            float x = UnityEngine.Random.Range(min_x, max_x);
            float y = UnityEngine.Random.Range(min_y, max_y);
            return new Vector3(x, y, 0f); 
        }

        public static Vector3 GetRandomirPoint(float min_, float max_)
        {
            float x = UnityEngine.Random.Range(min_, max_);
            return new Vector3(x, 0f, 0f); // y и z компоненты равны 0
        }
        public static float RandomFloatBetween(float min, float max)
        {
            System.Random random = new System.Random();
            double range = max - min;
            double sample = random.NextDouble();
            double scaled = (sample * range) + min;
            return (float)scaled;
        }
    }
}
