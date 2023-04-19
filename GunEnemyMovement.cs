using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunEnemyMovement : MonoBehaviour
{
    public float speed;
    private Rigidbody2D rb;
    private Transform playerTransform;
    private WeaponController weaponController;
    public float moveBackThreshold;
    private float moveBackThresholdSqrd;
    private void Start()
    {
        moveBackThresholdSqrd = moveBackThreshold * moveBackThreshold;
        playerTransform = Player.player.transform;
        rb = GetComponent<Rigidbody2D>();
        weaponController = GetComponent<WeaponController>();
    }

    private void Update()
    {
        if (weaponController.target == null)
        {
            rb.velocity = (playerTransform.position - transform.position).normalized * speed;
        }
        else if ((playerTransform.position - transform.position).sqrMagnitude < moveBackThresholdSqrd)
        {
            rb.velocity = (transform.position - playerTransform.position).normalized * speed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

}
