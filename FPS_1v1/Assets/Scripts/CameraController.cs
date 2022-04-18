using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    //Helper data
    Transform playerTransform;
    float mousePosX;
    float mousePosY;
    float xRotation;

    [HideInInspector] public bool bCanMove = true;

    // Start is called before the first frame update
    void Start()
    {
        //Find player transform
        playerTransform = transform.parent;

        //Hide and lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (bCanMove)
        {
            //Get mouse position
            mousePosX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            mousePosY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            //Rotate camera around X axis (clamped)
            xRotation -= mousePosY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            //Rotate player around Y axis
            playerTransform.Rotate(Vector3.up * mousePosX);
        }
    }
}
