using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntityManager : MonoBehaviour
{
    public static PlayerEntityManager instance;
    public HashSet<Entity> entityMasterList = new HashSet<Entity>();

    private void Awake()
    {
        instance = this;
    }
}
