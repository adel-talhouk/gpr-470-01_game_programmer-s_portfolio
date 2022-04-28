using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] [Range(1.0f, 10.0f)] float moveSpeed = 2.5f;
    //[SerializeField] [Range(1.0f, 3.0f)] float sprintSpeedMultiplier = 1.5f;
    //[SerializeField] [Range(0.1f, 1.0f)] float crouchSpeedMultiplier = 0.5f;
    //[SerializeField] [Range(0.25f, 0.75f)] float crouchHeightMultiplier = 0.5f;

    [Header("Combat")]
    [SerializeField] [Range(0.1f, 0.5f)] float reactionTime = 0.4f;
    [SerializeField] [Range(4f, 10f)] float coverDetectionDistance = 5f;
    [SerializeField] [Range(0f, 45f)] float coverDetectionMaxAngleFromNormal = 45f;

    //Components
    Rigidbody rb;

    //Coroutines
    Coroutine hideBehindCover;

    //Helper data
    Vector3 targetMovePosition;
    bool bIsCrouching = false;
    bool bIsHidden = false;

    // Start is called before the first frame update
    void Start()
    {
        //Components
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        LookForCover();
        MoveTowardsTargetPosition();
    }

    void LookForCover()
    {
        //Sweep direction down and up


        //Ray forward if hasn't found cover yet
        if (hideBehindCover == null && )
        {
            hideBehindCover = HideBehindCover();
            StartCoroutine(hideBehindCover);
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

            //If close enough to cover
            if (Vector3.Distance(transform.position, targetMovePosition) <= 0.25f)
            {
                //Crouch
                if (!bIsCrouching)
                    ToggleCrouch();

                bIsHidden = true;
            }
        }
    }

    void MoveTowardsTargetPosition()
    {
        //Move
        rb.velocity = new Vector3(0f, rb.velocity.y, 0f) + (targetMovePosition - transform.position).normalized * moveSpeed * Time.deltaTime * 10f;

        //If close enough
        if (Vector3.Distance(transform.position, targetMovePosition) <= 0.25f)
        {
            //Stop moving
            rb.velocity = Vector3.zero;
        }
    }

    void ToggleCrouch()
    {
        if (!bIsCrouching)
        {
            //Crouch

            bIsCrouching = true;
        }
        else
        {
            //Stand

            bIsCrouching = false;

            //If hidden, no longer
            if (bIsHidden)
                bIsHidden = false;
        }
    }
}
