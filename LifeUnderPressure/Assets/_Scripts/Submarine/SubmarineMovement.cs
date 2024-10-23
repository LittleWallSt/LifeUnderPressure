using TMPro;
using UnityEngine;
using FMOD.Studio;
using System.Collections;

public class SubmarineMovement : MonoBehaviour
{
    [SerializeField] private bool debugMode = false;
    [SerializeField] private Camera submarineCamera = null;
    [Header("Screen Shake")]
    [SerializeField] private float screenShakeTimer = 0.5f;
    [SerializeField] private float screenShakeFrequency = 0.1f;
    [SerializeField] private Vector2 shakePositionOffset = new Vector2(0.001f, 0.005f);
    [SerializeField] private Vector2 shakeRotationOffset = new Vector2(-3f, 3f);
    [Header("Movement")]
    [SerializeField] private MovementVector movementVector;
    [Header("Rotation")]
    [SerializeField] private float rotationLimit = 20f;
    [SerializeField] private float rotationSpeed = 20f;
    [Header("Bumping")]
    [SerializeField] private float maxBumpDuration = 2f;
    [SerializeField] private float bumpDurationModifier = 0.25f;
    [SerializeField] private float bumpStrength = 0.8f;
    [SerializeField] private float bumpDamageModifier = 10f;

    private bool bumped = false;
    private float bumpDuration = 0f;    

    private bool shaking = false;
    private float shakeDuration = 0f;

    private Rigidbody rb;
    private Vector3 input;
    private Vector2 mouse;
    private Vector2 rotationVelocity;

    // Janko and Aleksis
    private EventInstance propellerSFX;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        input = new Vector3();
    }
    private void OnEnable()
    {
        ResetMovement();
    }

    private void Start()
    {
        // Janko and Aleksis
        propellerSFX = AudioManager.instance.CreateInstance(FMODEvents.instance.propellerSFX);
        propellerSFX.setParameterByName("Input", 0f);
        propellerSFX.start();
    }

    public void ResetMovement()
    {
        rotationVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
    }
    public void SetMovementVector(MovementVector newVector)
    {
        movementVector = newVector;
    }
    private void Update()
    {
        // Get input
        float inputUp = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Space) ? 1f : Input.GetKey(KeyCode.LeftControl) ? -1f : 0f;
        input = new Vector3(Input.GetAxisRaw("Horizontal"), inputUp, Input.GetAxisRaw("Vertical"));
        mouse = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        // Janko and Aleksis 
        propellerSFX.setParameterByName("Input", input.magnitude);
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
            bumpDuration -= deltaTime;
            if (bumpDuration <= 0f) bumped = false;
            return;
        }

        // Update position
        Vector3 inputModified = new Vector3(
            input.x * movementVector.side,
            input.y * movementVector.upward,
            input.z * (input.z >= 0f ? movementVector.forward : movementVector.backward)
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

        if (UnderwaterCurrent.Triggering.Count != 0)
        {
            UnderwaterCurrent curr = UnderwaterCurrent.Triggering[0];
            Vector3 v = curr.GetPointOnCurrent(transform.position);
            float distance = Vector3.Distance(transform.position, v);
            float strength = Mathf.Abs(1f - Mathf.Clamp01(distance / curr.Radius));

            velocityChange += curr.Direction * (curr.Strength * strength) * Time.deltaTime;
        }
        rb.velocity += new Vector3(
            velocityChange.x,
            velocityChange.y,
            velocityChange.z
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
        if(collision.transform.gameObject.layer == 8 || collision.transform.gameObject.layer == 4) // just for now to block going higher than water surface, 8 is fish layer
        {
            return;
        }
        // Bumping
        Vector3 impulse = collision.GetContact(0).impulse;

        bumpDuration = impulse.magnitude * bumpDurationModifier;
        bumped = impulse.magnitude > 0f ? true : false;
        rb.velocity += impulse * bumpStrength;

        float damage = impulse.magnitude * bumpDamageModifier;
        GetComponent<Health>().DealDamage(damage, DamageType.Crashed);

        if (shaking)
        {
            shakeDuration = 0f;
        }
        else
        {
            StartCoroutine(ScreenShake());
        }

        // Janko >>
        AudioManager.instance.PlayOneShot(FMODEvents.instance.SFX_Collision, collision.transform.position);
        // Janko <<
    }
    private IEnumerator ScreenShake()
    {
        shaking = true;
        shakeDuration = 0f;
        Vector3 startPosition = submarineCamera.transform.localPosition;
        Vector3 recPosition = submarineCamera.transform.localPosition;
        Vector3 targetPosition = submarineCamera.transform.localPosition;

        float power = 0f;

        targetPosition = startPosition + new Vector3(Random.Range(shakePositionOffset.x, shakePositionOffset.y), Random.Range(shakePositionOffset.x, shakePositionOffset.y), Random.Range(shakePositionOffset.x, shakePositionOffset.y));

        // lerp to random spots while timer is on
        while (shakeDuration < screenShakeTimer)
        {
            submarineCamera.transform.localPosition = Vector3.Lerp(recPosition, targetPosition, power);

            power += Time.deltaTime / screenShakeFrequency;
            if (power >= 1f)
            {
                power = 0f;
                recPosition = targetPosition;
                targetPosition = startPosition + new Vector3(Random.Range(shakePositionOffset.x, shakePositionOffset.y), Random.Range(shakePositionOffset.x, shakePositionOffset.y), Random.Range(shakePositionOffset.x, shakePositionOffset.y));
            }
            shakeDuration += Time.deltaTime;
            yield return null;
        }

        // lerp to start position
        power = 0f;
        recPosition = submarineCamera.transform.localPosition;
        while(power < 1f)
        {
            submarineCamera.transform.localPosition = Vector3.Lerp(recPosition, startPosition, power);

            power += Time.deltaTime / screenShakeFrequency;
            yield return null;
        }

        submarineCamera.transform.localPosition = startPosition;
        shaking = false;
    }
    public static Vector3 PositionFlat(Vector3 position)
    {
        return new Vector3(position.x, 0f, position.z);
    }
    public static Vector3 PositionFlatNormalized(Vector3 position)
    {
        return new Vector3(position.x, 0f, position.z).normalized;
    }

    private void OnGUI()
    {
        if (!debugMode) return;

        GUI.Label(new Rect(10, 10, 500, 80), string.Format("Input: {0}; {1}; {2}", input.x, input.y, input.z), InternalSettings.Get.DebugStyle);
        GUI.Label(new Rect(10, 80, 500, 80), string.Format("Mouse: {0}; {1}", mouse.x, mouse.y), InternalSettings.Get.DebugStyle);
        GUI.Label(new Rect(10, 150, 500, 80), string.Format("Velocity: {0}", rb.velocity), InternalSettings.Get.DebugStyle);
        GUI.Label(new Rect(10, 220, 500, 80), string.Format("Rotation: {0}; {1}; {2}", transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z), InternalSettings.Get.DebugStyle);
        GUI.Label(new Rect(10, 290, 500, 80), string.Format("Speed: {0}", rb.velocity.magnitude), InternalSettings.Get.DebugStyle);
        GUI.Label(new Rect(10, 360, 500, 80), string.Format("Bumped: {0}", bumped), InternalSettings.Get.DebugStyle);
        GUI.Label(new Rect(10, 430, 500, 80), string.Format("R. Velocity: {0}", rotationVelocity), InternalSettings.Get.DebugStyle);
        GUI.Label(new Rect(10, 500, 500, 80), string.Format("Time: {0}", Time.time), InternalSettings.Get.DebugStyle);
    }

    private void OnDestroy()
    {
        // Janko and Aleksis
        propellerSFX.stop(STOP_MODE.ALLOWFADEOUT);
    }
}