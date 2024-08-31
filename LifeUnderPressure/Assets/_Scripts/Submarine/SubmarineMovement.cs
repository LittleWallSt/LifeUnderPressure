using TMPro;
using UnityEngine;

public class SubmarineMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float sideSpeedLimit = 3f;
    [SerializeField] private float forwardSpeedLimit = 4f;
    [SerializeField] private float floatSpeedLimit = 2f;
    [Header("Rotation")]
    [SerializeField] private float rotationLimit = 20f;
    [SerializeField] private float rotationSpeed = 20f;
    [Header("Debug")]
    [SerializeField] private TMP_Text inputText = null;
    [SerializeField] private TMP_Text inputMouseText = null;
    [SerializeField] private TMP_Text velocityText = null;
    [SerializeField] private TMP_Text rotationText = null;
    [SerializeField] private TMP_Text speedText = null;

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
        speedText.text = "Speed: " + rb.velocity.magnitude;
    }
    private void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;
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
        if(input == Vector3.zero)
        { 
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, deltaTime / 2f);
        }
        else rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, deltaTime);

        Vector3 inputModified = new Vector3(
            input.x * sideSpeedLimit, 
            input.y * floatSpeedLimit, 
            input.z * forwardSpeedLimit
            );
        if(input.x != 0f && input.z != 0f)
        {
            inputModified.x *= 0.71f;
            inputModified.z *= 0.71f;
        }
        Vector3 velocityChange = transform.rotation * new Vector3(
            inputModified.x * deltaTime,
            inputModified.y * deltaTime,
            inputModified.z * deltaTime
            );

        rb.velocity = new Vector3(
            rb.velocity.x + velocityChange.x, 
            rb.velocity.y + velocityChange.y, 
            rb.velocity.z + velocityChange.z
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
