using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EzyScript : MonoBehaviour
{
    Animator animator;
    int jumpHash = Animator.StringToHash("Jump");
    int doubleJumpHash = Animator.StringToHash("DoubleJump");
    int fallingHash = Animator.StringToHash("Falls");
    int groundedHash = Animator.StringToHash("Grounded");
    public float distToGrounded = 15.1f;
    public LayerMask ground;

    private bool falls=false;
    private bool Jumped = false; //checks if character jumps for doublejump animation
    private bool doublejumped = false;
    public float raisingDistance;
    private Vector3 oldPosition;


    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        oldPosition = transform.position;
    }

    //checks if character is grounded
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, distToGrounded, ground);
    }

    bool IsFalling()
    {
        
        if ((transform.position.y - oldPosition.y) < -0.3)
        {
            oldPosition = transform.position;
            if (!falls)
            {
                animator.SetTrigger(fallingHash);
                falls = true;
            }
            return true;
        }
        oldPosition = transform.position;
        return false;
    }

    void LateUpdate()
    {
    }

    void jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Jumped == false) //if character doesn't jump, it jumps
            {
                animator.SetTrigger(jumpHash);
                Jumped = true;
                //falls = false;
            }
            else //if character jumped, performed the doublejump animation
            {
                animator.SetTrigger(doubleJumpHash);
               
                //falls = false;
            }
        }
    }


// Update is called once per frame
    void Update()
    {
        System.Console.WriteLine(raisingDistance);
        float move = Input.GetAxis("Vertical");
        animator.SetFloat("Speed", move);

        if (!IsFalling())
        {
            jump();
            
        }
        else
        {
            if (IsGrounded())
            {
                if (falls || Jumped)
                {
                    animator.SetTrigger(groundedHash);
                    falls = false;
                }
                Jumped = false;
                
            }
           

        }
    }
}