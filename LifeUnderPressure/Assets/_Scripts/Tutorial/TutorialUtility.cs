using FMODUnity;
using System;
using TMPro;
using UnityEngine;

public class TutorialUtility : MonoBehaviour
{

    public static TutorialUtility Instance { get; private set; }
    public GameObject submarine; 
    [SerializeField] TextMeshProUGUI promt;
    [SerializeField] Promt[] promts;


    public float cdTime = 1f;
    float timer;


    bool actionLock = false;
    int currentStep = 0;
    

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentStep = 0;
        PlayNextStep();
    }


    private void Update()
    {

        timer += Time.deltaTime; 

        if (timer >= cdTime)
        {

            if (Input.anyKey && !actionLock)
            {
                PlayNextStep();
                timer = 0;
            }
        }

        if (actionLock)
        {
            if (promts[currentStep].tutorialStep.isCompleted)
            {
                actionLock= false;
                if (currentStep < promts.Length - 1) currentStep++;
                PlayNextStep();
            } else
            {
                promts[currentStep].tutorialStep.CheckForCompleting();
            }
        }
    }

    


    void PlayNextStep()
    {
        Promt nextPromt = promts[currentStep];
        if (nextPromt.action)
        {
            actionLock = true;
            nextPromt.tutorialStep.StartStep();
        }

        //StartTyping(nextPromt.textPromt, promt);

        promt.text = nextPromt.textPromt;
       
    }

   


}

[Serializable]
public struct Promt
{
    public bool action;
    [TextArea(5, 15)]
    public string textPromt;
    public TutorialStep tutorialStep;
    public EventReference voiceline;
}
