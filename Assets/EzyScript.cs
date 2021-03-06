﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EzyScript : MonoBehaviour
{
    Animator animator;
    Rigidbody rBody;
    int jumpHash = Animator.StringToHash("Jump");
    int doubleJumpHash = Animator.StringToHash("DoubleJump");
    int fallingHash = Animator.StringToHash("Falls");
    int groundedHash = Animator.StringToHash("Grounded");
    int gunTriggerHash = Animator.StringToHash("ArmUnarm");
    int jumpStartHash = Animator.StringToHash("JumpStart");
    private int isGrounded = Animator.StringToHash("isGrounded");
    public float distToGrounded = 15.1f;
    public LayerMask ground;

    private bool falls = false;
    private bool Jumped = false; //checks if character jumps for doublejump animation
    private bool doublejumped = false;
    public float raisingDistance = 0;
    private Vector3 oldPosition;

    public bool hit=false;

    // Use this for initialization
    void Start()
    {
       
        animator = GetComponent<Animator>();
        if (GetComponent<Rigidbody>())
            rBody = GetComponent<Rigidbody>();
        else
            Debug.LogError("the Character needs a rigidbody, DODOING!!!");

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
            animator.SetBool(isGrounded, false);
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
            animator.SetBool(isGrounded, false);
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
        float move = Input.GetAxis("Vertical");
        raisingDistance = (int)rBody.velocity.z;

        if ((int) rBody.velocity.z != 0)
        {
            animator.SetFloat("Speed", move);
        }
        else
        {
            animator.SetFloat("Speed", 0.0f);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            animator.SetTrigger(gunTriggerHash);
            if (animator.GetBool("Armed"))
            {
                animator.SetBool("Armed", false);
            }
            else
            {
                animator.SetBool("Armed",true);
            }
        }


        if (!IsFalling())
        {
            jump();
        }
        else
        {
            if (IsGrounded())
            {
                // if (falls || Jumped)
                animator.SetTrigger(groundedHash);

                falls = false;
                // }
                Jumped = false;
            }
        }

        if (IsGrounded())
        {
            animator.SetBool(isGrounded, true);
        }
        else
        {
            animator.SetBool(isGrounded, false);
        }


        //var currentBaseState = animator.GetCurrentAnimatorStateInfo(0);
        //if (/*IsGrounded() &&*/ currentBaseState.nameHash == jumpHash)
        //{
        //animator.SetTrigger(groundedHash);

        ////  falls = false;
        //// }
        //Jumped = false;
        //}
    }
}