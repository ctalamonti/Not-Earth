using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    bool trigger = false;
   private Vector3 Movement;
   [Tooltip("This changes how the object moves in the scene")] public Vector3 settingMovement;
    private void Update()
    {
        
        if (trigger)
        {
            
            Movement = transform.position;
            Moves(settingMovement);
            transform.position = Movement;
        }
    }

    void Moves(Vector3 directionsToMoveIn)
    {
        Movement.x += directionsToMoveIn.x * Time.deltaTime;
        Movement.y += directionsToMoveIn.y * Time.deltaTime;
        Movement.z += directionsToMoveIn.z * Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            trigger = true;
        }
    }
}


