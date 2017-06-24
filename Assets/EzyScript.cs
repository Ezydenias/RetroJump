using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EzyScript : MonoBehaviour
{
    Animator animator;
    int jumpHash = Animator.StringToHash("Jump");


	// Use this for initialization
	void Start ()
	{
	    animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    float move = Input.GetAxis("Vertical");
	    animator.SetFloat("Speed", move);

	    if (Input.GetKeyDown(KeyCode.Space))
	    {
	        animator.SetTrigger(jumpHash);
	    }
	}
}
