using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePieces : MonoBehaviour
{
    public static MovePieces Instance;
    BeFractioned game;

    NodePiece moving;
    Point newIndex;
    Vector2 mouseStart;

    public void Awake() {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.game = GetComponent<BeFractioned>();
    }

    // Update is called once per frame
    void Update()
    {
        if(this.moving != null) {
            Vector2 dir = ((Vector2)Input.mousePosition - this.mouseStart);
            Vector2 nDir = dir.normalized;
            Vector2 aDir = new Vector2(Mathf.Abs(dir.x), Mathf.Abs(dir.y));

            this.newIndex = Point.clone(moving.index);
            Point add = Point.zero;
            if(dir.magnitude > 32) {
                if(aDir.x > aDir.y) {
                    add = new Point((nDir.x > 0) ? 1 : -1, 0);
                }
                else if(aDir.y > aDir.x) {
                    add = new Point(0, (nDir.y > 0) ? -1 : 1);
                }
            }
            this.newIndex.add(add);

            Vector2 pos = game.getPositionFromPoint(moving.index);
            if(!this.newIndex.Equals(moving.index)) pos += Point.mult((new Point(add.x, -add.y)), 16).toVector();
            this.moving.MovePositionTo(pos);
        }
    }

    public void MovePiece(NodePiece piece) {
        if(this.moving != null) return;
        this.moving = piece;
        this.mouseStart = Input.mousePosition;
    }

    public void DropPiece() {
        if(this.moving == null) return;
        if(!this.newIndex.Equals(this.moving.index)) this.game.FlipPieces(this.moving.index, this.newIndex, true);
        else game.ResetPiece(this.moving);
        this.game.ResetPiece(this.moving);
        this.moving = null;
    }
}
