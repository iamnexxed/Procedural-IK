using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFootSolver : MonoBehaviour
{
    public Rigidbody rb;

    public LayerMask groundLayer;
    public Transform leg0Transform;
    public Transform leg1Transform;
    public Transform leg2Transform;
    public Transform leg3Transform;
    public Transform body;

    public float speed = 5f;

    public Transform rootLocation;
    public float footSpacing = 2f;

    public float stepDistance = 1f;
    public float stepHeight = 0.25f;

    Vector3 zerothQ;
    Vector3 firstQ;
    Vector3 secondQ;
    Vector3 thirdQ;

    Vector3[] newPosition;
    Vector3[] oldPosition;
    Vector3[] currentPosition;

    Vector3 _prevPosition;
    Vector3 vel;

    bool setZigZag;

    bool[] movingLegs;

    float[] lerpSteps;

    bool startGround;

    // Start is called before the first frame update
    void Start()
    {
        zerothQ = (body.right + body.forward).normalized;
        firstQ = (body.right - body.forward).normalized;
        secondQ = (-body.right + body.forward).normalized;
        thirdQ = (-body.right - body.forward).normalized;

        oldPosition = new Vector3[4];
        newPosition = new Vector3[4];
        currentPosition = new Vector3[4];
        movingLegs = new bool[4];
        lerpSteps = new float[4];

        for(int i = 0; i < 4; ++i)
        {
            movingLegs[i] = false;
        }

        currentPosition[0] = newPosition[0] = oldPosition[0] = leg0Transform.position;
        currentPosition[1] = newPosition[1] = oldPosition[1] = leg1Transform.position;
        currentPosition[2] = newPosition[2] = oldPosition[2] = leg2Transform.position;
        currentPosition[3] = newPosition[3] = oldPosition[3] = leg3Transform.position;

        setZigZag = true;
        startGround = false;
    }

    

    void Update()
    {
        vel = (transform.position - _prevPosition) / Time.deltaTime;
        _prevPosition = transform.position;
    }

    // Update is called once per frame
    /// <summary>
    /// 
    /// </summary>
    void FixedUpdate()
    {
        // Debug.Log("Velocity : " + vel);

        /*
        if(setZigZag)
        {
            if (!movingLegs[0] && !movingLegs[3])
            {
                movingLegs[1] = PerformStep(leg1Transform, firstQ, 1);
                movingLegs[2] = PerformStep(leg2Transform, secondQ, 2);
                if(!movingLegs[1] && !movingLegs[2])
                {
                    setZigZag = false;
                    // Debug.Log("Change Legs Moving 1, 2");
                }
            }
        }
        else
        {
            if(!movingLegs[1] && !movingLegs[2])
            {
                movingLegs[0] = PerformStep(leg0Transform, zerothQ, 0);

                movingLegs[3] = PerformStep(leg3Transform, thirdQ, 3);
                if (!movingLegs[0] && !movingLegs[3])
                {
                    setZigZag = true;
                    // Debug.Log("Change Legs Moving 0, 3");
                }
            }
        }
        
          */
        PerformStep(leg1Transform, firstQ, 1);
        PerformStep(leg2Transform, secondQ, 2);
        PerformStep(leg0Transform, zerothQ, 0);
        PerformStep(leg3Transform, thirdQ, 3);
    }

    private bool PerformStep(Transform leg, Vector3 relativeLocation, int index)
    {
        leg.position = currentPosition[index];

        Ray ray = new Ray(rootLocation.position + (relativeLocation * footSpacing), Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit info, 10f, groundLayer.value))
        {
            Debug.DrawLine(rootLocation.position + (relativeLocation * footSpacing), info.point, Color.yellow);
            
            // Debug.Log("Distance to new Pos : " + Vector3.Distance(leg0Transform.position, info0.point));
            if (Vector3.Distance(newPosition[index], info.point) > (stepDistance))
            {
                // Debug.Log("Step 0");
                newPosition[index] = info.point;

                lerpSteps[index] = 0;
                
                
            }
        }
        if(lerpSteps[index] < 1)
        {
            Vector3 footPosition = Vector3.Lerp(oldPosition[index], newPosition[index], lerpSteps[index]);
            footPosition.y = Mathf.Abs(Mathf.Sin(lerpSteps[index] * Mathf.PI) * stepHeight);
            currentPosition[index] = footPosition;

            lerpSteps[index] += Time.deltaTime * speed;
            return true;
        }
        else
        {
            oldPosition[index] = newPosition[index];
            return false;
            
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        /*if(newPosition[0] != null)
            Gizmos.DrawSphere(newPosition[0], 0.5f);*/
    }
}
