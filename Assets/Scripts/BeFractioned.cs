using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeFractioned : MonoBehaviour
{
    public ArrayLayout boardLayout;

    [Header("UI Elements")]
    public Sprite[] pieces;
    public Sprite[] highlights;
    public RectTransform gameBoard;
    public RectTransform overlay;

    [Header("Prefabs")]
    public GameObject nodePiece;
    public GameObject nodePieceOverlay;

    int width = 10;
    int height = 10;

    bool updating = false;
    Node[,] board;

    List<NodePiece> update;
    List<NodePiece> dead;
    List<NodePiece> highlighted;

    System.Random random;

    // Start is called before the first frame update
    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        string seed = "0";
        random = new System.Random(seed.GetHashCode());
        this.update = new List<NodePiece>();
        this.dead = new List<NodePiece>();
        this.highlighted = new List<NodePiece>();

        InitializeBoard();
        //VerifyBoard();
        InstantiateBoard();
    }

    void InitializeBoard()
    {
        board = new Node[this.width, this.height];
        for (int y = 0; y < this.height; y++)
        {
            for (int x = 0; x < this.width; x++)
            {
                board[x, y] = new Node((boardLayout.rows[y].row[x]) ? -1 : fillPiece(), new Point(x, y));
            }
        }
    }

    public void ResetPiece(NodePiece piece)
    {
        piece.ResetPosition();
        this.update.Add(piece);
    }

    void InstantiateBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Node node = getNodeAtPoint(new Point(x, y));

                int val = node.value;
                if (val <= 0) continue;
                //highlight
                GameObject p = Instantiate(nodePieceOverlay, overlay);
                Overlay over = p.GetComponent<Overlay>();
                RectTransform rect = p.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
                over.Initialize(new Point(x, y), highlights[0]);
                //object
                p = Instantiate(nodePiece, gameBoard);
                NodePiece piece = p.GetComponent<NodePiece>();
                rect = p.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
                piece.Initialize(val, new Point(x, y), pieces[val - 1]);
                piece.SetHighlight(over);
                node.SetPiece(piece);
                node.SetOverlay(over);
            }
        }
    }

    void AddPoints(ref List<Point> points, List<Point> add)
    {
        foreach (Point p in add)
        {
            bool doAdd = true;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].Equals(p))
                {
                    doAdd = false;
                    break;
                }
            }

            if (doAdd) points.Add(p);
        }
    }

    int fillPiece()
    {
        int val = 1;
        val = (random.Next(0, 100) / (100 / (pieces.Length - 1))) + 1;
        return val;
    }

    int getValueAtPoint(Point p)
    {
        if (p.x < 0 || p.x >= width || p.y < 0 || p.y >= height) return -1;
        return board[p.x, p.y].value;
    }

    void setValueAtPoint(Point p, int v)
    {
        board[p.x, p.y].value = v;
    }

    Node getNodeAtPoint(Point p)
    {
        return board[p.x, p.y];
    }

    public void addHighlighted(NodePiece piece)
    {
        if (this.highlighted.Contains(piece)) return;
        this.highlighted.Add(piece);
        piece.Highlighted(true);
    }
    public void doneHighlighting()
    {
        if(this.highlighted.Count == 0) return;
        for (int i = 0; i < this.highlighted.Count; i++)
        {
            this.highlighted[i].Highlighted(false);
        }
        this.update.AddRange(this.highlighted);
    }

    public bool isUpdating()
    {
        return this.updating;
    }

    void Update()
    {
        List<NodePiece> finishedUpdating = new List<NodePiece>();

        if (this.update.Count != 0) this.updating = true;
        for (int i = 0; i < this.update.Count; i++)
        {
            if (!this.update[i].UpdatePiece()) finishedUpdating.Add(this.update[i]);
        }
        for (int i = 0; i < finishedUpdating.Count; i++)
        {
            if (addsToWhole(this.highlighted))
            {
                foreach (NodePiece pizza in highlighted)
                {
                    if (pizza != null)
                    {
                        pizza.gameObject.SetActive(false);
                        dead.Add(pizza);
                    }
                    Node node = getNodeAtPoint(pizza.GetPoint());
                    node.SetPiece(null);
                }
            }
            ApplyGravityToBoard();
            this.highlighted.Clear();
            this.update.Remove(finishedUpdating[i]);
        }

        if (this.update.Count == 0) this.updating = false;
    }

    void ApplyGravityToBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = (height - 1); y >= 0; y--)
            {
                Point p = new Point(x, y);
                Node node = getNodeAtPoint(p);
                int val = getValueAtPoint(p);
                if (val != 0) continue;
                for (int ny = (y - 1); ny >= -1; ny--)
                {
                    Point next = new Point(x, ny);
                    int nextVal = getValueAtPoint(next);
                    if (nextVal == 0)
                        continue;
                    if (nextVal != -1)
                    {
                        Node got = getNodeAtPoint(next);
                        NodePiece piece = got.getPiece();

                        node.SetPiece(piece);
                        piece.SetHighlight(getNodeAtPoint(new Point(x, y)).GetOverlay());
                        this.update.Add(piece);

                        got.SetPiece(null);
                    }
                    else
                    {
                        int newVal = fillPiece();
                        NodePiece piece;


                        if (this.dead.Count > 0)
                        {
                            NodePiece revived = dead[0];
                            revived.gameObject.SetActive(true);
                            piece = revived;
                            this.dead.RemoveAt(0);
                        }
                        else
                        {
                            GameObject obj = Instantiate(nodePiece, gameBoard);
                            NodePiece n = obj.GetComponent<NodePiece>();
                            RectTransform rect = obj.GetComponent<RectTransform>();
                            piece = n;
                        }
                        piece.Initialize(newVal, p, pieces[newVal - 1]);
                        piece.SetHighlight(getNodeAtPoint(p).GetOverlay());
                        piece.rect.anchoredPosition = getPositionFromPoint(new Point(x, -1));

                        Node hole = getNodeAtPoint(p);
                        hole.SetPiece(piece);
                        ResetPiece(piece);
                    }
                    break;
                }
            }
        }
    }

    bool addsToWhole(List<NodePiece> nodePieces)
    {
        Dictionary<int, double> pizzaTypes = new Dictionary<int, double>();
        pizzaTypes.Add(1, 0.5);
        pizzaTypes.Add(2, 0.33);
        pizzaTypes.Add(3, 0.25);
        pizzaTypes.Add(4, 0.66);
        pizzaTypes.Add(5, 0.75);

        double sum = 0;
        List<Point> whole = new List<Point>();
        foreach (NodePiece nodePiece in nodePieces)
        {
            sum += pizzaTypes[nodePiece.value];
        }
        if (sum % 1 <= 0.05 || sum % 1 >= 0.95)
        {
            Debug.Log(sum%1);
            Debug.Log("Valid");
            return true;
        }
        Debug.Log("Invalid");
        return false;
    }

    int newValue(ref List<int> remove)
    {
        List<int> available = new List<int>();
        for (int i = 0; i < pieces.Length; i++)
            available.Add(i + 1);
        foreach (int i in remove)
            available.Remove(i);

        if (available.Count <= 0) return 0;
        return available[random.Next(0, available.Count)];
    }

    public Vector2 getPositionFromPoint(Point p)
    {
        return new Vector2(32 + (64 * p.x), -32 - (64 * p.y));
    }
}

[System.Serializable]
public class Node
{
    public int value;
    public Point index;
    NodePiece piece;
    Overlay overlay;

    public Node(int v, Point i)
    {
        this.value = v;
        this.index = i;
        this.piece = null;
        this.overlay = null;
    }

    public void SetPiece(NodePiece p)
    {
        piece = p;
        value = (piece == null) ? 0 : piece.value;
        if (piece == null) return;
        piece.SetIndex(index);
    }

    public void SetOverlay(Overlay overlay)
    {
        this.overlay = overlay;
        if (piece == null) return;
        overlay.SetIndex(index);
    }

    public NodePiece getPiece()
    {
        return piece;
    }

    public Overlay GetOverlay()
    {
        return this.overlay;
    }
}