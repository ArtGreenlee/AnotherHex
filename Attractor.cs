using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractor : MonoBehaviour
{
    public float attractionMagnitude;
    private float collisionRangeSqrd;
    private void Start()
    {
        collisionRangeSqrd = .5f;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Attractable"))
        {
            Attractable a = collision.gameObject.GetComponent<Attractable>();
            if (!a.beingAttracted)
            {
                StartCoroutine(attractToPlayer(a));
            }
        }
    }

    public IEnumerator attractToPlayer(Attractable attractable)
    {
        attractable.collider.enabled = false;
        attractable.beingAttracted = true;
        Rigidbody2D rb = attractable.rb;
        Transform attractableTransform = attractable.transform;
        Vector3 diff;
        do
        {
            diff = transform.position - attractableTransform.position;
            rb.AddForce(diff.normalized * attractionMagnitude / 2);//diff.normalized * Player.player.GetComponent<Rigidbody2D>().velocity.magnitude / 4);
            yield return new WaitForEndOfFrame();
        } while (diff.sqrMagnitude > collisionRangeSqrd);
        attractable.attractableEffect(this);
        Destroy(attractable.gameObject);
    }
}
