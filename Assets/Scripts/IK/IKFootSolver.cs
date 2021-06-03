using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFootSolver : MonoBehaviour
{
    public LayerMask groundLayer;
    public Transform leg0Transform;
    public Transform leg1Transform;
    public Transform leg2Transform;
    public Transform leg3Transform;
    public Transform body;



    public Transform rootLocation;
    public float footSpacing = 2f;

    public float stepDistance = 1f;

    Vector3 zerothQ;
    Vector3 firstQ;
    Vector3 secondQ;
    Vector3 thirdQ;

    Vector3[] newPosition;
    Vector3[] oldPosition;


    float[] lerpSteps;

    // Start is called before the first frame update
    void Start()
    {
        zerothQ = (body.right + body.forward).normalized;
        firstQ = (body.right - body.forward).normalized;
        secondQ = (-body.right + body.forward).normalized;
        thirdQ = (-body.right - body.forward).normalized;

        oldPosition = new Vector3[4];
        newPosition = new Vector3[4];

        lerpSteps = new float[4];

        oldPosition[0] = leg0Transform.position;
        oldPosition[1] = leg1Transform.position;
        oldPosition[2] = leg2Transform.position;
        oldPosition[3] = leg3Transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*Ray ray0 = new Ray(rootLocation.position + (zerothQ * footSpacing), Vector3.down);
        if(Physics.Raycast(ray0, out RaycastHit info0, 10, groundLayer.value))
        {
            Debug.DrawRay(rootLocation.position + (zerothQ * footSpacing), Vector3.down, Color.red);
            leg0Transform.position = oldPosition[0];
            // Debug.Log("Distance to new Pos : " + Vector3.Distance(leg0Transform.position, info0.point));
            if (Vector3.Distance(leg0Transform.position, info0.point) > stepDistance)
            {
                Debug.Log("Step 0");
                newPosition[0] = info0.point;
                leg0Transform.position = newPosition[0];
                oldPosition[0] = newPosition[0];
            }
            
                

            // Debug.Log("Touch Ground : " + info0.point);
        }*/

        /*Ray ray1 = new Ray(body.position + (firstQ * footSpacing), Vector3.down);
        if (Physics.Raycast(ray1, out RaycastHit info1, 10, groundLayer.value))
        {
            leg1Transform.position = info1.point;

            // Debug.Log("Touch Ground : " + info1.point);
        }*/

        PerformRaycast(leg0Transform, zerothQ, 0);
        PerformRaycast(leg1Transform, firstQ, 1);
        PerformRaycast(leg2Transform, secondQ, 2);
        PerformRaycast(leg3Transform, thirdQ, 3);

       /* Ray ray2 = new Ray(body.position + (secondQ * footSpacing), Vector3.down);
        if (Physics.Raycast(ray2, out RaycastHit info2, 10, groundLayer.value))
        {
            leg2Transform.position = info2.point;

            // Debug.Log("Touch Ground : " + info2.point);
        }

        Ray ray3 = new Ray(body.position + (thirdQ * footSpacing), Vector3.down);
        if (Physics.Raycast(ray3, out RaycastHit info3, 10, groundLayer.value))
        {
            leg3Transform.position = info3.point;

            // Debug.Log("Touch Ground : " + info3.point);
        }*/
    }

    private void PerformRaycast(Transform leg, Vector3 relativeLocation, int index)
    {
        Ray ray = new Ray(rootLocation.position + (relativeLocation * footSpacing), Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit info, 10, groundLayer.value))
        {
            Debug.DrawRay(rootLocation.position + (relativeLocation * footSpacing), Vector3.down, Color.red);
            leg.position = oldPosition[index];
            // Debug.Log("Distance to new Pos : " + Vector3.Distance(leg0Transform.position, info0.point));
            if (Vector3.Distance(leg.position, info.point) > stepDistance)
            {
                // Debug.Log("Step 0");
                newPosition[index] = info.point;
                leg.position = newPosition[index];
                oldPosition[index] = newPosition[index];
            }
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        /*if(newPosition[0] != null)
            Gizmos.DrawSphere(newPosition[0], 0.5f);*/
    }
}
