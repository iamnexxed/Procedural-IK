using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float _speed = 1f;

    private Rigidbody _rigidbody;

    private enum BodyType
    {
        Dynamic,
        Kinematic
    }
    BodyType type;

    public float legSpeed = 1;
    public float stepDistance = 3;

    public float stepHeight = 1;
    public Vector3 footOffset = default;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if(_rigidbody.isKinematic)
        {
            type = BodyType.Kinematic;
        }
        else
        {
            type = BodyType.Dynamic;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float multiplier = 1f;
        if (Input.GetKey(KeyCode.LeftShift))
            multiplier = 2f;

        if(type == BodyType.Dynamic)
        {
            if (_rigidbody.velocity.magnitude < _speed * multiplier)
            {

                float value = Input.GetAxis("Vertical");
                if (value != 0)
                    _rigidbody.AddForce(0, 0, value * Time.fixedDeltaTime * 1000f);
                value = Input.GetAxis("Horizontal");
                if (value != 0)
                    _rigidbody.AddForce(value * Time.fixedDeltaTime * 1000f, 0f, 0f);
            }
        }
        else
        {
            // transform.Translate(transform.position + new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * _speed * Time.deltaTime * multiplier);
        }
        
    }
}