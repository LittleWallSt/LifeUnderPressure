using TMPro;
using UnityEngine;

public class SubmarineMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float sideSpeedLimit = 3f;
    [SerializeField] private float forwardSpeedLimit = 4f;
    [SerializeField] private float backwardSpeedLimit = 2f;
    [SerializeField] private float floatSpeedLimit = 2f;
    [Header("Rotation")]
    [SerializeField] private float rotationLimit = 20f;
    [SerializeField] private float rotationSpeed = 20f;
    [Header("Bumping")]
    [SerializeField] private float maxBumpDuration = 2f;
    [Header("Debug")]
    [SerializeField] private TMP_Text inputText = null;
    [SerializeField] private TMP_Text inputMouseText = null;
    [SerializeField] private TMP_Text velocityText = null;
    [SerializeField] private TMP_Text rotationText = null;
    [SerializeField] private TMP_Text speedText = null;
    [SerializeField] private TMP_Text bumpText = null;
    [SerializeField] private TMP_Text rotationVelocityText = null;

    private bool bumped = false;
    private float bump = 0f;

    private Rigidbody rb;
    private Vector3 input;
    private Vector2 mouse;
    private Vector2 rotationVelocity;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        input = new Vector3();
        rotationVelocity = Vector3.zero;

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

        DebugUpdate();
    }

    private void DebugUpdate()
    {
        // Debug texts
        if (inputText) inputText.text = "Input: " + input.x + "; " + input.y + "; " + input.z;
        if (inputMouseText) inputMouseText.text = "Mouse: " + mouse.x + "; " + mouse.y;
        if (velocityText) velocityText.text = "Velocity: " + rb.velocity;
        if (rotationText) rotationText.text = "Rotation: " + transform.eulerAngles.x + "; " + transform.eulerAngles.y + "; " + transform.eulerAngles.z;
        if (speedText) speedText.text = "Speed: " + rb.velocity.magnitude;
        if (bumpText) bumpText.text = "bumped: " + bumped;
        if (rotationVelocityText) rotationVelocityText.text = "R. Velocity: " + rotationVelocity;
    }

    private void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;

        VelocityUpdate(deltaTime);
        RotationUpdate(deltaTime);
        PositionUpdate(deltaTime);

    }

    private void PositionUpdate(float deltaTime)
    {
        // Bumping process
        if (bumped)
        {
            bump -= deltaTime;
            if (bump <= 0f) bumped = false;
            return;
        }

        // Update position
        Vector3 inputModified = new Vector3(
            input.x * sideSpeedLimit,
            input.y * floatSpeedLimit,
            input.z * (input.z >= 0f ? forwardSpeedLimit : backwardSpeedLimit)
            );
        if (input.x != 0f && input.z != 0f)
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

    private void VelocityUpdate(float deltaTime)
    {
        // Update rotation velocity
        if (mouse == Vector2.zero)
        {
            rotationVelocity = Vector2.Lerp(rotationVelocity, Vector2.zero, deltaTime / 2f);
        }
        else rotationVelocity = Vector2.Lerp(rotationVelocity, Vector2.zero, deltaTime);

        // Update Velocity
        if (input == Vector3.zero)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, deltaTime / 2f);
        }
        else rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, deltaTime);
    }

    private void RotationUpdate(float deltaTime)
    {


        // Calculate new rotation
        rotationVelocity += new Vector2(-mouse.y, mouse.x) * deltaTime;
        Vector3 rotation = transform.eulerAngles + (Vector3)rotationVelocity;

        // Check for rotation X limit
        if (rotation.x > rotationLimit && rotation.x < 180f)
        {
            rotation.x = rotationLimit;
        }
        else if (rotation.x < 360f - rotationLimit && rotation.x >= 180f)
        {
            rotation.x = 360f - rotationLimit;
        }

        // Update rotation
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, rotation, rotationSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        BumpCollision(collision);
    }

    private void BumpCollision(Collision collision)
    {
        // Bumping
        Vector3 impulse = collision.GetContact(0).impulse;

        bump = impulse.magnitude * 0.25f;
        bumped = impulse.magnitude > 0f ? true : false;
        rb.velocity += impulse * 0.8f;
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
