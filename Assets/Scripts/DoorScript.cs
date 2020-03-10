using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public bool isOpen = true;
    public bool isToggling = false;
    public float rotSpeed = 250f;

    private void Update()
    {
        float eulerAngY = transform.localEulerAngles.y;
        print(eulerAngY);
        if (isToggling && isOpen)
        {
            // Close
            if (eulerAngY <= 180)
            {
                transform.Rotate(new Vector3(0f, rotSpeed, 0f) * Time.deltaTime);
            } else
            {
                isToggling = false;
                isOpen = false;
            }
        } else if (isToggling && !isOpen)
        {
            // Open
            if (eulerAngY >= 90)
            {
                transform.Rotate(new Vector3(0f, -1*rotSpeed, 0f) * Time.deltaTime);
            } else
            {
                isToggling = false;
                isOpen = true;
            }
        }
    }

    public void ToggleDoor()
    {
        isToggling = true;
    }
}
