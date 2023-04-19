using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
    private List<Weapon> weapons = new List<Weapon>();
    private HashSet<Transform> targets = new HashSet<Transform>();
    private Dictionary<Weapon, Transform> weaponTargetDict = new Dictionary<Weapon, Transform>();
    public static PlayerWeapons playerWeapons;
    int projectileLayer;
    private bool autoshootEnabled = false;
    private void Awake()
    {
        projectileLayer = LayerMask.NameToLayer("PlayerProjectile");
        playerWeapons = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (Weapon weapon in GetComponentsInChildren<Weapon>()) {
            weapons.Add(weapon);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            autoshootEnabled = !autoshootEnabled;
        }
        if (Input.GetMouseButton(0))
        {
            foreach (Weapon weapon in weapons)
            {
                weapon.rotateWeaponTransform(Helper.mousePosition - weapon.transform.position);
            }
            foreach (Weapon weapon in weapons)
            {
                if (Time.time > weapon.shootTimer)
                {
                    weapon.shootTimer = Time.time + weapon.entity.getSpeed();
                    weapon.shoot(projectileLayer);
                }
            }
        }
        else if (autoshootEnabled)
        {
            foreach (Weapon w in weapons)
            {
                Transform target = w.rangeHelper.getNearestTransform();     
                if (target != null)
                {
                    w.rotateWeaponTransform(target.position - w.transform.position);
                    if (Time.time > w.shootTimer)
                    {
                        w.shootTimer = Time.time + w.entity.getSpeed();
                        w.shoot(projectileLayer); ;
                    }
                }
            }
        }
            /*foreach (Weapon w in weapons)
            {
                Transform nearest = w.rangeHelper.getNearestTransform();
                if (targets.Contains(nearest) && weaponTargetDict[w] != nearest)
                {
                    //find next closest target that is not in the target set
                }
                else
                {
                    targets.Add(nearest);
                    w.rotateWeaponTransform(nearest.position - w.transform.position);
                    weaponTargetDict[w]
                }
            }

            foreach (Weapon w in weapons)
            {
                if (Time.time > w.shootTimer && weaponTargetDict.ContainsKey(w))
                {
                    w.shootTimer = Time.time + w.entity.getSpeed();
                    Projectile p = w.shoot();
                    p.gameObject.layer = LayerMask.n
                }
            }*/
        
    }

    // Update is called once per frame
    /*void Update()
    {
        foreach (Weapon weapon in weapons) 
        {
            weapon.rotateWeaponTransform(Helper.mousePosition - transform.position);
        }

        
    }*/

    public void addWeapon(Weapon weapon)
    {
        weapons.Add(weapon);
    }
    
    public void removeWeapon(Weapon weapon)
    {
        weapons.Remove(weapon);
    }
}
