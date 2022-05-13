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
    Node powerNode = null;
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
        VerifyBoard();
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
            if (this.highlighted.totalValue)
            {
                power = true;
                powerNode = getNodeAtPoint(this.highlighted[0].GetPoint());
                powerValue = this.highlighted.total;
            }
            foreach (NodePiece piece in this.highlighted)
            {
                Node node = getNodeAtPoint(piece.GetPoint());
                if (piece != null)
                {
                    piece.gameObject.SetActive(false);
                    dead.Add(piece);
                }
                node.SetPiece(null);
            }
            if (powerNode != null)
            {
                NodePiece revived = dead[0];
                revived.gameObject.SetActive(true);
                piece = revived;
                this.dead.RemoveAt(0);
                piece.Initialize(newVal, powerNode.getPoint, powerUpPieces[powerValue]);
                piece.SetHighlight(powerNode.GetOverlay());
                piece.rect.anchoredPosition = getPositionFromPoint(new Point(x, -1));

                powerNode.SetPiece(piece);
                ResetPiece(piece);
                power = false;
                powerNode = null;
            }
            ApplyGravityToBoard();
            this.highlighted.Clear();

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

    void VerifyBoard()
    {
        List<int> remove;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Point p = new Point(x, y);
                int val = getValueAtPoint(p);
                if (val <= 0) continue;

                remove = new List<int>();
                while (isConnected(p, true).Count > 0)
                {
                    val = getValueAtPoint(p);
                    if (!remove.Contains(val))
                        remove.Add(val);
                    setValueAtPoint(p, newValue(ref remove));
                }
            }
        }
    }

    List<Point> isConnected(Point p, bool main)
    {
        List<Point> connected = new List<Point>();
        int val = getValueAtPoint(p);
        Point[] directions =
        {
            Point.up,
            Point.right,
            Point.down,
            Point.left
        };

        foreach (Point dir in directions)
        {
            List<Point> line = new List<Point>();

            int same = 0;
            for (int i = 1; i < 3; i++)
            {
                Point check = Point.add(p, Point.mult(dir, i));
                if (getValueAtPoint(check) == val)
                {
                    line.Add(check);
                    same++;
                }
            }

            if (same > 1)
                AddPoints(ref connected, line);
        }

        for (int i = 0; i < 2; i++)
        {
            List<Point> line = new List<Point>();

            int same = 0;
            Point[] check = { Point.add(p, directions[i]), Point.add(p, directions[i + 2]) };
            foreach (Point next in check)
            {
                if (getValueAtPoint(next) == val)
                {
                    line.Add(next);
                    same++;
                }
            }

            if (same > 1)
                AddPoints(ref connected, line);
        }

        for (int i = 0; i < 4; i++)
        {
            List<Point> square = new List<Point>();

            int same = 0;
            int next = i + 1;
            if (next >= 4)
                next -= 4;

            Point[] check = { Point.add(p, directions[i]), Point.add(p, directions[next]), Point.add(p, Point.add(directions[i], directions[next])) };
            foreach (Point pnt in check)
            {
                if (getValueAtPoint(pnt) == val)
                {
                    square.Add(pnt);
                    same++;
                }
            }

            if (same > 2)
                AddPoints(ref connected, square);
        }

        if (main)
        {
            for (int i = 0; i < connected.Count; i++)
                AddPoints(ref connected, isConnected(connected[i], false));
        }
        return connected;
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

    public NodePiece getPoint()
    {
        return index;
    }

    public Overlay GetOverlay()
    {
        return this.overlay;
    }
}