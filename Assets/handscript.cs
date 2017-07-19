using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class handscript : MonoBehaviour {

    Animator animator;
    public GameObject gunPrefab;

    private GameObject gun;

    // Use this for initialization
    void Start () {

        animator = GetComponent<Animator>();
        gun = Instantiate(gunPrefab, transform.position, transform.rotation);
        gun.transform.parent = this.transform;
        gun.transform.Translate(new Vector3(-2.6f,-0.5f,1));
        


    }
	
	// Update is called once per frame
	void Update () {

	    if (Input.GetKeyDown(KeyCode.Q))
	    {
	       
	        if (animator.GetBool("Armed"))
	        {
	            animator.SetBool("Armed", false);
	            gun.SetActive(false);
	        }
	        else
	        {
	            animator.SetBool("Armed", true);
	            gun.SetActive(true);
	        }
	    }


    }
}
