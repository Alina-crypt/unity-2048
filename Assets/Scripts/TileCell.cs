using UnityEngine;

public class TileCell : MonoBehaviour
{
    public Vector2Int coordinates
    {
        get => new Vector2Int(x, y);
        set
        {
            x = value.x;
            y = value.y;
        }
    }


    public int x;
    public int y;
    public Tile tile;

    public bool occupied => tile != null;
}
