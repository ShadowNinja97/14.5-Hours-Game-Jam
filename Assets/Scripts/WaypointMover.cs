using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WaypointMover : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Waypoints waypoints;

    [Header("Passenger Detection")]
    [SerializeField] private LayerMask passengerLayers;
    [SerializeField] private float topCheckHeight = 0.1f;
    [SerializeField] private float horizontalInset = 0.02f;

    private Collider2D platformCollider;

    private void Awake()
    {
        platformCollider = GetComponent<Collider2D>();
    }

    /// <summary>
    /// Moves along waypoint path based on normalized percent (0–1).
    /// </summary>
    public void SetMovePercent(float percent)
    {
        if (waypoints == null || waypoints.wayPoints == null || waypoints.wayPoints.Count < 2)
            return;

        percent = Mathf.Clamp01(percent);

        Vector3 oldPosition = transform.position;
        Vector3 newPosition = GetPositionAlongPath(percent);

        Vector3 delta = newPosition - oldPosition;

        if (delta.sqrMagnitude <= Mathf.Epsilon)
            return;

        MovePassengers(delta);

        transform.position = newPosition;
    }

    private Vector3 GetPositionAlongPath(float percent)
    {
        var points = waypoints.wayPoints;
        int count = points.Count;

        // Two points = simple lerp
        if (count == 2)
        {
            return Vector3.Lerp(points[0], points[1], percent);
        }

        // More than 2 points = segmented path
        float scaled = percent * (waypoints.continuous ? count : count - 1);

        int index = Mathf.FloorToInt(scaled);
        float t = scaled - index;

        int nextIndex;

        if (waypoints.continuous)
        {
            index = index % count;
            nextIndex = (index + 1) % count;
        }
        else
        {
            index = Mathf.Clamp(index, 0, count - 2);
            nextIndex = index + 1;
        }

        return Vector3.Lerp(points[index], points[nextIndex], t);
    }

    private void MovePassengers(Vector3 delta)
    {
        Bounds bounds = platformCollider.bounds;

        Vector2 boxCenter = new Vector2(
            bounds.center.x,
            bounds.max.y + (topCheckHeight * 0.5f)
        );

        Vector2 boxSize = new Vector2(
            Mathf.Max(0f, bounds.size.x - horizontalInset * 2f),
            topCheckHeight
        );

        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0f, passengerLayers);

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hit = hits[i];

            if (hit == null || hit == platformCollider)
                continue;

            Rigidbody2D rb = hit.attachedRigidbody;

            if (rb != null && !rb.isKinematic)
            {
                rb.position += (Vector2)delta;
            }
            else
            {
                hit.transform.position += delta;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (TryGetComponent(out Collider2D col))
        {
            Bounds bounds = col.bounds;

            Vector3 boxCenter = new Vector3(
                bounds.center.x,
                bounds.max.y + (topCheckHeight * 0.5f),
                transform.position.z
            );

            Vector3 boxSize = new Vector3(
                Mathf.Max(0f, bounds.size.x - horizontalInset * 2f),
                topCheckHeight,
                0f
            );

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(boxCenter, boxSize);
        }
    }
}