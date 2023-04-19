using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Projectile
{
    public float rotateSpeed;
    public List<float> damageRatioAtRadius;
    private Field field;
    public float explosionDelay;
    private bool exploded = false;
    public Color explosionColor;
    Helper helper;
    public float detonationVelocity;

    private void Start()
    {
        helper = Helper.helper;
        field = Field.field;
    }

    private void Update()
    {
        if (rb.velocity.magnitude < detonationVelocity && !exploded)
        {
            StartCoroutine(explode());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!exploded && collision.TryGetComponent<Entity>(out Entity e))
        {
            StartCoroutine(explode());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator explode()
    {
        exploded = true;
        GetComponent<SpriteRenderer>().enabled = false;
        col.enabled = false;
        Slot slot = field.getNearestSlotToPosition(transform.position);
        List<Slot> s_list = new List<Slot>() { slot };
        List<Slot> slotListNext = new List<Slot>();
        HashSet<Slot> visited = new HashSet<Slot>();
        HashSet<Entity> hits = new HashSet<Entity>();
        /*Vector3 diff = slot.transform.position - transform.position;
        if (Vector3.Distance(slot.transform.position, transform.position) > field.componentSpacing / 3)
        {
            s_list.Add()
        }*/
        for (int i = 0; i < damageRatioAtRadius.Count; i++)
        {
            slotListNext.Clear();
            foreach (Slot s in s_list)
            {
                helper.colorSwell(s.spriteRenderer, explosionColor, .1f);
                //helper.tra(s.transform, .1f, .1f);
                foreach (Collider2D col in Physics2D.OverlapCircleAll(s.transform.position, field.componentSpacing / 2)) // TODO: this radius is not right
                {       
                    if (col.TryGetComponent<Enemy>(out Enemy e) && !hits.Contains(e))
                    {
                        hits.Add(e);
                        e.health.applyOnHitEffects(pData);
                        if (!e.health.changeHealth(-pData.damage * damageRatioAtRadius[i]))
                        {
                            e.OnDeath();
                        }
                        
                    }
                }
                if (i != damageRatioAtRadius.Count - 1)
                {
                    foreach (Slot nextSlot in s.adjacentSlots)
                    {
                        if (!visited.Contains(nextSlot))
                        {
                            slotListNext.Add(nextSlot);
                            visited.Add(nextSlot);
                        }
                    }
                }
            }
            s_list.Clear();
            s_list.AddRange(slotListNext);
            yield return new WaitForSeconds(explosionDelay);
        }   
        Destroy(gameObject);
    }
}
