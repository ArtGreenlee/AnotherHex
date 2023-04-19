using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    Transform shieldTransform;
    public float shieldOffset;
    public float shieldSpeed;
    private Vector3 rotateDirection;
    public bool isEnemy;
    private Clickable clickable;
    private Transform playerTransform;

    void Start()
    {
        playerTransform = Player.player.transform;
        clickable = GetComponent<Clickable>();
        shieldTransform = GetComponentInChildren<Shield>().transform;
    }

    private void FixedUpdate()
    {
        if (isEnemy)
        {
            rotateDirection = playerTransform.position;
        }
        else if (clickable.attachedSlot != null && clickable.attachedSlot.slotType == Slot.SlotType.player)
        {
            rotateDirection = Helper.mousePosition;
        }
        else if (clickable.attachedSlot != null && clickable.attachedSlot.slotType == Slot.SlotType.field)
        {
            Debug.Log("IMPLEMENT BULLET SCANNING");
            rotateDirection = Vector3.zero;
        }
        else if (clickable.attachedSlot == null || (clickable.attachedSlot != null && clickable.attachedSlot.slotType == Slot.SlotType.inventory))
        {
            //deactivate shield
            rotateDirection = Vector3.zero;
        }
        Vector3 playerDirection = (rotateDirection - transform.position).normalized;
        shieldTransform.localPosition = Vector3.Lerp(shieldTransform.localPosition, playerDirection * shieldOffset, Time.fixedDeltaTime * shieldSpeed);
        Vector3 shieldRotation = shieldTransform.rotation.eulerAngles;
        shieldRotation.z = Mathf.Rad2Deg * Mathf.Atan2(shieldTransform.localPosition.y, shieldTransform.localPosition.x) - 90;
        shieldTransform.eulerAngles = shieldRotation;
    }
}
