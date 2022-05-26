using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    public static Highlight Instance;
    BeFractioned game;

    Point newIndex;

    public void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.game = GetComponent<BeFractioned>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HighlightPiece(NodePiece piece)
    {
        if (this.game.isUpdating()) return;
        this.game.addHighlighted(piece);
        
    }

    public void DropPiece()
    {
        this.game.doneHighlighting();
    }
}
