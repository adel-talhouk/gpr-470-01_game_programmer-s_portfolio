using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [Range(1.0f, 5.0f)] public float moveSpeed = 2.5f;
    [Range(1.0f, 3.0f)] public float sprintSpeedMultiplier = 1.5f;
    [Range(0.1f, 1.0f)] public float crouchSpeedMultiplier = 0.5f;
    [Range(0.25f, 0.75f)] public float crouchHeightMultiplier = 0.5f;

    [Header("Jumping")]
    [Range(1.0f, 5.0f)] public float jumpForce;
    [Range(0.01f, 0.5f)] public float groundDetectionDistance;
    public LayerMask groundLayer;
    public Transform groundCheckTransform;

    //Components
    Rigidbody rb;
    CapsuleCollider capsuleCol;

    //Helper data
    float xMove;
    float zMove;
    float originalCapsuleColHeight;
    bool bIsGrounded = false;
    bool bIsSprinting = false;
    bool bIsCrouching = false;

    // Start is called before the first frame update
    void Start()
    {
        //Get components
        rb = GetComponent<Rigidbody>();
        capsuleCol = GetComponent<CapsuleCollider>();

        originalCapsuleColHeight = capsuleCol.height;
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();

        Move();
    }

    void FixedUpdate()
    {
        //Check if grounded
        bIsGrounded = Physics.Raycast(groundCheckTransform.position, Vector3.down,groundDetectionDistance, groundLayer);
    }

    void GetInput()
    {
        xMove = Input.GetAxisRaw("Horizontal") * moveSpeed;
        zMove = Input.GetAxisRaw("Vertical") * moveSpeed;

        //Toggle sprint
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            //If crouching
            if (bIsCrouching)
            {
                Stand();
                return;
            }

            if (!bIsSprinting)
            {
                //Modify movement speed
                xMove *= sprintSpeedMultiplier;
                zMove *= sprintSpeedMultiplier;

                bIsSprinting = true;
            }
            else
            {
                bIsSprinting = false;
            }
        }

        //Toggle crouch
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (!bIsCrouching)
            {
                //Modify movement speed
                xMove *= crouchSpeedMultiplier;
                zMove *= crouchSpeedMultiplier;

                Crouch();
            }
            else
            {
                Stand();
            }
        }

        //Check if trying to jump
        if (Input.GetKeyDown(KeyCode.Space) && bIsGrounded)
        {
            //If crouching
            if (bIsCrouching)
            {
                Stand();
                return;
            }

            Jump();
        }
    }

    void Move()
    {
        //If-statement to increase efficiency - don't make a new Vector3 if you don't have to
        if (xMove != 0 || zMove != 0)
            rb.velocity = new Vector3(xMove, rb.velocity.y, zMove);
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce);
    }

    void Crouch()
    {
        capsuleCol.height *= crouchHeightMultiplier;
        bIsCrouching = true;
    }

    void Stand()
    {
        capsuleCol.height = originalCapsuleColHeight;
        bIsCrouching = false;
    }
}
