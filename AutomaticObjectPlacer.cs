using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticObjectPlacer : MonoBehaviour
{
    public GameObject boxObject;
    private bool aopActive = false;
    private Vector3 startPos;
    public LayerMask pickupMask;
    Field field;
    private void Update()
    {
        if (aopActive)
        {
            boxObject.transform.position = Helper.mousePosition;
            boxObject.transform.localScale = (Helper.mousePosition - startPos) * 2;
        }
    }
    private void Start()
    {
        field = Field.field;
        boxObject = Instantiate(boxObject);
        boxObject.SetActive(false);
    }
    public void aopStart(Vector3 pos)
    {
        startPos = pos;
        boxObject.SetActive(true);
        boxObject.transform.position = pos;
        aopActive = true;
        boxObject.transform.localScale = Vector3.zero;
    }

    public void aopEnd()
    {
        if (!aopActive)
        {
            return;
        }
        Collider2D[] temp = Physics2D.OverlapBoxAll(boxObject.transform.position, boxObject.transform.localScale, 0);
        List<BuffTower> buffTowers = new List<BuffTower>();
        List<HealingTower> healingTowers = new List<HealingTower>();
        List<Weapon> weapons = new List<Weapon>();

        foreach (Collider2D col in temp)
        {
            if (col.TryGetComponent<BuffTower>(out BuffTower b))
            {
                buffTowers.Add(b);
            }
            else if (col.TryGetComponent<HealingTower>(out HealingTower h))
            {
                healingTowers.Add(h);
            }
            else if (col.TryGetComponent<Weapon>(out Weapon we))
            {
                weapons.Add(we);
            }
        }
        
        int count = buffTowers.Count + healingTowers.Count + weapons.Count;

        //search for empty space that can fit all these towers;
        //establish a shape and a sliding window
        List<Slot> slots = new List<Slot>();
        Stack<Slot> s = new Stack<Slot>();
        s.Push(field.getFieldComponentAtPoint(boxObject.transform.position));
        while (!s.TryPeek(out Slot curSlot))
        {
            if (curSlot.slottedClickable == null)
            {
                slots.Clear();
                slots.Add(curSlot);
                //search the window;
                int emptyCount = 1;
                Stack<Slot> search = new Stack<Slot>();
                search.Push(curSlot);
                while (search.TryPeek(out Slot test))
                {
                 
                    if (test.slottedClickable == null)
                    {
                        emptyCount++;   
                        slots.Add(curSlot);
                    }
                    foreach (Slot adj in test.adjacentSlots)
                    {
                        search.Push(adj);
                    }
                }
            }
            foreach (Slot adj in curSlot.adjacentSlots)
            {
                s.Push(adj);
            }
        }

        boxObject.SetActive(false);
        aopActive = false; 
    }

}
