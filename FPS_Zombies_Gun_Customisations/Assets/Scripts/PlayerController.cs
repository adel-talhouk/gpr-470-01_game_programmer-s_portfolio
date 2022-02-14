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
    [Range(5.0f, 25.0f)] public float jumpForce = 10f;
    [Range(0.01f, 0.5f)] public float groundDetectionDistance = 0.1f;
    public LayerMask groundLayer;
    public Transform groundCheckTransform;

    //Components
    Rigidbody rb;
    CapsuleCollider capsuleCol;

    //Helper data - Movement
    float xMove;
    float zMove;
    bool bIsGrounded = false;
    bool bIsSprinting = false;
    bool bIsCrouching = false;

    //Helper data - components
    float originalCapsuleColHeight;
    Vector3 originalGroundCheckLocalPos;

    // Start is called before the first frame update
    void Start()
    {
        //Get components
        rb = GetComponent<Rigidbody>();
        capsuleCol = GetComponent<CapsuleCollider>();

        originalCapsuleColHeight = capsuleCol.height;
        originalGroundCheckLocalPos = groundCheckTransform.localPosition;
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

        //Re-position ground check ---------------------------------------THIS MAY NOT WORK FOR ALL VALUES, INSTEAD FIND POINT CLOSEST TO GROUND, OR FREEZE ITS POSITION IN WORLD SPACE (un-child then re-child? probably not)
        groundCheckTransform.localPosition += new Vector3(0f, capsuleCol.height * crouchHeightMultiplier, 0f);
        bIsCrouching = true;
    }

    void Stand()
    {
        capsuleCol.height = originalCapsuleColHeight;
        groundCheckTransform.localPosition = originalGroundCheckLocalPos;
        bIsCrouching = false;
    }
}
