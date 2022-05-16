using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NodePiece : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
{
    public int value;
    public Point index;
    public bool power;

    [HideInInspector]
    public Vector2 pos;
    [HideInInspector]
    public RectTransform rect;

    bool updating;
    Image img;
    Overlay highlight;

    public void Initialize(int value, Point index, Sprite piece, bool power) {
        this.img = GetComponent<Image>();
        this.rect = GetComponent<RectTransform>();
        this.power = power;

        this.value = value;
        SetIndex(index);
        this.img.sprite = piece;
        this.highlight = null;
    }

    public void SetIndex(Point p) {
        this.index = p;
        ResetPosition();
        UpdateName();
    }

    public void SetHighlight(Overlay highlight) {
        this.highlight = highlight;
    }

    public void ResetPosition() {
        pos = new Vector2(32 + (64 * index.x), -32 - (64 * index.y));
    }

    public void MovePosition(Vector2 move) {
        this.rect.anchoredPosition += move * Time.deltaTime * 8f;
    }

    public void MovePositionTo(Vector2 move) {
        this.rect.anchoredPosition = Vector2.Lerp(this.rect.anchoredPosition, move, Time.deltaTime * 16f);
    }

    public bool UpdatePiece() {
        if(Vector2.Distance(this.rect.anchoredPosition, pos) > 1) {
            MovePositionTo(this.pos);
            this.updating = true;
            return true;
        }
        else {
            rect.anchoredPosition = pos;
            updating = false;
            return false;
        }
    }

    public Point GetPoint() {
        return this.index;
    }

    public void Highlighted(bool yes) {
        this.highlight.SetVisible(yes);
    }
    void UpdateName() {
        transform.name = "Node [" + index.x + ", " + index.y + "]";
    }

    public void OnPointerDown(PointerEventData eventData) {
        if(this.updating) return;
        Highlight.Instance.HighlightPiece(this);
    }

    public void OnPointerUp(PointerEventData eventData) {
        Highlight.Instance.DropPiece();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.eligibleForClick == true)
        {
            Highlight.Instance.HighlightPiece(this);
        }
    }
}
