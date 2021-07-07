using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyControl : MonoBehaviour
{
    public ProceduralAnimation[] legs;
    public float offset;
    public float bodyMoveSpeed = 10f;
    Vector3 bodyPos;
    bool shouldReposition;

    // Start is called before the first frame update
    void Start()
    {
        shouldReposition = true;
    }

    // Update is called once per frame
    void Update()
    {
        shouldReposition = true;
/*
        for (int i = 0; i < legs.Length; i++)
        {
            if (legs[i].IsMoving())
            {
                shouldReposition = false;
                // Debug.Log("Leg Moving : " + i);
                break;
            }

        }
*/
        // Debug.Log("Should Reposition : " + shouldReposition);

        if (shouldReposition)
        {
            CalculateAverageBodyPos();
        }
    }


    void CalculateAverageBodyPos()
    {
        Vector3 average = Vector3.zero;
        for (int i = 0; i < legs.Length; i++)
        {
            average += legs[i].transform.position;
        }
        average /= legs.Length;

        // LineLineIntersection(bodyPos, legs[0].transform.position);

        bodyPos = average + new Vector3(0, offset, 0);
        // Debug.Log("Body Pos : " + bodyPos);

        /*Vector3 vec1 = legs[0].transform.position - legs[3].transform.position;
        Vector3 vec2 = legs[1].transform.position - legs[2].transform.position;
        if(LineLineIntersection(out bodyPos, legs[0].transform.position, vec1, legs[1].transform.position, vec2))
        {
            transform.position = Vector3.Lerp(transform.position, bodyPos + new Vector3(0, offset, 0), Time.deltaTime * bodyMoveSpeed);
        }*/

        transform.position = Vector3.Lerp(transform.position, bodyPos, Time.deltaTime * bodyMoveSpeed);

        // RotateBody();
    }

    void RotateBody()
    {
        float deltaZ = legs[1].transform.position.y - legs[2].transform.position.y;
        float deltaX = legs[0].transform.position.y - legs[3].transform.position.y;

        float angleZ = Mathf.Rad2Deg * Mathf.Atan2(deltaZ, 2.5f);
        float angleX = Mathf.Rad2Deg * Mathf.Atan2(deltaX, 2.5f);

        Quaternion getRotation = transform.localRotation;
        getRotation = Quaternion.Euler(angleX, transform.localRotation.eulerAngles.y, angleZ);
        Debug.Log("Body Rotation : " + getRotation.eulerAngles);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, getRotation, Time.deltaTime * bodyMoveSpeed);
    }


    bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1,
        Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {
        intersection = Vector3.zero;

        // Find a vector that points from P1 to P2 in order to check the plane on which they are located 
        Vector3 lineVec3 = linePoint2 - linePoint1;

        // Determine a vector perpendicular to Vec1 and Vec2. It's magnitude > 0 suggests that Vec1 and Vec2 are not parallel.
        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);

        // Determine a vector perpendicular to Vec3 and Vec2
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

        // Find Planar factor to check if Vec3 and CrossVec1and2 perpendicular to check if the point of intersection lies on the same plane
        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

        //is coplanar, and not parallel
        if (Mathf.Abs(planarFactor) < 0.0001f
                && crossVec1and2.sqrMagnitude > 0.0001f)
        {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            intersection = linePoint1 + (lineVec1 * s);
            return true;
        }
        
            
        return false;
        
    }
}
