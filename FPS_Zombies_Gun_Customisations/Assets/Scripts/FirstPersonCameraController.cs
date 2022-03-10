using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Thank you Brackeys for camera controls: https://youtu.be/_QajrabyTJc
public class FirstPersonCameraController : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    [Header("References")]
    public GameObject attachmentsMenu;

    //Helper data
    Transform playerTransform;
    float mousePosX;
    float mousePosY;
    float xRotation;
    bool bMenuIsOpen = false;

    public bool BMenuIsOpen { get { return bMenuIsOpen; } }

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
        //INPUT KEY DOWN - TAB - Open/Close attachments menu
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            attachmentsMenu.SetActive(!attachmentsMenu.activeSelf);
            bMenuIsOpen = attachmentsMenu.activeSelf;

            if (bMenuIsOpen)
            {
                //Show and free cursor
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                //Hide and lock cursor
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        if (!bMenuIsOpen)
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
