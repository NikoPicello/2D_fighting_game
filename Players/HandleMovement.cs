using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleMovement : MonoBehaviour
{

    Rigidbody2D rb; 
    StateManager states;
    HandleAnimations anim; 

    public float acceleration = 30; 
    public float airAcceleration = 15; 
    public float maxSpeed = 20; 
    public float jumpSpeed = 8; 
    public float jumpDuration = 150; 
    float actualSpeed; 
    bool justJumped; 
    bool canJump; 
    float jumpTimer; 

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        states = GetComponent<StateManager>();
        anim = GetComponent<HandleAnimations>();
        rb.freezeRotation = true; 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!states.dontMove){
            HorizontalMovement();
            Jump();
        }
    }

    void HorizontalMovement()
    {
        actualSpeed = this.maxSpeed;

        if (states.onGround && !states.currentlyAttacking){
            rb.AddForce(new Vector2((states.horizontal * actualSpeed) - rb.velocity.x * this.acceleration, 0 )); // 0 is referred to the vertical axis
        }

        // in case there's sliding
        if (states.horizontal == 0 && states.onGround){
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    void Jump()
    {
        if (states.vertical > 0){
            if (!justJumped){
                justJumped = true; 
                if (states.onGround){
                    anim.JumpAnim();

                    rb.velocity = new Vector3(rb.velocity.x, this.jumpSpeed);
                    jumpTimer = 0; 
                    canJump = true;
                }else{
                    if (canJump){
                        jumpTimer += Time.deltaTime; 
                        if (jumpTimer < this.jumpDuration / 1000){
                            rb.velocity = new Vector3(rb.velocity.x, this.jumpSpeed);
                        }
                    }
                }
            }else{
                justJumped = false;
            }
        }
    }

    public void AddVelocityOnCharacter(Vector3 direction, float timer)
    {
        StartCoroutine(AddVelocity(timer, direction));
    }

    IEnumerator AddVelocity(float timer, Vector3 direction)
    {
        float t = 0; 

        while (t < timer){
            t += Time.deltaTime;

            rb.velocity = direction;
            yield return null; 
        }
    }
}

