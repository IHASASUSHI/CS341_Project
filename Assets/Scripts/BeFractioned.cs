using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    public GameObject childNode;
    public GameObject nodePieceOverlay;

    public GameObject timerBar;
    public TMP_Text fractionPreview;

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
        this.fractionPreview.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200);
        this.fractionPreview.rectTransform.anchoredPosition = new Vector2(this.fractionPreview.rectTransform.anchoredPosition.x, this.fractionPreview.rectTransform.anchoredPosition.y - 10);
        this.fractionPreview.bounds.SetMinMax(new Vector2(0, 0), new Vector2(200, 200));

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

    public void WipeBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Node node = getNodeAtPoint(new Point(x, y));

                NodePiece nodePiece = node.getPiece();
                if (nodePiece != null)
                {
                    nodePiece.gameObject.SetActive(false);
                    dead.Add(nodePiece);
                }
                node.SetPiece(null);
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
            piece.SetVisible(false);
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
                    piece.SetVisible(false);
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
                for (int i = 0; i < 12; i++)
                {
                    p = Instantiate(childNode, gameBoard);
                    ChildNode child = p.GetComponent<ChildNode>();
                    rect = p.GetComponent<RectTransform>();
                    rect.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
                    child.Initialize(val, new Point(x, y), pieces[val - 1]);
                    child.setImage(i);
                    piece.childPieces.Add(child);
                }
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
        int val = random.Next(0, pieces.Length) + 1;
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
        if (!piece.type.Equals("power"))
        {
            string value = "";
            switch (piece.value)
            {
                case 1:
                    value = "1/2";
                    break;
                case 2:
                    value = "1/3";
                    break;
                case 3:
                    value = "1/4";
                    break;
                case 4:
                    value = "1/6";
                    break;
                case 5:
                    value = "2/3";
                    break;
                case 6:
                    value = "3/4";
                    break;
                case 7:
                    value = "5/6";
                    break;
            }
            this.highlightedValue.Add(value);
        }
        else this.powerHighlighted = true;
        piece.Highlighted(true);

        if (this.highlightedValue.Count > 0)
        {
            int[] total = FractToValue.ToValue(this.highlightedValue);
            string previewString = this.highlightedValue[0];
            for (int i = 1; i < this.highlightedValue.Count; i++)
            {
                previewString += " + " + this.highlightedValue[i];
            }
            string fraction = "";
            string whole = "";
            if (total[0] >= total[1]) whole = (total[0] / total[1]).ToString();
            if (total[0] % total[1] != 0)
            {
                int multiple = FractToValue.gcf(total[0], total[1]);
                fraction = (total[0] % total[1] / multiple) + "/" + total[1] / multiple;
            }
            previewString += string.Format(" = {0} {1}", whole, fraction);
            fractionPreview.text = previewString;
        }
    }

    public void doneHighlighting()
    {
        if (this.highlighted.Count == 0) return;
        for (int i = 0; i < this.highlighted.Count; i++)
        {
            this.highlighted[i].Highlighted(false);
            fractionPreview.text = "";
        }
        this.update.AddRange(this.highlighted);
    }

    public bool isUpdating()
    {
        return this.updating;
    }

    public Point getPointAtPosition(RectTransform pos)
    {
        int x = width * (int)pos.anchoredPosition.x / (int)gameBoard.sizeDelta.x;
        int y = height * (int)pos.anchoredPosition.y / (-(int)gameBoard.sizeDelta.y);
        return new Point(x, y);
    }

    public bool checkPower(NodePiece piece)
    {
        if (piece.type.Equals("power"))
        {
            if (piece.value != 3) applyPowerUp(piece.value, piece.index, 0);
            else if (this.highlighted.Count > 1)
            {
                int value = 0;
                foreach (NodePiece np in this.highlighted)
                {
                    if (!np.type.Equals("power"))
                    {
                        value = np.value;
                        break;
                    }
                }
                applyPowerUp(piece.value, piece.index, value);
            }
            else
            {
                return false;
            }
            Node node = getNodeAtPoint(piece.GetPoint());
            piece.gameObject.SetActive(false);
            foreach (ChildNode child in piece.childPieces) child.gameObject.SetActive(false);
            if (!dead.Contains(piece)) dead.Add(piece);
            node.SetPiece(null);
            return true;
        }
        return false;
    }

    public void applyPowerUp(int value, Point point, int pieceValue)
    {
        if (value == 1) // x
        {
            this.cutters[point.x].SetVisible(true);
            this.cutters[point.x].pos = new Vector2(32 + (64 * point.x), -32 - (64 * height));
            this.update.Add(this.cutters[point.x]);
        }
        if (value == 2) // y
        {
            this.rollers[point.y].SetVisible(true);
            this.rollers[point.y].pos = new Vector2(32 + (64 * width), -32 - (64 * point.y));
            this.update.Add(this.rollers[point.y]);
        }
        if (value == 3) // nuke
        {
            foreach (Node node in board)
            {
                NodePiece piece = node.getPiece();
                if (piece.value == pieceValue && piece.type.Equals("tile"))
                {
                    node.getPiece().gameObject.SetActive(false);
                    foreach (ChildNode child in node.getPiece().childPieces) child.gameObject.SetActive(false);
                    dead.Add(node.getPiece());
                    node.SetPiece(null);
                }
            }
        }
    }

    void Update()
    {
        List<NodePiece> finishedUpdating = new List<NodePiece>();

        if (this.update.Count != 0) this.updating = true;
        for (int i = 0; i < this.update.Count; i++)
        {
            if (!this.update[i].UpdatePiece())
            {
                if (this.update[i].wasHitByPower())
                {
                    this.update[i].gameObject.SetActive(false);
                    foreach (ChildNode child in this.update[i].childPieces) child.gameObject.SetActive(false);
                    if (!dead.Contains(this.update[i])) dead.Add(this.update[i]);
                }
                ApplyGravityToBoard();
                if (this.update[i].type.Equals("cutter") || this.update[i].type.Equals("roller"))
                {
                    this.update[i].SetVisible(false);
                    this.update[i].ResetPosition();
                    this.update[i].rect.anchoredPosition = this.update[i].pos;
                }
                finishedUpdating.Add(this.update[i]);
            }
            else if (this.update[i].type.Equals("roller") && this.update[i].rect.anchoredPosition.x >= 0 && this.update[i].rect.anchoredPosition.x <= gameBoard.sizeDelta.x)
            {
                Node node = getNodeAtPoint(getPointAtPosition(this.update[i].rect));
                NodePiece piece = node.getPiece();
                if (piece != null)
                {
                    if (piece.type.Equals("tile"))
                    {
                        piece.hitBy("roller");
                        this.update.Add(piece);
                    }
                    else if (piece.type.Equals("power"))
                    {
                        if (piece.value == 1) applyPowerUp(piece.value, piece.index, 0);
                        else
                        {
                            int value = 0;
                            foreach (NodePiece np in this.highlighted)
                            {
                                if (!np.type.Equals("power"))
                                {
                                    value = np.value;
                                    break;
                                }
                            }
                            applyPowerUp(piece.value, piece.index, fillPiece());
                        }
                    }
                }
                node.SetPiece(null);
            }
            else if (this.update[i].type.Equals("cutter") && this.update[i].rect.anchoredPosition.y <= 0 && this.update[i].rect.anchoredPosition.y >= -gameBoard.sizeDelta.y)
            {
                Node node = getNodeAtPoint(getPointAtPosition(this.update[i].rect));
                NodePiece piece = node.getPiece();
                if (piece != null)
                {
                    if (piece.type.Equals("tile"))
                    {
                        piece.gameObject.SetActive(false);
                        foreach (ChildNode child in piece.childPieces) child.gameObject.SetActive(false);
                        if (!dead.Contains(piece)) dead.Add(piece);
                    }
                    else if (piece.type.Equals("power"))
                    {
                        if (piece.value == 2) applyPowerUp(piece.value, piece.index, 0);
                        else
                        {
                            int value = 0;
                            foreach (NodePiece np in this.highlighted)
                            {
                                if (!np.type.Equals("power"))
                                {
                                    value = np.value;
                                    break;
                                }
                            }
                            applyPowerUp(piece.value, piece.index, fillPiece());
                        }
                    }
                }
                node.SetPiece(null);
            }
        }
        for (int i = 0; i < finishedUpdating.Count; i++)
        {
            if (this.highlighted.Count > 1 || this.powerHighlighted)
            {
                int[] frac = FractToValue.ToValue(this.highlightedValue);
                if (frac[0] % frac[1] == 0 && frac[0] != 0)
                {
                    power = true;
                    powerNode = getNodeAtPoint(this.highlighted[0].GetPoint());
                    powerValue = frac[0] / frac[1];
                    foreach (NodePiece pizza in this.highlighted)
                    {
                        if (!checkPower(pizza))
                        {
                            Node node = getNodeAtPoint(pizza.GetPoint());
                            if (pizza != null)
                            {
                                pizza.gameObject.SetActive(false);
                                foreach (ChildNode child in pizza.childPieces) child.gameObject.SetActive(false);
                                if (!dead.Contains(pizza)) dead.Add(pizza);
                                //FindObjectOfType<AudioManager>().PlaySound("Combine Pizzas");
                                //timerBar.GetComponent<Timer>().IncreaseScore(50);
                                //timerBar.GetComponent<Timer>().IncreaseTime(1.5f);
                            }
                            node.SetPiece(null);
                        }
                    }
                    if (powerValue >= 2)
                    {
                        NodePiece revived = dead[0];
                        revived.gameObject.SetActive(true);
                        foreach (ChildNode child in revived.childPieces) child.gameObject.SetActive(true);
                        this.dead.RemoveAt(0);
                        if (powerValue == 2)
                        {
                            int value = random.Next(0, 2);
                            foreach (ChildNode child in revived.childPieces) child.Initialize(value + 1, powerNode.getPoint(), powerUpPieces[value]);
                            revived.Initialize(value + 1, powerNode.getPoint(), powerUpPieces[value], "power");
                        }
                        else
                        {
                            foreach (ChildNode child in revived.childPieces) child.Initialize(3, powerNode.getPoint(), powerUpPieces[2]);
                            revived.Initialize(3, powerNode.getPoint(), powerUpPieces[2], "power");
                        }
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
                        checkPower(piece);
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
                        foreach (ChildNode child in nodePiece.childPieces) child.gameObject.SetActive(false);
                        dead.Add(nodePiece);
                    }
                    node.SetPiece(null);
                }
                ApplyGravityToBoard();
            }

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
                            foreach (ChildNode child in revived.childPieces) child.gameObject.SetActive(true);
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
                        foreach (ChildNode child in piece.childPieces) child.Initialize(newVal, p, pieces[newVal - 1]);
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

    public Point getPoint()
    {
        return index;
    }

    public Overlay GetOverlay()
    {
        return this.overlay;
    }
}