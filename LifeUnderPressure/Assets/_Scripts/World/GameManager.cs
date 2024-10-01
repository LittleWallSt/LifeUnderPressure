using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Submarine submarine = null;
    [SerializeField] private UpgradeCanvas upgradeCanvas = null;
    [SerializeField] private Terrain terrain = null;
    [SerializeField] private Quest[] questLine = null;
    [SerializeField] private float delayToStartNewQuest = 2.5f;
    [SerializeField] private float distanceLoadFrequency = 0.5f;
    [SerializeField] private float distanceToLoad = 25f;
    public static GameManager Instance { get; private set; }

    private List<IDistanceLoad> idls = new List<IDistanceLoad>();

    private float distanceLoadTimer = 0f;
    private int questIndex = -1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);
        QuestSystem.Reset();
    }
    private void Start()
    {
        upgradeCanvas.SetupCanvas(submarine);
    }
    public void AssignIDL(IDistanceLoad idl)
    {
        idls.Add(idl);
    }
    private void Update()
    {
        if (QuestSystem.HasQuest() && QuestSystem.GetQuestType() == Quest.QuestType.Location)
        {
            float distance = Vector3.Distance(Submarine.Instance.transform.position, QuestSystem.GetQuestLocation().position);
            if (distance < QuestSystem.GetQuestLocation().closeDistance)
            {
                QuestSystem.InQuestLocation();
            }
        }
        DistanceLoadProcess();

        if(!QuestSystem.HasQuest() && Time.time - QuestSystem.TimeLastQuestFinished > delayToStartNewQuest)
        {
            StartNextQuest();
        }
    }

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
        foreach(IDistanceLoad idl in idls)
        {
            float distance = Vector3.Distance(idl.IDL_GetPosition(), submarine.transform.position);
            if(distance > distanceToLoad)
            {
                idl.IDL_OffDistance();
            }
            else
            {
                idl.IDL_InDistance();
            }
        }
    }
    private void StartNextQuest()
    {
        questIndex++;
        if(questLine.Length > questIndex)
        {
            QuestSystem.AssignQuest(questLine[questIndex]);
        }
        else
        {
            throw new System.Exception("NO MORE QUESTS AVAILABLE. REMOVE THIS OR ADD IMPLEMENTATION");
        }
    }
    public float GetTerrainHeight(Vector3 position)
    {
        return terrain.SampleHeight(position) + terrain.transform.position.y;
    }
    private void OnDestroy()
    {
        QuestSystem.Reset();
    }
}
