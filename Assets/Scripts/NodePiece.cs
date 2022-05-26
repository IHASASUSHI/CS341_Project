using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NodePiece : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
{
    public int value;
    public Point index;
    public string type;
    public List<ChildNode> childPieces = new List<ChildNode>();

    [HideInInspector]
    public Vector2 pos;
    [HideInInspector]
    public RectTransform rect;

    bool updating;
    string hitByPower;
    Image img;
    Overlay highlight;
    float velocity = 0;
    float xPos;
    float yPos;

    System.Random random;

    public void Initialize(int value, Point index, Sprite piece, string type)
    {
        this.img = GetComponent<Image>();
        this.rect = GetComponent<RectTransform>();

        this.value = value;
        this.type = type;
        SetIndex(index);
        this.img.sprite = piece;
        // This is for later use when we want to rotate
        // this.rect.transform.rotation = Quaternion.Euler(new Vector3(0,0,90));
        this.highlight = null;
        this.hitByPower = "";
        string seed = "0";
        random = new System.Random(seed.GetHashCode());
        this.xPos = (float)random.NextDouble();
        this.yPos = (float)random.NextDouble();
    }

    public void SetIndex(Point p)
    {
        this.index = p;
        ResetPosition();
        UpdateName();
    }

    public void setImage(int index)
    {
        if (index == 0)
        {
            //this.img.transform.Rotate(xAngle, )
        }
    }

    public void hitBy(string power)
    {
        this.hitByPower = power;
    }

    public bool wasHitByPower()
    {
        return !this.hitByPower.Equals("");
    }

    public void SetHighlight(Overlay highlight)
    {
        this.highlight = highlight;
    }

    public void ResetPosition()
    {
        this.pos = new Vector2(32 + (64 * index.x), -32 - (64 * index.y));
        foreach (ChildNode piece in childPieces) piece.pos = new Vector2(32 + (64 * index.x), -32 - (64 * index.y));
    }

    public void MovePositionTo(Vector2 move, float speed)
    {
        Vector2 diff = this.rect.anchoredPosition - move;
        float xPos = 0;
        float yPos = 0;
        if (Math.Abs(diff.x) > 1) xPos = -1 * diff.x / Math.Abs(diff.x) * speed;
        else xPos = -1 * diff.x;
        if (Math.Abs(diff.y) > 1) yPos = -1 * diff.y / Math.Abs(diff.y) * speed;
        else yPos = -1 * diff.y;
        this.rect.anchoredPosition += new Vector2(xPos, yPos);
        foreach (ChildNode piece in childPieces) piece.MovePositionTo(new Vector2(xPos, yPos));
    }

    public void cutterSlice()
    {
    }

    public void rollerFling(float speed)
    {
        this.yPos += this.velocity * speed + (1f / 2f * (-9.8f * (float)Math.Pow(speed, 2)));
        foreach (ChildNode piece in childPieces) piece.MovePositionTo(new Vector2(this.pos.x + this.xPos, this.yPos));
    }

    public bool UpdatePiece()
    {
        //Debug.Log(Vector2.Distance(this.rect.anchoredPosition, this.pos));
        if (Vector2.Distance(this.rect.anchoredPosition, this.pos) > 1)
        {
            if (this.type.Equals("cutter") || this.type.Equals("roller")) MovePositionTo(this.pos, 0.01f);
            if (!this.hitByPower.Equals("") && this.rect.anchoredPosition.y > -620)
            {
                if (this.hitByPower.Equals("roller")) rollerFling(100f);
                else if (this.hitByPower.Equals("cutter")) cutterSlice();
            }
            else MovePositionTo(this.pos, 1f);
            this.updating = true;
            return true;
        }
        else
        {
            rect.anchoredPosition = pos;
            updating = false;
            return false;
        }
    }

    public Point GetPoint()
    {
        return this.index;
    }

    public void Highlighted(bool yes)
    {
        this.highlight.SetVisible(yes);
    }

    public void SetVisible(bool on)
    {
        this.img.enabled = on;
    }
    void UpdateName()
    {
        transform.name = "Node [" + index.x + ", " + index.y + "]";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.updating || this.type.Equals("roller") || this.type.Equals("cutter")) return;
        Highlight.Instance.HighlightPiece(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (this.type.Equals("tile") || this.type.Equals("power")) Highlight.Instance.DropPiece();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.eligibleForClick == true && (this.type.Equals("tile") || this.type.Equals("power")))
        {
            Highlight.Instance.HighlightPiece(this);
        }
    }
}
