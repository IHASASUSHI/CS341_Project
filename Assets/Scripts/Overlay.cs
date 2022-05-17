using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Overlay : MonoBehaviour
{
    public Point index;

    [HideInInspector]
    public Vector2 pos;

    [HideInInspector]
    public RectTransform rect;

    bool updating;
    Image img;

    public void Initialize(Point index, Sprite piece){
        this.img = GetComponent<Image>();
        this.rect = GetComponent<RectTransform>();

        SetIndex(index);
        this.img.sprite = piece;
        this.img.enabled = false;
    }

    public void SetIndex(Point p) {
        this.index = p;
        UpdateName();
    }

    public void SetVisible(bool on){
        this.img.enabled = on;
    }

    void UpdateName() {
        transform.name = "Overlay [" + index.x + ", " + index.y + "]";
    }
}
