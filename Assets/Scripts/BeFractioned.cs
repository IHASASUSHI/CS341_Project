using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeFractioned : MonoBehaviour
{
    int width = 10;
    int height = 10;
    Node[,] board;

    System.Random Random;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void StartGame()
    {
        string seed = "0";
        random = new System.Random(seed.GetHashCode());
    }

    void InitializeBoard()
    {
        board = new Node[this.width, this.height];

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class Node
{
    public int value; //represents the object at node
    public point index;

    public Node (int value, Point index) {
        this.value = v;
        this.index = i;
    }
}