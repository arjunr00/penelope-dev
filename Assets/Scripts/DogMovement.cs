
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogMovement : MonoBehaviour
{
    // GameObject that dog follows
    public GameObject player;
    private Vector3 target;
    
    // Base speed of dog
    public float base_speed;

    // Distance from target before dog stops/starts following
    // NOTE: too small min_dist will cause dog to try to move into player, note the player model's size
    public float min_dist, max_dist;

    // Speed multiplier for dog based on distance to target; range:[min_dash, max_dash]
    private float dash_speed;
    // Minimum/maximum speed multipliers
    public float min_dash, max_dash;

    private bool following;
    private float distance;

    // Start is called before the first frame update
    void Start()
    {
        // Hardcoded starting values
        player = GameObject.Find("eve_j_gonzales");
        base_speed = 1f;
        min_dist = 1f;
        max_dist = 4f;
        min_dash = 1f;
        max_dash = 1.5f;

        following = false;
    }

    void FixedUpdate()
    {
        /* Following Behavior
         * If the dog is not currently following a target, check that the player is some distance away
         * - Place a target at the player's current location
         * - Continue to move toward target
         * Once the dog reaches some distance to the target
         * - Stop moving toward the target
         * - Wait for new movement command
         * If the player moves again some distance away while the dog is moving to the target
         * - Recalculate target <--- may be somewhat unnatural, fixes dash speed bug
         * - Perhaps have another variable greater than max_dist that specifies when to recalculate target
         * 
         * Following Behavior (OLD)
         * If the dog is not currently following a target, check that the player is some distance away
         * - Place a target at the player's current location
         * - Continue to move toward target (regardless of Player's new location)
         * Once the dog reaches some distance to the target
         * - Stop moving toward the target
         * - Wait for new movement command
         * 
         * TODO: Add randomness/naturalness to dog's movement
         * - Maybe +/- some random value to target so dog doesnt always move exactly to player's location
         * - Make dog move in zig-zag or some other type of non-straight path to target
         * 
         * TODO: Have dog path around walls/other objects
         * 
         * TODO: Perhaps have a list (stack) of targets for dog to follow (based on where player has gone)
         */

        // Get the distance between the dog and player
        distance = Vector3.Distance(player.transform.position, transform.position);
            
        // If the player is some distance away and the dog is not following, place target at player's current location
        if (distance > max_dist && !following)
        {
            following = true;
            target = player.transform.position;
        }

        // Continue following until the dog nearly reaches the target
        if (following)
        {
            // Calculate dash multiplier based on distance from target
            distance = Vector3.Distance(target, transform.position);
            float oldrange = max_dist - min_dist;
            float newrange = max_dash - min_dash;
            // Scales a value in range [min_dist, max_dist] to a value in range [min_dash, max_dash]
            dash_speed = (((Mathf.Min(distance, max_dist) - min_dist) * newrange) / oldrange) + min_dash;
            // bug fix for old ver: change distance to min(distance, max_dist), so dash_speed will be set to max_dash even if distance exceeds max_dist

            // Move toward target
            transform.position = Vector3.MoveTowards(transform.position, target, dash_speed * base_speed * Time.deltaTime);

            // Stop following when dog reaches some distance from the player
            // For old ver: remove distance calculation line below
            distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance < min_dist)
            {
                following = false;
            }
            // Recalculate target if player gets too far
            // For old ver: remove below if block
            else if (distance > max_dist)
            {
                target = player.transform.position;
            }
        }
    }
}
