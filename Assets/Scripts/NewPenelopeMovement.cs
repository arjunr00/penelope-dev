using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPenelopeMovement : MonoBehaviour
{
    private Camera main_cam;
    private Animator m_Animator;
    private Rigidbody m_Rigidbody;
    private GameObject m_Wardrobe;
    private bool walkingToHopping = false;
    public AnimationClip hopping;
    private int State;
    public bool walking;
    public bool running;
    public bool hide_wardrobe;
    public bool rotated_wardrobe;
    public bool peeking_in_progress;
    public bool peeking;
    public bool unpeeking_in_progress;
    public bool isCrouching;
    public bool isCrouchMoving;
    public bool isProne;
    public bool isProneMoving;
    public bool leftLegGone;
    public bool rightLegGone;
    public bool isStanding = true;
    public float speed;

    public float cPressTimeRequired = 0.5f;
    public float cPressTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        main_cam = Camera.main;
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Wardrobe = GameObject.FindWithTag("Interact_Wardrobe");
        State = 0;
        walking = false;
        running = false;
        hide_wardrobe = false;
        rotated_wardrobe = false;
        peeking_in_progress = false;
        peeking = false;
        unpeeking_in_progress = false;
        isCrouching = false;
        isCrouchMoving = false;
        isProne = false;
        isProneMoving = false;
        isStanding = true;
        leftLegGone = false;
        rightLegGone = false;

        speed = 1.2f;
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.L)) //Pop a leg off!
        {
            //USE RuntimeAnimatorController TO DO THIS
            leftLegGone = !leftLegGone;
            m_Animator.SetBool("Left Leg Gone", leftLegGone);
        }

        if((leftLegGone || rightLegGone) && (!walkingToHopping))
        {
            AnimatorOverrideController aoc = new AnimatorOverrideController(m_Animator.runtimeAnimatorController);
            m_Animator.runtimeAnimatorController = aoc;
            aoc["Walking (1)"] = aoc["Jumping Rope"];
            aoc["Running (1)"] = aoc["Jumping Rope"];
            Debug.Log("Walking/Running switched to hopping");
            m_Animator.Play("Walking");
            walkingToHopping = true;
        }

        if (m_Wardrobe.GetComponent<Wardrobe>().wardrobe_range)
        {
            Transform wdTx = m_Wardrobe.transform;
            peeking_in_progress = main_cam.GetComponent<CameraMovement>().peeking_in_progress;
            unpeeking_in_progress = main_cam.GetComponent<CameraMovement>().unpeeking_in_progress;

            if (Input.GetKey(KeyCode.I)) //if I is pressed, enter the wardrobe
            {
                walking = false;
                running = false;
                isCrouching = false;
                isCrouchMoving = false;
                isProne = false;
                isProneMoving = false;
                isStanding = true;
                hide_wardrobe = true;
                m_Animator.SetBool("IsCrouching", isCrouching);
                m_Animator.SetBool("IsProne", isProne);
                m_Animator.SetBool("IsStanding", isStanding);

                transform.position = new Vector3(wdTx.position.x-0.19f, transform.position.y, wdTx.position.z); //change position s.t. Penelope is standing in front of the wardrobe
                transform.rotation = Quaternion.Euler(0f, -90f, 0f); // change rotation to align Penelope with the wardrobe

                m_Animator.SetBool("HideWardrobe", hide_wardrobe);

            }
            if (hide_wardrobe && m_Animator.GetCurrentAnimatorStateInfo(0).IsTag("inside_wardrobe")) //if Penelope is currently inside the wardrobe
            {
                if (!rotated_wardrobe)
                {
                    transform.position = m_Wardrobe.GetComponent<Wardrobe>().Pen_pos;
                    transform.rotation = m_Wardrobe.GetComponent<Wardrobe>().Pen_rot;
                    rotated_wardrobe = true;
                }

                if(!peeking && !peeking_in_progress && !unpeeking_in_progress && Input.GetKey(KeyCode.P)) // if no peeking or unpeeking is going on and P is pressed
                {
                    main_cam.GetComponent<CameraMovement>().Peek_Camera(); //start peeking process
                }

                if (peeking_in_progress) //if peeking is currently in progress but hasn't completed
                {
                    if(!main_cam.GetComponent<CameraMovement>().Peek_Camera()) //continue the peeking process but check if it gets over
                    {
                        peeking_in_progress = false; // when it gets over set this to false and set peeking to true (stationary peeking starts)
                        peeking = true;
                    }
                }
                if (peeking && !peeking_in_progress && !unpeeking_in_progress && Input.GetKey(KeyCode.U)) //if stationary peeking, but peeking is not in progress and U is pressed, start unpeeking
                {
                    peeking = false;
                    main_cam.GetComponent<CameraMovement>().UnPeek_Camera(); //start unpeeking process
                }

                if(unpeeking_in_progress)
                {
                    if(!main_cam.GetComponent<CameraMovement>().UnPeek_Camera()) //check to see if unpeeking is completed
                    {
                        unpeeking_in_progress = false;
                    }
                }
            }
            if (!peeking_in_progress && !unpeeking_in_progress && hide_wardrobe && Input.GetKey(KeyCode.O))
            {
                if(peeking) //O is pressed while peeking
                {
                    main_cam.transform.position = m_Wardrobe.GetComponent<Wardrobe>().Pen_Cam_pos;
                    main_cam.transform.rotation = m_Wardrobe.GetComponent<Wardrobe>().Pen_Cam_rot;

                    transform.position = m_Wardrobe.GetComponent<Wardrobe>().Pen_pos;
                    transform.rotation = m_Wardrobe.GetComponent<Wardrobe>().Pen_rot;

                    peeking = false;
                }
                rotated_wardrobe = false;
                hide_wardrobe = false;
                m_Animator.SetBool("HideWardrobe", hide_wardrobe);
            }
        }

        if(!hide_wardrobe)
        {

            if(Input.GetKey(KeyCode.C)) // update cPressTime to see how long C has been pressed
            {
                cPressTime += Time.deltaTime;
            }
            if(Input.GetKeyUp(KeyCode.C) && cPressTime != 0) //if C has stopped being pressed
            {
                if(cPressTime < cPressTimeRequired) //if cPressTime is less than required, crouch/uncrouch
                {
                    isCrouching = !isCrouching;
                    if(!isCrouching && isCrouchMoving)
                    {
                        isCrouchMoving = false;
                    }

                    isProne = false;
                    isProneMoving = false;
                }
                else //prone/unprone
                {
                    isProne = !isProne;
                    if(!isProne && isProneMoving)
                    {
                        isProneMoving = false;
                    }

                    isCrouching = false;
                    isCrouchMoving = false;
                }
                cPressTime = 0.0f;
            }

           

            if(!isCrouching && !isProne)
            {
                isStanding = true;
            }
            else
            {
                isStanding = false; 
            }

            m_Animator.SetBool("IsCrouching", isCrouching);
            m_Animator.SetBool("IsProne", isProne);
            m_Animator.SetBool("IsStanding", isStanding);

            Movement();
            LookRotation();
        }
        else
        {
            if(!peeking && !peeking_in_progress && !unpeeking_in_progress)
            {
                LookRotationInWardrobe(); //Penelope can only look up or down
            }
        }


    }

    void LookRotation()
    {
        float yRot = Input.GetAxis("Mouse X") * 2f;
        float xRot = Input.GetAxis("Mouse Y") * 2f;

        transform.rotation *= Quaternion.Euler(0f, yRot, 0f);

        Vector3 target = main_cam.transform.eulerAngles;
        target.x += -xRot;
        target.y += yRot;
        target.z = 0f;
        main_cam.transform.eulerAngles = target;
    }

    void LookRotationInWardrobe()
    {
        if(!main_cam.GetComponent<CameraMovement>().peeking)
        {
            float xRot = Input.GetAxis("Mouse Y") * 2f;

            transform.rotation *= Quaternion.Euler(0f, 0f, 0f);

            Vector3 target = main_cam.transform.eulerAngles;
            target.x += -xRot;
            target.y += 0f;
            target.z = 0f;
            main_cam.transform.eulerAngles = target;
        }

    }

    void Movement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);

        if(hasHorizontalInput || hasVerticalInput)
        {
            if (Input.GetKey(KeyCode.Space) && !isCrouching && !isProne) //running
            {
                speed = 4.2f;
                State = 2;
                walking = false;
                running = true;
                isCrouching = false;
                isStanding = false;

            }
            else if(isCrouching)
            {
                speed = 0.6f;
                walking = false;
                running = false;
                isProne = false;
                isStanding = false;
                isCrouchMoving = true;

            }
            else if(isProne)
            {
                speed = 0.3f;
                walking = false;
                running = false;
                isCrouching = false;
                isStanding = false;
                isProneMoving = true;
            }
            else //regular walking
            {
                speed = 1.2f;
                State = 1;
                walking = true;
                running = false;
                isCrouching = false;
                isProne = false;
                isStanding = false;
            }


            bool Up = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
            bool Down = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
            bool Right = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
            bool Left = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);

            Vector3 velocityVec = Vector3.zero;

            if (Up)
            {
                velocityVec += transform.forward * speed;
            }
            if (Down)
            {

                velocityVec += -transform.forward * speed;
            }
            if (Left)
            {
                velocityVec += -transform.right * speed;
            }
            if (Right)
            {
                velocityVec += transform.right * speed;
            }

            m_Rigidbody.velocity = velocityVec;

        }
        else
        {
            walking = false;
            running = false;
            isStanding = true;
            isCrouchMoving = false;
            isProneMoving = false;
            State = 0;
        }
        m_Animator.SetInteger("State", State);

    }
}
