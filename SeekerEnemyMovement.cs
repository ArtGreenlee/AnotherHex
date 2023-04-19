using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerEnemyMovement : Movement
{
    private Transform playerTransform;

    private void Start()
    {
        playerTransform = Player.player.transform;
    }

    public override void move()
    {
        rb.AddForce((playerTransform.position - transform.position).normalized * movespeedScaler);
    }
}
