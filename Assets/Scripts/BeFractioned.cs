using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeFractioned : MonoBehaviour
{
    public ArrayLayout boardLayout;

    [Header("UI Elements")]
    public Sprite[] pieces;
    public Sprite[] powerUpPieces;
    public Sprite[] highlights;
    public Sprite roller;
    public Sprite cutter;
    public Sprite fire;
    public RectTransform gameBoard;
    public RectTransform overlay;

    [Header("Prefabs")]
    public GameObject nodePiece;
    public GameObject nodePieceOverlay;

    int width = 10;
    int height = 10;
    int powerValue = 0;
    bool updating = false;
    bool power = false;
    bool powerHighlighted = false;
    Node powerNode = null;
    Node[,] board;

    List<NodePiece> update;
    List<NodePiece> dead;
    List<NodePiece> highlighted;
    List<NodePiece> rollers;
    List<NodePiece> cutters;
    List<string> highlightedValue;

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
        this.highlightedValue = new List<string>();
        this.rollers = new List<NodePiece>();
        this.cutters = new List<NodePiece>();

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
            GameObject p = Instantiate(nodePiece, gameBoard);
            NodePiece piece = p.GetComponent<NodePiece>();
            RectTransform rect = p.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * -1));
            piece.Initialize(0, new Point(x, -1), cutter, "cutter");
            cutters.Add(piece);
            for (int y = 0; y < height; y++)
            {
                if (x == 0)
                {
                    p = Instantiate(nodePiece, gameBoard);
                    piece = p.GetComponent<NodePiece>();
                    rect = p.GetComponent<RectTransform>();
                    rect.anchoredPosition = new Vector2(32 + (64 * -1), -32 - (64 * y));
                    piece.Initialize(0, new Point(-1, y), roller, "roller");
                    rollers.Add(piece);
                }
                Node node = getNodeAtPoint(new Point(x, y));

                int val = node.value;
                if (val <= 0) continue;
                //highlight
                p = Instantiate(nodePieceOverlay, overlay);
                Overlay over = p.GetComponent<Overlay>();
                rect = p.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
                over.Initialize(new Point(x, y), highlights[0]);
                //object
                p = Instantiate(nodePiece, gameBoard);
                piece = p.GetComponent<NodePiece>();
                rect = p.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
                piece.Initialize(val, new Point(x, y), pieces[val - 1], "tile");
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
        if (this.highlighted.Contains(piece) || this.highlighted.Count == 8) return;
        this.highlighted.Add(piece);
        if (!piece.type.Equals("power")) this.highlightedValue.Add(string.Format("1/{0}", piece.value));
        else this.powerHighlighted = true;
        piece.Highlighted(true);
    }

    public void doneHighlighting()
    {
        if (this.highlighted.Count == 0) return;
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

    public void applyPowerUp(int value, Point point)
    {
        if (value == 1) // y
        {
            this.cutters[point.x].pos = new Vector2(32 + (64 * point.x), -32 - (64 * height));
            this.update.Add(this.cutters[point.x]);
        }
        if (value == 2) // x
        {
            this.rollers[point.y].pos = new Vector2(32 + (64 * width), -32 - (64 * point.y));
            this.update.Add(this.rollers[point.y]);
        }
        if (value == 3) // nuke
        {

        }
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
            if ((this.highlighted.Count > 1 || this.powerHighlighted))
            {
                Debug.Log("dame");
                int[] frac = FractToValue.ToValue(this.highlightedValue);
                if (frac[0] % frac[1] == 0 && frac[0] != 0)
                {
                    power = true;
                    powerNode = getNodeAtPoint(this.highlighted[0].GetPoint());
                    powerValue = frac[0] / frac[1];
                    foreach (NodePiece piece in this.highlighted)
                    {
                        Node node = getNodeAtPoint(piece.GetPoint());
                        if (piece != null)
                        {
                            piece.gameObject.SetActive(false);
                            if (!dead.Contains(piece)) dead.Add(piece);
                        }
                        node.SetPiece(null);
                    }
                    if (powerValue - 2 >= 0)
                    {
                        NodePiece revived = dead[0];
                        revived.gameObject.SetActive(true);
                        this.dead.RemoveAt(0);
                        revived.Initialize(powerValue - 1, powerNode.getPoint(), powerUpPieces[powerValue - 2], "power");
                        revived.SetHighlight(powerNode.GetOverlay());
                        revived.rect.anchoredPosition = getPositionFromPoint(new Point(powerNode.getPoint().x, powerNode.getPoint().y));

                        powerNode.SetPiece(revived);
                        ResetPiece(revived);
                        power = false;
                        powerNode = null;
                    }
                    ApplyGravityToBoard();
                }
                else
                {
                    foreach (NodePiece piece in this.highlighted)
                    {
                        if (piece.type.Equals("power"))
                        {
                            Debug.Log(this.update.Count);
                            applyPowerUp(piece.value, piece.index);
                        }
                    }
                }
            }
            this.highlighted.Clear();
            this.highlightedValue.Clear();
            this.powerHighlighted = false;
            if (finishedUpdating[i].type.Equals("tile") || finishedUpdating[i].type.Equals("power"))
            {
                List<Point> connected = isConnected(finishedUpdating[i].index, true);
                foreach (Point pnt in connected)
                {
                    Node node = getNodeAtPoint(pnt);
                    NodePiece nodePiece = node.getPiece();
                    if (nodePiece != null)
                    {
                        nodePiece.gameObject.SetActive(false);
                        dead.Add(nodePiece);
                    }
                    node.SetPiece(null);
                }
                ApplyGravityToBoard();
            }

            this.update.Remove(finishedUpdating[i]);
        }
        if (this.update.Count == 0)
        {
            this.updating = false;
        }
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
                        piece.Initialize(newVal, p, pieces[newVal - 1], "tile");
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
        pizzaTypes.Add(2, 0.3333);
        pizzaTypes.Add(3, 0.25);
        pizzaTypes.Add(4, 0.1666);
        pizzaTypes.Add(5, 0.6666);
        pizzaTypes.Add(6, 0.75);
        pizzaTypes.Add(7, 0.8333);

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

    public Point getPoint()
    {
        return index;
    }

    public Overlay GetOverlay()
    {
        return this.overlay;
    }
}