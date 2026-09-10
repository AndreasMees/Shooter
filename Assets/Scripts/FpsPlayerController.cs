using UnityEngine;

// Move a CharacterController using WASD, jump, and simple gravity.
[RequireComponent(typeof(CharacterController))]
public class FpsPlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpHeight = 1.2f;
    public float gravity = -20f;

    [Header("First-Person View")]
    public bool hidePlayerVisual = true;

    CharacterController characterController;
    float verticalVelocity;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();

        // A player standing at world Y = 0 needs its controller centered one unit up.
        if (transform.position.y == 0f && characterController.center.y == 0f)
            characterController.center = Vector3.up * (characterController.height * 0.5f);

        if (hidePlayerVisual)
        {
            foreach (Renderer playerRenderer in GetComponentsInChildren<Renderer>())
            {
                // Keep camera children visible so a weapon can be parented to the camera later.
                if (playerRenderer.GetComponentInParent<Camera>() == null)
                    playerRenderer.enabled = false;
            }
        }
    }

    void Update()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        input = Vector3.ClampMagnitude(input, 1f);
        Vector3 movement = transform.TransformDirection(input) * moveSpeed;

        if (characterController.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        if (Input.GetButtonDown("Jump") && characterController.isGrounded)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVelocity += gravity * Time.deltaTime;
        movement.y = verticalVelocity;
        characterController.Move(movement * Time.deltaTime);
    }
}
