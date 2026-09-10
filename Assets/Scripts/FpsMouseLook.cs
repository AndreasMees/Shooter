using UnityEngine;

// Rotate the player horizontally and the child camera vertically.
public class FpsMouseLook : MonoBehaviour
{
    public Transform playerBody;
    public float mouseSensitivity = 200f;
    public float verticalLookLimit = 85f;

    float verticalRotation;

    void Awake()
    {
        if (playerBody == null)
            playerBody = transform.root;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        verticalRotation = Mathf.Clamp(verticalRotation - mouseY, -verticalLookLimit, verticalLookLimit);
        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
