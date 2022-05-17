using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NodePiece : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
{
    public int value;
    public Point index;
    public string type;

    [HideInInspector]
    public Vector2 pos;
    [HideInInspector]
    public RectTransform rect;

    bool updating;
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
    }

    public void SetIndex(Point p)
    {
        this.index = p;
        ResetPosition();
        UpdateName();
    }

    public void SetHighlight(Overlay highlight)
    {
        this.highlight = highlight;
    }

    public void ResetPosition()
    {
        pos = new Vector2(32 + (64 * index.x), -32 - (64 * index.y));
    }

    public void MovePositionTo(Vector2 move, float speed)
    {
        this.rect.anchoredPosition = Vector2.Lerp(this.rect.anchoredPosition, move, speed);
    }

    public bool UpdatePiece()
    {
        if (Vector2.Distance(this.rect.anchoredPosition, this.pos) > 1)
        {
            if (this.type.Equals("cutter") || this.type.Equals("roller")) MovePositionTo(this.pos, 0.01f);
            else MovePositionTo(this.pos, 0.02f);
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
