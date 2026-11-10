using UnityEngine;
using System.Collections.Generic;

public class ColliderPenetrationResolver : MonoBehaviour
{
    [Header("Collider")]
    [SerializeField]
    private Collider targetCollider;

    [Header("Detection")]
    [SerializeField]
    private LayerMask collisionLayers = ~0;

    [SerializeField]
    private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;

    [SerializeField]
    private float skinWidth = 0.001f;

    [Header("Resolution")]
    [SerializeField]
    private bool resolvePenetration = true;

    [SerializeField]
    private int maxIterations = 5;

    [SerializeField]
    private float positionEpsilon = 0.00001f;

    [Header("Debug")]
    [SerializeField]
    private bool drawDebug = true;

    [SerializeField]
    private bool drawContactPoint = true;

    [SerializeField]
    private float debugDuration = 0.05f;

    [SerializeField]
    private Color penetrationColor = Color.red;

    [SerializeField]
    private Color resolvedColor = Color.green;

    private readonly Collider[] overlapResults = new Collider[64];

    private readonly HashSet<Collider> previousOverlaps = new HashSet<Collider>();

    private readonly HashSet<Collider> currentOverlaps = new HashSet<Collider>();


    private void Reset()
    {
        targetCollider = GetComponent<Collider>();
    }


    private void Awake()
    {
        if (targetCollider == null)
            targetCollider = GetComponent<Collider>();
    }


    private void FixedUpdate()
    {
        if (targetCollider == null)
            return;

        DetectAndResolve();
    }


    private void DetectAndResolve()
    {
        currentOverlaps.Clear();

        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            int overlapCount = FindPotentialOverlaps();

            bool foundPenetration = false;

            for (int i = 0; i < overlapCount; i++)
            {
                Collider other = overlapResults[i];

                if (!IsValidCollider(other))
                    continue;

                currentOverlaps.Add(other);

                if (!Physics.ComputePenetration(targetCollider, targetCollider.transform.position, targetCollider.transform.rotation, other, other.transform.position, other.transform.rotation, out Vector3 direction, out float distance))
                {
                    continue;
                }

                foundPenetration = true;

                if (distance <= positionEpsilon)
                    continue;

                float correctionDistance = distance + skinWidth;

                Vector3 correction = direction * correctionDistance;

                if (drawDebug)
                {
                    Vector3 origin = targetCollider.bounds.center;

                    Debug.DrawRay(origin, direction * distance, penetrationColor, debugDuration);

                    if (drawContactPoint)
                    {
                        Debug.DrawRay(origin + direction * distance, direction * skinWidth, resolvedColor, debugDuration);
                    }
                }

                if (resolvePenetration)
                {
                    transform.position += correction;

                    if (drawDebug)
                    {
                        Debug.DrawRay(targetCollider.bounds.center, correction, resolvedColor, debugDuration);
                    }
                }
            }

            if (!foundPenetration)
                break;
        }

        UpdateCollisionEvents();
    }


    private int FindPotentialOverlaps()
    {
        Bounds bounds = targetCollider.bounds;

        return Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, overlapResults, Quaternion.identity, collisionLayers, triggerInteraction);
    }


    private bool IsValidCollider(Collider other)
    {
        if (other == null)
            return false;

        if (other == targetCollider)
            return false;

        if (other.transform == transform)
            return false;

        Rigidbody ownRigidbody = targetCollider.attachedRigidbody;

        Rigidbody otherRigidbody = other.attachedRigidbody;

        if (ownRigidbody != null && ownRigidbody == otherRigidbody)
        {
            return false;
        }

        return true;
    }


    private void UpdateCollisionEvents()
    {
        foreach (Collider collider in currentOverlaps)
        {
            if (!previousOverlaps.Contains(collider))
            {
                OnPenetrationEnter(collider);
            }
            else
            {
                OnPenetrationStay(collider);
            }
        }

        foreach (Collider collider in previousOverlaps)
        {
            if (!currentOverlaps.Contains(collider))
            {
                OnPenetrationExit(collider);
            }
        }

        previousOverlaps.Clear();

        foreach (Collider collider in currentOverlaps)
        {
            previousOverlaps.Add(collider);
        }
    }


    private void OnPenetrationEnter(Collider other)
    {
        Debug.Log($"Penetration Enter: {other.name}", other);
    }


    private void OnPenetrationStay(Collider other)
    {
    }


    private void OnPenetrationExit(Collider other)
    {
        Debug.Log($"Penetration Exit: {other.name}", other);
    }


    private void OnDrawGizmosSelected()
    {
        if (targetCollider == null)
            return;

        Gizmos.matrix = Matrix4x4.TRS(targetCollider.bounds.center, Quaternion.identity, Vector3.one);

        Gizmos.DrawWireCube(Vector3.zero, targetCollider.bounds.size);
    }
}