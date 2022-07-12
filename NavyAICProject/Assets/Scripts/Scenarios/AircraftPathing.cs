using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftPathing : MonoBehaviour
{
    [System.Serializable]
    public class Waypoint
    {
        [Range(0.0f, 100.0f)]
        public float turnSpeed = 40.0f;
        [Range(0.0f, 4.0f)]
        public float velocity = 1.0f;
        [Range(0.0f, 3.0f)]
        public float acceleration = 1.0f;
        public Vector2 position;
    }

    public Waypoint[] waypoints = new Waypoint[1];
    private float speedRate = 0.0f;

    private int currentWaypoint = 0;

    private void Update()
    {
        // Rotate towards target
        Vector2 direction = (waypoints[currentWaypoint].position - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(Vector3.forward * (angle - 90)),
            waypoints[currentWaypoint].turnSpeed * Time.deltaTime);
        // Move towards target
        speedRate += waypoints[currentWaypoint].acceleration * Time.deltaTime;
        speedRate = Mathf.Min(1.0f, speedRate);
        //transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.up, waypoints[currentWaypoint].velocity * speedRate * Time.deltaTime);
        transform.position += Vector3.up * Time.deltaTime;
        if (Vector2.Distance(transform.position, waypoints[currentWaypoint].position) < 0.05f)
        {
            ++currentWaypoint;
        }
    }

    private void OnDrawGizmosSelected()
    {
        foreach(Waypoint waypoint in waypoints)
        {
            Gizmos.DrawIcon(waypoint.position, "US_Navy_Emblem");
        }
    }
}
