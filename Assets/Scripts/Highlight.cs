using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    public static Highlight Instance;
    BeFractioned game;

    List<NodePiece> highlighted;
    Point newIndex;
    Vector2 mouseStart;

    public void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.game = GetComponent<BeFractioned>();
        this.highlighted = new List<NodePiece>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MovePiece(NodePiece piece)
    {
        if (this.highlighted.Contains(piece) || this.game.isUpdating()) return;
        this.highlighted.Add(piece);
        this.mouseStart = Input.mousePosition;
        this.game.addHighlighted(piece);
        piece.Highlighted(true);
    }

    public void DropPiece()
    {
        if(this.highlighted.Count == 0) return;
        for (int i = 0; i < this.highlighted.Count; i++)
        {
            this.highlighted[i].Highlighted(false);
        }
        this.highlighted.Clear();
    }
}
