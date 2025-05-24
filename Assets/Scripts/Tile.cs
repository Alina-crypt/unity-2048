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
            case 2: background.color = new Color32(255, 230, 204, 255); break; // Светлый персик
            case 4: background.color = new Color32(255, 204, 204, 255); break; // Светло-розовый
            case 8: background.color = new Color32(255, 179, 255, 255); break; // Светлая фуксия
            case 16: background.color = new Color32(204, 204, 255, 255); break; // Светлый лавандовый
            case 32: background.color = new Color32(153, 204, 255, 255); break; // Светлый голубой
            case 64: background.color = new Color32(153, 255, 204, 255); break; // Светлый мятный
            case 128: background.color = new Color32(204, 255, 153, 255); break; // Светло-зеленый
            case 256: background.color = new Color32(255, 255, 153, 255); break; // Светло-желтый
            case 512: background.color = new Color32(255, 204, 153, 255); break; // Светлый оранжевый
            case 1024: background.color = new Color32(255, 153, 153, 255); break; // Светлый коралловый
            case 2048: background.color = new Color32(255, 153, 204, 255); break; // Светло-розово-персиковый
            default: background.color = new Color32(128, 128, 128, 255); break; // Серый для больших значений
        }
    }



}
