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
    int updateTick;
    string hitByPower;
    Image img;
    Overlay highlight;

    public void Initialize(int value, Point index, Sprite piece, string type)
    {
        this.img = GetComponent<Image>();
        this.rect = GetComponent<RectTransform>();

        this.value = value;
        this.type = type;
        SetIndex(index);
        this.img.sprite = piece;
        this.highlight = null;
        this.hitByPower = "";
        this.updateTick = 0;
    }

    public void SetIndex(Point p)
    {
        this.index = p;
        foreach (ChildNode piece in childPieces) piece.SetIndex(p);
        ResetPosition();
        UpdateName();
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
    }

    public void MovePositionTo(Vector2 move, float speed)
    {
        Vector2 diff = this.rect.anchoredPosition - move;
        float xPos = 0;
        float yPos = 0;
        if (Math.Abs(diff.x) > 1 * speed) xPos = -1 * Math.Sign(diff.x) * speed;
        else xPos = -1 * diff.x;
        if (Math.Abs(diff.y) > 1 * speed) yPos = -1 * Math.Sign(diff.y) * speed;
        else yPos = -1 * diff.y;
        this.rect.anchoredPosition += new Vector2(xPos, yPos);
        foreach (ChildNode piece in childPieces) piece.MovePositionTo(new Vector2(xPos, yPos));
    }

    public void TeleportTo(Vector2 destination)
    {
        this.rect.anchoredPosition = destination;
        foreach (ChildNode piece in childPieces) piece.TeleportTo(destination);
    }

    public void cutterSlice()
    {
    }

    public bool UpdatePiece()
    {
        if (Vector2.Distance(this.rect.anchoredPosition, this.pos) > 1)
        {
            if (this.type.Equals("cutter") || this.type.Equals("roller")) MovePositionTo(this.pos, 5f);
            else MovePositionTo(this.pos, 4f);
            this.updating = true;
            return true;
        }
        else if (!this.hitByPower.Equals("") && this.updateTick < 300)
        {
            if (!this.hitByPower.Equals("")) foreach (ChildNode piece in childPieces) piece.Fling(0.05f);
            this.updateTick += 5;
            this.updating = true;
            return true;
        }
        else
        {
            if (!this.hitByPower.Equals("")) foreach (ChildNode piece in childPieces) piece.resetVelocityAndPositions();
            rect.anchoredPosition = pos;
            updating = false;
            this.updateTick = 0;
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
