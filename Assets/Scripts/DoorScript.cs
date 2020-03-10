using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public bool isOpen = true;
    public float rotSpeed = 1.0f;
    Quaternion startRot, endRot;

    private void Start()
    {
        startRot = Quaternion.LookRotation(transform.forward);
        endRot = Quaternion.LookRotation(transform.right);
    }

    private void Update()
    {
        if (transform.rotation != endRot)
        {
            transform.rotation = Quaternion.Slerp(startRot, endRot, Time.time * rotSpeed);
        }
    }

    public void ToggleDoor()
    {
        print("toggling door");
        if (isOpen)
        {
            startRot = Quaternion.LookRotation(transform.forward);
            endRot = Quaternion.LookRotation(transform.right);
        } else
        {
            endRot = Quaternion.LookRotation(transform.forward);
            startRot = Quaternion.LookRotation(transform.right);
        }
        isOpen = !isOpen;
    }
}
