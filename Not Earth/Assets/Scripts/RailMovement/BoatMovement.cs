using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    
    [Tooltip("The list of waypoints for the boat to hit as it travles")]
    public List<GameObject> waypoints = new List<GameObject>();

    [Tooltip("How fast to move from waypoint to waypoint. Can be adjusted")]
    public float moveTime = 10;
    
    /// <summary>
    /// The curren target waypoint
    /// </summary>
    private GameObject currentWaypoint;
    
    /// <summary>
    /// The number of waypoints the boat has hit
    /// </summary>
    private int waypointsHit = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (currentWaypoint == null)
        {
            currentWaypoint = waypoints[waypointsHit];
        }

        transform.position = Vector3.Slerp(transform.position,
            currentWaypoint.transform.position, moveTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("waypoint"))
        {
            ++waypointsHit;
            currentWaypoint = waypoints[waypointsHit];
        }
    }
}
