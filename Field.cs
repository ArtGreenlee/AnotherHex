using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    public int size;
    public static Field field;
    public GameObject FieldComponentObject;
    public GameObject WallObject;
    private GameObject NorthWall;
    private GameObject SouthWall;
    private GameObject EastWall;
    private GameObject WestWall;
    public float y_max;
    public float x_max;
    public float y_min;
    public float x_min;
    public LayerMask slotLayerMask;
    public LayerMask freeAreaLayerMask;

    public Slot[,] fieldSlots;
    public float componentSpacing;
    public float fieldOffset;
    public HashSet<Clickable> clickables = new HashSet<Clickable>();

    public Slot getNearestSlotToPosition(Vector3 pos)
    {
        float min = float.MaxValue;
        Slot nearest = null;
        foreach (Collider2D hit in Physics2D.OverlapCircleAll(pos, 1))
        {
            if (hit.TryGetComponent<Slot>(out Slot slot))
            {
                float diff = (pos - hit.transform.position).sqrMagnitude;
                if (diff < min)
                {
                    min = diff;
                    nearest = slot;
                }
            }
        }
        if (nearest == null)
        {
            Debug.Log("what");
        }
        return nearest;
    }

    public Vector3 getRandomFreeSpaceOnField(float radius)
    {
        int attempts = 0;
        Vector3 point;
        Collider2D col;
        do
        {
            attempts++;
            point = getRandomPointOnField(radius);
            col = Physics2D.OverlapCircle(point, radius, freeAreaLayerMask);
        } while (col != null && attempts < 10);
        return point;
    }

    public Vector3 getRandomPointOnField()
    {
        return getRandomPointOnField(0);
    }

    public Vector3 getRandomPointOnField(float range)
    {
        return new Vector3(Random.Range(field.x_min + 2, field.x_max - 2),
                    Random.Range(field.y_min + 2, field.y_max - 2), 0);
    }

    private void Awake()
    {
        field = this;
    }

    public void onAttachToField(Clickable c)
    {
        foreach (Clickable other in clickables)
        {
            other.onOtherAttachToField(c);
        }
        clickables.Add(c);
    }

    public void onDetachFromField(Clickable c)
    {
        clickables.Remove(c);
        foreach (Clickable other in clickables)
        {
            other.onOtherDetachFromField(c);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        fieldOffset = size / 2 * componentSpacing;
        fieldSlots = new Slot[size, size];
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                float xPos = Mathf.Round(x - size / 2) * componentSpacing;
                float yPos = Mathf.Round(y - size / 2) * componentSpacing;
                if (y % 2 == 0)
                {
                    xPos += componentSpacing / 2;
                }
                GameObject newFieldComponentObject = Instantiate(FieldComponentObject, new Vector3(xPos, yPos, 0), Quaternion.identity, transform);
                fieldSlots[x, y] = newFieldComponentObject.GetComponent<Slot>();
                fieldSlots[x, y].indices = new Vector2Int(x, y);
                fieldSlots[x, y].adjacentSlots = new List<Slot>();
                
            }
        }
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (x > 0)
                {
                    fieldSlots[x, y].adjacentSlots.Add(fieldSlots[x - 1, y]);
                }
                if (x < size - 1)
                {
                    fieldSlots[x, y].adjacentSlots.Add(fieldSlots[x + 1, y]);
                }
                if (y > 0)
                {
                    fieldSlots[x, y].adjacentSlots.Add(fieldSlots[x, y - 1]);
                }
                if (y < size - 1)
                {
                    fieldSlots[x, y].adjacentSlots.Add(fieldSlots[x, y + 1]);
                }
                if (y < size - 1)
                {
                    if (y % 2 == 0 && x < size - 1)
                    {
                        fieldSlots[x, y].adjacentSlots.Add(fieldSlots[x + 1, y + 1]);
                    }
                    else if (x > 0)
                    {
                        fieldSlots[x, y].adjacentSlots.Add(fieldSlots[x - 1, y + 1]);
                    }
                }
                if (y > 0)
                {
                    if (y % 2 == 0 && x < size - 1)
                    {
                        fieldSlots[x, y].adjacentSlots.Add(fieldSlots[x + 1, y - 1]);
                    }
                    else if (x > 0)
                    {
                        fieldSlots[x, y].adjacentSlots.Add(fieldSlots[x - 1, y - 1]);
                    }
                }
            }
        }
                
        EastWall = Instantiate(WallObject, new Vector3(fieldOffset, 0), Quaternion.identity, transform);
        EastWall.transform.localScale = new Vector3(1, size * componentSpacing + componentSpacing, 1);
        WestWall = Instantiate(WallObject, new Vector3(-fieldOffset, 0), Quaternion.identity, transform);
        WestWall.transform.localScale = new Vector3(1, size * componentSpacing + componentSpacing, 1);

        NorthWall = Instantiate(WallObject, new Vector3(0, fieldOffset), Quaternion.identity, transform);
        NorthWall.transform.localScale = new Vector3(size * componentSpacing + componentSpacing, 1, 1);
        SouthWall = Instantiate(WallObject, new Vector3(0, -fieldOffset - componentSpacing), Quaternion.identity, transform);
        SouthWall.transform.localScale = new Vector3(size * componentSpacing + componentSpacing, 1, 1);


        y_max = NorthWall.transform.position.y;
        y_min = -y_max;
        x_max = EastWall.transform.position.x;
        x_min = -x_max;
    }

    public List<Clickable> getAdjacentClickables(Clickable c)
    {
        List<Clickable> returnList = new List<Clickable>();
        if (c.attachedSlot == null) return returnList;
        foreach (Slot s in c.attachedSlot.adjacentSlots)
        {
            if (s.slottedClickable != null)
            {
                returnList.Add(s.slottedClickable);
            }
        }
        return returnList;
       
    }

    public bool pointOOB(Vector3 point)
    {
        return point.x > x_max || point.x < x_min || point.y > y_max || point.y < y_min;
    }
    public bool pointOOB(Vector3 point, float padding)
    {
        float y_min_temp = y_min + padding;
        float y_max_temp = y_max - padding;
        float x_min_temp = x_min + padding;
        float x_max_temp = x_max - padding;
        return point.x > x_max_temp || point.x < x_min_temp || point.y > y_max_temp || point.y < y_min_temp;
    }

    public Vector2Int roundToField(Vector3 point)
    {
        return new Vector2Int(Mathf.RoundToInt(point.x + fieldOffset), Mathf.RoundToInt(point.y + fieldOffset));
    }

    public Slot getFieldComponentAtPoint(Vector3 point)
    {
        Vector2Int temp = roundToField(point);
        return fieldSlots[temp.x, temp.y];
    }
}
