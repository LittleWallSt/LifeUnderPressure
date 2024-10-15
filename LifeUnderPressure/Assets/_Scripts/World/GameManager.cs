using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Submarine submarine = null;
    [SerializeField] private UpgradeCanvas upgradeCanvas = null;
    [SerializeField] private Terrain terrain = null;
    [SerializeField] private Quest[] questLine = null;
    [SerializeField] private Vector3 initialSpawnPoint = Vector3.zero;
    [SerializeField] private Vector3 initialEulerAngles = Vector3.zero;
    [SerializeField] private float delayToStartNewQuest = 2.5f;
    [SerializeField] private float distanceLoadFrequency = 0.5f;
    [SerializeField] private float distanceToLoad = 25f;
    public static GameManager Instance { get; private set; }
    public Vector3 InitialSpawnPoint => initialSpawnPoint;
    public Vector3 InitialEulerAngles => initialEulerAngles;

    private List<IDistanceLoad> idls = new List<IDistanceLoad>();

    private Action onDataLoaded;

    private float distanceLoadTimer = 0f;
    private int questIndex = -1;
    private bool questsFinished = false;

    private bool inTutorial = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);
        QuestSystem.Reset();
        StartCoroutine(LoadDataProcess());
    }
    private IEnumerator LoadDataProcess()
    {
        DataManager.Init();
        yield return StartCoroutine(DataManager.LoadData());

        yield return null;
        DataManager.Assign_OnSaveData(StoreQuestData);
        Call_OnDataLoaded();
        questIndex = DataManager.Get("QuestIndex", 0) - 1;
        inTutorial = DataManager.Get("InTutorial", 0) == 1 ? true : false;
        submarine.Init();
        upgradeCanvas?.SetupCanvas(submarine);
    }
    private void Start()
    {
        InternalSettings.EnableCursor(false);
    }
    private void Update()
    {
        DistanceLoadProcess();

        if (questsFinished) return;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.O))
        {
            QuestSystem.ForceCompleteQuest();
        }
#endif
        if (QuestSystem.HasQuest() && QuestSystem.GetQuestType() == Quest.QuestType.Location)
        {
            float distance = Vector3.Distance(Submarine.Instance.transform.position, QuestSystem.GetQuestLocation().position);
            if (distance < QuestSystem.GetQuestLocation().closeDistance)
            {
                QuestSystem.InQuestLocation();
            }
        }

        if (inTutorial) return;

        if(!QuestSystem.HasQuest() && Time.time - QuestSystem.TimeLastQuestFinished > delayToStartNewQuest)
        {
            StartNextQuest();
        }
    }
    private void StartNextQuest()
    {
        questIndex++;
        if (questLine.Length > questIndex)
        {
            QuestSystem.AssignQuest(questLine[questIndex]);
        }
        else
        {
            questsFinished = true;
        }
    }
    private void StoreQuestData()
    {
        DataManager.Write("QuestIndex", questIndex);
    }
    public void ProcessWriteBool(string boolName, int value)
    {
        switch (boolName)
        {
            case "Upgrade_Hull":
                Submarine.Instance.UpgradeSubmarine(typeof(SubmarineHull));
                break;
            case "Upgrade_Motor":
                Submarine.Instance.UpgradeSubmarine(typeof(SubmarineMotor));
                break;
            case "Upgrade_Scanner":
                Submarine.Instance.UpgradeSubmarine(typeof(SubmarineScanner));
                break;
            case "Upgrade_Lights":
                Submarine.Instance.UpgradeSubmarine(typeof(SubmarineLights));
                break;
        }
        DataManager.Write(boolName, value);
    }
    // Distance Load
    private void DistanceLoadProcess()
    {
        distanceLoadTimer += Time.deltaTime;
        if (distanceLoadTimer > distanceLoadFrequency)
        {
            distanceLoadTimer = 0f;
            UpdateDistanceLoad();
        }
    }
    private void UpdateDistanceLoad()
    {
        foreach (IDistanceLoad idl in idls)
        {
            float distance = Vector3.Distance(idl.IDL_GetPosition(), submarine.transform.position);
            if (distance > distanceToLoad)
            {
                idl.IDL_OffDistance();
            }
            else
            {
                idl.IDL_InDistance();
            }
        }
    }
    public void AssignIDL(IDistanceLoad idl)
    {
        idls.Add(idl);
    }
    // Getters
    public float GetTerrainHeight(Vector3 position)
    {
        return terrain.SampleHeight(position) + terrain.transform.position.y;
    }
    // Setters
    public void SetInTutorial(bool state)
    {
        inTutorial = state;
        DataManager.Write("InTutorial", state ? 1 : 0);
        if (state) QuestSystem.Reset();
    }
    // Action
    public void Assign_OnDataLoaded(Action action)
    {
        onDataLoaded += action;
    }
    public void Remove_OnDataLoaded(Action action)
    {
        onDataLoaded -= action;
    }
    private void Call_OnDataLoaded()
    {
        if (onDataLoaded != null) onDataLoaded();
    }
    private void OnDestroy()
    {
        QuestSystem.Reset();
        DataManager.Remove_OnSaveData(StoreQuestData);
        DataManager.Reset();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(initialSpawnPoint, 0.35f);
    }
}
