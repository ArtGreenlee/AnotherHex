using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;
    public List<Clickable> inventory = new List<Clickable>();

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        foreach (Clickable c in GetComponentsInChildren<Clickable>())
        {
            inventory.Add(c);
        }
    }
}
