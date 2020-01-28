using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisappearZombie : MonoBehaviour
{

    public GameObject player;
    public GameObject zombie;
    public GameObject chair;

    private bool zombieSpotted;
    private bool zombieOutOfSight;
    private Quaternion chairOriginalRotation;

    void Start() {
        zombie.SetActive(false);
        zombieSpotted = false;
        zombieOutOfSight = false;
        chairOriginalRotation = chair.transform.rotation;
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject == player) {
            zombie.SetActive(true);
        }
    }

    void OnTriggerStay(Collider other) {
        if (other.gameObject == player) {
            zombieSpotted = true;
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject == player && zombieSpotted)
            zombieOutOfSight = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (zombieOutOfSight) {
            chair.transform.rotation = chairOriginalRotation * Quaternion.AngleAxis(-30, Vector3.up) * Quaternion.AngleAxis(90, Vector3.left);
            chair.transform.position = zombie.transform.position + 0.25f * Vector3.up;
            zombie.SetActive(false);
        }
    }
}
