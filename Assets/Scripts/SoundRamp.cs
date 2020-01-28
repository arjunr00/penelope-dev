using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundRamp : MonoBehaviour
{
    public GameObject player;
    public AudioSource source;
    public AudioClip heightenedAmbience;

    private int playerEntryCount;
    void Start()
    {
        playerEntryCount = 0;
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject == player && ++playerEntryCount == 2) {
            source.clip = heightenedAmbience;
            source.Play();
        }
    }
}
