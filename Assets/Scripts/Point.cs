using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Point
{
    public int x;
    public int y;

    public Point(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public void mult(int n) {
        this.x *= n;
        this.y *= n;
    }

    public void add(Point p) {
        this.x += p.x;
        this.y += p.y;
    }

    public Vector2 toVector() {
        return new Vector2(x, y);
    }

    public static Point fromVector(Vector2 v) {
        return new Point((int) v.x, (int) v.y);
    }

    public static Point fromVector(Vector3 v) {
        return new Point((int)v.x, (int) v.y);
    }

    public static Point mult(Point p, int n) {
        return new Point(p.x * n, p.y * n);
    }

    public static Point add(Point p, Point o) {
        return new Point(p.x + o.x, p.y + o.y);
    }

    public static Point clone(Point p) {
        return new Point(p.x, p.y);
    }

    public static Point zero {
        get { return new Point(0,0); }
    }

    public static Point one {
        get {return new Point(1,1);}
    }

    public static Point up {
        get {return new Point(0,1);}
    }

    public static Point down {
        get {return new Point(0,-1);}
    }

    public static Point left {
        get {return new Point(-1,0);}
    }

    public static Point right {
        get {return new Point(1,0);}
    }

    public bool Equals(Point other) {
      if (other == null)
         return false;

      if (this.x == other.x && this.y == other.y)
         return true;
      else
         return false;
    }
    public override bool Equals(object obj) { // override object.Equals
        //
        // See the full list of guidelines at
        //   http://go.microsoft.com/fwlink/?LinkID=85237
        // and also the guidance for operator== at
        //   http://go.microsoft.com/fwlink/?LinkId=85238
        //

        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        return base.Equals (obj);
    }
}
