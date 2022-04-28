using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum EnemyState
{
    STATE_DEFAULT,
    STATE_MOVING_TO_COVER
}

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] [Range(1.0f, 10.0f)] float moveSpeed = 2.5f;
    //[SerializeField] [Range(1.0f, 3.0f)] float sprintSpeedMultiplier = 1.5f;
    //[SerializeField] [Range(0.1f, 1.0f)] float crouchSpeedMultiplier = 0.5f;
    [SerializeField] [Range(0.25f, 0.75f)] float crouchHeightMultiplier = 0.5f;

    [Header("Combat")]
    [SerializeField] [Range(0.1f, 0.5f)] float reactionTime = 0.4f;
    [SerializeField] [Range(4f, 10f)] float coverDetectionDistance = 5f;
    [SerializeField] Vector3 coverDetectionSweepingAngles;
    [SerializeField] LayerMask coverLayer;

    //Components
    Rigidbody rb;
    CapsuleCollider capsuleCol;
    Transform groundCheckTransform;
    EnemyState currentState;

    //Coroutines
    Coroutine hideBehindCoverRoutine;

    //Helper data
    Vector3 targetMovePosition;
    Transform coverDetectionSweepingTransform;
    RaycastHit coverDetectionRayHit;
    bool bIsCrouching = false;
    bool bIsHidden = false;
    bool bFoundCover = false;

    //Helper data - components
    float originalCapsuleColHeight;
    Vector3 originalGroundCheckLocalPos;

    // Start is called before the first frame update
    void Start()
    {
        //Components
        rb = GetComponent<Rigidbody>();
        capsuleCol = GetComponent<CapsuleCollider>();
        groundCheckTransform = transform.Find("GroundCheck");

        originalCapsuleColHeight = capsuleCol.height;
        originalGroundCheckLocalPos = groundCheckTransform.localPosition;

        targetMovePosition = transform.position;
        coverDetectionSweepingTransform = transform.Find("CoverDetection");
        currentState = EnemyState.STATE_DEFAULT;
    }

    // Update is called once per frame
    void Update()
    {
        LookForCover();
        MoveTowardsTargetPosition();
    }

    void LookForCover()
    {
        //If is not hidden and didn't find cover yet
        if (!bIsHidden && !bFoundCover)
        {
            //Sweep direction down and up from the forward direction
            coverDetectionSweepingTransform.rotation = Quaternion.Euler(coverDetectionSweepingAngles.x * Mathf.Abs(Mathf.Sin(Time.time)), coverDetectionSweepingAngles.y, coverDetectionSweepingAngles.z);
            Debug.DrawRay(transform.position, coverDetectionSweepingTransform.forward * coverDetectionDistance, Color.red, 0.1f);

            //Ray forward if hasn't found cover yet
            bFoundCover = Physics.Raycast(transform.position, coverDetectionSweepingTransform.forward, out coverDetectionRayHit, coverDetectionDistance, coverLayer);
            if (hideBehindCoverRoutine == null && bFoundCover)
            {
                hideBehindCoverRoutine = StartCoroutine(HideBehindCover(coverDetectionRayHit.transform.Find("HidePoint").position));
            }

        }
    }

    IEnumerator HideBehindCover(Vector3 coverPos)
    {
        if (!bIsHidden)
        {
            //Simulate reaction time
            yield return new WaitForSeconds(reactionTime);

            //Go towards cover
            targetMovePosition = coverPos;
            currentState = EnemyState.STATE_MOVING_TO_COVER;
        }
    }

    void MoveTowardsTargetPosition()
    {
        //Move
        rb.velocity = new Vector3(0f, rb.velocity.y, (targetMovePosition - transform.position).normalized.z) * moveSpeed * Time.deltaTime * 100f;

        //If close enough
        if (Vector3.Distance(transform.position, targetMovePosition) <= 0.25f)
        {
            //Act according to current state
            switch (currentState)
            {
                case EnemyState.STATE_DEFAULT:
                    {
                        //Stop moving
                        rb.velocity = Vector3.zero;

                        break;
                    }

                case EnemyState.STATE_MOVING_TO_COVER:
                    {
                        //Crouch
                        if (!bIsCrouching)
                            ToggleCrouch();

                        //Stop moving
                        rb.velocity = Vector3.zero;

                        bIsHidden = true;
                        hideBehindCoverRoutine = null;
                        bFoundCover = false;

                        break;
                    }

                default:
                    {
                        //Stop moving
                        rb.velocity = Vector3.zero;

                        break;
                    }
            }
        }
    }

    void ToggleCrouch()
    {
        if (!bIsCrouching)
        {
            //Crouch
            Crouch();

            bIsCrouching = true;
        }
        else
        {
            //Stand
            Stand();

            bIsCrouching = false;

            //If hidden, no longer
            if (bIsHidden)
                bIsHidden = false;
        }
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
