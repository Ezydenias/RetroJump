using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EzyScript : MonoBehaviour
{
    Animator animator;
    int jumpHash = Animator.StringToHash("Jump");
    int doubleJumpHash = Animator.StringToHash("DoubleJump");
    int groundedHash = Animator.StringToHash("GroundedTotally");
    private bool Jumped = false;        //checks if character jumps for doublejump animation
    public float distToGrounded = 15.0f;
    public LayerMask ground;
    Transform tr;


    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        tr = transform;
    }

    //checks if character is grounded
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGrounded);
    }

    void LateUpdate()
    {
        if ((IsGrounded()) && Jumped)
        {
            animator.SetTrigger(groundedHash);
            Jumped = false;
        }
    }

// Update is called once per frame
    void Update()
    {
        float move = Input.GetAxis("Vertical");
        animator.SetFloat("Speed", move);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Jumped == false)        //if character doesn't jump, it jumps
            {
                animator.SetTrigger(jumpHash);
                animator.SetBool("Grounded", false);
                Jumped = true;
            }
            else                        //if character jumped, performed the doublejump animation
            {  
                animator.SetTrigger(doubleJumpHash);
                //Jumped = false;
            }
        }
    }
}