using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralLegs : MonoBehaviour
{
    [SerializeField] Transform[] legTargets;
    [SerializeField] float stepSize = 1.5f;
    [SerializeField] int smoothness = 5;
    [SerializeField] float stepHeight = 0.15f;
    [SerializeField] float sphereCastRadius = 0.125f;
    [SerializeField] bool setBodyOrientation = true;
    [SerializeField] float raycastRange = 1.5f;

    Vector3[] defaultLegPositions;
    Vector3[] lastLegPositions;
    Vector3 lastBodyUp;
    bool[] legsMoving;
    int noOfLegs;

    Vector3 velocity;
    Vector3 lastVelocity;
    Vector3 lastBodyPos;
    float velocityMultiplier = 15f;

    // Start is called before the first frame update
    void Start()
    {
        lastBodyUp = transform.up;
        noOfLegs = legTargets.Length;
        defaultLegPositions = new Vector3[noOfLegs];
        lastLegPositions = new Vector3[noOfLegs];
        legsMoving = new bool[noOfLegs];

        for(int i = 0; i < noOfLegs; ++i)
        {
            defaultLegPositions[i] = legTargets[i].localPosition;
            lastLegPositions[i] = legTargets[i].position;
            legsMoving[i] = false;
        }

        lastBodyPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        velocity = transform.position - lastBodyPos;
        velocity = (velocity + smoothness * lastVelocity) / (smoothness + 1f);

        if(velocity.magnitude < 0.000025f)
        {
            velocity = lastVelocity;
        }
        else
        {
            lastVelocity = velocity;
        }

        Vector3[] desiredPositions = new Vector3[noOfLegs];
        int indexToMove = -1;
        float maxDistance = stepSize;
        for (int i = 0; i < noOfLegs; ++i)
        {
            desiredPositions[i] = transform.TransformPoint(defaultLegPositions[i]);

            float distance = Vector3.ProjectOnPlane(desiredPositions[i] + velocity * velocityMultiplier - lastLegPositions[i], transform.up).magnitude;
            if(distance > maxDistance)
            {
                maxDistance = distance;
                indexToMove = i;
            }
        }

        for (int i = 0; i < noOfLegs; ++i)
        {
            if(i != indexToMove)
            {
                legTargets[i].position = lastLegPositions[i];
            }
        }

        if(indexToMove != 1 && !legsMoving[0])
        {
            Vector3 targetPoint = desiredPositions[indexToMove] + Mathf.Clamp(velocity.magnitude * velocityMultiplier, 0f, 1.5f) * (desiredPositions[indexToMove] - legTargets[indexToMove].position) + velocity * velocityMultiplier;
            // Might need to replace tranform.parent.up with vector3.up;
            Vector3[] positionAndNormalForward = MatchToSurfaceFromAbove(targetPoint + velocity * velocityMultiplier, raycastRange, (transform.parent.up - velocity * 100).normalized);
            Vector3[] positionAndNormalBackward = MatchToSurfaceFromAbove(targetPoint + velocity * velocityMultiplier, raycastRange * (1f + velocity.magnitude), (transform.parent.up + velocity * 75).normalized);

            legsMoving[0] = true;
            if(positionAndNormalForward[1] == Vector3.zero)
            {
                StartCoroutine(PerformStep(indexToMove, positionAndNormalBackward[0]));
            }
            else
            {
                StartCoroutine(PerformStep(indexToMove, positionAndNormalForward[0]));
            }
        }

        lastBodyPos = transform.position;
        if(noOfLegs > 3 && setBodyOrientation)
        {
            Vector3 v1 = legTargets[0].position - legTargets[1].position;
            Vector3 v2 = legTargets[2].position - legTargets[3].position;
            Vector3 normal = Vector3.Cross(v1, v2).normalized;
            Vector3 up = Vector3.Lerp(lastBodyUp, normal, 1 / (float)(smoothness - 1));
            transform.up = up;
            transform.rotation = Quaternion.LookRotation(transform.parent.forward, up);
            lastBodyUp = transform.up;
        }
    }

    Vector3[] MatchToSurfaceFromAbove(Vector3 point, float halfRange, Vector3 up)
    {
        Vector3[] result = new Vector3[2];
        result[1] = Vector3.zero;

        RaycastHit hit;
        Ray ray = new Ray(point + halfRange * up / 2f, -up);

        if(Physics.SphereCast(ray, sphereCastRadius, out hit, 2f * halfRange))
        {
            result[0] = hit.point;
            result[1] = hit.normal;
        }
        else
        {
            result[0] = point;
        }

        return result;
    }

    IEnumerator PerformStep(int index, Vector3 targetPoint)
    {
        Vector3 startPos = lastLegPositions[index];

        for(int i = 1; i <= smoothness; ++i)
        {
            legTargets[index].position = Vector3.Lerp(startPos, targetPoint, i / (float)(smoothness + 1f));
            legTargets[index].position += transform.up * Mathf.Sin(i / (float)(smoothness + 1f) * Mathf.PI) * stepHeight;
            yield return new WaitForFixedUpdate();
        }
        legTargets[index].position = targetPoint;
        lastLegPositions[index] = legTargets[index].position;
        legsMoving[0] = false;
    }

    private void OnDrawGizmosSelected()
    {
        for(int i = 0; i < noOfLegs; ++i)
        {
            Gizmos.DrawSphere(legTargets[i].position, 0.05f);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.TransformPoint(defaultLegPositions[i]), stepSize);
        }
    }
}
