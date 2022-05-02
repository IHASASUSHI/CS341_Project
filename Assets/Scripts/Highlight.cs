using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    public static Highlight Instance;
    BeFractioned game;

    NodePiece highlighted;
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
    }

    // Update is called once per frame
    void Update()
    {
        if(this.highlighted != null)
        {
            Debug.Log("highlighting");
            this.highlighted.Highlighted(true);
        }
    }

    public void MovePiece(NodePiece piece)
    {
        if(this.highlighted != null) return;
        this.highlighted = piece;
        this.mouseStart = Input.mousePosition;
    }

    public void DropPiece()
    {
        if(this.highlighted == null) return;
        Debug.Log("dropped");
        this.highlighted.Highlighted(false);
        this.highlighted = null;
    }
}
