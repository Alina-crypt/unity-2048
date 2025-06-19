using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileBoard : MonoBehaviour
{
    public GameManager gameManager;
    public Tile tilePrefab;
    public TileState[] tileStates;
    private TileGrid grid;
    private List<Tile> tiles;
    AudioManager audioManager;
    private bool waiting;
    private Vector2 startTouch;
    private Vector2 endTouch;
    private float swipeThreshold = 50f;

    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>(16);
    }
    private void Start()
    {
        transform.localScale = Vector3.zero;
        StartCoroutine(ScaleIn());
    }

    private IEnumerator ScaleIn()
    {
        float duration = 0.5f; // Продолжительность анимации
        float time = 0f;
        Vector3 targetScale = Vector3.one;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
            yield return null;
        }

        transform.localScale = targetScale;
    }

    public string SerializeBoard()
    {
        List<string> tileData = new List<string>();

        foreach (var tile in tiles)
        {
            int x = tile.cell.x;
            int y = tile.cell.y;
            int number = tile.number;
            tileData.Add($"{x}:{y}:{number}");
        }

        return string.Join("|", tileData); // Пример: "0:0:2|1:0:4|2:0:2"
    }
    public void DeserializeBoard(string data)
    {
        ClearBoard();

        if (string.IsNullOrEmpty(data))
            return;

        string[] entries = data.Split('|');
        foreach (var entry in entries)
        {
            string[] parts = entry.Split(':');
            if (parts.Length != 3)
                continue;

            int x = int.Parse(parts[0]);
            int y = int.Parse(parts[1]);
            int number = int.Parse(parts[2]);

            TileCell cell = grid.GetCell(x, y);
            if (cell != null)
            {
                Tile tile = Instantiate(tilePrefab, grid.transform);
                tile.SetState(GetStateForNumber(number), number);
                tile.Spawn(cell); // без анимации перемещения
                tiles.Add(tile);
            }
        }
    }
    private TileState GetStateForNumber(int number)
    {
        foreach (var state in tileStates)
        {
            if (state.number == number)
                return state;
        }
        return tileStates[0]; // fallback на 2
    }

    public void ClearBoard()
    {

        foreach(var cell in grid.cells)
        {
            cell.tile = null;
        }
        foreach (var tile in tiles)
        {
            Destroy(tile.gameObject);
        }
        tiles.Clear();
        }


   public void CreateTile()
    {
        Tile tile = Instantiate(tilePrefab, grid.transform);
        tile.SetState(tileStates[0], 2);
        tile.Spawn(grid.GetRAndomEmptyCell());
        tiles.Add(tile);

    }
    private void Update()
    {
        if (waiting || riddleActive) return;

        // ПК-клавиши
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            HandleSwipe(Vector2Int.up);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            HandleSwipe(Vector2Int.down);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            HandleSwipe(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            HandleSwipe(Vector2Int.right);
        }

        // Мобильные свайпы
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouch = touch.position;
                    break;

                case TouchPhase.Ended:
                    endTouch = touch.position;
                    Vector2 swipeDelta = endTouch - startTouch;

                    if (swipeDelta.magnitude < swipeThreshold)
                        return;

                    Vector2 direction = swipeDelta.normalized;

                    if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                    {
                        if (direction.x > 0)
                            HandleSwipe(Vector2Int.right);
                        else
                            HandleSwipe(Vector2Int.left);
                    }
                    else
                    {
                        if (direction.y > 0)
                            HandleSwipe(Vector2Int.up);
                        else
                            HandleSwipe(Vector2Int.down);
                    }
                    break;
            }
        }
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            startTouch = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            endTouch = Input.mousePosition;
            Vector2 swipeDelta = endTouch - startTouch;

            if (swipeDelta.magnitude >= swipeThreshold)
            {
                Vector2 direction = swipeDelta.normalized;

                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                {
                    if (direction.x > 0)
                        HandleSwipe(Vector2Int.right);
                    else
                        HandleSwipe(Vector2Int.left);
                }
                else
                {
                    if (direction.y > 0)
                        HandleSwipe(Vector2Int.up);
                    else
                        HandleSwipe(Vector2Int.down);
                }
            }
        }
#endif

    }

    public void HandleSwipe(Vector2Int direction)
    {
        if (waiting || riddleActive) return;

        if (direction == Vector2Int.up)
            MoveTiles(Vector2Int.up, 0, 1, 1, 1);
        else if (direction == Vector2Int.down)
            MoveTiles(Vector2Int.down, 0, 1, grid.height - 2, -1);
        else if (direction == Vector2Int.left)
            MoveTiles(Vector2Int.left, 1, 1, 0, 1);
        else if (direction == Vector2Int.right)
            MoveTiles(Vector2Int.right, grid.width - 2, -1, 0, 1);
    }


    private void MoveTiles(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        bool changed = false;

        for (int x = startX; x >= 0 && x < grid.width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < grid.height; y += incrementY)
            {
                TileCell cell = grid.GetCell(x, y);
                if (cell.occupied)
                {
                   changed |= MoveTile(cell.tile, direction);
                }
            }
        }
        if (changed)
        {
            StartCoroutine(WaitForChanges());
            
        }
        if (CheckForGameOver())
        {
            gameManager.GameOver();
        }

       
        gameManager.SaveGame();

    }
    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjacentCell(tile.cell, direction);

        while (adjacent != null)
        {
            if (adjacent.occupied)
            {
                if (CanMerge(tile, adjacent.tile))
                {
                    Merge(tile, adjacent.tile);
                    return true;
                }
                break;
            }

            newCell = adjacent;
            adjacent = grid.GetAdjacentCell(adjacent, direction);
        }

        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
        }

        return false;
    }


    private bool CanMerge( Tile a , Tile b)
    {
      return a.number == b.number && !b.locked;
    }

    private void Merge(Tile a, Tile b)
    {
        AudioManager.Instance.PlayMerge();
        tiles.Remove(a);
        a.MergeTo(b.cell);

        int Index = Mathf.Clamp(IndexOf(b.TileState) + 1, 0, tileStates.Length - 1);
        int number = b.number * 2;
        b.SetState(tileStates[Index], number);

        gameManager.IncreaseScore(number);
    }

    private int IndexOf(TileState state)
    {
        for (int i = 0; i < tileStates.Length; i++)
        {
            if (state== tileStates[i])
            {
                return i;
            }
        }
        return -1;
    }
    private IEnumerator WaitForChanges()
    {
        waiting = true;
        yield return new WaitForSeconds(0.2f);

        waiting = false;

        foreach (var tile in tiles)
        {
            tile.locked=false;
        }

        if (tiles.Count!=grid.size)
        {
            CreateTile();
        }
       
        
        if (CheckForGameOver())
        {
            gameManager.GameOver();
            gameManager.SaveGame();

        }
    }
    private bool CheckForGameOver()
    {
        if (tiles.Count != grid.size)
        {
            return false;
        }

        foreach (var tile in tiles)
        {
            TileCell up=grid.GetAdjacentCell(tile.cell,Vector2Int.up);
            TileCell down = grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            TileCell left = grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            TileCell right = grid.GetAdjacentCell(tile.cell, Vector2Int.right);

            if (up !=null && CanMerge(tile, up.tile))
            {
                return false;
            }
            if (down != null && CanMerge(tile, down.tile))
            {
                return false;
            }
            if (left != null && CanMerge(tile, left.tile))
            {
                return false;
            }
            if (right != null && CanMerge(tile, right.tile))
            {
                return false;
            }

        }
        return true;
    }
    private bool riddleActive;

    public void Freeze(bool value)
    {
        riddleActive = value;
    }


}
