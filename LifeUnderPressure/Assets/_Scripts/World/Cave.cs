using System;
using System.Collections;
using UnityEngine;

public class Cave : MonoBehaviour, IDistanceLoad
{
    [SerializeField] private float updateTimer = 1f;
    [SerializeField] private float distanceLoadOffset = 20f;
    [SerializeField] private float collapseDelay = 3f;
    [SerializeField] private GameObject collapseParticles = null;
    [SerializeField] private GameObject entrance = null;
    [SerializeField] private GameObject exit = null;
    [SerializeField] private Animator animator = null;

    [SerializeField] private Voiceline collapseStart = null;
    [SerializeField] private Voiceline collapseEnd = null;

    private bool collapsed = false;
    private Vector3 position;

    private float updateTime = 0f;
    private int invokeIndex = -1;

    private static Action OnCaveInsideChanged;

    private static bool inside = false;
    public static bool Inside 
    {
        get => inside; 
        private set
        {
            if (inside != value)
            {
                inside = value;
                Call_OnInsideChanged();
            }
        }
    }
    private IEnumerator Start()
    {
        IDL_AssignToGameManager();
        Assign_OnInsideChanged(UpdateInside);
        SetPosition();
        Submarine.Instance.getSubmarineHealth().Assign_OnDie(OnPlayerDeath);
        yield return InternalSettings.WaitForDataLoading();
        OnDataLoaded();
    }

    private void SetPosition()
    {
        foreach (var coll in GetComponents<Collider>())
        {
            position += coll.bounds.center;
        }
        position /= GetComponents<Collider>().Length;
    }

    private void OnDataLoaded()
    {
        collapsed = DataManager.Get("CaveCollapsed", 0) == 1 ? true : false;
        UpdateCollapsed();
    }
    private void Update()
    {
        updateTime += Time.fixedDeltaTime;
        if (updateTime < updateTimer) return;

        updateTime = 0f;
        Inside = GameManager.Instance.IsUnderground(Submarine.Instance.transform.position);
        Submarine.Instance.UpdateZoneText();
    }
    private void UpdateCollapsed()
    {
        entrance.gameObject.SetActive(collapsed);

        if (Inside) exit.gameObject.SetActive(!collapsed);
        else exit.gameObject.SetActive(collapsed);

        animator.SetFloat("Offset", collapsed ? 0.95f : 0);
        animator.SetBool("Collapsed", collapsed);
    }
    public void StartCaveCollapseSequence(int invokeIndex)
    {
        AudioManager.instance.StartCaveCollapse();
        VoicelinesUI.Instance.CallVoiceline(collapseStart);
        collapseParticles.SetActive(true);
        this.invokeIndex = invokeIndex;
        animator.SetFloat("Offset", 0f);
        StartCoroutine(Collapse(collapseDelay));
    }
    public IEnumerator Collapse(float delay)
    {
        Submarine.Instance.getSubmarineMovement().SetScreenShakeContinuous(true);
        yield return new WaitForSeconds(delay);

        collapsed = true;
        UpdateCollapsed();
        DataManager.Write("CaveCollapsed", 1);
    }
    private void UpdateInside()
    {
        if(!Inside && collapsed)
        {
            Debug.Log("outside when collapsed");
            collapseParticles.SetActive(false);
            Submarine.Instance.getSubmarineMovement().SetScreenShakeContinuous(false);
            StartCoroutine(BlockExit());
            VoicelinesUI.Instance.CallVoiceline(collapseEnd);
        }
        else if (Inside && !collapsed)
        {
            exit.SetActive(false);
        }
        else
        {
            Debug.Log("what that");
        }
    }
    private IEnumerator BlockExit()
    {
        bool playerOnExit = CheckIfPlayerOnExit();

        int count = 0;
        while (playerOnExit)
        {
            yield return new WaitForSeconds(1f);

            playerOnExit = CheckIfPlayerOnExit();
            count++;
            if(count > 50)
            {
                Debug.LogError("SUBMARINE IS ALWAYS ON EXIT. WHAT IS HAPPENING?");
            }
        }

        exit.SetActive(true);
    }
    private bool CheckIfPlayerOnExit()
    {
        return Physics.CheckSphere(exit.transform.position, 10f, InternalSettings.SubmarineLayer);
    }
    private void PlayerDiedWhileCollapsing()
    {
        Submarine submarine = Submarine.Instance;
        collapseParticles.SetActive(false);
        collapsed = false;
        UpdateCollapsed();
        GameManager.Instance.ResetEventForScannedFish(invokeIndex);
        DataManager.Write("CaveCollapsed", 0);

        if (submarine && submarine.getSubmarineMovement().GetContinuousShaking())
            submarine.getSubmarineMovement().SetScreenShakeContinuous(false);
    }
    private void OnPlayerDeath(Vector3 direction, DamageType dType)
    {
        if (Inside && collapsed)
        {
            PlayerDiedWhileCollapsing();
        }
    }
    private void OnDisable()
    {
        if (Submarine.Instance)
        {
            Inside = GameManager.Instance.IsUnderground(Submarine.Instance.transform.position);
            Submarine.Instance.UpdateZoneText();
        }
        else
        {
            Inside = false;
        }
    }
    private void OnDestroy()
    {
        Submarine submarine = Submarine.Instance;
        if(submarine) submarine.getSubmarineHealth().Remove_OnDie(OnPlayerDeath);
    }
    // IDL
    public void IDL_OffDistance()
    {
        enabled = false;
    }
    public void IDL_InDistance()
    {
        enabled = true;
    }
    public void IDL_AssignToGameManager()
    {
        GameManager.Instance.AssignIDL(this);
    }
    public Vector3 IDL_GetPosition(out float distanceOffset)
    {
        distanceOffset = distanceLoadOffset;
        return position;
    }

    // Action
    public static void Assign_OnInsideChanged(Action action)
    {
        OnCaveInsideChanged += action;
    }
    public static void Call_OnInsideChanged()
    {
        if(OnCaveInsideChanged != null) OnCaveInsideChanged();
    }
    public static void Remove_OnInsideChanged(Action action)
    {
        OnCaveInsideChanged -= action;
    }
}
