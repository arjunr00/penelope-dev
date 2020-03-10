using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public bool isOpen = true;

    public void ToggleDoor()
    {
        print("toggling door");
        if (isOpen)
        {
            transform.Rotate(0, 90, 0);
        } else
        {
            transform.Rotate(0, -90, 0);
        }
        isOpen = !isOpen;
    }
}
