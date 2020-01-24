using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    
    [Tooltip("An array of waypoints for the boat to hit as it travles. The first waypoint is the starting location")]
    public GameObject[] waypoints = new GameObject[21];

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

    /// <summary>
    /// Is the object moving
    /// </summary>
    public static bool isMoving = false;

    private bool isMovingHold = false;

    // Update is called once per frame
    void LateUpdate()
    {
        if (isMoving)
        {
            // Update the current waypoint the straightline object is moving to
            if (currentWaypoint == null)
            {
                NextWaypoint();
            }

            // How far the boat has already travelled
            float distCovered = (Time.time - startTime) * moveTime;
        
            // The fraction of the journey completed
            float fracComplete = distCovered / totalLength;


            // Interpolates where to move the object, and moves it
            transform.position = Vector3.Lerp(
                waypoints[waypointsHit - 1].transform.position,
                currentWaypoint.transform.position, fracComplete);
        }
    }

    /// <summary>
    /// Sets the current waypoint once it has been reached
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        // Verify the object collided with a waypoint
        if (other.CompareTag("waypoint"))
        {
            Debug.Log("Hit waypoint");
            NextWaypoint();
        }
    }

    /// <summary>
    /// Get the next waypoint, calculate the distance to it, and increment the waypoints hit
    /// </summary>
    void NextWaypoint()
    {
        // Set the start time of the travel
        startTime = Time.time;
        ++waypointsHit;
        // If there is a next waypoint, set the current waypoint to it
        if (waypointsHit < waypoints.Length) {
            currentWaypoint = waypoints[waypointsHit];
        }
        // Calculate how far the target is from the current position
        totalLength = Vector3.Distance(waypoints[waypointsHit - 1].transform.position,
            currentWaypoint.transform.position);
    }
}
