using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] SubmarineMovement movement;
    [SerializeField] TextMeshProUGUI promt;

    [SerializeField] Promt[] promts;

    public float cdTime = 1f;
    float timer;

    public bool tutorialOn;
    bool actionLock = false;
    int currentStep = 0;
    TutorControls currentControl;

    #region movementLock
    public bool rotLock;
    public bool WLock;
    public bool SLock ;
    public bool ADLock;
    public bool UpDownLock;
    #endregion

    private void Start()
    {
        rotLock = true;
        WLock = true;
        SLock = true;
        ADLock = true;
        UpDownLock = true;


        PlayNextStep();
        tutorialOn = true;
    }

    private void Update()
    {
        if (actionLock)
        {
            CheckForInput();
            return;
        }

        timer += Time.deltaTime;

        if (timer >= cdTime)
        {
            if (Input.anyKey && !actionLock)
            {
                PlayNextStep();
                timer = 0;
            }
        }


        
    }

    

    void CheckForInput()
    {
        switch (currentControl)
        {
            case TutorControls.W:
                if (Input.GetKeyDown(KeyCode.W))
                {
                    WLock = false;
                    actionLock = false;
                    currentControl = TutorControls.None;
                }
                break;
            case TutorControls.Rotation:
                if (MouseMovement())
                {
                    rotLock = false;
                    actionLock = false;
                    currentControl = TutorControls.None;
                }
                break;
            case TutorControls.S:
                if (Input.GetKeyDown(KeyCode.S))
                {
                    SLock = false;
                    actionLock = false;
                    currentControl = TutorControls.None;
                }
                break;
            case TutorControls.AD:
                if (Input.GetKeyDown(KeyCode.A))
                {
                    ADLock = false;
                    actionLock = false;
                    currentControl = TutorControls.None;
                }
                break;
            case TutorControls.UpDown:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    UpDownLock = false;
                    actionLock = false;
                    currentControl = TutorControls.None;
                }
                break;

        }
    }

    void PlayNextStep()
    {
        Promt nextPromt = promts[currentStep];
        if (nextPromt.action)
        {
            actionLock = true;
            currentControl = nextPromt.tutorControls;
        }

        //StartTyping(nextPromt.textPromt, promt);
        promt.text = nextPromt.textPromt;
        if (currentStep < promts.Length - 1) currentStep++;
    }

    bool MouseMovement()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        return mouseX != 0f || mouseY != 0f;
    }




    /*public void StartTyping(string fullText, TextMeshProUGUI uiText)
    {
        StartCoroutine(TypeText(fullText, uiText));
    }

    
    private IEnumerator TypeText(string fullText, TextMeshProUGUI uiText)
    {
        uiText.text = ""; 

        foreach (char letter in fullText.ToCharArray())
        {
            uiText.text += letter; 
            yield return new WaitForSeconds(0.05f); 
        }
    }*/
}


[Serializable]
public struct Promt
{
    public bool action;
    [TextArea(5, 15)]
    public string textPromt;
    public AudioClip voicelinePromt;
    public TutorControls tutorControls;
}

public enum TutorControls
{
    Rotation, S, W, AD, UpDown, None
}





