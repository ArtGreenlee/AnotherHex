using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSlots : MonoBehaviour
{
    public static PlayerSlots instance;
    public List<Slot> slots = new List<Slot>();
    private void Awake()
    {
        instance = this;
        foreach (Slot slot in GetComponentsInChildren<Slot>())
        {
            if (slot.slotType == Slot.SlotType.player)
            {
                slots.Add(slot);
            }
        }
    }

    public List<Clickable> getSlottedClickables()
    {
        List<Clickable> ret = new List<Clickable>();
        foreach (Slot slot in slots)
        {
            if (slot.slottedClickable != null)
            {
                ret.Add(slot.slottedClickable); 
            }
        }
        return ret;
    }
}
