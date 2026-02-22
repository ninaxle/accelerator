using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; //car transform
    private Vector3 offset;
    private Quaternion rotationOffset;

    private void Start()
    {
        if (target != null)
        {
            offset = transform.position - target.position;
            rotationOffset = Quaternion.Inverse(target.rotation) * transform.rotation;
        }
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + target.rotation * offset;
            transform.rotation = target.rotation * rotationOffset;
        }
    }
}
