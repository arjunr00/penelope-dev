/* Dog Movements
 * 1) General wandering around Penelope (done) 
 * 2) Follow Penelope
 * 3) Run off and sniff/bark at something
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderAI : MonoBehaviour
{
    public GameObject wander_target;
    public float wander_time; // how much time of wandering in the same direction
    public float wander_radius; // how far the dog should wander
    public float wander_speed; // speed with which the dog will wander
    public float wander_space; // minimum space between Penelope and the dog
    public float max_angle_change; // maximum angle change when wandering in the same direction
    public float min_angle_change; // minimum angle change when wandering in the opposite direction
    public float max_mvmt; // maximum distance per new destination
    public float min_mvmt; // minium distance per new destination
    public float stray_radius; // range within which if there is a suspicious object, dog should stray and sniff/bark at object
    public float stray_speed; // speed with which 
    public float stray_proximity; // how close the dog should stray to the object
    private float timer;
    private Vector3 dest;
    private bool wait;
    private bool stray;
    private bool walk_to_suspicious;
    private bool sniffing_and_barking;
    private bool walk_from_suspicious;
    private float follow_speed;
    private Animator m_Animator;
    private GameObject SuspiciousObjectInRange;
    void Start()
    {
        timer = wander_time;
        dest = wander_target.transform.position;
        wait = false;
        stray = false;
        walk_to_suspicious = false;
        sniffing_and_barking = false;
        walk_from_suspicious = false;
        follow_speed = 0f;
        m_Animator = GetComponent<Animator>();

    }

    void Update()
    {
        //****** General wandering around Penelope *********//
        /*
         * Short straight line paths from one random point to another 
         * (within a certain angle range - enough so that the dog isn't just moving in one direction).
         * Sometimes (random) after it has reached a point, it will wait (idle)
         * If it decides to wait, you can choose the next point such 
         * that it is more than 90 degrees away from the first point
         * 
        */
        if(wander_target.GetComponent<NewPenelopeMovement>().isStanding || (wander_target.GetComponent<NewPenelopeMovement>().isCrouching && !wander_target.GetComponent<NewPenelopeMovement>().isCrouchMoving) || (wander_target.GetComponent<NewPenelopeMovement>().isProne && !wander_target.GetComponent<NewPenelopeMovement>().isProneMoving)) // if Penelope is not moving
        {
            if(!wait) //not waiting/rotating
            {
                if(!stray) //not straying and barking at a suspicious object
                {
                    StartStray();
                    timer += Time.deltaTime;
                    if ((timer > wander_time) || (Vector3.Distance(transform.position, dest) < 0.01f)) //check if wander_time is up or if dog has reached its destination
                    {
                        if (Random.value < 0.25f) //randomly wait before moving
                        {
                            dest = NextDestination(false); //get next random destination in the opposite-ish direction
                            wait = true;
                            float wait_time = Random.Range(5, 20); //how long the dog should be idle before moving again
                            Invoke("SetWait", wait_time); //change wait back to false after a random number seconds
                            m_Animator.SetBool("Rotating", false);
                            m_Animator.SetBool("Walking", false);
                            m_Animator.SetBool("Idle", true);
                            Invoke("StartRotating", wait_time / 5); //start rotating after some time of being idle
                            Invoke("StopRotating", 4 * wait_time / 5); //stop rotating and become idle again
                        }
                        else
                        {
                            dest = NextDestination(true); //get next random destination somewhere in the forward direction
                            Rotate(dest);
                        }
                        timer = 0;

                    }
                    transform.position = Vector3.MoveTowards(transform.position, dest, wander_speed * Time.deltaTime); //move towards dest
                }
                else
                {
                    ContinueStray();
                }
            }
        }
        //**************************************************//
    }


    void StartStray()
    {
        SuspiciousObjectInRange = GetSuspiciousObjectInRange();
        if(SuspiciousObjectInRange != null) //start straying
        {
            stray = true;
            m_Animator.SetBool("Walking", true);
            m_Animator.SetBool("Idle", false);
            walk_to_suspicious = true;
        }
        else
        {
            stray = false;
        }
    }

    void ContinueStray()
    {
        if(walk_to_suspicious)
        {
            Vector3 target_position = SuspiciousObjectInRange.transform.position;
            target_position.y = 0.2f;
            transform.position = Vector3.MoveTowards(transform.position, target_position, stray_speed * Time.deltaTime); //move towards suspicious object
            if(Vector3.Distance(transform.position, target_position) < stray_proximity) // if the dog is close enough to the object
            {
                walk_to_suspicious = false;
                sniffing_and_barking = true; // start sniffing and barking animation
                m_Animator.SetBool("Walking", false);
                m_Animator.SetBool("Sniffing", true); 
            }
        }
        if(sniffing_and_barking)
        {
            sniffing_and_barking = false;
            Invoke("SetWalkingFromSus", 10); // After 10 seconds start walking back to Penelope
            //TODO - figure out a better way to do this (using the commented code below)

            /* idk why this doesn't work lmao
            Debug.Log(m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Walking"));
            if(m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Walking"))
            {
                Debug.Log("Walking now");
                sniffing_and_barking = false;
                walk_from_suspicious = true;
            }
            */ 
        }
        if(walk_from_suspicious)
        {
            walk_from_suspicious = false;
            EndStray(); 
        }
    }

    void EndStray()
    {
        stray = false;
        SuspiciousObjectInRange.GetComponent<Wardrobe>().RemoveSuspicious(); // remove suspicion from the oject
        timer = wander_time;
        //TODO - figure out when to make the objects suspicious and when to reset them as suspicious 
        //after the dog has strayed 
    }

    GameObject GetSuspiciousObjectInRange()
    {
        Collider[] CollidersInRange = Physics.OverlapSphere(transform.position, stray_radius); // get all the colliders in the range
        for(int i = 0; i < CollidersInRange.Length; i++)
        {
            foreach (Transform child in CollidersInRange[i].gameObject.transform) // get the child transform of the objects
            {
                if (child.gameObject.tag == "Suspicious") // check if the child object is suspicious
                {
                    return CollidersInRange[i].gameObject; // return suspicious object
                }
            }
        }
        return null;
    }


    Vector3 NextDestination(bool forward_movement) //find next random destination for general wandering 
    {
        Vector3 new_dest = Random.insideUnitSphere * wander_radius + wander_target.transform.position; //choose a random point in Penelope's range
        new_dest.y = wander_target.transform.position.y + 0.2f; // 0.2f is just to make sure the sphere isn't partially under the ground
        Vector3 new_dest_direction = (new_dest - transform.position).normalized; //get the new direction of movement
        int check_stuck = 0;
        if(forward_movement) //find a new point in the same general direction as the current movement
        {
            /*
             * 1) new destination shouldn't be too close to Peneleope
             * 2) new destination should be in the same general direction as the current movement
             * 3) new destination shouldn't be too far away
             * 4) new destination shouldn't be too close            
            */            
            while ((Vector3.Distance(new_dest, wander_target.transform.position) < wander_space) || (Vector3.Angle(new_dest_direction, transform.forward) > max_angle_change) || (Vector3.Distance(new_dest, transform.position) > max_mvmt) || (Vector3.Distance(new_dest, transform.position) < min_mvmt))
            {
                new_dest = Random.insideUnitSphere * wander_radius + wander_target.transform.position;
                new_dest.y = wander_target.transform.position.y + 0.2f; // 0.2f is just to make sure the sphere isn't partially under the ground
                new_dest_direction = (new_dest - transform.position).normalized;
                check_stuck++;

                if (check_stuck > 100000) //if finding a new point given the restrictions is taking too long
                {
                    return MoveToMidpoint();
                }
            }
            check_stuck = 0;
        }
        else // find a new point in the general opposite direction as the current movement
        {
            /*
             * 1) new destination shouldn't be too close to Peneleope
             * 2) new destination should be in the opposite-ish direction as the current movement
             * 3) new destination shouldn't be too far away
             * 4) new destination shouldn't be too close            
            */
            while ((Vector3.Distance(new_dest, wander_target.transform.position) < wander_space) || (Vector3.Angle(new_dest_direction, transform.forward) < min_angle_change) || (Vector3.Distance(new_dest, transform.position) > max_mvmt) || (Vector3.Distance(new_dest, transform.position) < min_mvmt))
            {
                new_dest = Random.insideUnitSphere * wander_radius + wander_target.transform.position;
                new_dest.y = wander_target.transform.position.y + 0.2f;
                new_dest_direction = (new_dest - transform.position).normalized;
                check_stuck++;

                if (check_stuck > 100000) //if finding a new point given the restrictions is taking too long
                {
                    return MoveToMidpoint();
                }
            }
            check_stuck = 0;
        }
        return new_dest; 
    }

    Vector3 MoveToMidpoint() //function that returns a point next to Penelope (if NextDestination gets stuck)
    {
        Vector3 toObject = (transform.position - wander_target.transform.position).normalized; //get unit vector from Peneleope to current position
        return wander_target.transform.position + toObject * ((wander_radius + Vector3.Distance(transform.position, wander_target.transform.position))/2);
        // return the midpoint of this unit vector (safe place for the code to continue working from)
    }

    void SetWait() // set wait to false after wait time is over
    {
        wait = false;
        m_Animator.SetBool("Idle", false);
        m_Animator.SetBool("Rotating", false);
        m_Animator.SetBool("Walking", true);
    }

    void StartRotating()
    {
        m_Animator.SetBool("Idle", false);
        m_Animator.SetBool("Rotating", true);
        Rotate(dest);
    }

    void StopRotating()
    {
        m_Animator.SetBool("Rotating", false);
        m_Animator.SetBool("Idle", true);
    }

    void SetWalkingFromSus()
    {
        walk_from_suspicious = true;
    }

    void Rotate(Vector3 destination)
    {
        Vector3 vector_dir = (destination - transform.position).normalized;
        if(vector_dir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(vector_dir);
        }
    }
}
