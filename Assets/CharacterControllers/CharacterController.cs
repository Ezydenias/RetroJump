using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Persistence;


public class CharacterController : MonoBehaviour
{
    public float inputDelay = 0.1f;
    public float forwardVelocity = 12f;
    public float rotateVelocity = 100f;

    private Quaternion targetRotation;
    Rigidbody rBody;
    float forwardInput, turnInput;

    public Quaternion TargetRotation
    {
        get { return targetRotation; }
    }

    void Start()
    {
        targetRotation = transform.rotation;
        if (GetComponent<Rigidbody>())
            rBody = GetComponent<Rigidbody>();
        else
            Debug.LogError("the Character needs a rigidbody, DODOING!!!");

        forwardInput = turnInput = 0;
    }

    void GetInput()
    {
        forwardInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");
    }

    void Update()
    {
        GetInput();
        Turn();
    }

    void FixedUpdate()
    {
        Run();
    }

    void Run()
    {
        if (Mathf.Abs(forwardInput) > inputDelay)
        {
            //go
            rBody.velocity = transform.forward * forwardInput * forwardVelocity;
        }
        else
        {
            //stay
            rBody.velocity = Vector3.zero;
        }
    }

    void Turn()
    {
        targetRotation*=Quaternion.AngleAxis(rotateVelocity*turnInput*Time.deltaTime,Vector3.up);
        transform.rotation = targetRotation;
    }
}