using System;
using System.Collections;
using UnityEngine;

public class InternalSettings : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Color selectedCellColor = Color.white;
    [SerializeField] private Color defaultCellColor = Color.white;
    [Header("Layers")]
    [SerializeField] private LayerMask fishLayer = new LayerMask();
    [SerializeField] private LayerMask environmentLayer = new LayerMask();
    [SerializeField] private LayerMask submarineLayer = new LayerMask();
    [Header("Debug")]
    [SerializeField] private GUIStyle debugStyle = null;

    private Vector2 lastMousePosition = Vector3.zero;
    private Vector2 mouseDelta = Vector3.zero;

    public static bool DataLoaded { get; private set; } = false;
    public static int SunlightZone { get; } = 0;
    public static int TwilightZone { get; } = 1;
    public static int MidnightZone { get; } = 2;
    public static int CaveZone { get; } = 3;
    public static int MainMenu { get; } = 4;
    public static string MainMenuSceneName { get; } = "MainMenu";
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
        DataLoaded = false;

        StartCoroutine(LoadDataProcess());
    }
    private IEnumerator LoadDataProcess()
    {
        DataManager.Init();
        yield return StartCoroutine(DataManager.LoadAllData());

        yield return null;
        DataLoaded = true;
    }
    public static IEnumerator WaitForDataLoading()
    {
        while (!DataLoaded)
        {
            Debug.Log("waitng");
            if (!Get) throw new Exception("NO INTERNAL SETTINGS IN THE SCENE");
            yield return null;
        }
        Debug.Log("stop waitng");
        yield return null;
    }
    public static LayerMask FishLayer => Get.fishLayer;
    public static LayerMask EnvironmentLayer => Get.environmentLayer;
    public static LayerMask SubmarineLayer => Get.submarineLayer;
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
    public static void EnableCursor(bool state)
    {
        // Lock and hide cursor
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = state;
    }
    // Debug
    public GUIStyle DebugStyle => debugStyle;
}
