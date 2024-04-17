using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CamControl : MonoBehaviour
{
    public float speed = 5.0f;
    public float sensitivity = 5.0f;
    public Image CameraImage;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                CameraImage.enabled = false;
            }
            else if (Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
                CameraImage.enabled = true;
            }
        }

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            // Move the camera forward, backward, left, and right
            transform.position += transform.forward * Input.GetAxis("Vertical") * speed * Time.deltaTime;
            transform.position += transform.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            transform.position += transform.up * Input.GetAxis("Mouse ScrollWheel") * speed * Time.deltaTime;

            if (Input.GetKey(KeyCode.E))
            {
                transform.position += transform.up * speed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                transform.position -= transform.up * speed * Time.deltaTime;
            }

            // Rotate the camera based on the mouse movement
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            transform.eulerAngles += new Vector3(-mouseY * sensitivity, mouseX * sensitivity, 0);
        }
    }
}
