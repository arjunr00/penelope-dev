using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private float lookSpeed;
    private Transform HeadTransform;
    private GameObject m_Player;
    private GameObject m_PenCamPos;
    private GameObject m_Hips;
    private GameObject m_Wardrobe;
    //private Vector3 peek_cam_pos_vel;
    private float peek_cam_pos_movement_time;
    private float peek_cam_rot_speed;
    public bool peeking_in_progress;
    public bool peeking;
    public bool unpeeking_in_progress;

    //bool table_rotation_done;


    void Start()
    {
        m_Player = GameObject.FindWithTag("Player");
        m_PenCamPos = GameObject.Find("PenCamPos");
        m_Hips = GameObject.Find("Hips");
        m_Wardrobe = GameObject.FindWithTag("Interact_Wardrobe");
        peek_cam_pos_movement_time = 2.0f;
        peek_cam_rot_speed = 100f;
        peeking_in_progress = false;
        peeking = false;
        unpeeking_in_progress = false;
        //table_rotation_done = false;
        lookSpeed = Time.deltaTime * 2.0f;
    }

    void Update()
    {
        GameObject head = GameObject.Find("PenCamPos");
        HeadTransform = head.transform;
        peeking = m_Player.GetComponent<NewPenelopeMovement>().peeking;
        if(!peeking && !peeking_in_progress && !unpeeking_in_progress)
        {

            Vector3 rot = transform.rotation.eulerAngles;
            rot.y = HeadTransform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Euler(rot);
            // TODO - REMOVE
            Vector3 temp = HeadTransform.position;
            temp.x += 3;
            //
            transform.position = temp;
        }

        if (m_Player.GetComponent<NewPenelopeMovement>().hide_wardrobe)
        {
            if(!peeking && !peeking_in_progress && !unpeeking_in_progress)
            {
                transform.position = m_Player.transform.position + new Vector3(0f, 1.4f, 0.09f); 
            }

        }
        /*
        else if(m_Player.GetComponent<PenelopeMovement>().hide_table)
        {

            Vector3 pos = transform.position;
            pos.y = m_Hips.transform.position.y;
            transform.position = pos;

            if (!table_rotation_done)
            {
                Vector3 rot_under_table = HeadTransform.rotation.eulerAngles;
                rot_under_table.x = rot_under_table.x + 270;
                if (rot_under_table.x > 360)
                {
                    rot_under_table.x = rot_under_table.x - 360;
                }
                table_rotation_done = true;
                transform.rotation = Quaternion.Euler(rot_under_table);

            }
        }
        */
    }

    public bool Peek_Camera()
    {
        peeking_in_progress = true;

        Vector3 wardrobe_pos = m_Wardrobe.GetComponent<Wardrobe>().Pen_Cam_pos;
        Quaternion wardrobe_rot = m_Wardrobe.GetComponent<Wardrobe>().Pen_Cam_rot;

        Vector3 target_position = new Vector3(wardrobe_pos.x - 0.2f, wardrobe_pos.y, wardrobe_pos.z); //final peek position
        Quaternion target_rotation = Quaternion.Euler(wardrobe_rot.eulerAngles.x + 10.0f, wardrobe_rot.eulerAngles.y - 75.0f, wardrobe_rot.eulerAngles.z + 10.0f); //final target rotation


        transform.position = Vector3.MoveTowards(transform.position, target_position, peek_cam_pos_movement_time * lookSpeed); //slowly move towards target position
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target_rotation, peek_cam_rot_speed * lookSpeed); //slowly rotate towards target rotation

        if(transform.position == target_position && transform.rotation == target_rotation) //check if you have reached final peeking position and rotation
        {
            peeking_in_progress = false;
        }

        return peeking_in_progress;
    }

    public bool UnPeek_Camera()
    {
        unpeeking_in_progress = true;

        Vector3 wardrobe_pos = m_Wardrobe.GetComponent<Wardrobe>().Pen_Cam_pos;
        Quaternion wardrobe_rot = m_Wardrobe.GetComponent<Wardrobe>().Pen_Cam_rot;

        Vector3 target_position = new Vector3(wardrobe_pos.x, wardrobe_pos.y, wardrobe_pos.z);
        Quaternion target_rotation = Quaternion.Euler(wardrobe_rot.eulerAngles.x, wardrobe_rot.eulerAngles.y, wardrobe_rot.eulerAngles.z);


        transform.position = Vector3.MoveTowards(transform.position, target_position, peek_cam_pos_movement_time * lookSpeed);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target_rotation, peek_cam_rot_speed * lookSpeed);

        if (transform.position == target_position && transform.rotation == target_rotation)
        {
            unpeeking_in_progress = false;
        }

        return unpeeking_in_progress;
    }
}
