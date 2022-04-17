using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeFractioned : MonoBehaviour
{
    public ArrayLayout boardLayout;
    int width = 10;
    int height = 10;
    Node[,] board;

    System.Random random;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void StartGame()
    {
        string seed = "0";
        random = new System.Random(seed.GetHashCode());

        InitializeBoard();
    }

    void InitializeBoard()
    {
        board = new Node[this.width, this.height];
        for(int y = 0; y < height; y++){
            for(int x = 0; x < width; x++) {
                board[x, y] = new Node(-1, new Point(x, y));
            }
        }

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
    public Point index;

    public Node (int value, Point index) {
        this.value = value;
        this.index = index;
    }
}