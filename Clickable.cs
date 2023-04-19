using System;
using System.Collections.Generic;
using UnityEngine;

public class Clickable : MonoBehaviour
{
    protected Slot nearestPlayerSlot;
    protected Slot nearestFieldSlot;
    protected Slot nearestInventorySlot;
    public Slot attachedSlot;
    ClickableInfoDisplay clickableInfoDisplay;
    protected PlayerSlots playerSlots;
    private LineRendererObjectPool lineRendererPool;
    private LineRenderer slotIndicator;
    protected Field field;
    private PlayerInventory playerInventory;
    public float playerScale;
    public float fieldScale;
    private Vector3 playerScaleVector;
    private Vector3 fieldScaleVector;
    public Entity entity;
    public List<Action<Slot>> onAttachToPlayerActions = new List<Action<Slot>>();
    public List<Action> onDetachFromPlayerActions = new List<Action>();
    public List<Action<Slot>> onAttachToFieldActions = new List<Action<Slot>>();
    public List<Action> onDetachFromFieldActions = new List<Action>();
    public List<Action<Clickable>> onOtherAttachToPlayerActions = new List<Action<Clickable>>();
    public List<Action<Clickable>> onOtherDetachFromPlayerActions = new List<Action<Clickable>>();
    public List<Action<Clickable>> onOtherAttachToFieldActions = new List<Action<Clickable>>();
    public List<Action<Clickable>> onOtherDetachFromFieldActions = new List<Action<Clickable>>();
    public List<Action<Slot>> onAttachToInventoryActions = new List<Action<Slot>>();
    public List<Action> onDetachFromInventoryActions = new List<Action>();

    public virtual void Awake()
    {
        playerScaleVector = new Vector3(playerScale, playerScale, playerScale);
        fieldScaleVector = new Vector3(fieldScale, fieldScale, fieldScale);
        
        if (!TryGetComponent<Entity>(out entity))
        {
            Debug.LogWarning("ERROR: CLICKABLE DOES NOT HAVE AN ENTITY");
            Destroy(gameObject);
        }
    }

    public virtual void Start()
    {
        field = Field.field;
        clickableInfoDisplay = ClickableInfoDisplay.instance;
        playerInventory = PlayerInventory.Instance;
        playerSlots = PlayerSlots.instance;
        lineRendererPool = LineRendererObjectPool.lineRendererObjectPool;
    }


    public virtual void WhileClicked()
    {
        float playerMin = float.MaxValue;
        float fieldMin = float.MaxValue;
        float inventoryMax = float.MaxValue;
        nearestPlayerSlot = null;
        nearestFieldSlot = null;
        nearestInventorySlot = null;
        bool found = false;
        foreach (Collider2D hit in Physics2D.OverlapCircleAll(transform.position, PlayerItemInteractionController.placementRange))
        {
            float diff = (transform.position - hit.transform.position).sqrMagnitude;
            if (hit.TryGetComponent<Slot>(out Slot slot) && slot.slottedClickable == null)
            {
                found = true;
                if (slot.slotType == Slot.SlotType.player && diff < playerMin)
                {
                    playerMin = diff;
                    nearestPlayerSlot = slot;
                }
                else if (slot.slotType == Slot.SlotType.field && diff < fieldMin)
                {
                    fieldMin = diff;
                    nearestFieldSlot = slot;
                }
                else if (slot.slotType == Slot.SlotType.inventory && diff < inventoryMax)
                {
                    inventoryMax = diff;
                    nearestInventorySlot = slot;
                }
            }
        }
        
        if (found)
        {
            if (slotIndicator == null)
            {
                slotIndicator = lineRendererPool.Pool.Get();
                slotIndicator.startColor = Color.red;
                slotIndicator.endColor = Color.green;
                slotIndicator.positionCount = 2;
            }
            slotIndicator.SetPosition(0, transform.position);
            if (nearestPlayerSlot != null)
            {
                slotIndicator.SetPosition(1, nearestPlayerSlot.transform.position);
                transform.localScale = playerScaleVector;
            }
            else
            {
                transform.localScale = fieldScaleVector;
                if (nearestInventorySlot != null)
                {
                    slotIndicator.SetPosition(1, nearestInventorySlot.transform.position);
                }
                else if (nearestFieldSlot != null)
                {
                    slotIndicator.SetPosition(1, nearestFieldSlot.transform.position);
                }
            }
        }
        else if (slotIndicator != null)
        {
            lineRendererPool.Pool.Release(slotIndicator);
            slotIndicator = null;
        }
    }

    public virtual void onAttachToPlayer(Slot slot)
    {
        foreach (Action<Slot> action in onAttachToPlayerActions)
        {
            action(slot);
        }
        slot.slottedClickable = this;
        transform.position = slot.transform.position;
        transform.parent = slot.transform;
        attachedSlot = slot;
            
        foreach (Clickable c in playerSlots.getSlottedClickables())
        {
            c.onOtherAttachToPlayer(this);
        }
    }
    public virtual void onDetachFromPlayer()
    {
        foreach (Action action in onDetachFromPlayerActions)
        {
            action();
        }
        foreach (Clickable c in playerSlots.getSlottedClickables())
        {
            c.onOtherDetachFromPlayer(this);
        }
        foreach (Clickable c in field.clickables)
        {
            c.onOtherDetachFromPlayer(this);
        }
        attachedSlot.slottedClickable = null;
        attachedSlot = null;
        transform.parent = null;
    }

    public virtual void onAttachToInventory(Slot slot)
    {
        foreach (Action<Slot> action in onAttachToInventoryActions)
        {
            action(slot);
        }
        entity.rangeHelper.enabled = false;
        slot.slottedClickable = this;
        transform.position = slot.transform.position;
        transform.parent = slot.transform;
        attachedSlot = slot;
        playerInventory.inventory.Add(this);
    }

    public virtual void onDetachFromInventoy()
    {
        foreach (Action action in onDetachFromInventoryActions)
        {
            action();
        }
        entity.rangeHelper.enabled = true;
        playerInventory.inventory.Remove(this);
        attachedSlot.slottedClickable = null;
        transform.parent = null;
        attachedSlot = null;
    }

    public virtual void onOtherAttachToPlayer(Clickable other)
    {
        foreach (Action<Clickable> action in onOtherAttachToPlayerActions)
        {
            action(other);
        }
    }

    public virtual void onOtherDetachFromPlayer(Clickable other)
    {
        foreach (Action<Clickable> action in onOtherDetachFromPlayerActions)
        {
            action(other);
        }
    }

    public virtual void onOtherAttachToField(Clickable other)
    {
        foreach (Action<Clickable> action in onOtherAttachToFieldActions)
        {
            action(other);
        }
    }

    public virtual void onOtherDetachFromField(Clickable other)
    {
        foreach (Action<Clickable> action in onOtherDetachFromFieldActions)
        {
            action(other);
        }
    }

    public virtual void onAttachToField(Slot slot)
    {
        gameObject.layer = LayerMask.NameToLayer("PlayerObject");
        entity.healthBar.gameObject.SetActive(true);
        attachedSlot = slot;
        transform.position = slot.transform.position;
        transform.parent = slot.transform;
        slot.slottedClickable = this;
        transform.localScale = fieldScaleVector;
        field.onAttachToField(this);
        foreach (Action<Slot> action in onAttachToFieldActions)
        {
            action(slot);
        }

        /*foreach (Slot s in attachedSlot.adjacentSlots)
        {
            s.GetComponent<SpriteRenderer>().color = Color.red;
        }*/
    }

    public virtual void onDetachFromFieldSlot()
    {
        gameObject.layer = LayerMask.NameToLayer("Pickup");
        foreach (Action action in onDetachFromFieldActions)
        {
            action();
        }
        field.onDetachFromField(this);
        entity.healthBar.gameObject.SetActive(false);
        attachedSlot.slottedClickable = null;
        attachedSlot = null;
        transform.localScale = playerScaleVector;
        transform.parent = null;
    }

    public virtual void OnClickDown()
    {
        clickableInfoDisplay.displayClickable(this);
        if (attachedSlot != null)
        {
            if (attachedSlot.slotType == Slot.SlotType.player)
            {
                onDetachFromPlayer();
            }
            else if (attachedSlot.slotType == Slot.SlotType.field)
            {
                onDetachFromFieldSlot();
            }
            else if (attachedSlot.slotType == Slot.SlotType.inventory)
            {
                onDetachFromInventoy();
            }
        }
    }

    public virtual void OnClickUp()
    {
        clickableInfoDisplay.stopDisplayingClickable();
        if (nearestFieldSlot != null || nearestInventorySlot != null || nearestPlayerSlot != null)
        {
            lineRendererPool.Pool.Release(slotIndicator);
            slotIndicator = null;
        }
        if (nearestPlayerSlot != null)
        {
            onAttachToPlayer(nearestPlayerSlot);
        }
        else if (nearestInventorySlot != null)
        {
            onAttachToField(nearestInventorySlot);
        }
        else if (nearestFieldSlot != null)
        {
            onAttachToField(nearestFieldSlot);
        }
    }
}
