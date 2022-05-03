using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NodePiece : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
{
    public int value;
    public Point index;

    [HideInInspector]
    public Vector2 pos;
    [HideInInspector]
    public NodePiece flipped;
    [HideInInspector]
    public RectTransform rect;

    bool updating;
    Image img;
    Overlay highlight;

    public void Initialize(int value, Point index, Sprite piece) {
        this.flipped = null;
        this.img = GetComponent<Image>();
        this.rect = GetComponent<RectTransform>();

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
        this.rect.anchoredPosition += move * Time.deltaTime * 16f;
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

    public void Highlighted(bool yes) {
        this.highlight.SetVisible(yes);
    }
    void UpdateName() {
        transform.name = "Node [" + index.x + ", " + index.y + "]";
    }

    public void OnPointerDown(PointerEventData eventData) {
        if(this.updating) return;
        MovePieces.Instance.MovePiece(this);
        Highlight.Instance.MovePiece(this);
    }

    public void OnPointerUp(PointerEventData eventData) {
        MovePieces.Instance.DropPiece();
        Highlight.Instance.DropPiece();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.eligibleForClick == true)
        {
            Highlight.Instance.MovePiece(this);
        }
    }
}
