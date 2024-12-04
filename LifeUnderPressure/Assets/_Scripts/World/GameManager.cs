using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [Header("Required Components")]
    [SerializeField] private Submarine submarine = null;
    [SerializeField] private Encyclopedia encyclopedia = null;
    [SerializeField] private Terrain terrain = null;

    [Header("Game Process")]
    [SerializeField] private Quest[] questLine = null;
    [SerializeField] private ScannedFishInfo[] scannedFishEvents = null;
    [SerializeField] private Vector3 initialSpawnPoint = Vector3.zero;
    [SerializeField] private Vector3 initialEulerAngles = Vector3.zero;
    [SerializeField] private float delayToStartNewQuest = 2.5f;

    public static GameManager Instance { get; private set; }
    public Vector3 InitialSpawnPoint => initialSpawnPoint;
    public Vector3 InitialEulerAngles => initialEulerAngles;

    private int questIndex = -1;
    private bool questsFinished = false;
    private bool inTutorial = false;

    [Serializable]
    public struct ScannedFishInfo
    {
        public FishInfo[] fish;
        public UnityEvent _event;
        public bool invoked;
    }

    [Header("Distance Load")]
    [SerializeField] private float distanceLoadFrequency = 0.5f;
    [SerializeField] private float distanceToLoad = 25f;

    private List<IDistanceLoad> idls = new List<IDistanceLoad>();
    private float distanceLoadTimer = 0f;

    [Header("Debug")]
    [SerializeField] private int eventIndex = 0;

    [ContextMenu("Invoke Fish Event")]
    private void InvokeEventDebug()
    {
        scannedFishEvents[eventIndex]._event.Invoke();
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        QuestSystem.Reset();
    }
    private IEnumerator Start()
    {
        InternalSettings.EnableCursor(false);

        yield return InternalSettings.WaitForDataLoading();

        if (encyclopedia) encyclopedia.LoadFishData();
        DataManager.Assign_OnSaveData(StoreQuestData);
        questIndex = DataManager.Get("QuestIndex", 0) - 1;
        inTutorial = DataManager.Get("InTutorial", 0) == 1 ? true : false;
        if (submarine) submarine.Init(initialSpawnPoint, initialEulerAngles);
    }
    private void Update()
    {
        DistanceLoadProcess();

        if (questsFinished) return;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.O))
        {
            QuestSystem.ForceScanFish();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            DataManager.DebugString();
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
        QuestSystem.SaveCurrentQuest();
    }
    public void ProcessWriteBool(string boolName, int value)
    {
        switch (boolName)
        {
            case "Upgrade_Hull":
                /// what neeed forr skill tree
                //Submarine.Instance.UpgradeSubmarine(typeof(SubmarineHull));
                UpgradeTreeCanvas.Instance.UnlockQuestNode(UpgradeType.Hull); 
                break;
            case "Upgrade_Motor":
                Submarine.Instance.UpgradeSubmarine(typeof(SubmarineMotor));
                break;
            case "Upgrade_Scanner":
                Submarine.Instance.UpgradeSubmarine(typeof(SubmarineScanner));
                break;
            case "Upgrade_Lights":
                //Submarine.Instance.UpgradeSubmarine(typeof(SubmarineLights));
                UpgradeTreeCanvas.Instance.UnlockQuestNode(UpgradeType.Lights);
                break;
        }
        DataManager.Write(boolName, value);
    }
    public void ScannedFish(FishInfo fishInfo)
    {
        if (fishInfo.locked)
        {
            Submarine.Instance.AddMoney(1);
            fishInfo.locked = false;
            if (fishInfo.OnLockedChange != null) fishInfo.OnLockedChange.Invoke();
        }

        DataManager.Write(fishInfo.name, 1);
        for (int i = 0; i < scannedFishEvents.Length; i++)
        {
            if (scannedFishEvents[i].invoked) continue;

            bool allScanned = true;
            foreach(FishInfo scannedFish in scannedFishEvents[i].fish)
            {
                if (!DataManager.IsFishScanned(scannedFish.name)) allScanned = false;
            }
            if (!allScanned) continue;

            scannedFishEvents[i]._event.Invoke();
            scannedFishEvents[i].invoked = true;
        }

        QuestSystem.ScannedFish(fishInfo);
    }
    public void ResetEventForScannedFish(int index)
    {
        scannedFishEvents[index].invoked = false;
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
            float distance = Vector3.Distance(idl.IDL_GetPosition(out float distanceOffset), submarine.transform.position);
            if (distance > distanceToLoad + distanceOffset)
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
        if (terrain == null) return -1000f;
        return terrain.SampleHeight(position) + terrain.transform.position.y;
    }
    public bool IsUnderground(Vector3 position)
    {
        float terrainHeight = GetTerrainHeight(position);
        return position.y <= terrainHeight;
    }
    // Setters
    public void SetInTutorial(bool state)
    {
        inTutorial = state;
        DataManager.Write("InTutorial", state ? 1 : 0);
        if (state) QuestSystem.Reset();
    }

    private void OnDestroy()
    {
        QuestSystem.Reset();
        DataManager.Remove_OnSaveData(StoreQuestData);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(initialSpawnPoint, 0.35f);
    }
}
