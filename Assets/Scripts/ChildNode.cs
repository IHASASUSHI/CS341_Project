using System.Collections;
using System.Collections.Generic;
using System;
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
    public int orientation;

    Image img;
    float velocity;
    float xPos;
    float yPos;
    int spinDirection;
    float spin;
    float speed;

    System.Random random;

    public void Initialize(int value, Point index, Sprite piece, int rotation)
    {
        this.img = GetComponent<Image>();
        this.rect = GetComponent<RectTransform>();
        if (rotation != -1)
        {
            this.rect.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -rotation));
            orientation = rotation;
        }
        SetIndex(index);
        this.img.sprite = piece;
        random = new System.Random();
        this.xPos = (float)(random.NextDouble() * 2 - 1) * 10;
        this.yPos = (float)random.NextDouble();
        this.speed = (float)random.NextDouble() * 5;
        this.velocity = (float)random.NextDouble() * 100;
        this.spinDirection = random.Next(2) - 1;
        this.spin = 0;
    }

    public void SetIndex(Point p)
    {
        this.index = p;
        ResetPosition();
        UpdateName();
    }

    public void setImage(int index)
    {
    }

    public void ResetPosition()
    {
        this.pos = new Vector2(32 + (64 * index.x), -32 - (64 * index.y));
    }

    public void MovePositionTo(Vector2 position)
    {
        this.rect.anchoredPosition += position;
    }

    public void TeleportTo(Vector2 destination)
    {
        this.rect.anchoredPosition = destination;
    }

    public void Fling(float speed)
    {
        this.yPos += this.velocity * speed;
        this.xPos += Math.Sign(this.xPos) * speed * 4;
        this.velocity = this.velocity - (9.8f * speed);
        this.spin += this.spinDirection * this.speed;
        this.rect.transform.rotation = Quaternion.Euler(new Vector3(0, 0, this.orientation + this.spin));
        TeleportTo(new Vector2(this.pos.x + this.xPos, this.pos.y + this.yPos));
    }

    public void resetVelocityAndPositions()
    {
        this.xPos = (float)(random.NextDouble() * 2 - 1) * 10;
        this.yPos = (float)random.NextDouble();
        this.velocity = (float)random.NextDouble() * 100;
        this.spin = 0;
        this.rect.transform.rotation = Quaternion.Euler(new Vector3(0, 0, this.orientation));
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
