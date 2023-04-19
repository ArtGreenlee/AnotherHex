using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemInteractionController : MonoBehaviour
{
    public static float placementRange = 1;
    public static PlayerItemInteractionController playerItemInteractionController;
    public Clickable clickedObject;
    public GameObject cursorObject;

    private void Start()
    {
        cursorObject = Instantiate(cursorObject, transform.position, Quaternion.identity);   
    }
    private void Update()
    {
        cursorObject.transform.position = Helper.mousePosition;
        if (Input.GetMouseButtonDown(1))
        {
            Collider2D[] hit = Physics2D.OverlapCircleAll(Helper.mousePosition, 1);

            float dist = float.MaxValue;
            foreach (Collider2D col in hit)
            {
                float temp = (col.transform.position - Helper.mousePosition).sqrMagnitude;
                if (col.gameObject.TryGetComponent<Clickable>(out Clickable clickable) &&
                    temp < dist)
                {
                    dist = temp;
                    clickedObject = clickable;
                }
            }
            
            if (clickedObject != null)
            {
                clickedObject.OnClickDown();
            }
        }


        if (clickedObject != null) {
            if (Input.GetMouseButton(1))
            {
                clickedObject.WhileClicked();
                clickedObject.transform.position = Helper.mousePosition;
            }
            if (Input.GetMouseButtonUp(1))
            {
               
                clickedObject.OnClickUp();
                clickedObject = null;
            }
        }
    }
}
