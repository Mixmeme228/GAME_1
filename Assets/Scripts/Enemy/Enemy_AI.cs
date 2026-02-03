using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using Pathfinding;

public class Enemy_AI : MonoBehaviour
{
    public Transform target;

    public float nextWaypointDistance = 0.1f;

    public Path path;
    public int currentWaypoint = 0;
    public bool reachedEndOfPath = false;

    public Seeker seeker;
    public Rigidbody2D rb_2;

    public Vector2 direction;
    public Vector2 force;
    public float distanceToPath;
    private void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb_2.position, target.position, OnPathComplete);
        }
    }
    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    public void Move()
    {
        if (path == null)
        {
            return;
        }
        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }
        direction = ((Vector2)path.vectorPath[currentWaypoint] - (Vector2)transform.position).normalized;
        //force = direction * speed * Time.deltaTime;

        //rb_2.AddForce(force);

        distanceToPath = Vector2.Distance((Vector2)transform.position, path.vectorPath[currentWaypoint]);

        if (distanceToPath < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }
}
