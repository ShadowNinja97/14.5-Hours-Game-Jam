using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    [Header("Waypoint Data")]
    public List<Vector2> wayPoints;
    public bool continuous = false;



    void OnDrawGizmos()
    {
        if (wayPoints == null || wayPoints.Count == 0)
            return;

        for (int i = 0; i < wayPoints.Count; i++)
        {
            Vector3 currentPoint = new Vector3(wayPoints[i].x, wayPoints[i].y, transform.position.z);

            // Set color per node
            if (i == 0)
            {
                Gizmos.color = Color.green;
            }
            else if (!continuous && i == wayPoints.Count - 1)
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.cyan;
            }

            // Draw waypoint node
            Gizmos.DrawWireSphere(currentPoint, 0.15f);

            // Draw line to next waypoint
            if (i < wayPoints.Count - 1)
            {
                Gizmos.color = Color.cyan;

                Vector3 nextPoint = new Vector3(wayPoints[i + 1].x, wayPoints[i + 1].y, transform.position.z);
                Gizmos.DrawLine(currentPoint, nextPoint);
            }
        }

        // Continuous loop connection
        if (continuous && wayPoints.Count > 2)
        {
            Gizmos.color = Color.cyan;

            Vector3 firstPoint = new Vector3(wayPoints[0].x, wayPoints[0].y, transform.position.z);
            Vector3 lastPoint = new Vector3(wayPoints[wayPoints.Count - 1].x, wayPoints[wayPoints.Count - 1].y, transform.position.z);

            Gizmos.DrawLine(lastPoint, firstPoint);
        }
    }
}
