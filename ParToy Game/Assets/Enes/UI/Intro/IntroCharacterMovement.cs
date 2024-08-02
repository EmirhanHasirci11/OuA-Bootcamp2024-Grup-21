using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCharacterMovement : MonoBehaviour
{
    Transform tf;
    Animator anim;

    void Start()
    {
        tf = GetComponent<Transform>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (tf.position.z > -250)
        {
            tf.position = new Vector3(tf.position.x, tf.position.y, tf.position.z - Time.deltaTime * 4f);

            if (anim.GetBool("isWalking") == false)
            {
                anim.SetBool("isWalking", true);
                anim.SetBool("isRunning", true);
            }
                
        }
        else
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isRunning", false);
        }
            

        
    }
}
