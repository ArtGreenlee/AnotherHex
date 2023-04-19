using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEditor.Progress;
using System;

public class BuffTower : Entity
{
    private LineRendererObjectPool lineRendererObjectPool;
    private Effect[] buffs;
    private Dictionary<Entity, LineRenderer> buffIndicators = new Dictionary<Entity, LineRenderer>();
    public Color buffIndicatorColor;
    public GameObject playerBuffRangeIndicatorObject;
    HashSet<Entity> buffedEntities = new HashSet<Entity>();
    public Clickable clickable;
    private PlayerSlots playerSlots;
    Field field;
    public float buffUpdateCooldown;
    private float buffUpdateCooldownTimer = 0;

    public override void Awake()
    {
        base.Awake();
        buffs = GetComponents<Effect>();
        if (!TryGetComponent<Clickable>(out clickable))
        {
            Debug.LogWarning("ERROR: BUFF TOWER DOES NOT AVE CLICKABLE");
            Destroy(gameObject);
        }
    }
    
    public override void Start()
    {
        base.Start();
        clickable.onAttachToFieldActions.Add(buffTowerAttachToField);
        clickable.onDetachFromFieldActions.Add(buffTowerDetachFromField);

        clickable.onAttachToPlayerActions.Add(buffTowerAttachToPlayer);
        clickable.onDetachFromPlayerActions.Add(buffTowerDetachFromPlayer);

        clickable.onOtherAttachToFieldActions.Add(buffTowerOtherAttachToField);
        clickable.onOtherDetachFromFieldActions.Add(buffTowerOtherDetachFromField);

        clickable.onOtherAttachToPlayerActions.Add(buffTowerOtherAttachToPlayer);
        clickable.onOtherDetachFromPlayerActions.Add(buffTowerOtherDetachFromPlayer);

        playerSlots = PlayerSlots.instance;
        playerBuffRangeIndicatorObject = Instantiate(playerBuffRangeIndicatorObject);
        playerBuffRangeIndicatorObject.SetActive(false);
        lineRendererObjectPool = LineRendererObjectPool.lineRendererObjectPool;
        field = Field.field;
        if (rangeHelper != null)
        {
            rangeHelper.setEnterCallbackFunction(onRangeEnter);
            rangeHelper.setExitCallbackFunction(onRangeExit);
        }
        
    }

    private void buffEntity(Entity c)
    {
        if (c.TryGetComponent<BuffTower>(out BuffTower trash)) return;
        if (buffedEntities.Contains(c)) return;
        buffedEntities.Add(c);
        buffIndicators.Add(c, lineRendererObjectPool.Pool.Get());
        buffIndicators[c].sortingOrder = 1;
        buffIndicators[c].positionCount = 2;
        buffIndicators[c].startColor = buffIndicatorColor;
        buffIndicators[c].endColor = buffIndicatorColor;
        foreach (Effect b in buffs)
        {
            Weapon w = c.GetComponent<Weapon>();
            if (w != null && b.TryGetComponent<OnShootEffect>(out OnShootEffect onShootEffect))
            {
                w.onShootEffects.Add(onShootEffect);
            }
            else if (w != null && b.TryGetComponent<OnHitEffect>(out OnHitEffect onHitEffect))
            {
                w.onHitEffects.Add(onHitEffect);
            }
            else
            {
                StartCoroutine(b.applyEffect(c, this));
            }
        }
    }

    private void onRangeEnter(Collider2D other)
    {
        if (other.TryGetComponent<Entity>(out Entity e) &&
            other.TryGetComponent<Clickable>(out Clickable c) &&
            c.attachedSlot != null && c.attachedSlot.slotType != Slot.SlotType.inventory &&
            clickable.attachedSlot != null && clickable.attachedSlot.slotType != Slot.SlotType.inventory &&
            !other.TryGetComponent<BuffTower>(out BuffTower b))
        {
            buffEntity(e);
        }
    }

    private void onRangeExit(Collider2D other)
    {
        if (other.TryGetComponent<Entity>(out Entity e) &&
            other.TryGetComponent<Clickable>(out Clickable c) &&
            c.attachedSlot != null && c.attachedSlot.slotType != Slot.SlotType.inventory &&
            clickable.attachedSlot != null && clickable.attachedSlot.slotType != Slot.SlotType.inventory)
        {
            unbuffEntity(e);
        }
    }

    private void unbuffEntity(Entity c)
    {
        if (!buffedEntities.Contains(c)) return;
        if (c.TryGetComponent<BuffTower>(out BuffTower trash)) return;
        buffedEntities.Remove(c);
        lineRendererObjectPool.Pool.Release(buffIndicators[c]);
        buffIndicators.Remove(c);
        foreach (Effect b in buffs)
        {
            Weapon w = c.GetComponent<Weapon>();
            if (w != null && b.TryGetComponent<OnShootEffect>(out OnShootEffect onShootEffect))
            {
                w.onShootEffects.Remove(onShootEffect);
            }
            else if (w != null && b.TryGetComponent<OnHitEffect>(out OnHitEffect onHitEffect))
            {
                w.onHitEffects.Remove(onHitEffect);
            }
            else if (b.isActiveAndEnabled)
            {
                StartCoroutine(b.removeEffect(c, this));
            }
        }
    }

    public void buffTowerAttachToField(Slot slot)
    {
        if (slot.slotType == Slot.SlotType.player)
        {
            return;
        }
        foreach (Clickable c in field.getAdjacentClickables(clickable))
        {
            if (c != this)
            {
                buffEntity(c.entity);
            }
        }
    }

    private void OnDestroy()
    {
        unbuffBuffedEntities();
    }

    private void unbuffBuffedEntities()
    {
        List<Entity> l = buffedEntities.ToList();
        foreach (Entity c in l)
        {
            unbuffEntity(c);
        }
    }
    public void buffTowerAttachToPlayer(Slot slot)
    {
        foreach (Clickable c in playerSlots.getSlottedClickables())
        {
            if (c != this)
            {
                buffEntity(c.entity);
            }
        }
    }

    public void buffTowerDetachFromField()
    {
        unbuffBuffedEntities();
    }

    public void buffTowerDetachFromPlayer()
    {
        List<Clickable> temp = playerSlots.getSlottedClickables();
        foreach (Clickable c in temp)
        {
            if (c.entity != this)
            {
                unbuffEntity(c.entity);
            }
        }
    }

    public override void onOtherEntityDestroy(Entity other)
    {
        if (buffedEntities.Contains(other))
        {
            unbuffEntity(other);
        }
        base.onOtherEntityDestroy(other);
    }

    public void buffTowerOtherAttachToField(Clickable otherClickable)
    {
        if (otherClickable.gameObject.GetInstanceID() != gameObject.GetInstanceID() &&
            clickable.attachedSlot != null && otherClickable.attachedSlot != null &&
            clickable.attachedSlot.slotType != Slot.SlotType.player && 
            field.getAdjacentClickables(otherClickable).Contains(clickable))
        {
            buffEntity(otherClickable.entity);
        }
    }

    public void buffTowerOtherDetachFromField(Clickable otherClickable)
    {
        if (otherClickable.gameObject.GetInstanceID() != gameObject.GetInstanceID() && 
            otherClickable.attachedSlot != null && clickable.attachedSlot != null &&
            clickable.attachedSlot.slotType != Slot.SlotType.player &&
            field.getAdjacentClickables(otherClickable).Contains(clickable))
        {
            unbuffEntity(otherClickable.entity);
        }
    }

    public void buffTowerOtherAttachToPlayer(Clickable otherClickable)
    {
        if (otherClickable.gameObject.GetInstanceID() != gameObject.GetInstanceID() && 
            clickable.attachedSlot != null && otherClickable.attachedSlot != null &&
            otherClickable.attachedSlot.slotType == Slot.SlotType.player && 
            clickable.attachedSlot.slotType == Slot.SlotType.player)
        {
            buffEntity(otherClickable.entity);
        }
    }

    public void buffTowerOtherDetachFromPlayer(Clickable otherClickable)
    {
        if (otherClickable.gameObject.GetInstanceID() != gameObject.GetInstanceID() && 
            otherClickable.attachedSlot != null && clickable.attachedSlot != null &&
            otherClickable.attachedSlot.slotType == Slot.SlotType.player && 
            clickable.attachedSlot != null && clickable.attachedSlot.slotType == Slot.SlotType.player)
        {
            unbuffEntity(otherClickable.entity);
        }
    }

    private void Update()
    {
        /*if (entity.rangeHelper.playerInRange() && attachedSlot != null && attachedSlot.slotType != Slot.SlotType.player)
        {
            /*playerBuffRangeIndicatorObject.transform.localScale = new Vector3(range, range, range) * 2;
            playerBuffRangeIndicatorObject.SetActive(true);
            playerBuffRangeIndicatorObject.transform.position = transform.position;
        }
        else
        {
            playerBuffRangeIndicatorObject.SetActive(false);
        }*/
        /*if (Time.time > buffUpdateCooldownTimer && 
            clickable.attachedSlot != null && 
            clickable.attachedSlot.slotType != Slot.SlotType.inventory)
        {
            buffUpdateCooldownTimer = Time.time + buffUpdateCooldown;
            foreach (Entity e in rangeHelper.entitiesInRange)
            {
                if (!buffedEntities.Contains(e))
                {
                    buffEntity(e);
                }
            }
        }*/
        foreach (Entity e in buffedEntities)
        {
            if (e == null)
            {
                continue;
            }
            buffIndicators[e].SetPosition(0, transform.position);
            buffIndicators[e].SetPosition(1, e.transform.position);
        }
    }
}
