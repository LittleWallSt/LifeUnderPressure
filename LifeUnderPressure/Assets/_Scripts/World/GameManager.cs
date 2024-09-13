using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Quest startingQuest = null;
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);
        QuestSystem.Reset();
        QuestSystem.AssignQuest(startingQuest);
    }
    private void OnDestroy()
    {
        QuestSystem.Reset();
    }
}
