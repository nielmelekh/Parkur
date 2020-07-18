using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Rigidbody playerRB;
    public Transform groundCheck;
    public Transform rightCheck;
    public Transform leftCheck;
    public LayerMask groundMask;

    public float speed;
    public float gravity;
    public float jumpHight;
    public float sideJumpFromWalls;
    public float groundDistance = 0.1f;
    public float sideCheckRadius;

    Vector3 velocity;
    bool isGrounded;
    bool isRightCheck;
    bool isLeftCheck;
    float currentSpeed;
    float currentGravity;

    public float cameraEngleOnWallRun; 

    int state;
    public const int normal = 0;
    public const int wallRunState = 1;

    void Start()
    {
        
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded)
        {
            currentSpeed = speed;
        }
        else
        {
            currentSpeed = 3*speed / 4;
        }

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        wallRun();

        //state = normal;

        if(state == wallRunState)
        {
            x = 0;
            z = 1;
            currentGravity = gravity / 10;
            velocity.y = -2f;
        }
        else
        {
            currentGravity = gravity;
        }

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * currentSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump"))
        {
            if(state == wallRunState && isRightCheck && !isLeftCheck)
            {
                velocity.y = Mathf.Sqrt(jumpHight * -2f * currentGravity);
                //controller.Move(transform.right * -sideJumpFromWalls);
                playerRB.AddRelativeForce(transform.right * -sideJumpFromWalls, ForceMode.Impulse);
            }
            else if(state == wallRunState && isLeftCheck && !isRightCheck)
            {
                velocity.y = Mathf.Sqrt(jumpHight * -2f * currentGravity);
                //controller.Move(transform.right * sideJumpFromWalls);
                playerRB.AddRelativeForce(transform.right * -sideJumpFromWalls, ForceMode.Impulse);
                Debug.Log("Entered");
            }
            else if (isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHight * -2f * currentGravity);
            }
        }

        velocity.y += currentGravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }

    private void wallRun()
    {
        isRightCheck = Physics.CheckSphere(rightCheck.position, sideCheckRadius, groundMask);
        isLeftCheck = Physics.CheckSphere(leftCheck.position, sideCheckRadius, groundMask);

        if (!isGrounded && isRightCheck && !isLeftCheck)
        {
            state = wallRunState;
            //Camera.main.transform.rotation = Quaternion.AngleAxis(cameraEngleOnWallRun, Vector3.forward);
        }
        else if (!isGrounded && isLeftCheck && !isRightCheck)
        {
            state = wallRunState;
            //Camera.main.transform.rotation = Quaternion.AngleAxis(-cameraEngleOnWallRun, Vector3.forward);
        }
        else if (!isGrounded && isRightCheck && isLeftCheck)
        {
            state = wallRunState;
            //Camera.main.transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
        }
        else
        {
            state = normal;
            //transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(rightCheck.position, sideCheckRadius);
        Gizmos.DrawWireSphere(leftCheck.position, sideCheckRadius);
    }
}
