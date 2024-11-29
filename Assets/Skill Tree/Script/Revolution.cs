using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revolution : MonoBehaviour
{
    [SerializeField] float Speed;
    
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, Time.time * Speed);
    }
}
