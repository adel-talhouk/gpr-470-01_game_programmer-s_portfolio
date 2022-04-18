using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField][Range(1.0f, 10.0f)] float moveSpeed = 2.5f;
    [SerializeField][Range(1.0f, 3.0f)] float sprintSpeedMultiplier = 1.5f;
    [SerializeField][Range(0.1f, 1.0f)] float crouchSpeedMultiplier = 0.5f;
    [SerializeField][Range(0.25f, 0.75f)] float crouchHeightMultiplier = 0.5f;

    [Header("Jumping")]
    [SerializeField][Range(250.0f, 1000.0f)] float jumpForce = 500f;
    [SerializeField] [Range(0.01f, 0.5f)] float groundDetectionDistance = 0.1f;
    [SerializeField] LayerMask groundLayer;

    [Header("Misc.")]
    public GameObject pauseMenu;
    public GameObject parentUI;

    //Components
    Rigidbody rb;
    CapsuleCollider capsuleCol;
    Transform groundCheckTransform;
    Health healthScript;

    //Helper data - Movement
    float xMove;
    float zMove;
    bool bIsGrounded = false;
    bool bIsSprinting = false;
    bool bIsCrouching = false;
    bool bCanMove = true;

    //Helper data - components
    float originalCapsuleColHeight;
    Vector3 originalGroundCheckLocalPos;

    // Start is called before the first frame update
    void Start()
    {
        //Get components
        rb = GetComponent<Rigidbody>();
        capsuleCol = GetComponent<CapsuleCollider>();
        groundCheckTransform = transform.Find("GroundCheck");
        healthScript = GetComponent<Health>();

        originalCapsuleColHeight = capsuleCol.height;
        originalGroundCheckLocalPos = groundCheckTransform.localPosition;

        //Pause menu and UI
        pauseMenu.SetActive(false);
        parentUI.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (healthScript.BIsAlive)
        {
            //Look for pause
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //Toggle bCanMove
                bCanMove = !bCanMove;
                GetComponentInChildren<CameraController>().bCanMove = bCanMove;

                //Toggle menu and UI
                pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
                parentUI.SetActive(!parentUI.activeInHierarchy);

                //Toggle cursor
                if (bCanMove)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }

            if (bCanMove)
            {
                //Get input
                GetInput();

                //Move - If-statement to increase efficiency - don't make a new Vector3 if you don't have to
                if (xMove != 0 || zMove != 0)
                    rb.velocity = new Vector3(0f, rb.velocity.y, 0f) + transform.TransformDirection(new Vector3(xMove, 0f, zMove)) * moveSpeed * Time.deltaTime * 100f;

            }
        }
    }

    void FixedUpdate()
    {
        //Check if grounded
        bIsGrounded = Physics.Raycast(groundCheckTransform.position, Vector3.down, groundDetectionDistance, groundLayer);
    }

    void GetInput()
    {
        xMove = Input.GetAxisRaw("Horizontal");
        zMove = Input.GetAxisRaw("Vertical");

        //Toggle sprint if moving forward 
        if (Input.GetKeyDown(KeyCode.LeftShift) && zMove > 0f)
        {
            //If crouching
            if (bIsCrouching)
            {
                Stand();
                //return;
            }

            if (!bIsSprinting)
            {
                bIsSprinting = true;
            }
            else
            {
                bIsSprinting = false;
            }
        }

        //Apply sprint speed multipler
        if (bIsSprinting)
        {
            xMove *= sprintSpeedMultiplier;
            zMove *= sprintSpeedMultiplier;

            //If sprinting but stops moving forward
            if (zMove <= 0f)
                bIsSprinting = false;
        }

        //Toggle crouch
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (!bIsCrouching)
            {
                Crouch();
            }
            else
            {
                Stand();
            }
        }

        //Apply crouch speed multiplier

        if (bIsCrouching)
        {
            xMove *= crouchSpeedMultiplier;
            zMove *= crouchSpeedMultiplier;
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
