using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections; 

public class Tile : MonoBehaviour
{
    public TileState TileState { get; private set; }
    public TileCell cell { get; private set; }
    public int number { get; private set; }
    public bool locked { get; set; }

    private Image background;
    private TextMeshProUGUI text;

    private Vector3 mouseOffset;
    private float mouseZCoord;
    private bool isDragging;

    private void Awake()
    {
        background = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnMouseDown()
    {
        
        if (locked) return; 

        isDragging = true;
        mouseZCoord = Camera.main.WorldToScreenPoint(transform.position).z;
        mouseOffset = transform.position - GetMouseWorldPos();
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            
            transform.position = GetMouseWorldPos() + mouseOffset;
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;

        
        TileCell closestCell = GetClosestCell(transform.position);
        MoveTo(closestCell);
    }

    private Vector3 GetMouseWorldPos()
    {
        
        Vector3 mouseScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, mouseZCoord);
        return Camera.main.ScreenToWorldPoint(mouseScreenPos);
    }

    private TileCell GetClosestCell(Vector3 position)
    {
        
        TileCell[] cells = FindObjectsOfType<TileCell>(); 
        TileCell closest = null;
        float minDistance = float.MaxValue;

        foreach (TileCell cell in cells)
        {
            float distance = Vector3.Distance(cell.transform.position, position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = cell;
            }
        }

        return closest;
    }

    public void SetState(TileState state, int number)
    {
        this.TileState = state;
        this.number = number;
        background.color = state.backgroundColor;
        text.color = state.textColor;
        text.text = number.ToString();

        SetColorByNumber(number);
    }

    public void Spawn(TileCell cell)
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }

        this.cell = cell;
        this.cell.tile = this;

        transform.position = cell.transform.position;
    }

    public void MoveTo(TileCell Cell)
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }

        cell = Cell;
        cell.tile = this;

        StartCoroutine(Animate(cell.transform.position, false));
    }

    public void MergeTo(TileCell cell)
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }
        this.cell = null;
        cell.tile.locked = true;
        StartCoroutine(Animate(cell.transform.position, true));
    }

    private IEnumerator Animate(Vector3 to, bool merging)
    {
        float elapsed = 0f;
        float duration = 0.2f;

        Vector3 from = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = to;
        if (merging)
        {
            Destroy(gameObject);
        }
    }

    private void SetColorByNumber(int number)
    {
        switch (number)
        {
            case 2: background.color = new Color32(238, 228, 218, 255); break;
            case 4: background.color = new Color32(237, 224, 200, 255); break;
            case 8: background.color = new Color32(242, 177, 121, 255); break;
            case 16: background.color = new Color32(245, 149, 99, 255); break;
            case 32: background.color = new Color32(246, 124, 95, 255); break;
            case 64: background.color = new Color32(246, 94, 59, 255); break;
            case 128: background.color = new Color32(237, 207, 114, 255); break;
            case 256: background.color = new Color32(237, 204, 97, 255); break;
            case 512: background.color = new Color32(237, 200, 80, 255); break;
            case 1024: background.color = new Color32(237, 197, 63, 255); break;
            case 2048: background.color = new Color32(237, 194, 46, 255); break;
            default: background.color = new Color32(60, 58, 50, 255); break;
        }
    }
}
