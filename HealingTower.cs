using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HealingTower : Entity
{
    float healTimer = 0;
    private Clickable clickable;
    private PlayerSlots playerSlots;
    private Field field;
    public Color healColor;

    public override void Start()
    {
        base.Start();
        field = Field.field;
        playerSlots = PlayerSlots.instance;
        if (!TryGetComponent<Clickable>(out clickable))
        {
            Destroy(gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Time.time > healTimer)
        {
            healTimer = Time.time + getSpeed();
            if (clickable.attachedSlot != null)
            {
                if (clickable.attachedSlot.slotType == Slot.SlotType.player)
                {
                    foreach (Clickable c in playerSlots.getSlottedClickables())
                    {
                        if (c != clickable)
                        {
                            c.entity.health.changeHealth(getMagnitude());
                        }
                    }
                }
                else if (clickable.attachedSlot.slotType == Slot.SlotType.field)
                {
                    healSlottedPlayerObjects();
                }
            }
            health.changeHealth(getMagnitude());
        }
    }

    public void healAdjacentPlayerObjects()
    {
        foreach (Clickable c in playerSlots.getSlottedClickables())
        {
            if (c != clickable)
            {
                c.entity.health.changeHealth(getMagnitude());
            }
        }
    }

    public void healSlottedPlayerObjects()
    {
        foreach (Slot s in clickable.attachedSlot.adjacentSlots)
        {
            if (s.slottedClickable != null)
            {
                s.slottedClickable.entity.health.changeHealth(getMagnitude());
            }
            helper.colorSwellKeepAlpha(s.spriteRenderer, Color.green, 1);
        }
    }
}
