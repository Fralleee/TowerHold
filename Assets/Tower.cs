using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public static Health asTarget;

    void Awake()
    {
        asTarget = GetComponent<Health>();
    }

}
