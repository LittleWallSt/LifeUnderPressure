using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SubmarineMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float sideSpeed = 5f;
    [SerializeField] private float forwardSpeed = 5f;
    [SerializeField] private float floatSpeed = 5f;
    [Header("Rotation")]
    [SerializeField] private float rotationLimit = 20f;
    [SerializeField] private float rotationSpeed = 20f;
    [Header("Debug")]
    [SerializeField] private TMP_Text inputText = null;
    [SerializeField] private TMP_Text inputMouseText = null;
    [SerializeField] private TMP_Text velocityText = null;
    [SerializeField] private TMP_Text rotationText = null;

    private Rigidbody rb;
    private Vector3 input;
    private Vector2 mouse;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        input = new Vector3();

        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        // Get input
        float inputUp = Input.GetKey(KeyCode.Space) ? 1f : Input.GetKey(KeyCode.LeftControl) ? -1f : 0f;
        input = new Vector3(Input.GetAxisRaw("Horizontal"), inputUp, Input.GetAxisRaw("Vertical"));
        mouse = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        // Debug texts
        inputText.text = "Input: " + input.x + "; " + input.y + "; " + input.z;
        inputMouseText.text = "Mouse: " + mouse.x + "; " + mouse.y;
        velocityText.text = "Velocity: " + rb.velocity;
        rotationText.text = "Rotation: " + transform.eulerAngles.x + "; " + transform.eulerAngles.y + "; " + transform.eulerAngles.z;
    }
    private void FixedUpdate()
    {
        // Calculate new rotation
        Vector3 rotation = transform.eulerAngles + new Vector3(-mouse.y, mouse.x, 0f);

        // Check for rotation X limit
        if (rotation.x > rotationLimit && rotation.x < 180f)
        {
            rotation.x = rotationLimit;
        }
        else if (rotation.x < 360f - rotationLimit && rotation.x >= 180f)
        {
            rotation.x = 360f - rotationLimit;
        }
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, rotation, rotationSpeed * Time.fixedDeltaTime);

        // Update Velocity
        rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.fixedDeltaTime);

        Vector3 inputModified = new Vector3(
            input.x * sideSpeed, 
            input.y * floatSpeed, 
            input.z * forwardSpeed
            );
        Vector3 velocityChange = new Vector3(
            inputModified.x * Time.fixedDeltaTime,
            inputModified.y * Time.fixedDeltaTime,
            inputModified.z * Time.fixedDeltaTime
            );
        rb.velocity = new Vector3(
            Mathf.Clamp(rb.velocity.x + velocityChange.x, -2f, 2f), 
            Mathf.Clamp(rb.velocity.y + velocityChange.y, -2f, 2f), 
            Mathf.Clamp(rb.velocity.z + velocityChange.z, -1f, 3f)
            );


    }


    public static Vector3 PositionFlat(Vector3 position)
    {
        return new Vector3(position.x, 0f, position.z);
    }
    public static Vector3 PositionFlatNormalized(Vector3 position)
    {
        return new Vector3(position.x, 0f, position.z).normalized;
    }
}
