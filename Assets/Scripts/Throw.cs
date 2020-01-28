using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour
{
    public GameObject Chunk;
    public Camera camera;
    public int shotForce;
    public int maxShotForce;
    int shotCharge = 0;
    bool holdingThrow = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            holdingThrow = true;



        }


            if (Input.GetMouseButton(0))
        {


            if (holdingThrow)
            {
                shotCharge += 1;
            }

        }
       if (Input.GetMouseButtonUp(0) && holdingThrow || shotCharge >= 40)
        {
           // if (holdingThrow) //|| shotCharge == maxShotForce)
           // {
                holdingThrow = false;
                Ray shot = camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(shot, out hit))
                {
                    Vector3 direction = hit.point - transform.position;
                    // direction.Normalize();

                    GameObject clayChunk = Instantiate(Chunk, camera.transform.position, Quaternion.identity);
                    clayChunk.GetComponent<Rigidbody>().AddForce(direction * (shotForce) *((shotCharge/5) +3));
                    shotCharge = 0;
                    holdingThrow = false;
                    Debug.LogError(shotCharge);
                  
                }
                
            }





        }








}
