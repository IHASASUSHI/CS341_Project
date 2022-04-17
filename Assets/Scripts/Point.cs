using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Point : MonoBehaviour
{
    public int x;
    public int y;

    public Point(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public Vector2 toVector(int x, int y) {
        return new Vector2(x, y);
    }

    public static Point fromVector(Vector2 v) {
        return new Point((int) v.x, (int) v.y);
    }

    public static Point fromVector(Vector3 v) {
        return new Point((int)v.x, (int) v.y);
    }

    public static Point clone(Point p) {
        return new Point(p.x, p.y);
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
