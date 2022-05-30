using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class BeFractioned : MonoBehaviour
{
    public ArrayLayout boardLayout;

    [Header("UI Elements")]
    public Sprite plate;
    public Sprite blank;
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

    public void StartGame()
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
                    this.dead.Add(nodePiece);
                }
                node.SetPiece(null);
            }
        }
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
                piece.Initialize(val, new Point(x, y), plate, "tile");
                int degree = 0;
                for (int i = 0; i < 12; i++)
                {
                    p = Instantiate(childNode, gameBoard);
                    ChildNode child = p.GetComponent<ChildNode>();
                    rect = p.GetComponent<RectTransform>();
                    rect.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
                    initializeChild(i, child, val, new Point(x, y), degree);
                    piece.childPieces.Add(child);
                    if (Array.Exists(new int[] { 2, 3, 8, 9}, e => e == i)) degree += 30;
                    else if (Array.Exists(new int[] { 0 , 5, 6, 11 }, e => e == i)) degree += 45;
                    else degree += 15;
                }
                piece.SetHighlight(over);
                node.SetPiece(piece);
                node.SetOverlay(over);
            }
        }
    }

    void initializeChild(int index, ChildNode child, int value, Point p, int degree)
    {
        switch (value)
        {
            case 1:
                if (index == 0) child.Initialize(value, p, pieces[value - 1], degree);
                else child.Initialize(value, p, blank, degree);
                break;
            case 2:
                if (index == 0) child.Initialize(value, p, pieces[value - 1], degree);
                else child.Initialize(value, p, blank, degree);
                break;
            case 3:
                if (index == 0) child.Initialize(value, p, pieces[value - 1], degree);
                else child.Initialize(value, p, blank, degree);
                break;
            case 4:
                if (index == 0) child.Initialize(value, p, pieces[value - 1], degree);
                else child.Initialize(value, p, blank, degree);
                break;
            case 5:
                if (Array.Exists(new int[] { 0, 4 }, e => e == index)) child.Initialize(value, p, pieces[value - 1], degree);
                else child.Initialize(value, p, blank, degree);
                break;
            case 6:
                if (Array.Exists(new int[] { 0, 3, 6 }, e => e == index)) child.Initialize(value, p, pieces[value - 1], degree);
                else child.Initialize(value, p, blank, degree);
                break;
            case 7:
                if (Array.Exists(new int[] { 0, 2, 4, 6, 8 }, e => e == index)) child.Initialize(value, p, pieces[value - 1], degree);
                else child.Initialize(value, p, blank, degree);
                break;
        }
    }

    int fillPiece()
    {
        int val = random.Next(pieces.Length) + 1;
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
                    this.dead.Add(node.getPiece());
                    foreach (ChildNode child in node.getPiece().childPieces) child.gameObject.SetActive(false);
                    node.SetPiece(null);
                }
            }
        }
        Node powernode = getNodeAtPoint(point); ;
        powernode.getPiece().gameObject.SetActive(false);
        this.dead.Add(powernode.getPiece());
        foreach (ChildNode child in powernode.getPiece().childPieces) child.gameObject.SetActive(false);
        powernode.SetPiece(null);
        timerBar.GetComponent<Timer>().IncreaseScore(300);
        timerBar.GetComponent<Timer>().IncreaseTime(3f);
        if (value == 3) ApplyGravityToBoard();
    }

    void Update()
    {
        List<NodePiece> finishedUpdating = new List<NodePiece>();

        if (this.update.Count != 0) this.updating = true;
        else
        {
            ApplyGravityToBoard();
            if (this.dead.Count != 0) SpawnNewTiles();
            this.updating = false;
        }
        for (int i = 0; i < this.update.Count; i++)
        {
            if (!this.update[i].UpdatePiece())
            {
                if (this.update[i].wasHitByPower())
                {
                    this.update[i].gameObject.SetActive(false);
                    this.dead.Add(this.update[i]);
                    foreach (ChildNode child in this.update[i].childPieces) child.gameObject.SetActive(false);
                    getNodeAtPoint(this.update[i].GetPoint()).SetPiece(null);
                }
                if (this.update[i].type.Equals("cutter") || this.update[i].type.Equals("roller"))
                {
                    this.update[i].SetVisible(false);
                    this.update[i].ResetPosition();
                    this.update[i].TeleportTo(this.update[i].pos);
                }
                finishedUpdating.Add(this.update[i]);
            }
            else if (this.update[i].type.Equals("roller") && this.update[i].rect.anchoredPosition.x >= 0 && this.update[i].rect.anchoredPosition.x < gameBoard.sizeDelta.x)
            {
                Node node = getNodeAtPoint(getPointAtPosition(this.update[i].rect));
                if (node.value > 0)
                {
                    NodePiece piece = node.getPiece();
                    if (piece.type.Equals("tile"))
                    {
                        piece.hitBy("roller");
                        if (!this.update.Contains(piece)) this.update.Add(piece);
                    }
                    else if (piece.type.Equals("power"))
                    {
                        if (piece.value == 1) applyPowerUp(piece.value, piece.index, 0);
                        else if (piece.value == 3)
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
            }
            else if (this.update[i].type.Equals("cutter") && this.update[i].rect.anchoredPosition.y <= 0 && this.update[i].rect.anchoredPosition.y > -gameBoard.sizeDelta.y)
            {
                Node node = getNodeAtPoint(getPointAtPosition(this.update[i].rect));
                if (node.value > 0)
                {
                    NodePiece piece = node.getPiece();
                    if (piece.type.Equals("tile"))
                    {
                        piece.hitBy("cutter");
                        if (!this.update.Contains(piece)) this.update.Add(piece);
                    }
                    else if (piece.type.Equals("power"))
                    {
                        if (piece.value == 2) applyPowerUp(piece.value, piece.index, 0);
                        else if (piece.value == 3)
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
                        if (pizza.type.Equals("tile"))
                        {
                            Node node = getNodeAtPoint(pizza.GetPoint());
                            if (pizza != null)
                            {
                                pizza.gameObject.SetActive(false);
                                this.dead.Add(pizza);
                                foreach (ChildNode child in pizza.childPieces) child.gameObject.SetActive(false);
                                if (!dead.Contains(pizza)) dead.Add(pizza);
                                FindObjectOfType<AudioManager>().PlaySound("Combine Pizzas");
                                timerBar.GetComponent<Timer>().IncreaseScore(10);
                                timerBar.GetComponent<Timer>().IncreaseTime(1.5f);
                            }
                            node.SetPiece(null);
                        }
                    }
                    if (powerValue >= 2)
                    {
                        NodePiece revived = null;
                        foreach (NodePiece piece in this.highlighted)
                        {
                            if (piece.type.Equals("tile"))
                            {
                                revived = piece;
                                break;
                            }
                        }
                        revived.gameObject.SetActive(true);
                        foreach (ChildNode child in revived.childPieces) child.gameObject.SetActive(true);
                        List<ChildNode> temp = revived.childPieces;
                        if (powerValue == 2)
                        {
                            int value = random.Next(0, 2);

                            Debug.Log(revived.childPieces.Count);
                            foreach (ChildNode child in revived.childPieces) child.Initialize(value + 1, powerNode.getPoint(), blank, -1);
                            revived.Initialize(value + 1, powerNode.getPoint(), powerUpPieces[value], "power");
                        }
                        else
                        {
                            Debug.Log(revived.childPieces.Count);
                            foreach (ChildNode child in revived.childPieces) child.Initialize(3, powerNode.getPoint(), blank, -1);
                            revived.Initialize(3, powerNode.getPoint(), powerUpPieces[2], "power");
                        }
                        revived.childPieces = temp;
                        revived.SetHighlight(powerNode.GetOverlay());
                        revived.rect.anchoredPosition = getPositionFromPoint(new Point(powerNode.getPoint().x, powerNode.getPoint().y));
                        this.dead.Remove(revived);
                        powerNode.SetPiece(revived);
                        this.update.Add(revived);
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

            this.update.Remove(finishedUpdating[i]);
        }
    }

    void ApplyGravityToBoard()
    {
        for (int y = (height - 1); y > 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                Point p = new Point(x, y);
                Node node = getNodeAtPoint(p);
                int val = getValueAtPoint(p);
                if (val == 0)
                {
                    for (int ay = (y - 1); ay >= 0; ay--)
                    {
                        Point above = new Point(x, ay);
                        int aboveVal = getValueAtPoint(above);
                        if (aboveVal != 0)
                        {
                            Node aboveNode = getNodeAtPoint(above);
                            NodePiece abovePiece = getNodeAtPoint(above).getPiece();
                            NodePiece piece = node.getPiece();
                            node.SetPiece(abovePiece);
                            abovePiece.SetHighlight(node.GetOverlay());
                            aboveNode.SetPiece(null);
                            if (!this.update.Contains(abovePiece)) this.update.Add(abovePiece);
                            node = aboveNode;
                        }
                    }
                }
            }
        }
    }

    void SpawnNewTiles()
    {
        for (int y = (height - 1); y >= 0; y--)
        {
            bool spawned = false;
            for (int x = 0; x < width; x++)
            {
                Point p = new Point(x, y);
                int val = getValueAtPoint(p);
                if (val == 0)
                {
                    Node node = getNodeAtPoint(p);
                    int newVal = fillPiece();
                    NodePiece revived = this.dead[0];
                    Vector2 spawn = new Vector2(32 + (x * 64), 32 + (1 * 64));

                    revived.gameObject.SetActive(true);
                    List<ChildNode> temp = revived.childPieces;
                    foreach (ChildNode child in revived.childPieces) child.gameObject.SetActive(true);
                    foreach (ChildNode child in revived.childPieces) child.TeleportTo(spawn);
                    for (int i = 0; i < 12; i++) initializeChild(i, revived.childPieces[i], newVal, p, -1);
                    revived.TeleportTo(spawn);
                    revived.Initialize(newVal, p, plate, "tile");
                    revived.childPieces = temp;
                    revived.SetHighlight(node.GetOverlay());
                    node.SetPiece(revived);
                    spawned = true;
                    this.dead.RemoveAt(0);
                    this.update.Add(revived);
                }
            }
            if (spawned) break;
        }
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