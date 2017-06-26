using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Persistence;


public class CharacterControl : MonoBehaviour
{
    [System.Serializable]
    public class MoveSettings
    {
        public float forwardVelocity = 24f;
        public float rotateVelocity = 100f;
        public float jumpVel = 25f;
        public float distToGrounded = 15.1f;
        public float doubleJumpDistToGrounded = 20f;
        public LayerMask ground;
    }

    [System.Serializable]
    public class PhysSettings
    {
        public float downAccel = 0.75f;
    }

    [System.Serializable]
    public class InputSettings
    {
        public float inputDelay = 0.1f;
        public string FORWARD_AXIS = "Vertical";
        public string TURN_AXIS = "Horizontal";
        public string JUMP_AXIS = "Jump";
    }

    public MoveSettings moveSetting = new MoveSettings();
    public PhysSettings physSetting = new PhysSettings();
    public InputSettings inputSetting = new InputSettings();

    public bool doubleJump = true;
    Vector3 velocity = Vector3.zero;
    private Quaternion targetRotation;
    Rigidbody rBody;
    float forwardInput, turnInput, jumpInput;

    public Quaternion TargetRotation
    {
        get { return targetRotation; }
    }

    bool Grounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, moveSetting.distToGrounded, moveSetting.ground);
    }

    bool doubleJumpAble()
    {
        return Physics.Raycast(transform.position, Vector3.down, moveSetting.doubleJumpDistToGrounded,
            moveSetting.ground);
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
        forwardInput = Input.GetAxis(inputSetting.FORWARD_AXIS); //interpolated
        turnInput = Input.GetAxis(inputSetting.TURN_AXIS); //interpolated
        jumpInput = Input.GetAxisRaw(inputSetting.JUMP_AXIS); //non-interpolated
    }

    void Update()
    {
        GetInput();
        Turn();
    }

    void FixedUpdate()
    {
        Run();
        Jump();

        rBody.velocity = transform.TransformDirection(velocity);
    }

    void Run()
    {
        if (Mathf.Abs(forwardInput) > inputSetting.inputDelay)
        {
            //go
            velocity.z = moveSetting.forwardVelocity * forwardInput;
        }
        else
        {
            //stay
            velocity.z = 0;
        }
    }

    void Turn()
    {
        if (Mathf.Abs(turnInput) > inputSetting.inputDelay)
        {
            targetRotation *= Quaternion.AngleAxis(moveSetting.rotateVelocity * turnInput * Time.deltaTime, Vector3.up);
        }
        transform.rotation = targetRotation;
    }

    void Jump()
    {
        if (jumpInput > 0 && Grounded())
        {
            //jump
            velocity.y = moveSetting.jumpVel;
        }
        else if (jumpInput == 0 && Grounded())
        {
            //zero out our velocity.y
            doubleJump = true;
            velocity.y = 0;
        }
        else if (jumpInput > 0 && Grounded() == false && doubleJump && doubleJumpAble() == false)
        {
            //double jump
            doubleJump = false;
            velocity.y = 2*moveSetting.jumpVel;
        }
        else
        {
            //decrease velocity.y
            velocity.y -= physSetting.downAccel;
        }
    }
}