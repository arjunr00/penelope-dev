using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wardrobe : MonoBehaviour
{
    public GameObject player;
    public bool wardrobe_range;
    public Vector3 Pen_Cam_pos;
    public Quaternion Pen_Cam_rot;
    public Vector3 Pen_pos;
    public Quaternion Pen_rot;
    // Start is called before the first frame update

    void Start()
    {
        Pen_Cam_pos = transform.position + 0.2f * Vector3.up;
        Pen_Cam_rot = Quaternion.Euler(0f, -90f, 0f);

        Pen_pos = transform.position - 1.0f * Vector3.up - 0.1f * Vector3.forward + 0.1f * Vector3.left;
        Pen_rot = Quaternion.Euler(0f, -90f, 0f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            wardrobe_range = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject == player)
        {
            wardrobe_range = false;
        }
    }
}
