using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChildNode : MonoBehaviour
{
    public Point index;
    [HideInInspector]
    public Vector2 pos;
    [HideInInspector]
    public RectTransform rect;

    Image img;

    public void Initialize(int value, Point index, Sprite piece)
    {
        this.img = GetComponent<Image>();
        this.rect = GetComponent<RectTransform>();

        SetIndex(index);
        this.img.sprite = piece;
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
            this.rect.transform.Rotate(0,0,90);
        }
    }

    public void ResetPosition()
    {
        this.pos = new Vector2(32 + (64 * index.x), -32 - (64 * index.y));
    }

    public void MovePositionTo(Vector2 position)
    {
        this.rect.anchoredPosition += position;
    }
    public void SetVisible(bool on)
    {
        this.img.enabled = on;
    }
    void UpdateName()
    {
        transform.name = "Child [" + index.x + ", " + index.y + "]";
    }
}
