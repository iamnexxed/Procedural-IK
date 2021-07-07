using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralAnimation : MonoBehaviour
{
    [SerializeField] LayerMask terrainLayer = default;
    [SerializeField] Transform hint = default;
    [SerializeField] ProceduralAnimation otherFoot_0 = default;
    [SerializeField] ProceduralAnimation otherFoot_1 = default;
    [SerializeField] PlayerController playerController;
    
    Vector3 oldPosition, currentPosition, newPosition;
    

    float lerp;

    private void Start()
    {
        
        currentPosition = newPosition = oldPosition = transform.position;
       
        lerp = 1;
    }

    // Update is called once per frame

    void FixedUpdate()
    {
        transform.position = currentPosition;
        

        Ray ray = new Ray(hint.position, Vector3.down);
        // Physics.SphereCast(transform.position, playerController.stepDistance, Vector3.down, out )
        if (Physics.Raycast(ray, out RaycastHit info, 50, terrainLayer.value))
        {
            Debug.DrawLine(hint.position, info.point, Color.red);
            if (Vector3.Distance(newPosition, info.point) - 0.3f > playerController.stepDistance && !otherFoot_0.IsMoving() && !otherFoot_1.IsMoving() && lerp >= 1)
            {
                lerp = 0;
                newPosition = info.point + playerController.footOffset;
               
            }
        }

        if (IsMoving())
        {
            Vector3 tempPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
            tempPosition.y += Mathf.Sin(lerp * Mathf.PI) * playerController.stepHeight;

            currentPosition = tempPosition;
            
            lerp += Time.deltaTime * playerController.legSpeed;
        }
        else
        {
            oldPosition = newPosition;
            
        }
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(newPosition, 0.5f);
    }



    public bool IsMoving()
    {
        return lerp < 1;
    }



}
