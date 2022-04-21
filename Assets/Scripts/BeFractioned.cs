using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeFractioned : MonoBehaviour
{
    public ArrayLayout boardLayout;
    [Header("UI Elements")]
    public Sprite[] pieces;
    public RectTransform gameBoard;

    [Header("Prefabs")]
    public GameObject nodePiece;

    int width = 10;
    int height = 10;
    Node[,] board;

    List<NodePiece> update;

    System.Random random;

    // Start is called before the first frame update
    void Start() {
        StartGame();
    }

    void StartGame() {
        string seed = "0";
        this.random = new System.Random(seed.GetHashCode());
        this.update = new List<NodePiece>();

        InitializeBoard();
        InstantiateBoard();
    }

    void InitializeBoard() {
        board = new Node[this.width, this.height];
        for(int y = 0; y < this.height; y++){
            for(int x = 0; x < this.width; x++) {
                board[x, y] = new Node((boardLayout.rows[y].row[x]) ? - 1 : fillPiece(), new Point(x, y));
            }
        }

    }

    public void ResetPiece(NodePiece piece) {
        piece.ResetPosition();
        piece.flipped = null;
        this.update.Add(piece);
    }

    public void FlipPieces(Point one, Point two) {
        if(getValueAtPoint(one) < 0) return;
        Node nodeOne = getNodeAtPoint(one);
        NodePiece pieceOne = nodeOne.getPiece();
        if(getValueAtPoint(two) > 0) {
            Node nodeTwo = getNodeAtPoint(two);
            NodePiece pieceTwo = nodeTwo.getPiece();
            nodeOne.SetPiece(pieceTwo);
            nodeTwo.SetPiece(pieceOne);

            pieceOne.flipped = pieceTwo;
            pieceTwo.flipped = pieceOne;

            this.update.Add(pieceOne);
            this.update.Add(pieceTwo);
        }
        else ResetPiece(pieceOne);
    }

    void verifyBoard() {
        for(int y = 0; y < height; y++){
            for(int x = 0; x < width; x++) {
                Point p = new Point(x, y);
                int val = getValueAtPoint(p);
                if (val <= 0) continue;


            }
        }
    }

    void InstantiateBoard() {
        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                Node node = getNodeAtPoint(new Point(x, y));
                int val = board[x, y].value;
                if(val <= 0) continue;
                GameObject p = Instantiate(nodePiece, gameBoard);
                NodePiece piece = p.GetComponent<NodePiece>();
                RectTransform rect = p.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
                piece.Initialize(val, new Point(x, y), pieces[val - 1]);
                node.SetPiece(piece);
            }
        }
    }

    void addPoints(ref List<Point> points, List<Point> add) {

    }

    int fillPiece() {
        int val = 1;
        val = (random.Next(0, 100) / (100 / pieces.Length)) + 1;
        return val;
    }

    int getValueAtPoint(Point p) {
        return board[p.x, p.y].value;
    }

    Node getNodeAtPoint(Point p) {
        return this.board[p.x, p.y];
    }

    // Update is called once per frame
    void Update() {
        List<NodePiece> finishedUpdating = new List<NodePiece>();
        for(int i = 0; i < this.update.Count; i++) {
            if(!update[i].UpdatePeice()) finishedUpdating.Add(update[i]);
        }
        for(int i = 0; i < finishedUpdating.Count; i++) {
            this.update.Remove(finishedUpdating[i]);
        }
    }

    public Vector2 getPositionFromPoint(Point p) {
        return new Vector2(32 + (64 * p.x), -32 - (64 * p.y));
    }
}

[System.Serializable]
public class Node
{
    public int value; //represents the object at node
    public Point index;
    public NodePiece piece;

    public Node (int value, Point index) {
        this.value = value;
        this.index = index;
    }

    public void SetPiece(NodePiece p) {
        this.piece = p;
        this.value = (piece == null) ? 0 : piece.value;
        if(this.piece == null) return;
        this.piece.SetInddex(index);
    }

    public NodePiece getPiece() {
        return piece;
    }
}