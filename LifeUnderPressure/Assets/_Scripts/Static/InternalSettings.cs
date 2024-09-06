using UnityEngine;

public class InternalSettings : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Color selectedCellColor = Color.white;
    [SerializeField] private Color defaultCellColor = Color.white;
    [Header("Debug")]
    [SerializeField] private GUIStyle debugStyle = null;

    private Vector2 lastMousePosition = Vector3.zero;
    private Vector2 mouseDelta = Vector3.zero;

    public static InternalSettings Get { get; private set; } = null;
    private void Awake()
    {
        if (Get == null) Get = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        lastMousePosition = Input.mousePosition;
    }
    public static Vector2 MouseDelta => Get.mouseDelta;

    private void Update()
    {
        mouseDelta = (Vector2)Input.mousePosition - lastMousePosition;
        lastMousePosition = Input.mousePosition;
    }
    // Debug
    public GUIStyle DebugStyle => debugStyle;
}
