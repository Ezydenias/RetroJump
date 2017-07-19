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
        public float distToWall = 1.0f;
        public float dampening = 0.01f;
        public float superSpeed = 5.0f;
        public Vector3 lowWall = new Vector3(0, -10, 0);
        public Vector3 highWall = new Vector3(0, 10, 0);
        public LayerMask ground;
        public LayerMask wall;
    }

    [System.Serializable]
    public class PhysSettings
    {
        public float downAccel = 0.75f;
        public float groundFriction = 10.0f;
        public float groundFrictionOverdrive = 100.0f;
        public float airFriction = 100.0f;
        public float pushVelocity = 10.0f;

        [HideInInspector] public float lastPositionY = 0.0f;
        [HideInInspector] public Vector3 pushXDistance = new Vector3(5.0f, 0.0f, 0.0f);
        [HideInInspector] public Vector3 pushZDistance = new Vector3(0.0f, 0.0f, 5.0f);
    }

    [System.Serializable]
    public class InputSettings
    {
        public float inputDelay = 0.1f;
        public string FORWARD_AXIS = "Vertical";
        public string TURN_AXIS = "Horizontal";
        public string JUMP_AXIS = "Jump";
    }

    [System.Serializable]
    public class DebugInfo
    {
        public bool wallHitting = false;
        public float fallVelocityShow = 0.0f;
    }

    public MoveSettings moveSetting = new MoveSettings();
    public PhysSettings physSetting = new PhysSettings();
    public InputSettings inputSetting = new InputSettings();
    public DebugInfo debugInfo = new DebugInfo();

    public bool doubleJump = true;
    Vector3 targetVelocity = Vector3.zero;
    Vector3 currentVelocity = Vector3.zero;
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

    private void PushFromGround()
    {
        if (Physics.Raycast(transform.position + physSetting.pushXDistance, Vector3.down, moveSetting.distToGrounded,
            moveSetting.ground))
        {
            targetVelocity.x += physSetting.pushVelocity;
        }
        else if (Physics.Raycast(transform.position - physSetting.pushXDistance, Vector3.down,
            moveSetting.distToGrounded,
            moveSetting.ground))
        {
            targetVelocity.x -= physSetting.pushVelocity;
        }
        if (Physics.Raycast(transform.position + physSetting.pushZDistance, Vector3.down, moveSetting.distToGrounded,
            moveSetting.ground))
        {
            targetVelocity.z += physSetting.pushVelocity;
        }
        else if (Physics.Raycast(transform.position - physSetting.pushZDistance, Vector3.down,
            moveSetting.distToGrounded,
            moveSetting.ground))
        {
            targetVelocity.z -= physSetting.pushVelocity;
        }

        targetVelocity.y = 3;
    }

    bool HitWall(Vector3 v)
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(v),
                moveSetting.distToWall, moveSetting.wall) ||
            Physics.Raycast(transform.position + moveSetting.lowWall, transform.TransformDirection(v),
                moveSetting.distToWall, moveSetting.wall) ||
            Physics.Raycast(transform.position + moveSetting.highWall, transform.TransformDirection(v),
                moveSetting.distToWall, moveSetting.wall))
        {
            return true;
        }
        else
        {
            return false;
        }
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
        //debug infos
        Debug.DrawLine(transform.position, transform.TransformDirection(Vector3.forward));
        //debugInfo.wallHitting = HitWall();
        debugInfo.fallVelocityShow = transform.position.y;
        //end debug infos
        setCurrentVelocity();

        rBody.velocity = transform.TransformDirection(currentVelocity);
        targetVelocity.z = 0;
        targetVelocity.x = 0;
    }

    void setCurrentVelocity()
    {
        currentVelocity.y = targetVelocity.y;


        if (currentVelocity.x > 0)
        {
            if (targetVelocity.x > currentVelocity.x)
            {
                currentVelocity.x = targetVelocity.x;
            }
            if (targetVelocity.x < 0)
            {
                currentVelocity.x += targetVelocity.x;
            }
        }
        else if (currentVelocity.x < 0)
        {
            if (targetVelocity.x < currentVelocity.x)
            {
                currentVelocity.x = targetVelocity.x;
            }
            if (targetVelocity.x > 0)
            {
                currentVelocity.x += targetVelocity.x;
            }
        }
        else
        {
            currentVelocity.x = targetVelocity.x;
        }

        if (currentVelocity.z > 0)
        {
            if (targetVelocity.z > currentVelocity.z)
            {
                currentVelocity.z = targetVelocity.z;
            }
            if (targetVelocity.z < 0)
            {
                currentVelocity.z += targetVelocity.z;
            }
        }
        else if (currentVelocity.z < 0)
        {
            if (targetVelocity.z < currentVelocity.z)
            {
                currentVelocity.z = targetVelocity.z;
            }
            if (targetVelocity.z > 0)
            {
                currentVelocity.z += targetVelocity.z;
            }
        }
        else
        {
            currentVelocity.z = targetVelocity.z;
        }

        if (Grounded())
        {
            currentVelocity.x -= currentVelocity.x / physSetting.groundFriction;
            if (currentVelocity.z < moveSetting.forwardVelocity)
            {
                currentVelocity.z -= currentVelocity.z / physSetting.groundFriction;
            }
            else
            {
                currentVelocity.z -= currentVelocity.z / physSetting.groundFrictionOverdrive;
            }
        }
        else
        {
            currentVelocity.x -= currentVelocity.x / physSetting.airFriction;
            currentVelocity.z -= currentVelocity.z / physSetting.airFriction;
        }
    }

    void Run()
    {
        if (Mathf.Abs(forwardInput) > inputSetting.inputDelay)
        {
            //go check if in front of obstacle for forwards
            if (HitWall(Vector3.forward) == false && forwardInput > 0)
            {
                targetVelocity.z = moveSetting.forwardVelocity * forwardInput;
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    targetVelocity.z *= moveSetting.superSpeed;
                }
            } //check if an obstacle is behind if backwards
            if (HitWall(Vector3.back) == false && forwardInput < 0)
            {
                targetVelocity.z = moveSetting.forwardVelocity * forwardInput;
            }
        }
        else
        {
            //else stay put

            targetVelocity.z = 0;
            targetVelocity.x = 0;
        }
        //check if player needs to be pushed from ledge still very buggy
        //if (physSetting.pushVelocity != Vector3.zero)
        //{
        //    targetVelocity.z += -physSetting.pushVelocity.z;
        //    targetVelocity.x += -physSetting.pushVelocity.x;

        //    physSetting.pushVelocity.x = physSetting.pushVelocity.x * (4.0f / 5.0f);
        //    physSetting.pushVelocity.z = physSetting.pushVelocity.z * (4.0f / 5.0f);
        //    if (Mathf.Abs(physSetting.pushVelocity.x) < 1)
        //    {
        //        physSetting.pushVelocity.x = 0;
        //    }
        //    if (Mathf.Abs(physSetting.pushVelocity.z) < 1)
        //    {
        //        physSetting.pushVelocity.z = 0;
        //    }
        //}
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
            targetVelocity.y = moveSetting.jumpVel;
        }
        else if (jumpInput == 0 && Grounded())
        {
            //zero out our targetVelocity.y
            doubleJump = true;
            targetVelocity.y = 0;
        }
        else if (jumpInput > 0 && Grounded() == false && doubleJump && doubleJumpAble() == false)
        {
            //double jump
            doubleJump = false;
            targetVelocity.y = 2 * moveSetting.jumpVel;
        }
        else
        {
            //decrease targetVelocity.y
            targetVelocity.y -= physSetting.downAccel;

            //check if negative felocity.y results in a downward movement still buggy
            if (targetVelocity.y < -10)
            {
                float temp = physSetting.lastPositionY - transform.position.y;
                if (temp > -0.1 && temp < 0.1)
                {
                    PushFromGround();
                }
            }

            physSetting.lastPositionY = transform.position.y;
        }
    }
}