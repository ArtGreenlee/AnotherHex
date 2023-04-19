using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PlayerMovement : Movement
{
    public float moveSpeed;
    public float shootingMoveSpeed;
    public float shootingMoveSpeedDecayRate;
    private float currentMoveSpeed;
    public float boostMagnitude;

    private void FixedUpdate()
    {
        Vector3 Movement = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        rb.AddForce(Movement * currentMoveSpeed);
    }

    // Update is called once per frame
    public override void move()
    {
        if (Input.GetMouseButton(0))
        {
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, shootingMoveSpeed, shootingMoveSpeedDecayRate * Time.deltaTime);
        }
        else
        {
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, moveSpeed, shootingMoveSpeedDecayRate * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(boostMagnitude * new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0), ForceMode2D.Impulse);
        }
    }
}
