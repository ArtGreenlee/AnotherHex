using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float movespeedScaler;
    private bool movementEnabled = true;
    protected Rigidbody2D rb;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void disableMovement()
    {
        rb.velocity = Vector3.zero;
        movementEnabled = false;
    }

    public void enableMovement()
    {   
        movementEnabled = true;
    }

    public virtual void movementOnCollision()
    {
        rb.velocity = -rb.velocity;
    }

    public virtual void move()
    {
    }

    private void Update()
    {
        if (movementEnabled)
        {
            move();
        }
    }
}
