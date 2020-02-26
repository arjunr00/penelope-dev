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
    public float wander_time;
    public float wander_radius;
    public float wander_speed;
    public float wander_space;
    public float max_angle_change;
    public float min_angle_change;
    public float max_mvmt;
    public float min_mvmt;
    private float timer;
    private Vector3 dest;
    private bool wait;
    private float rotation_speed_fast;
    private bool general_wander;
    private bool follow;
    private bool change_speed;
    private float follow_speed;
    private Animator m_Animator;
    void Start()
    {
        timer = wander_time;
        dest = wander_target.transform.position;
        wait = false;
        rotation_speed_fast = 1f;
        general_wander = true;
        follow = true;
        change_speed = true;
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
            if (!wait)
            {
                timer += Time.deltaTime;
            }
            if (((timer > wander_time) || (Vector3.Distance(transform.position, dest) < 0.01f)) && !wait) //check if wander_time is up or if dog has reached its destination
            {

                if (Random.value < 0.25f) //randomly wait before moving
                {
                    dest = NextDestination(wander_target.transform.position, wander_radius, false); //get next random destination in the opposite-ish direction
                    wait = true;
                    float wait_time = Random.Range(10, 20);
                    Invoke("SetWait", wait_time); //change wait back to false after a random number seconds
                    m_Animator.SetBool("Rotating", false);
                    m_Animator.SetBool("Walking", false);
                    m_Animator.SetBool("Idle", true);
                    Invoke("StartRotating", wait_time / 5);
                    Invoke("StopRotating", 4 * wait_time / 5);
                }
                else
                {
                    dest = NextDestination(wander_target.transform.position, wander_radius, true); //get next random destination somewhere in the forward direction
                    Rotate(dest);
                }
                timer = 0;

            }
            if (!wait)
            {
                transform.position = Vector3.MoveTowards(transform.position, dest, wander_speed * Time.deltaTime); //move towards dest
            }
        }
        //**************************************************//


        //****** Follow Penelope *********//
        /*
         * As soon as Penelope moves ahead of the dog, start moving along the same direction as Penelope 
         * Little bit of left and right movement (random)
         * Speed changes (random according to Penelope's speed)
        

        else // if Penelope is moving
        {
            if(Vector3.Angle(wander_target.transform.forward, transform.position - wander_target.transform.position) > 90) // if the dog is behind Penelope
            {
                if(change_speed)
                {
                    Invoke("SetChangeSpeed", 3);
                    follow_speed = wander_target.GetComponent<NewPenelopeMovement>().speed - 1f + Random.Range(-0.5f, 0.5f);
                    change_speed = false;
                }
                Quaternion new_rotation = wander_target.transform.rotation;
                //new_rotation.y = new_rotation.y + Random.Range(-5, 5);
                transform.rotation = new_rotation;

                Debug.Log(transform.forward);

                transform.position += transform.forward * follow_speed * Time.deltaTime; //TODO - try using her forward vector and her speed 
            }
        }

        */
        //**************************************************//
    }



    Vector3 NextDestination(Vector3 target_pos, float radius, bool forward_movement) //find next random destination for general wandering 
    {
        Vector3 new_dest = Random.insideUnitSphere * radius + target_pos; //choose a random point in Penelope's range
        new_dest.y = target_pos.y + 0.2f;
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
                new_dest = Random.insideUnitSphere * radius + target_pos;
                new_dest.y = target_pos.y + 0.2f;
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
                new_dest = Random.insideUnitSphere * radius + target_pos;
                new_dest.y = target_pos.y + 0.2f;
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

    void SetChangeSpeed() // set change_speed to true 
    {
        change_speed = true;
    }

    void Rotate(Vector3 destination)
    {
        Vector3 vector_dir = (destination - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(vector_dir);
    }
}
