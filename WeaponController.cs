using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WeaponController : MonoBehaviour
{
    private Clickable clickable;
    public Weapon weapon;
    public Transform target;    
    public bool isEnemy;
    public float rangeBuffer = 3;
    private Rigidbody2D targetRb;
    public bool interceptTarget;
    private Gun gun;
    private void Awake()
    {
        gun = GetComponent<Gun>();
        clickable = GetComponent<Clickable>();
        if (clickable != null)
        {
            clickable.onDetachFromFieldActions.Add(onWeaponControllerDetachFromFieldAction);
        }
    }

    private void Start()
    {
        if (weapon.rangeHelper != null)
        {
            
            if (isEnemy)
            {
                weapon.rangeHelper.setRange(weapon.entity.getRange());
            }
            weapon.rangeHelper.onNewNearestActions.Add(newNearestTransformAction);
        }
        else
        {
            Debug.LogError("weapon controller with no range helper");
        }
        
    }

    public void newNearestTransformAction(Transform newNearest)
    {
        //Debug.Log(gameObject.name + " Recieved new nearest transform: "); 
        //Debug.Log(newNearest.gameObject.name);
        target = newNearest;
        if (target != null && interceptTarget)
        {
            targetRb = target.GetComponent<Rigidbody2D>();
        }
    }

    public void onWeaponControllerDetachFromFieldAction()
    {
        target = null;
    }

    private void Update()
    {
        if (target != null && (isEnemy || (clickable.attachedSlot != null &&
            clickable.attachedSlot.slotType == Slot.SlotType.field)))
        {
            if (targetRb != null && interceptTarget)
            {
                Vector3 interceptPos = Helper.CalculateInterceptCourse(target.position, targetRb.velocity, transform.position, gun.bulletSpeed);
                if (interceptPos == Vector3.zero)
                {
                    weapon.rotateWeaponTransform(target.position - transform.position);
                }
                else
                {
                    weapon.rotateWeaponTransform(interceptPos - transform.position);
                }
            }
            else
            {
                weapon.rotateWeaponTransform(target.position - transform.position);
            }
            
            if (Time.time > weapon.shootTimer)
            {
                weapon.shootTimer = Time.time + weapon.entity.getSpeed();
                if (isEnemy)
                {
                    weapon.shoot(LayerMask.NameToLayer("EnemyProjectile"));
                }
                else
                {

                    weapon.shoot(LayerMask.NameToLayer("PlayerProjectile"));
                }
            }
        }
    }
}
