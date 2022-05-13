using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FractToValue
{
    public string Initialize(List<String> input)
    {
        List<int> data = new  List<int>();
        int multiple = 1;
        foreach (string fract in input)
        {
            String[] value = fract.Split('/');
            data.Add(Int32.parse(value[0]));
            multiple = lcm(multiple, Int32.parse(value[1]));
        }

        for (int i = 0; i < data[0].Count; i++)
        {
            data[i] = data[i] * multiple/data[1];
        }
        return string.Format("%d/%d", data.Sum(), multiple);
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