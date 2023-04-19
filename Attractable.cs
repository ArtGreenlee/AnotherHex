using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractable : MonoBehaviour
{
    public CircleCollider2D collider;
    public Rigidbody2D rb;
    public bool beingAttracted = false;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<CircleCollider2D>();
    }

    public virtual void attractableEffect(Attractor attractor) { }
}
