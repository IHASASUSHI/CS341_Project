using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FractToValue
{
    public static int[] ToValue(List<string> input)
    {
        List<int> numerator  = new List<int>();
        List<int> denominator  = new List<int>();
        int multiple = 1;
        foreach (string fract in input)
        {
            int[] value = Array.ConvertAll(fract.Split('/'), s => Int32.Parse(s));
            numerator.Add(value[0]);
            denominator.Add(value[1]);
            multiple = lcm(multiple, value[1]);
        }

        for (int i = 0; i < numerator.Count; i++)
        {
            numerator[i] = numerator[i] * (multiple / denominator[i]);
        }
        return new int[] {numerator.Sum(), multiple};
    }
    static int gcf(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    static int lcm(int a, int b)
    {
        return (a / gcf(a, b)) * b;
    }
}