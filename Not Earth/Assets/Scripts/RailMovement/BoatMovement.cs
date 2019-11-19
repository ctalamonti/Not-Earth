using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    
    [Tooltip("The list of waypoints for the boat to hit as it travles. The first waypoint is the starting location")]
    public List<GameObject> waypoints = new List<GameObject>();

    [Tooltip("How fast to move from waypoint to waypoint in units per second. Can be adjusted")]
    public float moveTime = 10;

    /// <summary>
    /// The start time for the slerp between waypoints
    /// </summary>
    private float startTime;
    
    /// <summary>
    /// The current target waypoint
    /// </summary>
    public GameObject currentWaypoint;
    
    /// <summary>
    /// The number of waypoints the boat has hit.
    /// </summary>
    private int waypointsHit = 0;

    /// <summary>
    /// The total length between the two waypoints being moved between
    /// </summary>
    private float totalLength;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (currentWaypoint == null)
        {
            NextWaypoint();
        }

        // How far the boat has already travelled
        float distCovered = (Time.time - startTime) * moveTime;
        
        // The fraction of the journey completed
        float fracComplete = distCovered / totalLength;

        // Interpolates where to move the object, and moves it
        transform.position = Vector3.Lerp(waypoints[waypointsHit - 1].transform.position,currentWaypoint.transform.position, fracComplete);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("waypoint"))
        {
            Debug.Log("Hit waypoint");
            NextWaypoint();
        }
    }

    void NextWaypoint()
    {
        startTime = Time.time;
        ++waypointsHit;
        currentWaypoint = waypoints[waypointsHit];
        totalLength = Vector3.Distance(waypoints[waypointsHit - 1].transform.position,
            currentWaypoint.transform.position);
    }
}
