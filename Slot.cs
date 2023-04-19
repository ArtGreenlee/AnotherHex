using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public enum SlotType
    {
        field,
        player,
        inventory
    }
    public Clickable slottedClickable;
    public SpriteRenderer spriteRenderer;
    public Vector2Int indices;
    public SlotType slotType;
    public List<Slot> adjacentSlots;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
