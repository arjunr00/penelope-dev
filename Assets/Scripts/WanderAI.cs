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
    public float wait_time;
    private float timer;
    private Vector3 dest;
    private bool wait;
    private float rotation_speed;

    void Start()
    {
        timer = wander_time;
        dest = wander_target.transform.position;
        wait = false;
        rotation_speed = 50.0f;
    }

    void Update()
    {
        //****** General wandering around Penelope *********//
        if(!wait)
        {
            timer += Time.deltaTime;
        }
        if (((timer > wander_time) || (Vector3.Distance(transform.position, dest) < 0.01f)) && !wait) //check if wander_time is up or if dog has reached its destination
        {
            dest = NextDestination(wander_target.transform.position, wander_radius);
            wait = true;
            Invoke("SetWait", wait_time);
            StartCoroutine(Rotate(dest));
            timer = 0;
        }
        if(!wait)
        {
            transform.position = Vector3.MoveTowards(transform.position, dest, wander_speed * Time.deltaTime);
        }

        //*********************************************//


    }

    Vector3 NextDestination(Vector3 target_pos, float radius) //find next random destination for general wandering
    {
        Vector3 new_dest = Random.insideUnitSphere * radius + target_pos;
        new_dest.y = target_pos.y;
        while (Vector3.Distance(new_dest, wander_target.transform.position) < wander_space)
        {
            new_dest = Random.insideUnitSphere * radius + target_pos;
            new_dest.y = target_pos.y;
        }

        return new_dest; 
    }

    void SetWait() // set wait to false after wait time is over
    {
        wait = false;
    }

    IEnumerator Rotate(Vector3 destination) // Coroutine to smoothly rotate the dog in the direction of the next destination
    {
        Quaternion start_rotation = transform.rotation;
        Vector3 vector_dir = (destination - transform.position).normalized;
        Quaternion final_rotation = Quaternion.LookRotation(vector_dir);
        float angle_change = Quaternion.Angle(start_rotation, final_rotation);
        float duration = angle_change / rotation_speed;
        float t = 0f;
        while(duration > t)
        {
            transform.rotation = Quaternion.Slerp(start_rotation, final_rotation, t / duration);
            yield return null;
            t += Time.deltaTime;
        }
        transform.rotation = final_rotation;
    }
}
